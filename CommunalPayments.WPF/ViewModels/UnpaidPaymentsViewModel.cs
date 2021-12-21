using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;

namespace CommunalPayments.WPF.ViewModels
{
    public class UnpaidPaymentsViewModel : DockWindowViewModel
    {
        private readonly IPayment _paymentDataAccess;
        private readonly IDataAccess<Bill> _billDataAccess;
        private readonly IDialogService _dialogService;
        private readonly INetRepository _netRepository;
        protected override Type GridItemType
        {
            get
            {
                return typeof(CheckedPayment);
            }
        }
        public override string ItemTypeName { get => App.ResGlobal.GetString("Payment"); }
        #region Selected Person
        private Person _selectedPerson;
        public Person SelectedPerson

        {
            get
            {
                return _selectedPerson;
            }
            set
            {
                if (_selectedPerson != value)
                {
                    _selectedPerson = value;
                    RaisePropertyChanged(() => SelectedPerson);
                }
            }
        }
        #endregion
        #region SelectedItem
        private CheckedPayment _selectedItem;
        public CheckedPayment SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged(() => SelectedItem);
                    RaisePropertyChanged(() => Sum);
                }
            }
        }
        #endregion
        public decimal Sum
        {
            get
            {
                return Payments == null? 0m : Payments.Where(x => x.Checked).Select(x => x.Sum).Sum();
            }
        }
        public ObservableCollection<CheckedPayment> Payments { get; private set; }
        public ObservableCollection<Person> Persons { get; private set; }
        public UnpaidPaymentsViewModel(IDialogService dialogService, IPayment paymentDataAccess, IDataAccess<Person> persons, 
            IDataAccess<Bill> billDataAccess, INetRepository netRepository, ILog logger) : base(logger)
        {
            _paymentDataAccess = paymentDataAccess;
            _billDataAccess = billDataAccess;
            _dialogService = dialogService;
            _netRepository = netRepository;
            _columns.Add(new ColDescript("Checked", "Checked", ColumnType.CheckBoxColumn));
            _columns.Add(new ColDescript("ErcId", "ErcId"));
            _columns.Add(new ColDescript("Address", "Address"));
            _columns.Add(new ColDescript("Name", "Name"));
            _columns.Add(new ColDescript("Sum", "Sum"));
            _columns.Add(new ColDescript("Comment", "Comment"));
            
            Persons = new ObservableCollection<Person>(persons.ItemsList);
        }
        #region SelectPersonCmd
        public RelayCommand<object> SelectPersonCmd { get { return new RelayCommand<object>(OnSelectPerson, obj => (obj != null), false); } }
        private void OnSelectPerson(object obj)
        {
            Person item = obj as Person;
            if (item != null)
            {
                Payments = new ObservableCollection<CheckedPayment>(_paymentDataAccess.GetUnpaidPaymentsByPersonId(item.Id).OrderBy(p => p.AccountId).Select(p => new CheckedPayment(p)));
                RaisePropertyChanged(() => Payments);
            }
        }
        #endregion
        public ICommand PayCmd { get { return new RelayCommand(OnPay, () => (null != Payments && Payments.Where(x => x.Checked).Count() > 0)); } }
        private async void OnPay()
        { 
            if(this.Sum > 0)
            {
                if (string.IsNullOrEmpty(_netRepository.Login) || string.IsNullOrEmpty(_netRepository.Login))
                {
                    var loginViewModel = new LoginViewModel();
                    var success = _dialogService.ShowDialog(this, loginViewModel);
                    if (success == true)
                    {
                        _netRepository.Login = loginViewModel.Login;
                        _netRepository.Password = loginViewModel.Password;
                    }
                }
                this.Cursor = Cursors.Wait;
                var bill = await _netRepository.CreateInvoice(PaymentMode.BankTransfer, Payments.Where(x => x.Checked).Select(x => x.ErcId));
                if (null != bill)
                {
                    _billDataAccess.Create(new List<Bill>() { bill });
                    _dialogService.ShowMessageBox(this, App.ResGlobal.GetString("BillCreated"), App.ResGlobal.GetString("InfoTitle"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    OnSelectPerson(SelectedPerson);
                    var psi = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = $"/c {_netRepository.GetReportPath(PaymentMode.BankTransfer, bill.ErcId)}"
                    };
                    Process.Start(psi);
                }
                else
                {
                    _dialogService.ShowMessageBox(this, App.ResGlobal.GetString("BillNotCreated"), App.ResGlobal.GetString("InfoTitle"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                this.Cursor = Cursors.Arrow;
            }
        }
    }
}
