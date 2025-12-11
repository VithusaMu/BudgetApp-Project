using HomeBudgetAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetApp
{
    public class Presenter
    {
        private HomeBudget _model;

        public IViewInterface View { get; set; }

        private List<object> _searchList = new List<object>();
        private int _searchIndex = 0;


        public Presenter(IViewInterface view)
        {
            View = view;
        }

        public void ProcessFile(string databaseFile, bool newDB)
        {
            _model = new HomeBudget(databaseFile, newDB);
        }

        public void AddCategory(string? description, Category.CategoryType? categoryType)
        {
            StringBuilder errorMessage = new StringBuilder();
            if (string.IsNullOrEmpty(description))
                errorMessage.AppendLine("Name cannot be null or empty.");
            if (categoryType == null)
                errorMessage.AppendLine("Type cannot be empty.");

            if (string.IsNullOrEmpty(errorMessage.ToString()))
            {
                _model.categories.Add(description, (Category.CategoryType)categoryType!);
                View.ShowCompletion($"Name: {description}\nType: {categoryType}");
            }
            else
            {
                View.ShowError(errorMessage.ToString());
            }
        }

        public void AddExpense(DateTime? date, int? categoryId, string? amountInput, string description)
        {
            StringBuilder errorMessage = new StringBuilder();
            if (string.IsNullOrEmpty(description))
                errorMessage.AppendLine("Name cannot be null or empty.");
            if (date == null)
                errorMessage.AppendLine("Date must have a value.");
            if (categoryId is null)
                errorMessage.AppendLine("Expense must have a category.");
            if (string.IsNullOrEmpty(amountInput))
                errorMessage.AppendLine("Amount text box must not be empty.");

            if (!double.TryParse(amountInput, out double amount))
            {
                errorMessage.AppendLine("Amount must be a number.");
            }

            if (string.IsNullOrEmpty(errorMessage.ToString()))
            {
                _model.expenses.Add((DateTime)date!, (int)categoryId!, (double)amount!, description);
                View.ShowCompletion("Expense has been created");
            }
            else
            {
                View.ShowError(errorMessage.ToString());
            }
        }

        public void UpdateExpense(int id, DateTime? date, int? categoryId, string? amountInput, string description)
        {
            StringBuilder errorMessage = new StringBuilder();
            if (string.IsNullOrEmpty(description))
                errorMessage.AppendLine("Name cannot be null or empty.");
            if (date == null)
                errorMessage.AppendLine("Date must have a value.");
            if (categoryId is null)
                errorMessage.AppendLine("Expense must have a category.");
            if (string.IsNullOrEmpty(amountInput))
                errorMessage.AppendLine("Amount text box must not be empty.");

            if (!double.TryParse(amountInput, out double amount))
            {
                errorMessage.AppendLine("Amount must be a number.");
            }

            if (string.IsNullOrEmpty(errorMessage.ToString()))
            {
                _model.expenses.UpdateProperties(id, (DateTime)date!, (int)categoryId!, (double)amount!, description);
                View.ShowCompletion("Expense has been updated.");
            }
            else
            {
                View.ShowError(errorMessage.ToString());
            }
        }

        public void DeleteExpense(int id)
        {
            try
            {
                _model.expenses.Delete(id);
            }
            catch (Exception ex)
            {
                View.ShowError(ex.Message);
            }
        }

        public void SetCategories()
        {
            View.DisplayCategories(_model.categories.List());
        }
         
        public void SetBudgetItems(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryId, bool sortByCategory, bool sortByMonth, bool isGraph)
        {
            if (sortByCategory && sortByMonth && isGraph)
            {
                List<Dictionary<string, object>> budgetItemsByCategoryMonth = _model.GetBudgetDictionaryByCategoryAndMonth(startDate, endDate, filterFlag, categoryId);
                View.DisplayBudgetItemsByCategoryMonthAsGraph(budgetItemsByCategoryMonth);
            }
            else if (sortByCategory && sortByMonth)
            {
                List<Dictionary<string, object>> budgetItemsByCategoryMonth = _model.GetBudgetDictionaryByCategoryAndMonth(startDate, endDate, filterFlag, categoryId);
                View.DisplayBudgetItemsByCategoryMonth(budgetItemsByCategoryMonth);
            }
            else if (sortByCategory && !sortByMonth)
            {
                List<BudgetItemsByCategory> budgetItemsByCategory = _model.GetBudgetItemsByCategory(startDate, endDate, filterFlag, categoryId);
                View.DisplayBudgetItemsByCategory(budgetItemsByCategory);
            }
            else if (!sortByCategory && sortByMonth)
            {
                List<BudgetItemsByMonth> budgetItemsByMonth = _model.GetBudgetItemsByMonth(startDate, endDate, filterFlag, categoryId);
                View.DisplayBudgetItemsByMonth(budgetItemsByMonth);
            }
            else
            {
                List<BudgetItem> budgetItems = _model.GetBudgetItems(startDate, endDate, filterFlag, categoryId);
                View.DisplayBudgetItems(budgetItems);
            }

        }

        public void Search(string searchInput, List<BudgetItem> budgetItems, int selectedIndex)
        {
            if (string.IsNullOrEmpty(searchInput))
                View.ShowError("Search input is empty.");

            _searchList.Clear();
            _searchIndex = 0;

            for (int recordIndex = 0; recordIndex < budgetItems.Count; recordIndex++)
            {

                BudgetItem? record = budgetItems[recordIndex];

                if (record == null)
                { continue; }

                string description = record!.ShortDescription;
                string amount = record.Amount.ToString();

                if (description.Contains(searchInput) || amount.Contains(searchInput))
                {
                    _searchList.Add(budgetItems[recordIndex]);
                }

                if (selectedIndex == recordIndex)
                {
                    _searchIndex = _searchList.Count;
                }
            }

            if (_searchList.Count == 0)
                View.ShowError("Search not found.");
            else
            {
                if (_searchIndex > _searchList.Count - 1)
                {
                    _searchIndex = 0;
                }
                View.NextSearch(_searchList[_searchIndex]);
                
            }

        }


    }
}
