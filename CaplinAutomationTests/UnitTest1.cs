using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using static System.Net.Mime.MediaTypeNames;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Collections.Generic;



namespace CaplinAutomationTests
{
    public class Tests : PageTest
    {


        [SetUp]
        public async Task Setup()
        {
            
            await Page.GotoAsync("https://www.londonstockexchange.com/");

            await Page.GetByRole(AriaRole.Button, new() { Name = "Accept all cookies" }).ClickAsync();

            await Page.ClickAsync("text=View FTSE 100");
            
            await Page.GotoAsync("https://www.londonstockexchange.com/indices/ftse-100/constituents/table");

        }

        [Test]
        public async Task Test1()
        {

            //clicks "Change %" column header
            await Page.GetByText("Change %").ClickAsync();
            

            //selects Highest-lowest sorting option
            var sortOptions = Page.Locator("div[title='Highest - lowest']");

            for (int i = 0; i < await sortOptions.CountAsync(); i++)
            {
                var option = sortOptions.Nth(i);
                try
                {
                    await option.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
                    // Then click it
                    await option.ClickAsync();
                    
                    //waits until table fully loads
                    await Page.Locator("table tbody tr").First.WaitForAsync(new()
                    {
                        State = WaitForSelectorState.Attached,
                        Timeout = 10000
                    });
                    break;
                }
                catch (TimeoutException)
                {
                    // Not visible, try next
                }
            }



            // Get table headers and rows (up to 10)
            var allRows = await Page.Locator("table tbody tr").AllAsync();
            var first10 = allRows.Take(10).ToList();

            Assert.That(first10.Count, Is.EqualTo(10), "Expected exactly 10 rows.");

            var allHeaders = await Page.Locator("table thead tr th").AllInnerTextsAsync();
            int changeColumn = allHeaders.ToList().FindIndex(h => h.Contains("Change %", StringComparison.OrdinalIgnoreCase));

            Assert.That(changeColumn, Is.GreaterThanOrEqualTo(0), "Change % column not found.");

            //loops through rows and displayes the results to console
            var changeValues = new List<double>();
            int rowIndex = 1;

            foreach (var row in first10)
            {
                var cells = await row.Locator("td").AllInnerTextsAsync();
                Console.WriteLine($"Row {rowIndex++}: {string.Join(" | ", cells)}");
                var changeText = cells[changeColumn].Replace(",", "").Trim();

                if (double.TryParse(changeText, out double change))
                {
                    changeValues.Add(change);
                }
            }
            // validates sorting is descending
            var sortedDescending = changeValues.OrderByDescending(x => x).ToList();
            Assert.That(changeValues, Is.EqualTo(sortedDescending), "Change % values are not sorted from highest to lowest.");


        }
        [Test]
        public async Task Test2()
        {
            // Clicks "Change %" column to open the sort menu
            await Page.GetByText("Change %").ClickAsync();

            //Clicks "Lowest - Highest" 
            var sortOptions = Page.Locator("div[title='Lowest – highest']");
            for (int i = 0; i < await sortOptions.CountAsync(); i++)
            {
                var option = sortOptions.Nth(i);
                try
                {
                    await option.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
                    await option.ClickAsync();

                    //Waits for the table to reload after sorting
                    await Page.Locator("table tbody tr").First.WaitForAsync(new()
                    {
                        State = WaitForSelectorState.Visible,
                        Timeout = 5000
                    });
                    break;
                }
                catch (TimeoutException)
                {
                    // Not visible, try next
                }
            }




            // Gets all table rows
            var allRows = await Page.Locator("table tbody tr").AllAsync();

            // Take the first 10 rows 
            var first10 = allRows.Take(10).ToList();

            Assert.That(first10.Count, Is.EqualTo(10), "Expected exactly 10 rows.");

            //Finds Change % column
            var allHeaders = await Page.Locator("table thead tr th").AllInnerTextsAsync();
            int changeHeader = allHeaders.ToList().FindIndex(h => h.Contains("Change %", StringComparison.OrdinalIgnoreCase));
            Assert.That(changeHeader, Is.GreaterThanOrEqualTo(0), "Change % column not found.");

            var changeValues = new List<double>();

            int rowIndex = 1;
            foreach (var row in first10)
            {
                var cells = await row.Locator("td").AllInnerTextsAsync();
                var changeText = cells[changeHeader].Replace(",", "").Trim();
                Console.WriteLine($"Row {rowIndex++}: {string.Join(" | ", cells)}");

                if (double.TryParse(changeText, out double value))
                {
                    changeValues.Add(value);
                    
                }
                
            }

            var sortedAscending = changeValues.OrderByDescending(x => x).ToList();
            Assert.That(changeValues, Is.EqualTo(sortedAscending), "Change % values are not sorted from highest to lowest.");
        }
        [Test]
        public async Task Test3()
        {
            //wait for table data to load
            await Page.Locator("table tbody tr").First.WaitForAsync(new()
            {
                State = WaitForSelectorState.Attached,
                Timeout = 5000
            });

            //get all rows and headers
            var allRows = await Page.Locator("table tbody tr").AllAsync();
            if (allRows.Count == 0)
            {
                Console.WriteLine("No rows in a table has been found");
            }
            var allHeaders = await Page.Locator("table thead tr th").AllInnerTextsAsync();
            
            //finds "market cap" header
            int marketCapHeader = allHeaders.ToList().FindIndex(h => h.Contains("Market cap (m)", StringComparison.OrdinalIgnoreCase));
            if (marketCapHeader == -1)
            {
                Console.WriteLine("Market Cap haeder could not be found");
            }

            //loops through all rows
            bool anyAbove7m = false;
            foreach (var row in allRows)
            {
                var cells = await row.Locator("td").AllInnerTextsAsync();
                if (cells.Count <= marketCapHeader)
                    continue;

                var marketCapText = cells[marketCapHeader].Replace(",", "").Trim();


                if (double.TryParse(marketCapText, out double marketCap))
                {
                   
                    if (marketCap < 7_000_000)
                    {
                        
                        Console.WriteLine($"Row: {string.Join(" | ", cells)}");
                        anyAbove7m = true;
                    }
                }
                else
                {
                    Console.WriteLine("Couldn't parse market cap values");
                }
            }
                if (!anyAbove7m) 
                {
                    Console.WriteLine("There is no constituents where the ‘Market Cap’ exceeds 7 million");
                }




            }
        }

    }
   
   




