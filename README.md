This project contains tests for the FTSE 100 constituents table using Playwright for .NET (C#) and NUnit.
The tests verify sorting functionality based on "Change %" column and filter rows based on "Market Cap (m)" column.

Prerequisites to run the solution:
* .NET 6.0+ installed
* Visual Studio 2022 (or any other ID with C# support)
* Playwright Browsers (first run will prompt to install them)
Or you can install manually via terminal: pwsh bin/Debug/netX/playwright.ps1 install
(Replace netX with your .NET version)

Required NuGet Packages:
* Microsoft.Playwright
* Microsoft.Playwright.NUnit
* NUnit (testing framework)
* NUnit3TestAdapter (Visual Studio test runner)



Test Cases

Test1 – Top 10 Constituents (Highest % Change)
Steps:
1. Clicks the Change % column and selects "Highest – lowest".
2. Verifies the table loads 10 rows.
3. Asserts that % Change values are sorted in descending order.
4. Prints the top 10 rows to the console.

Test2 – Top 10 Constituents (Lowest % Change)
Steps:
1. Clicks the Change % column and selects "Lowest – highest".
2. Verifies the table loads 10 rows.
3. Asserts that % Change values are sorted in ascending order.
4. Prints the top 10 rows to the console.

Test3 – Market Cap Filter
1. Iterates through all rows
2. Prints rows where Market Cap (m) exceeds 7,000,000.
3. If no such rows exist, prints a message to the console.



