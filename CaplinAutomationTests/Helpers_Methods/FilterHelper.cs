using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaplinAutomationTests.Helpers_Methods
{
    public static class FilterHelper 
    {
        public static async Task<List<double>> GetAndPrintTop10ChangePercentAsync(IPage page, bool descending)
        {
            var allRows = await page.Locator("table tbody tr").AllAsync();
            var first10 = allRows.Take(10).ToList();
            Assert.That(first10.Count, Is.EqualTo(10), "Expected exactly 10 rows.");

            var allHeaders = await page.Locator("table thead tr th").AllInnerTextsAsync();
            int changeColumn = allHeaders.ToList()
                .FindIndex(h => h.Contains("Change %", StringComparison.OrdinalIgnoreCase));
            Assert.That(changeColumn, Is.GreaterThanOrEqualTo(0), "Change % column not found.");

            var changeValues = new List<double>();
            int rowIndex = 1;

            foreach (var row in first10)
            {
                var cells = await row.Locator("td").AllInnerTextsAsync();
                Console.WriteLine($"Row {rowIndex++}: {string.Join(" | ", cells)}");

                var changeText = cells[changeColumn].Replace(",", "").Trim();
                if (double.TryParse(changeText, out double value))
                {
                    changeValues.Add(value);
                }
            }

            var expected = descending
                ? changeValues.OrderByDescending(x => x).ToList()
                : changeValues.OrderBy(x => x).ToList();

            Assert.That(changeValues, Is.EqualTo(expected),
                $"Change % values are not sorted {(descending ? "descending" : "ascending")}.");

            return changeValues;
        }

    }
}
