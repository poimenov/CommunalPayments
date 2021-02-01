using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.WPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using MvvmDialogs;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace CommunalPayments.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IDataAccess<Service> _services;
        private readonly IDataAccess<Person> _persons;
        private readonly ILog _logger;
        private IEnumerable<Service> _servicesList;
        private IEnumerable<Person> _personsList;
        public MenuLanguageViewModel MenuLanguageViewModel { get; private set; }
        public MenuThemeViewModel MenuThemeViewModel { get; private set; }
        public DockManagerViewModel DockManagerViewModel { get; private set; }
        public IEnumerable<Service> Services
        {
            get
            {
                if (_servicesList == null)
                {
                    _servicesList = _services.ItemsList;
                }
                return _servicesList;
            }
        }
        public IEnumerable<Person> Persons
        {
            get
            {
                if (_personsList == null)
                {
                    _personsList = _persons.ItemsList;
                }
                return _personsList;
            }
        }

        #region Constructors
        public MainViewModel(IDialogService dialogService, IDataAccess<Service> services, IDataAccess<Person> persons, ILog logger)
        {
            this._dialogService = dialogService;
            this._services = services;
            this._persons = persons;
            this._logger = logger;
            this.DockManagerViewModel = new DockManagerViewModel();
            this.MenuLanguageViewModel = new MenuLanguageViewModel();
            this.MenuThemeViewModel = new MenuThemeViewModel();
        }
        #endregion

        #region Commands
        public ICommand ShowAboutDialogCmd { get { return new RelayCommand(() => _dialogService.ShowDialog<About>(this, new AboutViewModel()), AlwaysTrue); } }
        public ICommand ShowLineChartCmd { get { return new RelayCommand(() => _dialogService.ShowDialog<Chart>(this, App.Locator.LineChart), () => App.Locator.LineChart.Items.Count() > 0); } }
        public ICommand ShowCircleChartCmd { get { return new RelayCommand(() => _dialogService.ShowDialog<CircleChart>(this, App.Locator.CircleChart), () => App.Locator.CircleChart.Accounts.Count() > 0); } }
        public ICommand ExitCmd { get { return new RelayCommand(OnExitApp, AlwaysTrue); } }
        public ICommand ShowServicesCmd { get { return new RelayCommand(() => DockManagerViewModel.AddDocument(App.Locator.Services, App.ResGlobal.GetString("Services")), AlwaysTrue); } }
        public ICommand ShowRatesCmd { get { return new RelayCommand(() => DockManagerViewModel.AddDocument(App.Locator.Rates, App.ResGlobal.GetString("Rates")), AlwaysTrue); } }
        public ICommand ShowPersonsCmd { get { return new RelayCommand(() => DockManagerViewModel.AddDocument(App.Locator.Persons, App.ResGlobal.GetString("Persons")), AlwaysTrue); } }
        public ICommand ShowAccountsCmd { get { return new RelayCommand(() => DockManagerViewModel.AddDocument(App.Locator.Accounts, App.ResGlobal.GetString("Accounts")), ()=>Persons.Count()>0); } }
        public ICommand ShowPaymentsCmd { get { return new RelayCommand(() => DockManagerViewModel.AddDocument(App.Locator.Payments, App.ResGlobal.GetString("Payments")), AlwaysTrue); } }
        public ICommand CreatePaymentCmd { get { return new RelayCommand(() => DockManagerViewModel.AddDocument(App.Locator.PaymentDetail, App.ResGlobal.GetString("PaymentDetail")), AlwaysTrue); } }
        private bool AlwaysTrue() { return true; }
        private void OnExitApp()
        {
            System.Windows.Application.Current.MainWindow.Close();
        }
        #endregion
    }
}
