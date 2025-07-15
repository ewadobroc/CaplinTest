using Microsoft.Playwright;
using NUnit.Framework;
using CaplinAutomationTests.Helpers_Methods;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using System;
using System.Linq;



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

        public async Task Test1_Top10_Constituents_Highest_To_Lowest()
        {

           
            
            await SortHelper.SortByChangePercentAsync(Page,"Highest - lowest");
            await FilterHelper.GetAndPrintTop10ChangePercentAsync(Page,descending: true);


        }
        [Test]
        public async Task Test2_Top10_Constituents_Lowest_To_Highest()
        {
           
            await SortHelper.SortByChangePercentAsync(Page,"Lowest – highest");
            await FilterHelper.GetAndPrintTop10ChangePercentAsync(Page,descending: false);
            

        }
        [Test]
        public async Task Test3_Market_Cap_Filter()
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
   
   




