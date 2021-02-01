using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using log4net;
using MvvmDialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CommunalPayments.WPF.ViewModels
{
    public class CircleChartViewModel : ViewModelBase, IModalDialogViewModel
    {
        private readonly IPayment _payments;
        private readonly IDataAccess<Account> _accounts;
        public bool? DialogResult { get { return false; } }
        public CircleChartViewModel(IPayment payments, IDataAccess<Account> accounts, ILog logger)
        {
            _payments = payments;
            _accounts = accounts;
            Accounts = new ObservableCollection<Account>(_accounts.ItemsList);
        }
        public ObservableCollection<Account> Accounts { get; private set; }
        public SeriesCollection SeriesCollection { get; private set; }
        #region Selected Account
        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get
            {
                return _selectedAccount;
            }
            set
            {
                if (_selectedAccount != value)
                {
                    _selectedAccount = value;
                    RaisePropertyChanged(() => SelectedAccount);
                }
            }
        }
        #endregion
        #region SelectAccountCmd
        public RelayCommand<object> SelectAccountCmd { get { return new RelayCommand<object>(OnSelectAccount, obj => (obj != null), false); } }
        private void OnSelectAccount(object obj)
        {
            Account item = obj as Account;
            if (item != null)
            {
                var payments = new ObservableCollection<Payment>(_payments.GetPayments(null, item.Id));
                var items = payments.SelectMany(p => p.PaymentItems).ToList();
                var serviceNames = items.Select(pi => pi.Service.Name).Distinct();
                var serCollection = new SeriesCollection();
                var sumAll = items.Select(pi => pi.Amount).Sum();
                foreach (var name in serviceNames)
                {
                    var sum = items.Where(pi => pi.Service.Name == name).Select(pi => pi.Amount).Sum();
                    var pieSeries = new PieSeries();
                    pieSeries.Title = string.Format("{0}: {1} ({2:P})", name, sum, sum / sumAll);
                    pieSeries.Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(sum)) };
                    pieSeries.DataLabels = sum / sumAll > 0.05M;
                    pieSeries.LabelPosition = PieLabelPosition.InsideSlice;                    
                    pieSeries.LabelPoint = chartPoint => name;
                    serCollection.Add(pieSeries);
                }
                SeriesCollection = serCollection;
                RaisePropertyChanged(() => SeriesCollection);
            }
        }
        #endregion
    }
}
