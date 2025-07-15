using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaplinAutomationTests.Helpers_Methods
{
    public static class SortHelper
    {
        public static async Task SortByChangePercentAsync(IPage page, string sortOrder)
        {
            await page.GetByText("Change %").ClickAsync();
            var sortOption = page.Locator($"div[title='{sortOrder}']");
            for (int i = 0; i < await sortOption.CountAsync(); i++)
            {
                var option = sortOption.Nth(i);
                try
                {
                    await option.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
                    await option.ClickAsync();
                    await page.Locator("table tbody tr").First.WaitForAsync(new()
                    {
                        State = WaitForSelectorState.Attached,
                        Timeout = 10000
                    });
                    break;
                }
                catch (TimeoutException) { /* Try next */ }
            }
        }
    }
}
