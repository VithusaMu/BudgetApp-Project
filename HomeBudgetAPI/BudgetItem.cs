using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace HomeBudgetAPI
{
    // ====================================================================
    // CLASS: BudgetItem
    //        A single budget item, includes Category and Expense
    // ====================================================================
    /// <summary>
    /// A single bought item, that includes a category, expense, date, amount, description, balance, and amount
    /// </summary>
    public class BudgetItem
    {
        /// <summary>
        /// Gets and sets the category ID of the budget item.
        /// </summary>
        /// <value>The ID of the category.</value>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets and sets the expense ID of the budget item.
        /// </summary>
        /// <value>The ID of the expense.</value>
        public int ExpenseID { get; set; }

        /// <summary>
        /// Gets and sets the date of the budget item.
        /// </summary>
        /// <value>The date of when the item was purchased.</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets and sets the category string of the item.
        /// </summary>
        /// <value>The category name of the item.</value>
        public String Category { get; set; }

        /// <summary>
        /// Gets and sets a short description of the item.
        /// </summary>
        /// <value>A brief description of the item.</value>
        public String ShortDescription { get; set; }

        /// <summary>
        /// Gets and sets an amount for the budget item.
        /// </summary>
        /// <value>The cost of the item.</value>
        public Double Amount { get; set; }

        /// <summary>
        /// Gets and sets the balance left.
        /// </summary>
        /// <value>The balance left.</value>
        public Double Balance { get; set; }

    }

    /// <summary>
    /// Multiple budgets items from a month, that includes details, which is a list of BudgetItems, and total
    /// </summary>
    public class BudgetItemsByMonth
    {
        /// <summary>
        /// Gets and sets the month string for the items.
        /// </summary>
        /// <value>The month name for the collection of budget items.</value>
        public String Month { get; set; }

        /// <summary>
        /// Gets and sets the details, which is a list of budget items for the month.
        /// </summary>
        /// <value>A collection of budget items.</value>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// Gets and sets the total of the budget items by the month.
        /// </summary>
        /// <value>The total of the collection of budget items by the month.</value>
        public Double Total { get; set; }
    }

    /// <summary>
    /// Multiple budgets items from a category, that includes details, which is a list of BudgetItems and total
    /// </summary>
    public class BudgetItemsByCategory
    {
        /// <summary>
        /// Gets and sets the category string for the items.
        /// </summary>
        /// <value>The category name for the collection of budget items.</value>
        public String Category { get; set; }

        /// <summary>
        /// Gets and sets the details, which is a list of budget items for the category.
        /// </summary>
        /// <value>A collection of budget items.</value>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// Gets and sets the total of the budget items by the category.
        /// </summary>
        /// <value>The total of the collection of budget items by the category.</value>
        public Double Total { get; set; }

    }


}
