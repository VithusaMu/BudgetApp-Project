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
    // CLASS: Category
    //        - An individual category for budget program
    //        - Valid category types: Income, Expense, Credit, Saving
    // ====================================================================
    /// <summary>
    /// An individual group of things to classify budget items
    /// </summary>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Gets the category ID.
        /// </summary>
        /// <value>The ID of the category.</value>
        public int Id { get; }

        /// <summary>
        /// Gets the description of the category.
        /// </summary>
        /// <value>A description of the category.</value>
        public String Description { get; }

        /// <summary>
        /// Gets the type of category from the category type enum.
        /// </summary>
        /// <value>The type of the category.</value>
        public CategoryType Type { get; }

        /// <summary>
        /// Category types for the category object.
        /// </summary>
        public enum CategoryType
        {
            /// <summary>
            /// Income category.
            /// </summary>
            Income = 1,
            /// <summary>
            /// Expense category.
            /// </summary>
            Expense,
            /// <summary>
            /// Credit category.
            /// </summary>
            Credit,
            /// <summary>
            /// Savings category.
            /// </summary>
            Savings
        };

        // ====================================================================
        // Constructor
        // ====================================================================

        /// <summary>
        /// Creates a new category with the given id, description and type.
        /// </summary>
        /// <param name="id">The id of the category.</param>
        /// <param name="description">The description of the category.</param>
        /// <param name="type">The type of category.</param>
        public Category(int id, String description, CategoryType type = CategoryType.Expense)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }

        // ====================================================================
        // Copy Constructor
        // ====================================================================
        /// <summary>
        /// Creates a copy of a given category, for copying a list of categories into a new list.
        /// </summary>
        /// <param name="category">The category to copy.</param>
        public Category(Category category)
        {
            this.Id = category.Id;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        // ====================================================================
        // String version of object
        // ====================================================================

        /// <summary>
        /// An override of the ToString method for the category class.
        /// <example>
        /// <code>
        /// Category category = new Category(1, "Subscriptions", Category.CategoryType.Expense);
        /// Console.WriteLine(category.ToString());
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>The description of the category.</returns>
        public override string ToString()
        {
            return Description;
        }

    }
}

