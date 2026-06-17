import { test, expect } from '@playwright/test';

test.describe('Navigation & Login', () => {
  test('should redirect unauthenticated users to login', async ({ page }) => {
    await page.goto('/');
    await page.waitForURL('**/login');
    expect(page.url()).toContain('/login');
  });

  test('should allow login with demo credentials', async ({ page }) => {
    await page.goto('/login');

    await page.fill('input[name="email"]', 'demo@logiflow.com');
    await page.fill('input[name="password"]', 'Demo123!');

    await page.click('button[type="submit"]');
    await page.waitForURL('**/');

    expect(page.url()).not.toContain('/login');
  });

  test('should show error on invalid login', async ({ page }) => {
    await page.goto('/login');

    await page.fill('input[name="email"]', 'invalid@email.com');
    await page.fill('input[name="password"]', 'wrongpassword');

    await page.click('button[type="submit"]');
    await page.waitForURL('**/login?error=1');

    expect(page.url()).toContain('error=1');
  });

  test('should navigate to shipments page', async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[name="email"]', 'demo@logiflow.com');
    await page.fill('input[name="password"]', 'Demo123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/');

    await page.click('a:has-text("Shipments")');
    await page.waitForURL('**/shipments');

    expect(page.url()).toContain('/shipments');
  });

  test('should navigate to customers page', async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[name="email"]', 'demo@logiflow.com');
    await page.fill('input[name="password"]', 'Demo123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/');

    await page.click('a:has-text("Customers")');
    await page.waitForURL('**/customers');

    expect(page.url()).toContain('/customers');
  });

  test('should access settings page', async ({ page }) => {
    await page.goto('/login');
    await page.fill('input[name="email"]', 'demo@logiflow.com');
    await page.fill('input[name="password"]', 'Demo123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/');

    await page.click('a:has-text("Settings")');
    await page.waitForURL('**/settings');

    expect(page.url()).toContain('/settings');
  });
});
