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
    // CLASS: Expense
    //        - An individual expens for budget program
    // ====================================================================
    /// <summary>
    /// An individual cost required for a budget item
    /// </summary>
    public class Expense
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Gets the expense ID.
        /// </summary>
        /// <value>The ID of the expense.</value>
        public int Id { get; }

        /// <summary>
        /// Gets the date of the expense.
        /// </summary>
        /// <value>The date of when the expense was purchased.</value>
        public DateTime Date { get;  }

        /// <summary>
        /// Gets a cost for the expense.
        /// </summary>
        /// <value>The cost of the expense.</value>
        public Double Amount { get; }

        /// <summary>
        /// Gets the description of the expense.
        /// </summary>
        /// <value>A description of the expense.</value>
        public String Description { get; }

        /// <summary>
        /// Gets the category of the expense.
        /// </summary>
        /// <value>The category ID of the expense.</value>
        public int Category { get; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the expense category exists in the
        //        categories object
        // ====================================================================
        /// <summary>
        /// Creates an expense with a given id, date, category, amount and description.
        /// </summary>
        /// <param name="id">The id of the expense.</param>
        /// <param name="date">The date of the expense.</param>
        /// <param name="category">The category of the expense.</param>
        /// <param name="amount">The amount of the expense.</param>
        /// <param name="description">The description of the expense.</param>
        public Expense(int id, DateTime date, int category, Double amount, String description)
        {
            this.Id = id;
            this.Date = date;
            this.Category = category;
            this.Amount = amount;
            this.Description = description;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================
        /// <summary>
        /// Creates a copy of a given expense, for copying a list of expenses into a new list.
        /// </summary>
        /// <param name="obj">The expense to copy.</param>
        public Expense (Expense obj)
        {
            this.Id = obj.Id;
            this.Date = obj.Date;
            this.Category = obj.Category;
            this.Amount = obj.Amount;
            this.Description = obj.Description;
           
        }
    }
}
