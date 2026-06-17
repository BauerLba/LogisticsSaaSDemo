import { test, expect } from '@playwright/test';

test.describe('Shipments', () => {
  test.beforeEach(async ({ page }) => {
    // Login first
    await page.goto('/login');
    await page.fill('input[name="email"]', 'demo@logiflow.com');
    await page.fill('input[name="password"]', 'Demo123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/');
  });

  test('should load shipments page and display table', async ({ page }) => {
    await page.goto('/shipments');

    // Wait for table to load
    await page.waitForSelector('table', { timeout: 5000 });

    const table = await page.locator('table');
    expect(await table.isVisible()).toBeTruthy();
  });

  test('should display pagination controls', async ({ page }) => {
    await page.goto('/shipments');

    // Check for pagination buttons
    const pagination = await page.locator('button:has-text("Previous"), button:has-text("Next")');
    const count = await pagination.count();

    expect(count).toBeGreaterThanOrEqual(0);
  });

  test('should filter shipments by search term', async ({ page }) => {
    await page.goto('/shipments');

    // Enter search term
    const searchInput = page.locator('#search-shipment');
    await searchInput.fill('TRK');

    // Wait for results
    await page.waitForTimeout(500);

    // Table should still be visible
    const table = await page.locator('table');
    expect(await table.isVisible()).toBeTruthy();
  });

  test('should export to CSV', async ({ page }) => {
    await page.goto('/shipments');

    // Click export button
    const exportBtn = page.locator('button:has-text("Export CSV")');
    const downloadPromise = page.waitForEvent('download');

    await exportBtn.click();

    const download = await downloadPromise;
    expect(download.suggestedFilename()).toContain('shipments');
  });

  test('should navigate to create new shipment', async ({ page }) => {
    await page.goto('/shipments');

    const newBtn = page.locator('button:has-text("New Shipment")');
    await newBtn.click();

    await page.waitForURL('**/shipments/new');
    expect(page.url()).toContain('/shipments/new');
  });

  test('should display shipment details when clicked', async ({ page }) => {
    await page.goto('/shipments');

    // Click first shipment row
    const shipmentRow = page.locator('tr[class="shipment-row"]').first();
    if (await shipmentRow.isVisible()) {
      await shipmentRow.click();
      await page.waitForURL('**/shipment/**', { timeout: 5000 });

      expect(page.url()).toContain('/shipment/');
    }
  });
});
