class DarkModeManager {
    constructor() {
        this.storageKey = 'logiflow-theme';
        this.init();
    }

    init() {
        const saved = localStorage.getItem(this.storageKey);
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

        if (saved === 'dark' || (!saved && prefersDark)) {
            this.setDarkMode(true);
        } else if (saved === 'light') {
            this.setDarkMode(false);
        }

        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (!localStorage.getItem(this.storageKey)) {
                this.setDarkMode(e.matches);
            }
        });
    }

    setDarkMode(isDark) {
        const html = document.documentElement;
        if (isDark) {
            html.classList.add('dark-mode');
            html.classList.remove('light-mode');
            localStorage.setItem(this.storageKey, 'dark');
        } else {
            html.classList.add('light-mode');
            html.classList.remove('dark-mode');
            localStorage.setItem(this.storageKey, 'light');
        }
    }

    toggle() {
        const html = document.documentElement;
        const isDark = html.classList.contains('dark-mode');
        this.setDarkMode(!isDark);
        return !isDark;
    }

    isDarkMode() {
        return document.documentElement.classList.contains('dark-mode');
    }
}

window.darkMode = new DarkModeManager();
