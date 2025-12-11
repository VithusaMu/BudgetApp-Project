using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HomeBudgetAPI;

namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for EditExpense.xaml
    /// </summary>
    public partial class EditExpense : Window
    {
        private Presenter _presenter;
        private BudgetItem _budgetItem;

        public EditExpense(Presenter presenter, BudgetItem budgetItem)
        {
            InitializeComponent();
            _presenter = presenter;
            _budgetItem = budgetItem;

            exp_Date.SelectedDate = budgetItem.Date;
            exp_Amount.Text = $"{budgetItem.Amount}";
            exp_Name.Text = budgetItem.ShortDescription;

            ComboBox_Themes.ItemsSource = Enum.GetValues(typeof(ThemeOptions));
            ComboBox_Themes.SelectedIndex = (int)Theme.CurrentTheme;
        }

        private void Btn_Save_Clicked(object sender, RoutedEventArgs e)
        {
            Category? category = cmb_Categories?.SelectedItem as Category;
            _presenter.UpdateExpense(_budgetItem.ExpenseID, exp_Date.SelectedDate, category?.Id, exp_Amount.Text, exp_Name.Text);
            this.Close();
        }

        private void Btn_Cancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ComboBox_Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Theme.ChangeTheme((ThemeOptions)ComboBox_Themes.SelectedIndex);
        }
    }
}
