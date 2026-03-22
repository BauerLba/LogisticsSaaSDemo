# Deployment-Anleitung: Oracle Cloud Free Tier

## Voraussetzungen
- GitHub-Account (Repository muss Public oder mit Packages-Zugriff sein)
- Oracle Cloud Account (kostenlos: cloud.oracle.com)

---

## Schritt 1: Oracle Cloud Account erstellen

1. Gehe zu https://cloud.oracle.com → "Start for free"
2. Registriere dich (Kreditkarte nötig, wird NICHT belastet)
3. Region wählen: **Germany Central (Frankfurt)**

---

## Schritt 2: VM erstellen

1. Im Oracle Cloud Dashboard: **Compute → Instances → Create Instance**
2. Einstellungen:
   - Name: `logisticssaas`
   - Image: **Ubuntu 22.04**
   - Shape: **VM.Standard.A1.Flex** (Always Free)
     - OCPUs: `2`
     - Memory: `12 GB`
3. SSH-Key: Klicke "Generate a key pair for me" und lade beide Dateien herunter
   - Speichere `ssh-key-XXXX.key` sicher ab (z.B. unter `C:\Users\Lucas\.ssh\oracle.key`)
4. Klicke **Create** 
5. Warte bis Status = **Running**, dann notiere dir die **Public IP**

---

## Schritt 3: Firewall-Ports öffnen (Oracle Cloud)

1. Im Dashboard: **Networking → Virtual Cloud Networks → Dein VCN**
2. Klicke auf **Security Lists → Default Security List**
3. Klicke **Add Ingress Rules** und füge hinzu:
   - Source CIDR: `0.0.0.0/0`, Protocol: TCP, Port: `80`
   - Source CIDR: `0.0.0.0/0`, Protocol: TCP, Port: `443` (optional, für später)

---

## Schritt 4: VM einrichten (einmalig)

Öffne PowerShell auf deinem PC:

```powershell
# Berechtigungen für den SSH-Key setzen
icacls "C:\Users\Lucas\.ssh\oracle.key" /inheritance:r /grant:r "$env:USERNAME:(R)"

# Mit der VM verbinden (ersetze DEINE-IP)
ssh -i "C:\Users\Lucas\.ssh\oracle.key" ubuntu@DEINE-IP
```

In der SSH-Session auf der VM:

```bash
# Ubuntu Firewall Ports öffnen
sudo iptables -I INPUT -p tcp --dport 80 -j ACCEPT
sudo iptables -I INPUT -p tcp --dport 443 -j ACCEPT
sudo netfilter-persistent save

# Docker installieren
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker ubuntu
newgrp docker

# Docker Compose Plugin installieren
sudo apt install docker-compose-plugin -y

# Projektordner erstellen
mkdir ~/logisticssaas
```

---

## Schritt 5: .env-Datei auf der VM erstellen

Noch in der SSH-Session:

```bash
nano ~/logisticssaas/.env
```

Inhalt (ersetze `DEIN-GITHUB-USERNAME` und wähle ein sicheres Passwort):

```
GITHUB_USERNAME=DEIN-GITHUB-USERNAME
DB_PASSWORD=EinSicheresPasswort123!
```

Speichern: `Ctrl+X` → `Y` → `Enter`

---

## Schritt 6: docker-compose.yml auf die VM kopieren

Entweder per SCP von deinem PC:

```powershell
scp -i "C:\Users\Lucas\.ssh\oracle.key" docker-compose.yml ubuntu@DEINE-IP:~/logisticssaas/
```

Oder direkt auf der VM erstellen:

```bash
nano ~/logisticssaas/docker-compose.yml
```

(Inhalt aus der `docker-compose.yml` im Repository einfügen)

---

## Schritt 7: GitHub Secrets konfigurieren

Gehe in GitHub zu: **Repository → Settings → Secrets and variables → Actions**

Füge folgende Secrets hinzu:

| Name | Wert |
|------|------|
| `ORACLE_HOST` | Deine Public IP der VM (z.B. `132.145.xxx.xxx`) |
| `ORACLE_SSH_KEY` | Inhalt der `.key` Datei (öffne mit Editor, alles kopieren) |

---

## Schritt 8: GitHub Container Registry aktivieren

1. Gehe zu: **GitHub → Settings → Packages**
2. Stelle sicher, dass "Improved Container Support" aktiviert ist

Das war's! Jetzt ist alles eingerichtet.

---

## Schritt 9: Ersten Deploy auslösen

Pushe einfach eine Änderung auf den `main`-Branch:

```bash
git add .
git commit -m "feat: Deploy-Konfiguration"
git push
```

GitHub Actions baut jetzt automatisch:
1. Das Docker-Image für ARM64
2. Pusht es zu `ghcr.io`
3. SSH verbindet sich mit deiner Oracle-VM
4. Zieht das neue Image und startet die Container

**Die App ist dann erreichbar unter: `http://DEINE-IP`**

---

## Bei Problemen: Logs auf der VM prüfen

```bash
ssh -i "C:\Users\Lucas\.ssh\oracle.key" ubuntu@DEINE-IP

# Container-Status
docker compose -f ~/logisticssaas/docker-compose.yml ps

# App-Logs
docker compose -f ~/logisticssaas/docker-compose.yml logs app

# Datenbank-Logs
docker compose -f ~/logisticssaas/docker-compose.yml logs db
```

---

## Optional: Eigene Domain + HTTPS

Falls du eine Domain hast, kannst du Nginx + Let's Encrypt hinzufügen:

```bash
sudo apt install nginx certbot python3-certbot-nginx -y
sudo certbot --nginx -d deine-domain.de
```

Dann Nginx als Reverse Proxy auf Port 80 → 8080 konfigurieren.
