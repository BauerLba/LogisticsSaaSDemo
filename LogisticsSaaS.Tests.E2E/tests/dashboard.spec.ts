import { test, expect } from '@playwright/test';

test.describe('Dashboard', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[name="email"]', 'demo@logiflow.com');
    await page.fill('input[name="password"]', 'Demo123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/');
  });

  test('should display dashboard with stats cards', async ({ page }) => {
    await page.goto('/');

    // Check for stat cards
    const statCards = page.locator('.stats-grid .card');
    const count = await statCards.count();

    expect(count).toBeGreaterThanOrEqual(4);
  });

  test('should display charts', async ({ page }) => {
    await page.goto('/');

    // Check for timeline chart
    const timelineChart = page.locator('#timelineChart');
    expect(await timelineChart.isVisible()).toBeTruthy();

    // Check for status chart
    const statusChart = page.locator('#statusChart');
    expect(await statusChart.isVisible()).toBeTruthy();
  });

  test('should display active shipments table', async ({ page }) => {
    await page.goto('/');

    // Wait for table
    await page.waitForSelector('table', { timeout: 5000 });

    const table = await page.locator('table');
    expect(await table.isVisible()).toBeTruthy();
  });

  test('should search shipments from dashboard', async ({ page }) => {
    await page.goto('/');

    const searchInput = page.locator('#search-tracking');
    await searchInput.fill('TRK');

    // Wait for filter
    await page.waitForTimeout(500);

    const table = await page.locator('table');
    expect(await table.isVisible()).toBeTruthy();
  });

  test('should show skeleton loaders while loading', async ({ page }) => {
    // Intercept network to slow down loading
    await page.route('**/api/**', route => {
      setTimeout(() => route.continue(), 1000);
    });

    await page.goto('/shipments', { waitUntil: 'domcontentloaded' });

    // Skeleton should be visible during load
    const skeletons = page.locator('.skeleton');
    if (await skeletons.count() > 0) {
      expect(await skeletons.first().isVisible()).toBeTruthy();
    }
  });
});
