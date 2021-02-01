using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CommunalPayments.WPF.Themes
{
    public static class ThemeManager
    {        
        private static ResourceDictionary GetThemeResourceDictionary(string theme)
        {
            if (theme != null)
            {
                string packUri = String.Format(@"/Themes/{0}/Theme.xaml", theme);
                return Application.LoadComponent(new Uri(packUri, UriKind.Relative)) as ResourceDictionary;
            }
            return null;
        }
        public static string[] Themes
        {
            get
            {
                string[] retVal = new string[]
                {
                "ExpressionDark",
                "ExpressionLight"
                };
                return retVal;
            }
        }

        public static string Theme
        {
            get
            {
                return Properties.Settings.Default.DefaultTheme;
            }
            set
            {
                ResourceDictionary dict = ThemeManager.GetThemeResourceDictionary(value);

                if (dict != null)
                {
                    ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                                  where d.Source != null && d.Source.OriginalString.EndsWith("/Theme.xaml")
                                                  select d).FirstOrDefault();
                    if (oldDict != null)
                    {
                        int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                        Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                        Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                    }
                    else
                    {
                        Application.Current.Resources.MergedDictionaries.Add(dict);
                    }
                    Properties.Settings.Default.DefaultTheme = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

    }
}
