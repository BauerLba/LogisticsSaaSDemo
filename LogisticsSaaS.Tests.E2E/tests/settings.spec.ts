import { test, expect } from '@playwright/test';

test.describe('Settings', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[name="email"]', 'demo@logiflow.com');
    await page.fill('input[name="password"]', 'Demo123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/');
  });

  test('should load settings page', async ({ page }) => {
    await page.goto('/settings');

    const title = page.locator('h1:has-text("Settings")');
    expect(await title.isVisible()).toBeTruthy();
  });

  test('should display profile section', async ({ page }) => {
    await page.goto('/settings');

    const profileSection = page.locator('h2:has-text("Profile Information")');
    expect(await profileSection.isVisible()).toBeTruthy();

    const nameInput = page.locator('input[type="text"]').first();
    expect(await nameInput.isVisible()).toBeTruthy();
  });

  test('should display password change section', async ({ page }) => {
    await page.goto('/settings');

    const passwordSection = page.locator('h2:has-text("Change Password")');
    expect(await passwordSection.isVisible()).toBeTruthy();
  });

  test('should display notification preferences', async ({ page }) => {
    await page.goto('/settings');

    const notificationSection = page.locator('h2:has-text("Notification Preferences")');
    expect(await notificationSection.isVisible()).toBeTruthy();

    // Check for checkboxes
    const checkboxes = page.locator('input[type="checkbox"]');
    const count = await checkboxes.count();

    expect(count).toBeGreaterThanOrEqual(3);
  });

  test('should toggle notification preferences', async ({ page }) => {
    await page.goto('/settings');

    const checkboxes = page.locator('input[type="checkbox"]');
    const firstCheckbox = checkboxes.first();

    const initialChecked = await firstCheckbox.isChecked();
    await firstCheckbox.click();

    const afterClick = await firstCheckbox.isChecked();
    expect(afterClick).not.toBe(initialChecked);
  });
});
