using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace HomeBudgetAPI
{
    // ====================================================================
    // CLASS: HomeBudget
    //        - Combines categories Class and expenses Class
    //        - One File defines Category and Budget File
    //        - etc
    // ====================================================================
    /// <summary>
    /// One main plan that combines the categories and expenses class. Reads from database for categorization. Gets budget items by month, category, none, or both.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
    /// 
    /// List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null, true, filterCategory); 
    /// 
    /// List<BudgetItemsByMonth> budgetItemsByMonth = budget.GetBudgetItemsByMonth(startDate, endDate, false, 0);
    /// 
    /// List<BudgetItemsByCategory> budgetItemsByCategory = budget.GetBudgetItemsByCategory(null, null, true, 9);
    /// 
    /// List<Dictionary<string, object> budgetItemsByMonthCategory = homeBudget.GetBudgetDictionaryByCategoryAndMonth(startDate, endDate, true, 14);
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="Categories"/>
    /// <seealso cref="Expenses"/>
    public class HomeBudget
    {
        private Categories _categories;
        private Expenses _expenses;

        // ====================================================================
        // Properties
        // ===================================================================

        // Properties (categories and expenses object)
        /// <summary>
        /// Gets the categories object
        /// </summary>
        /// <value>The categories object</value>
        public Categories categories { get { return _categories; } }

        /// <summary>
        /// Gets the expenses object
        /// </summary>
        /// <value>The expenses object</value>
        public Expenses expenses { get { return _expenses; } }

        #region Constructor
        /// <summary>
        /// Creates a HomeBudget object and initializes the Categories and Expenses objects with the database connection.
        /// </summary>
        public HomeBudget(String databaseFile, bool newDB = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDB && File.Exists(databaseFile))
            {
                Database.existingDatabase(databaseFile);
            }

            // file did not exist, or user wants a new database, so open NEW DB
            else
            {
                Database.newDatabase(databaseFile);
                newDB = true;
            }

            _categories = new Categories(Database.dbConnection, newDB);

            _expenses = new Expenses(Database.dbConnection, newDB);
        }
        #endregion

        #region GetList
        // ============================================================================
        // Get all expenses list
        // NOTE: VERY IMPORTANT... budget amount is the negative of the expense amount
        // Reasoning: an expense of $15 is -$15 from your bank account.
        // ============================================================================
        /// <summary>
        /// Gets a list of budget items from the database between a start and end date filter. 
        /// </summary>
        /// <example>
        /// For all examples below, assume the budget database contains the following rows & columns:
        ///
        /// <code>
        /// Cat_ID     Expense_ID Date                   Description         Cost           Balance
        /// 10         1          2018-01-10 12:00:00 AM hat(on credit)      -10            -10
        /// 9          2          2018-01-11 12:00:00 AM hat                  10             0
        /// 10         3          2019-01-10 12:00:00 AM scarf(on credit)    -15            -15
        /// 9          4          2020-01-10 12:00:00 AM scarf                15             0
        /// 14         5          2020-01-11 12:00:00 AM McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM Wendys               -25            -70
        /// 14         10         2020-02-01 12:00:00 AM Pizza                -33.33         -103.33
        /// 9          13         2020-02-10 12:00:00 AM mittens              15             -88.33
        /// 9          12         2020-02-25 12:00:00 AM Hat                  25             -63.33
        /// 14         11         2020-02-27 12:00:00 AM Pizza                -33.33         -96.66
        /// 14         9          2020-07-11 12:00:00 AM Cafeteria            -11.11         -107.77
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items from the database.</b>
        ///
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        ///
        /// // Initialize a list of all budget items
        /// List<BudgetItem> budgetItems = budget.GetBudgetItems(null, null, false, 0);
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        /// Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        /// 
        /// // Print all budget items
        /// foreach (BudgetItem budgetItem in budgetItems)
        /// {
        ///     Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        /// }
        ///
        /// ]]>
        /// </code>
        ///
        /// Sample output:
        /// <code>
        /// Cat_ID     Expense_ID Date                   Description         Cost           Balance
        /// 10         1          2018-01-10 12:00:00 AM hat(on credit)      -10            -10
        /// 9          2          2018-01-11 12:00:00 AM hat                  10             0
        /// 10         3          2019-01-10 12:00:00 AM scarf(on credit)    -15            -15
        /// 9          4          2020-01-10 12:00:00 AM scarf                15             0
        /// 14         5          2020-01-11 12:00:00 AM McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM Wendys               -25            -70
        /// 14         10         2020-02-01 12:00:00 AM Pizza                -33.33         -103.33
        /// 9          13         2020-02-10 12:00:00 AM mittens              15             -88.33
        /// 9          12         2020-02-25 12:00:00 AM Hat                  25             -63.33
        /// 14         11         2020-02-27 12:00:00 AM Pizza                -33.33         -96.66
        /// 14         9          2020-07-11 12:00:00 AM Cafeteria            -11.11         -107.77
        /// </code>
        /// 
        /// <b>Getting a list of budget items within a date from the database.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// DateTime startDate = new DateTime(2019, 1, 10);
        /// DateTime endDate = new DateTime(2020, 2, 10);
        ///
        /// // Initialize a list of all budget items
        /// List<BudgetItem> budgetItems = budget.GetBudgetItems(startDate, endDate, false, 0);
        /// 
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        /// Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        /// 
        /// // Print all budget items
        /// foreach (BudgetItem budgetItem in budgetItems)
        /// {
        ///     Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items with a filter flag ON from the database.</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        ///
        /// // Initialize a list of all budget items
        /// List<BudgetItem> budgetItems = budget.GetBudgetItems(null, null, true, 9);
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        /// Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        /// 
        /// // Print all budget items
        /// foreach (BudgetItem budgetItem in budgetItems)
        /// {
        ///     Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             10
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             25
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             40
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             65
        /// </code>
        /// 
        /// <b>Getting a list of a budget items within a date with a filter flag ON from the database.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// DateTime startDate = new DateTime(2019, 1, 10);
        /// DateTime endDate = new DateTime(2020, 2, 10);
        ///
        /// // Get a list of all budget items
        /// List<BudgetItem> budgetItems = budget.GetBudgetItems(startDate, endDate, true, 14);
        /// 
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        /// Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        /// 
        /// // Print all budget items
        /// foreach (BudgetItem budgetItem in budgetItems)
        /// {
        ///     Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// </code>
        /// </example>
        /// <param name="Start">The start date of the budget items to be looked for.</param>
        /// <param name="End">The end date of the budget items to be looked for.</param>
        /// <param name="FilterFlag">If true, looks for budget items with one specific category. If false, looks for all budget items.</param>
        /// <param name="CategoryID">The category ID to be searched with if the filter flag is on.</param>
        /// <returns>A list of budget items between the start and end date.</returns>
        public List<BudgetItem> GetBudgetItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = "SELECT E.CategoryId, E.Id, E.Date, C.Description, E.Description, E.Amount FROM expenses E JOIN categories C ON E.CategoryId = C.Id WHERE E.Date > @start AND E.Date < @end ORDER BY E.Date";
            cmd.Parameters.AddWithValue("@start", Start);
            cmd.Parameters.AddWithValue("@end", End);
            SQLiteDataReader reader = cmd.ExecuteReader();

            const int CATEGORYID_ID = 0;
            const int EXPENSEID_ID = 1;
            const int DATE_ID = 2;
            const int CATEGORYDES_ID = 3;
            const int EXPENSEDES_ID = 4;
            const int AMOUNT_ID = 5;

            // ------------------------------------------------------------------------
            // create a BudgetItem list with totals,
            // ------------------------------------------------------------------------
            List<BudgetItem> items = new List<BudgetItem>();
            Double total = 0;

            while (reader.Read())
            {
                // filter out unwanted categories if filter flag is on
                if (FilterFlag && CategoryID != reader.GetInt32(CATEGORYID_ID))
                {
                    continue;
                }

                // keep track of running totals
                total = total + reader.GetDouble(AMOUNT_ID);
                items.Add(new BudgetItem
                {
                    CategoryID = reader.GetInt32(CATEGORYID_ID),
                    ExpenseID = reader.GetInt32(EXPENSEID_ID),
                    ShortDescription = reader.GetString(EXPENSEDES_ID),
                    Date = reader.GetDateTime(DATE_ID),
                    Amount = reader.GetDouble(AMOUNT_ID),
                    Category = reader.GetString(CATEGORYDES_ID),
                    Balance = total
                });
            }

            return items;
        }

        // ============================================================================
        // Group all expenses month by month (sorted by year/month)
        // returns a list of BudgetItemsByMonth which is 
        // "year/month", list of budget items, and total for that month
        // ============================================================================
        /// <summary>
        /// Gets a list of budget items using the data from the database between a start and end date using the categories and expenses list. Groups the list by year and month.
        /// </summary>
        /// <example>
        /// For all examples below, assume the database contains the following rows & columns:
        ///
        /// <code>
        /// Month of 2018/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         1          2018-01-10 12:00:00 AM    hat (on credit)      -10            -10
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             0
        /// Total: $0.00
        ///
        /// Month of 2019/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// Total: -$15.00
        ///
        /// Month of 2020/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// Total: -$55.00
        ///
        /// Month of 2020/02
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             -63.33
        /// 14         11         2020-02-27 12:00:00 AM    Pizza                -33.33         -96.66
        /// Total: -$26.66
        ///
        /// Month of 2020/07
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         9          2020-07-11 12:00:00 AM    Cafeteria            -11.11         -107.77
        /// Total: -$11.11
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items by month.</b>
        ///
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// // Initialize a list of all budget items by month
        /// List<BudgetItemsByMonth> budgetItemsByMonth = budget.GetBudgetItemsByMonth(null, null, false, 0);
        /// 
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print all budget items
        /// foreach (BudgetItemsByMonth budgetItemMonth in budgetItemsByMonth)
        /// {
        ///     Console.WriteLine($"Month of {budgetItemMonth.Month}\n");
        ///     Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///     
        ///     foreach (BudgetItem budgetItem in budgetItemMonth.Details)
        ///     {
        ///         Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        ///     }
        ///     
        ///     Console.WriteLine($"Total: {budgetItemMonth.Total:C}\n");
        /// }
        /// ]]>
        /// </code>
        ///
        /// Sample output:
        /// <code>
        /// Month of 2018/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         1          2018-01-10 12:00:00 AM    hat (on credit)      -10            -10
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             0
        /// Total: $0.00
        ///
        /// Month of 2019/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// Total: -$15.00
        ///
        /// Month of 2020/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// Total: -$55.00
        ///
        /// Month of 2020/02
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             -63.33
        /// 14         11         2020-02-27 12:00:00 AM    Pizza                -33.33         -96.66
        /// Total: -$26.66
        ///
        /// Month of 2020/07
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         9          2020-07-11 12:00:00 AM    Cafeteria            -11.11         -107.77
        /// Total: -$11.11
        /// </code>
        /// 
        /// <b>Getting a list of budget items by month within a date.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// DateTime startDate = new DateTime(2019, 1, 10);
        /// DateTime endDate = new DateTime(2020, 2, 10);
        ///
        /// // Get a list of all budget items by month
        /// List<BudgetItemsByMonth> budgetItemsByMonth = budget.GetBudgetItemsByMonth(startDate, endDate, false, 0);
        /// 
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print all budget items
        /// foreach (BudgetItemsByMonth budgetItemMonth in budgetItemsByMonth)
        /// {
        ///     Console.WriteLine($"Month of {budgetItemMonth.Month}\n");
        ///     Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///     
        ///     foreach (BudgetItem budgetItem in budgetItemMonth.Details)
        ///     {
        ///         Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        ///     }
        ///     
        ///     Console.WriteLine($"Total: {budgetItemMonth.Total:C}\n");
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month of 2019/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// Total: -$15.00
        ///
        /// Month of 2020/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// Total: -$55.00
        ///
        /// Month of 2020/02
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// Total: -$18.33
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items by month with a filter flag ON from the database.</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// // Initialize a list of all budget items by month
        /// List<BudgetItemsByMonth> budgetItemsByMonth = budget.GetBudgetItemsByMonth(null, null, true, 9);
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print all budget items
        /// foreach (BudgetItemsByMonth budgetItemMonth in budgetItemsByMonth)
        /// {
        ///     Console.WriteLine($"Month of {budgetItemMonth.Month}\n");
        ///     Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///     
        ///     foreach (BudgetItem budgetItem in budgetItemMonth.Details)
        ///     {
        ///         Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        ///     }
        ///     
        ///     Console.WriteLine($"Total: {budgetItemMonth.Total:C}\n");
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month of 2018/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             10
        /// Total: $10.00
        ///
        /// Month of 2020/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             25
        /// Total: $15.00
        ///
        /// Month of 2020/02
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             40
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             65
        /// Total: $40.00
        /// </code>
        /// 
        /// <b>Getting a list of a budget items by month within a date with a filter flag ON from the database.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// DateTime startDate = new DateTime(2019, 1, 10);
        /// DateTime endDate = new DateTime(2020, 2, 10);
        ///
        /// // Get a list of all budget items by month
        /// List<BudgetItemsByMonth> budgetItemsByMonth = budget.GetBudgetItemsByMonth(startDate, endDate, true, 14);
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print all budget items
        /// foreach (BudgetItemsByMonth budgetItemMonth in budgetItemsByMonth)
        /// {
        ///     Console.WriteLine($"Month of {budgetItemMonth.Month}\n");
        ///     Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///     
        ///     foreach (BudgetItem budgetItem in budgetItemMonth.Details)
        ///     {
        ///         Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        ///     }
        ///     
        ///     Console.WriteLine($"Total: {budgetItemMonth.Total:C}\n");
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month of 2020/01
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// Total: -$70.00
        ///
        /// Month of 2020/02
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// Total: -$33.33
        /// </code>
        /// </example>
        /// <param name="Start">The start date of the budget items to be looked for.</param>
        /// <param name="End">The end date of the budget items to be looked for.</param>
        /// <param name="FilterFlag">If true, looks for budget items with one specific category. If false, looks for all budget items.</param>
        /// <param name="CategoryID">The category ID to be searched with if the filter flag is on.</param>
        /// <returns>A list of budget items between the start and end date, grouped by month.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the given date is out of range, or incorrect.</exception>
        /// <exception cref="FormatException">Thrown when the ToString format is invalid.</exception>
        public List<BudgetItemsByMonth> GetBudgetItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);
            List<DateTime> dates = new List<DateTime>();

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = "SELECT substr(Date, 1, 7), CategoryId FROM expenses WHERE Date > @start AND Date < @end";
            cmd.Parameters.AddWithValue("@start", Start);
            cmd.Parameters.AddWithValue("@end", End);
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (FilterFlag && CategoryID != reader.GetInt32(1))
                {
                    continue;
                }
                DateTime date = DateTime.Parse(reader.GetString(0));

                if (!dates.Contains(date))
                    dates.Add(date);
            }

            reader.Close();


            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<BudgetItemsByMonth>();

            foreach (DateTime date in dates)
            {

                DateTime startDate = new DateTime(date.Year, date.Month, 1);
                int lastDay = DateTime.DaysInMonth(date.Year, date.Month);
                DateTime endDate = new DateTime(date.Year, date.Month, lastDay);

                List<BudgetItem> details = GetBudgetItems(startDate, endDate, FilterFlag, CategoryID);

                double total = 0;
                foreach (BudgetItem item in details)
                {
                    total += item.Amount;
                }

                summary.Add(new BudgetItemsByMonth
                {
                    Month = startDate.ToString("yyyy-MM").Replace('-', '/'),
                    Details = details,
                    Total = total
                });
            }

            return summary;
        }

        // ============================================================================
        // Group all expenses by category (ordered by category name)
        // ============================================================================
        /// <summary>
        /// Gets a list of budget items from the database between a start and end date. Groups the list by category.
        /// </summary>
        /// <example>
        /// For all examples below, assume the database contains the following rows % columns:
        ///
        /// <code>
        /// Category: Clothes
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         1          2018-01-10 12:00:00 AM    hat (on credit)      -10            -10
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// Total: -$25.00
        ///
        /// Category: Credit Card
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             0
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             -63.33
        /// Total: $65.00
        ///
        /// Category: Eating Out
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// 14         11         2020-02-27 12:00:00 AM    Pizza                -33.33         -96.66
        /// 14         9          2020-07-11 12:00:00 AM    Cafeteria            -11.11         -107.77
        /// Total: -$147.77
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items by category.</b>
        ///
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        ///
        /// // Get a list of all budget items by category
        /// List<BudgetItemsByCategory> budgetItemsByCategory = budget.GetBudgetItemsByCategory(null, null, false, 0);
        ///
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print all budget items
        /// foreach (BudgetItemsByCategory budgetItemCategory in budgetItemsByCategory)
        /// {
        ///     Console.WriteLine($"Category: {budgetItemCategory.Category}\n");
        ///     Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///     foreach (BudgetItem budgetItem in budgetItemCategory.Details)
        ///     {
        ///         Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        ///     }
        ///     Console.WriteLine($"Total: {budgetItemCategory.Total:C}\n");
        /// }
        /// ]]>
        /// </code>
        ///
        /// Sample output:
        /// <code>
        /// Category: Clothes
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         1          2018-01-10 12:00:00 AM    hat (on credit)      -10            -10
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// Total: -$25.00
        ///
        /// Category: Credit Card
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             0
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             -63.33
        /// Total: $65.00
        ///
        /// Category: Eating Out
        ///
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// 14         11         2020-02-27 12:00:00 AM    Pizza                -33.33         -96.66
        /// 14         9          2020-07-11 12:00:00 AM    Cafeteria            -11.11         -107.77
        /// Total: -$147.77
        /// </code>
        /// 
        /// <b>Getting a list of budget items by category within a date from the database.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// DateTime startDate = new DateTime(2019, 1, 10);
        /// DateTime endDate = new DateTime(2020, 2, 10);
        ///
        /// // Initialize a list of all budget items by category
        /// List<BudgetItemsByCategory> budgetItemsByCategory = budget.GetBudgetItemsByCategory(startDate, endDate, false, 0);
        /// 
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print all budget items
        /// foreach (BudgetItemsByCategory budgetItemCategory in budgetItemsByCategory)
        /// {
        ///     Console.WriteLine($"Category: {budgetItemCategory.Category}\n");
        ///     Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///     foreach (BudgetItem budgetItem in budgetItemCategory.Details)
        ///     {
        ///         Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        ///     }
        ///     Console.WriteLine($"Total: {budgetItemCategory.Total:C}\n");
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Category: Clothes
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// Total: -$15.00
        /// 
        /// Category: Credit Card
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// Total: $30.00
        /// 
        /// Category: Eating Out
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// Total: -$103.33
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items by category with a filter flag ON from the database.</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// // Initialize a list of all budget items by category
        /// List<BudgetItemsByCategory> budgetItemsByCategory = budget.GetBudgetItemsByCategory(null, null, true, 9);
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print all budget items
        /// foreach (BudgetItemsByCategory budgetItemCategory in budgetItemsByCategory)
        /// {
        ///     Console.WriteLine($"Category: {budgetItemCategory.Category}\n");
        ///     Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///     foreach (BudgetItem budgetItem in budgetItemCategory.Details)
        ///     {
        ///         Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        ///     }
        ///     Console.WriteLine($"Total: {budgetItemCategory.Total:C}\n");
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Category: Credit Card
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             10
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             25
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             40
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             65
        /// Total: $65.00
        /// </code>
        /// 
        /// <b>Getting a list of a budget items by category within a date with a filter flag ON.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// DateTime startDate = new DateTime(2019, 1, 10);
        /// DateTime endDate = new DateTime(2020, 2, 10);
        ///
        /// // Initialize a list of all budget items by category
        /// List<BudgetItemsByCategory> budgetItemsByCategory = budget.GetBudgetItemsByCategory(startDate, endDate, true, 14);
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print all budget items
        /// foreach (BudgetItemsByCategory budgetItemCategory in budgetItemsByCategory)
        /// {
        ///     Console.WriteLine($"Category: {budgetItemCategory.Category}\n");
        ///     Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///     foreach (BudgetItem budgetItem in budgetItemCategory.Details)
        ///     {
        ///         Console.WriteLine(string.Format(Format, budgetItem.CategoryID, budgetItem.ExpenseID, budgetItem.Date, budgetItem.ShortDescription, budgetItem.Amount, budgetItem.Balance));
        ///     }
        ///     Console.WriteLine($"Total: {budgetItemCategory.Total:C}\n");
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Category: Eating Out
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// Total: -$103.33
        /// </code>
        /// </example>
        /// <param name="Start">The start date of the budget items to be looked for.</param>
        /// <param name="End">The end date of the budget items to be looked for.</param>
        /// <param name="FilterFlag">If true, looks for budget items with one specific category. If false, looks for all budget items.</param>
        /// <param name="CategoryID">The category ID to be searched with if the filter flag is on.</param>
        /// <returns>A list of budget items between the start and end date, grouped by category.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the key condition to group the budget items, order the grouped categories or query is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a given date is out of range or incorrect.</exception>
        public List<BudgetItemsByCategory> GetBudgetItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);
            List<string> categories = new List<string>();
            List<int> categoryIds = new List<int>();

            // -----------------------------------------------------------------------
            // Group by Category
            // -----------------------------------------------------------------------
            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = "SELECT DISTINCT C.Description, C.Id FROM expenses E JOIN categories C ON C.Id = E.CategoryId WHERE Date > @start AND Date < @end";
            cmd.Parameters.AddWithValue("@start", Start);
            cmd.Parameters.AddWithValue("@end", End);
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (FilterFlag && CategoryID != reader.GetInt32(1))
                {
                    continue;
                }

                categories.Add(reader.GetString(0));
                categoryIds.Add(reader.GetInt32(1));
            }

            reader.Close();

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<BudgetItemsByCategory>();

            for (int i = 0; i < categoryIds.Count(); i++)
            {
                // -----------------------------------------------------------------------
                // get all items first
                // -----------------------------------------------------------------------
                List<BudgetItem> details = GetBudgetItems(Start, End, true, categoryIds[i]);

                double total = 0;
                foreach (BudgetItem item in details)
                {
                    total += item.Amount;
                }

                // Add new BudgetItemsByCategory to our list
                summary.Add(new BudgetItemsByCategory
                {
                    Category = categories[i],
                    Details = details,
                    Total = total
                });
            }

            return summary;
        }


        // ============================================================================
        // Group all events by category and Month
        // creates a list of Dictionary objects (which are objects that contain key value pairs).
        // The list of Dictionary objects includes:
        //          one dictionary object per month with expenses,
        //          and one dictionary object for the category totals
        // 
        // Each per month dictionary object has the following key value pairs:
        //           "Month", <the year/month for that month as a string>
        //           "Total", <the total amount for that month as a double>
        //            and for each category for which there is an expense in the month:
        //             "items:category", a List<BudgetItem> of all items in that category for the month
        //             "category", the total amount for that category for this month
        //
        // The one dictionary for the category totals has the following key value pairs:
        //             "Month", the string "TOTALS"
        //             for each category for which there is an expense in ANY month:
        //             "category", the total for that category for all the months
        // ============================================================================
        /// <summary>
        /// Gets a list of dictionaries of objects. The list of dictionaries contains dictionary objects with the month, the expenses per category for month, the total for the category for that month and the total for the month.
        /// The last element will include a total of the categories for the entire duration of the start and end date.
        /// </summary>
        /// <example>
        /// For all examples below, assume the database contains the following data:
        ///
        /// <code>
        /// Month of 2018/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         1          2018-01-10 12:00:00 AM    hat (on credit)      -10            -10
        /// Total for Clothes: -$10.00
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             0
        /// Total for Credit Card: $10.00
        /// 
        /// -------------------------------
        /// Total for the month: $0.00
        /// 
        /// Month of 2019/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// Total for Clothes: -$15.00
        /// 
        /// -------------------------------
        /// Total for the month: -$15.00
        /// 
        /// Month of 2020/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// Total for Credit Card: $15.00
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// Total for Eating Out: -$70.00
        /// 
        /// -------------------------------
        /// Total for the month: -$55.00
        /// 
        /// Month of 2020/02
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             -63.33
        /// Total for Credit Card: $40.00
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// 14         11         2020-02-27 12:00:00 AM    Pizza                -33.33         -96.66
        /// Total for Eating Out: -$66.66
        /// 
        /// -------------------------------
        /// Total for the month: -$26.66
        /// 
        /// Month of 2020/07
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         9          2020-07-11 12:00:00 AM    Cafeteria            -11.11         -107.77
        /// Total for Eating Out: -$11.11
        /// 
        /// -------------------------------
        /// Total for the month: -$11.11
        /// 
        /// Totals for all categories
        /// Credit Card: $65.00
        /// Clothes: -$25.00
        /// Eating Out: -$147.77
        /// </code>
        /// 
        /// <b>Getting a list of dictionary of ALL budget items by category and month using GetBudgetItemsByMonth()</b>
        ///
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        ///
        /// // Get a list of dictionaries
        /// List<Dictionary<string, object>> budgetItemsByMonthCategory = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 0);
        ///
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print the list of dictionaries
        /// foreach (Dictionary<string, object> budgetItemByMonth in budgetItemsByMonthCategory)
        /// {
        ///     string monthName = budgetItemByMonth["Month"].ToString();
        ///     if (monthName == "TOTALS")
        ///     {
        ///         Console.WriteLine("Totals for all categories");
        ///         foreach (KeyValuePair<string, object> entry in budgetItemByMonth)
        ///         {
        ///             if (entry.Value is double total)
        ///                 Console.WriteLine($"{entry.Key}: {total:C}");
        ///         }
        /// 
        ///     }
        ///     else
        ///     {
        ///         Console.WriteLine($"Month of {monthName}");
        /// 
        ///         foreach (KeyValuePair<string, object> entry in budgetItemByMonth)
        ///         {
        ///             string categoryName = "";
        ///             if (entry.Value is List<BudgetItem> details)
        ///             {
        ///                 Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///                 foreach (BudgetItem item in details)
        ///                 {
        ///                     categoryName = item.Category;
        ///                     Console.WriteLine(string.Format(Format, item.CategoryID, item.ExpenseID, item.Date, item.ShortDescription, item.Amount, item.Balance));
        ///                 }
        /// 
        ///                 Console.WriteLine($"Total for {categoryName}: {budgetItemByMonth[categoryName]:C}\n");
        ///             }
        /// 
        ///         }
        ///         Console.WriteLine("-------------------------------");
        ///         Console.WriteLine($"Total for the month: {budgetItemByMonth["Total"]:C}\n");
        ///     }
        /// 
        /// }
        /// ]]>
        /// </code>
        ///
        /// Sample output:
        /// <code>
        /// Month of 2018/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         1          2018-01-10 12:00:00 AM    hat (on credit)      -10            -10
        /// Total for Clothes: -$10.00
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             0
        /// Total for Credit Card: $10.00
        /// 
        /// -------------------------------
        /// Total for the month: $0.00
        /// 
        /// Month of 2019/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// Total for Clothes: -$15.00
        /// 
        /// -------------------------------
        /// Total for the month: -$15.00
        /// 
        /// Month of 2020/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// Total for Credit Card: $15.00
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// Total for Eating Out: -$70.00
        /// 
        /// -------------------------------
        /// Total for the month: -$55.00
        /// 
        /// Month of 2020/02
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             -63.33
        /// Total for Credit Card: $40.00
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// 14         11         2020-02-27 12:00:00 AM    Pizza                -33.33         -96.66
        /// Total for Eating Out: -$66.66
        /// 
        /// -------------------------------
        /// Total for the month: -$26.66
        /// 
        /// Month of 2020/07
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         9          2020-07-11 12:00:00 AM    Cafeteria            -11.11         -107.77
        /// Total for Eating Out: -$11.11
        /// 
        /// -------------------------------
        /// Total for the month: -$11.11
        /// 
        /// Totals for all categories
        /// Credit Card: $65.00
        /// Clothes: -$25.00
        /// Eating Out: -$147.77
        /// </code>
        /// 
        /// <b>Getting a list of dictionary of budget items by category and month within a date.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// DateTime startDate = new DateTime(2019, 1, 10);
        /// DateTime endDate = new DateTime(2020, 2, 10);
        ///
        /// // Get a list of dictionaries
        /// List<Dictionary<string, object>> budgetItemsByMonthCategory = homeBudget.GetBudgetDictionaryByCategoryAndMonth(startDate, endDate, false, 0);
        ///
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print the list of dictionaries
        /// foreach (Dictionary<string, object> budgetItemByMonth in budgetItemsByMonthCategory)
        /// {
        ///     string monthName = budgetItemByMonth["Month"].ToString();
        ///     if (monthName == "TOTALS")
        ///     {
        ///         Console.WriteLine("Totals for all categories");
        ///         foreach (KeyValuePair<string, object> entry in budgetItemByMonth)
        ///         {
        ///             if (entry.Value is double total)
        ///                 Console.WriteLine($"{entry.Key}: {total:C}");
        ///         }
        /// 
        ///     }
        ///     else
        ///     {
        ///         Console.WriteLine($"Month of {monthName}");
        /// 
        ///         foreach (KeyValuePair<string, object> entry in budgetItemByMonth)
        ///         {
        ///             string categoryName = "";
        ///             if (entry.Value is List<BudgetItem> details)
        ///             {
        ///                 Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///                 foreach (BudgetItem item in details)
        ///                 {
        ///                     categoryName = item.Category;
        ///                     Console.WriteLine(string.Format(Format, item.CategoryID, item.ExpenseID, item.Date, item.ShortDescription, item.Amount, item.Balance));
        ///                 }
        /// 
        ///                 Console.WriteLine($"Total for {categoryName}: {budgetItemByMonth[categoryName]:C}\n");
        ///             }
        /// 
        ///         }
        ///         Console.WriteLine("-------------------------------");
        ///         Console.WriteLine($"Total for the month: {budgetItemByMonth["Total"]:C}\n");
        ///     }
        /// 
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month of 2019/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 10         3          2019-01-10 12:00:00 AM    scarf (on credit)    -15            -15
        /// Total for Clothes: -$15.00
        /// 
        /// -------------------------------
        /// Total for the month: -$15.00
        /// 
        /// Month of 2020/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             0
        /// Total for Credit Card: $15.00
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// Total for Eating Out: -$70.00
        /// 
        /// -------------------------------
        /// Total for the month: -$55.00
        /// 
        /// Month of 2020/02
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             -88.33
        /// Total for Credit Card: $15.00
        /// 
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// Total for Eating Out: -$33.33
        /// 
        /// -------------------------------
        /// Total for the month: -$18.33
        /// 
        /// Totals for all categories
        /// Credit Card: $30.00
        /// Clothes: -$15.00
        /// Eating Out: -$103.33
        /// </code>
        /// 
        /// <b>Getting a list of dictionary of ALL budget items by category and month with a filter flag ON.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        ///
        /// // Get a list of dictionaries
        /// List<Dictionary<string, object>> budgetItemsByMonthCategory = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, true, 9);
        ///
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print the list of dictionaries
        /// foreach (Dictionary<string, object> budgetItemByMonth in budgetItemsByMonthCategory)
        /// {
        ///     string monthName = budgetItemByMonth["Month"].ToString();
        ///     if (monthName == "TOTALS")
        ///     {
        ///         Console.WriteLine("Totals for all categories");
        ///         foreach (KeyValuePair<string, object> entry in budgetItemByMonth)
        ///         {
        ///             if (entry.Value is double total)
        ///                 Console.WriteLine($"{entry.Key}: {total:C}");
        ///         }
        /// 
        ///     }
        ///     else
        ///     {
        ///         Console.WriteLine($"Month of {monthName}");
        /// 
        ///         foreach (KeyValuePair<string, object> entry in budgetItemByMonth)
        ///         {
        ///             string categoryName = "";
        ///             if (entry.Value is List<BudgetItem> details)
        ///             {
        ///                 Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///                 foreach (BudgetItem item in details)
        ///                 {
        ///                     categoryName = item.Category;
        ///                     Console.WriteLine(string.Format(Format, item.CategoryID, item.ExpenseID, item.Date, item.ShortDescription, item.Amount, item.Balance));
        ///                 }
        /// 
        ///                 Console.WriteLine($"Total for {categoryName}: {budgetItemByMonth[categoryName]:C}\n");
        ///             }
        /// 
        ///         }
        ///         Console.WriteLine("-------------------------------");
        ///         Console.WriteLine($"Total for the month: {budgetItemByMonth["Total"]:C}\n");
        ///     }
        /// 
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month of 2018/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          2          2018-01-11 12:00:00 AM    hat                  10             10
        /// Total for Credit Card: $10.00
        /// 
        /// -------------------------------
        /// Total for the month: $10.00
        /// 
        /// Month of 2020/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          4          2020-01-10 12:00:00 AM    scarf                15             25
        /// Total for Credit Card: $15.00
        /// 
        /// -------------------------------
        /// Total for the month: $15.00
        /// 
        /// Month of 2020/02
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 9          13         2020-02-10 12:00:00 AM    mittens              15             40
        /// 9          12         2020-02-25 12:00:00 AM    Hat                  25             65
        /// Total for Credit Card: $40.00
        /// 
        /// -------------------------------
        /// Total for the month: $40.00
        /// 
        /// Totals for all categories
        /// Credit Card: $65.00
        /// </code>
        /// 
        /// <b>Getting a list of dictionary of budget items by category and month within a date with a filter flag ON.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(String databaseFile, bool newDB);
        /// 
        /// DateTime startDate = new DateTime(2019, 1, 10);
        /// DateTime endDate = new DateTime(2020, 2, 10);
        /// 
        /// // Get a list of dictionaries
        /// List<Dictionary<string, object>> budgetItemsByMonthCategory = homeBudget.GetBudgetDictionaryByCategoryAndMonth(startDate, endDate, true, 14);
        ///
        /// const string Format = "{0,-10} {1,-10} {2,-25} {3,-20} {4,-14} {5,-15}";
        ///
        /// // Print the list of dictionaries
        /// foreach (Dictionary<string, object> budgetItemByMonth in budgetItemsByMonthCategory)
        /// {
        ///     string monthName = budgetItemByMonth["Month"].ToString();
        ///     if (monthName == "TOTALS")
        ///     {
        ///         Console.WriteLine("Totals for all categories");
        ///         foreach (KeyValuePair<string, object> entry in budgetItemByMonth)
        ///         {
        ///             if (entry.Value is double total)
        ///                 Console.WriteLine($"{entry.Key}: {total:C}");
        ///         }
        /// 
        ///     }
        ///     else
        ///     {
        ///         Console.WriteLine($"Month of {monthName}");
        /// 
        ///         foreach (KeyValuePair<string, object> entry in budgetItemByMonth)
        ///         {
        ///             string categoryName = "";
        ///             if (entry.Value is List<BudgetItem> details)
        ///             {
        ///                 Console.WriteLine(string.Format(Format, "Cat_ID", "Expense_ID", "Date", "Description", "Cost", "Balance"));
        ///                 foreach (BudgetItem item in details)
        ///                 {
        ///                     categoryName = item.Category;
        ///                     Console.WriteLine(string.Format(Format, item.CategoryID, item.ExpenseID, item.Date, item.ShortDescription, item.Amount, item.Balance));
        ///                 }
        /// 
        ///                 Console.WriteLine($"Total for {categoryName}: {budgetItemByMonth[categoryName]:C}\n");
        ///             }
        /// 
        ///         }
        ///         Console.WriteLine("-------------------------------");
        ///         Console.WriteLine($"Total for the month: {budgetItemByMonth["Total"]:C}\n");
        ///     }
        /// 
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Month of 2020/01
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         5          2020-01-11 12:00:00 AM    McDonalds            -45            -45
        /// 14         7          2020-01-12 12:00:00 AM    Wendys               -25            -70
        /// Total for Eating Out: -$70.00
        /// 
        /// -------------------------------
        /// Total for the month: -$70.00
        /// 
        /// Month of 2020/02
        /// Cat_ID     Expense_ID Date                      Description          Cost           Balance
        /// 14         10         2020-02-01 12:00:00 AM    Pizza                -33.33         -103.33
        /// Total for Eating Out: -$33.33
        /// 
        /// -------------------------------
        /// Total for the month: -$33.33
        /// 
        /// Totals for all categories
        /// Eating Out: -$103.33
        /// </code>
        /// </example>
        /// <remarks>Adding the category totals together for the month will give the total of the expenses of that month.</remarks>
        /// <param name="Start">The start date of the budget items to be looked for.</param>
        /// <param name="End">The end date of the budget items to be looked for.</param>
        /// <param name="FilterFlag">If true, looks for budget items with one specific category. If false, looks for all budget items.</param>
        /// <param name="CategoryID">The category ID to be searched with if the filter flag is on.</param>
        /// <returns>A list of dictionary objects. Contains a dictionary object with expenses per month, and a dictionary object for the total per category.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the key condition to group the budget items, the categories or order the query is null, or the key to find a value or to add a key and value is null.</exception>
        /// <exception cref="FormatException">Thrown when the ToString format is invalid.</exception>
        /// <exception cref="ArgumentException">Thrown when the key already exists in the dictionary</exception>
        public List<Dictionary<string, object>> GetBudgetDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<BudgetItemsByMonth> GroupedByMonth = GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalsPerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["Total"] = MonthGroup.Total;

                // break up the month details into categories
                var GroupedByCategory = MonthGroup.Details.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of details
                    double total = 0;
                    var details = new List<BudgetItem>();

                    foreach (var item in CategoryGroup)
                    {
                        total = total + item.Amount;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["details:" + CategoryGroup.Key] = details;
                    record[CategoryGroup.Key] = total;

                    // keep track of totals for each category
                    if (totalsPerCategory.TryGetValue(CategoryGroup.Key, out Double CurrentCatTotal))
                    {
                        totalsPerCategory[CategoryGroup.Key] = CurrentCatTotal + total;
                    }
                    else
                    {
                        totalsPerCategory[CategoryGroup.Key] = total;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalsPerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }




        #endregion GetList

    }
}
