using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HomeBudgetApp
{
    enum ThemeOptions
    {
        Budgeter,
        Light,
        Dark,
        Swamp,
        Crimson,
        Cobalt
    }

    static class Theme
    {
        private static ThemeOptions _currentTheme = ThemeOptions.Budgeter;

        public static ThemeOptions CurrentTheme
        {
            get { return _currentTheme; }
        }

        public static void ChangeTheme(ThemeOptions themeOption)
        {
            Uri themeUri = new Uri($"Themes/{themeOption.ToString()}.xaml", UriKind.Relative);
            //sets up the new theme
            ResourceDictionary theme = new ResourceDictionary();
            theme.Source = themeUri;

            //resets the resources (theme)
            App.Current.Resources.Clear();

            //set up new one
            App.Current.Resources.MergedDictionaries.Add(theme);
            _currentTheme = themeOption;
        }
    }
}
