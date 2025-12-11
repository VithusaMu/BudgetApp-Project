using HomeBudgetAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetApp
{
    public interface IViewInterface
    {
        public void ShowCompletion(string message);
        public void ShowError(string message);
        public void DisplayCategories(List<Category> categories);
        public void DisplayBudgetItems(List<BudgetItem> budgetItems);
        public void DisplayBudgetItemsByCategory(List<BudgetItemsByCategory> budgetItemsByCategory);
        public void DisplayBudgetItemsByMonth(List<BudgetItemsByMonth> budgetItemsByMonth);
        public void DisplayBudgetItemsByCategoryMonth(List<Dictionary<string, object>> budgetItemsByCategoryMonth);
        public void DisplayBudgetItemsByCategoryMonthAsGraph(List<Dictionary<string, object>> budgetItemsByCategoryMonth);
        public void NextSearch(object searchItem);
    }
}
