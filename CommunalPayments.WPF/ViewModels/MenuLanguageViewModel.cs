using CommunalPayments.WPF.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace CommunalPayments.WPF.ViewModels
{
    public class MenuLanguageViewModel : ViewModelBase
    {
        public ObservableCollection<MenuItem> Items { get; private set; }
        public MenuLanguageViewModel()
        {
            Items = new ObservableCollection<MenuItem>();
            foreach (var lang in LanguageManager.Languages)
            {
                Items.Add(GetMenuItem(lang));
            };
        }
        private MenuItem GetMenuItem(CultureInfo lang)
        {
            var retVal = new MenuItem();
            retVal.Header = lang.NativeName;
            retVal.IsChecked = lang.Equals(LanguageManager.Language);
            retVal.Tag = lang;
            retVal.Command = ChangeLanguageCmd;
            retVal.CommandParameter = lang;
            return retVal;
        }
        public RelayCommand<object> ChangeLanguageCmd { get { return new RelayCommand<object>(OnChangeLanguageCmd, false); } }
        private void OnChangeLanguageCmd(object obj)
        {
            var lang = obj as CultureInfo;
            if (null != lang)
            {
                LanguageManager.Language = lang;
                Items.ToList().ForEach(x => x.IsChecked = x.Tag.Equals(LanguageManager.Language));
            }
        }
    }
}
