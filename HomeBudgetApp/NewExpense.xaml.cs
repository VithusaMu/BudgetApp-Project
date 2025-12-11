using HomeBudgetAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for NewExpense.xaml
    /// </summary>
    public partial class NewExpense : Window
    {
        private Presenter _presenter;

        public NewExpense(Presenter presenter)
        {
            InitializeComponent();
            exp_Date.SelectedDate = DateTime.Today;
            _presenter = presenter;
            ComboBox_Themes.ItemsSource = Enum.GetValues(typeof(ThemeOptions));
            ComboBox_Themes.SelectedIndex = (int)Theme.CurrentTheme;
        }

        private void Btn_CreateExpense_Clicked(object sender, RoutedEventArgs e)
        {
            Category? category = cmb_Categories?.SelectedItem as Category;
            _presenter.AddExpense(exp_Date.SelectedDate, category?.Id, exp_Amount.Text, exp_Name.Text);
            Clear();
        }

        private void Clear()
        {
            exp_Name.Clear();
            exp_Date.SelectedDate = DateTime.Today;
            cmb_Categories.SelectedItem = null;
            exp_Amount.Clear();
        }

        private void Btn_CreateCategory_Clicked(object sender, RoutedEventArgs e)
        {
            NewCategory categoryMenu = new NewCategory(_presenter);
            categoryMenu.ShowDialog();
            _presenter.SetCategories();
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
