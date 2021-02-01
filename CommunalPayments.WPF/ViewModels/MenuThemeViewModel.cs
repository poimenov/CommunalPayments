using CommunalPayments.WPF.Themes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace CommunalPayments.WPF.ViewModels
{
    public class MenuThemeViewModel : ViewModelBase
    {
        public ObservableCollection<MenuItem> Items { get; private set; }
        private string _currentTheme;
        public string CurrentTheme
        {
            get
            {
                return _currentTheme;
            }
            set
            {
                if(_currentTheme != value)
                {
                    _currentTheme = value;
                    RaisePropertyChanged(() => CurrentTheme);
                }
                

            }
        }

        public MenuThemeViewModel()
        {
            Items = new ObservableCollection<MenuItem>();
            foreach (var themeName in ThemeManager.Themes)
            {
                Items.Add(GetMenuItem(themeName));
            }
        }
        private MenuItem GetMenuItem(string themeName)
        {
            var retVal = new MenuItem();
            retVal.Header = themeName;
            retVal.IsChecked = themeName.Equals(ThemeManager.Theme);
            retVal.Tag = themeName;
            retVal.Command = ChangeThemeCmd;
            retVal.CommandParameter = themeName;
            return retVal;
        }
        public RelayCommand<object> ChangeThemeCmd { get { return new RelayCommand<object>(OnChangeThemeCmd, false); } }
        private void OnChangeThemeCmd(object obj)
        {
            var themeName = obj as string;
            if (!string.IsNullOrEmpty(themeName))
            {
                ThemeManager.Theme = themeName;
                CurrentTheme = themeName;
                Items.ToList().ForEach(x => x.IsChecked = x.Header.Equals(themeName));
            }
        }
    }
}
