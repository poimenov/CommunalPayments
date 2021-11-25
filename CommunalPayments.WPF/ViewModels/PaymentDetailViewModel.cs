using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using MvvmDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CommunalPayments.WPF.ViewModels
{
    public class PaymentDetailViewModel : DockWindowViewModel
    {
        private IPayment _dataAccess;
        private IEnumerable<Rate> _actualRates;
        private IEnumerable<Service> _services;
        private bool _importInProcess;
        private readonly IDialogService _dialogService;
        private readonly INetRepository _netRepository;
        protected override Type GridItemType
        {
            get
            {
                return typeof(PaymentItem);
            }
        }
        public PaymentDetailViewModel(IDataAccess<Account> accounts, IDataAccess<Service> services, IPayment dataAccess, IDataAccess<Rate> rates,
            IDialogService dialogService, INetRepository netRepository, ILog logger) : base(logger)
        {
            this._dataAccess = dataAccess;
            this._actualRates = rates.ItemsList.Where(x => x.Enabled).ToList();
            this._services = services.ItemsList.Where(x => x.ErcId > 0).ToList();
            _columns.Add(new ColDescript("Id", "Id"));
            _columns.Add(new ColDescript("ServiceId", "Service.Name"));
            _columns.Add(new ColDescript("PeriodFrom", "PeriodFrom", ColumnType.TextColumn, "MMMM yyyy"));
            _columns.Add(new ColDescript("PeriodTo", "PeriodTo", ColumnType.TextColumn, "MMMM yyyy"));
            _columns.Add(new ColDescript("LastIndication", "LastIndication"));
            _columns.Add(new ColDescript("CurrentIndication", "CurrentIndication"));
            _columns.Add(new ColDescript("Value", "Value"));
            _columns.Add(new ColDescript("Amount", "Amount"));
            Accounts = new ObservableCollection<Account>(accounts.ItemsList);
            _importInProcess = false;
            _dialogService = dialogService;
            _netRepository = netRepository;
        }
        public override string ItemTypeName { get => App.ResGlobal.GetString("PaymentItem"); }
        public void ChangedItem(PaymentItem item)
        {
            if (item.LastIndication.HasValue && item.CurrentIndication.HasValue)
            {
                item.Value = item.CurrentIndication.Value - item.LastIndication.Value;
                if (item.ServiceId == 4 || item.ServiceId == 5)//горячая или холодная вода
                {
                    //пересчитать канализацию 
                    //если нет строки с канализацией
                    if (!ItemsList.Any(x => x.ServiceId == 6))
                    {
                        //добавить строку с канализацией
                        ItemsList.Add(new PaymentItem()
                        {
                            ServiceId = 6,
                            PeriodFrom = DateTime.Today.AddMonths(-1).AddDays(1 - DateTime.Today.Day),
                            PeriodTo = DateTime.Today.AddDays(-DateTime.Today.Day)
                        });
                    }
                    var canal = ItemsList.First(x => x.ServiceId == 6);
                    if (!canal.Value.HasValue)
                    {
                        canal.Value = 0;
                    }
                    //при изменении хол. или гор. воды метод будет вызываться дважды
                    //потому что меняется расход канализации
                    //TODO: как-то коряво написано. надо потом переписать
                    if (item.ServiceId == 4 && ItemsList.Any(x => x.ServiceId == 5) && ItemsList.First(x => x.ServiceId == 5).Value.HasValue)
                    {
                        canal.Value = item.Value + ItemsList.First(x => x.ServiceId == 5).Value.Value;
                    }
                    else if (item.ServiceId == 5 && ItemsList.Any(x => x.ServiceId == 4) && ItemsList.First(x => x.ServiceId == 4).Value.HasValue)
                    {
                        canal.Value = item.Value + ItemsList.First(x => x.ServiceId == 4).Value.Value;
                    }
                    else
                    {
                        canal.Value = item.Value;
                    }
                    canal.Amount = calculationAmount(canal.Value.Value, canal.ServiceId);
                }
            }
            if (item.Value.HasValue)
            {
                item.Amount = calculationAmount(item.Value.Value, item.ServiceId);
            }
            SelectedItem = item;
        }
        public void RefreshSelectedPayment(PaymentItem paymentItem = null)
        {
            var item = (null == paymentItem) ? SelectedItem : paymentItem;
            SelectedPayment = SelectedPayment;
            SelectedItem = item;
        }
        private decimal calculationAmount(decimal value, int serviceId)
        {
            decimal retVal = 0;
            var rates = _actualRates.Where(x => x.ServiceId == serviceId).OrderByDescending(x => x.VolumeFrom).ToList();
            foreach (var rate in rates)
            {
                if (rate.VolumeFrom < value)
                {
                    retVal += rate.Value * (value - rate.VolumeFrom);
                    value = rate.VolumeFrom;
                }
            }
            return Math.Round(retVal, 2);
        }
        public ObservableCollection<Account> Accounts { get; private set; }
        #region SelectedPayment
        private Payment _selectedPayment;
        public Payment SelectedPayment
        {
            get
            {
                if (null == _selectedPayment)
                {
                    _selectedPayment = new Payment() { Enabled = true };
                    ItemsList = null;
                }
                _selectedPayment.AccountId = SelectedAccountId;
                if (null != ItemsList)
                {
                    _selectedPayment.PaymentItems = ItemsList.ToList();
                }
                return _selectedPayment;
            }
            set
            {
                if (null == value)
                {
                    _selectedPayment = new Payment() { Enabled = true };
                    ItemsList = null;
                    RaisePropertyChanged(() => SelectedPayment);
                }
                else
                {
                    ItemsList = null;
                    _selectedPayment = value;
                    RaisePropertyChanged(() => SelectedPayment);
                    ItemsList = new ObservableCollection<PaymentItem>(_selectedPayment.PaymentItems);
                }
            }
        }

        #endregion
        #region Selected Payment item
        private PaymentItem _selectedItem;
        public PaymentItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }
        #endregion
        #region Payment items list
        private ObservableCollection<PaymentItem> _items;
        public ObservableCollection<PaymentItem> ItemsList
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                RaisePropertyChanged(() => ItemsList);
            }
        }
        #endregion
        #region Selected Account
        public Account SelectedAccount
        {
            get;
            private set;
        }
        #endregion
        #region SelectedAccountId
        private int _selectedAccountId;
        public int SelectedAccountId
        {
            get
            {
                return _selectedAccountId;
            }
            set
            {
                if (_selectedAccountId != value)
                {
                    _selectedAccountId = value;
                    SelectedAccount = Accounts.FirstOrDefault(x => x.Id == value);
                    ItemsList = null;
                    //if new paymment
                    if (_selectedAccountId > 0 && SelectedPayment.Id == 0)
                    {
                        var lastPayment = _dataAccess.LastPayment(_selectedAccountId);
                        var payment = new Payment() { Enabled = true };
                        var lst = new List<PaymentItem>();
                        if (null != lastPayment && null != lastPayment.PaymentItems)
                        {
                            foreach (var prev in lastPayment.PaymentItems)
                            {
                                var curr = new PaymentItem();
                                curr.Enabled = true;
                                curr.CurrentIndication = prev.CurrentIndication;
                                curr.LastIndication = prev.CurrentIndication;
                                curr.ServiceId = prev.ServiceId;
                                curr.Service = prev.Service;
                                curr.PeriodFrom = DateTime.Today.AddMonths(-1).AddDays(1 - DateTime.Today.Day);
                                curr.PeriodTo = DateTime.Today.AddDays(-DateTime.Today.Day);
                                payment.PaymentItems.Add(curr);
                            }
                        }
                        SelectedPayment = payment;
                    }
                    RaisePropertyChanged(() => SelectedAccountId);
                    RaisePropertyChanged(() => SelectedAccount);
                }
            }
        }
        #endregion
        #region SaveCmd
        public ICommand SaveCmd { get { return new RelayCommand(OnSave, () => (SelectedAccountId > 0)); } }
        private async void OnSave()
        {
            List<Payment> payments = new List<Payment>();
            payments.Add(SelectedPayment);
            if (SelectedPayment.Id > 0)
            {
                if (SelectedPayment.ErcId > 0 && !string.IsNullOrEmpty(SelectedPayment.Bbl))
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
                    if (await _netRepository.UpdatePayment(SelectedPayment))
                    {
                        _dataAccess.Update(payments);
                    }
                    else
                    {
                        _dialogService.ShowMessageBox(this, App.ResGlobal.GetString("PaymentNotUpdated"), App.ResGlobal.GetString("InfoTitle"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    this.Cursor = Cursors.Arrow;
                }
            }
            else
            {
                _dataAccess.Create(payments);
            }
            OnCancel();
        }
        #endregion
        #region CancelCmd
        public ICommand CancelCmd { get { return new RelayCommand(OnCancel, () => (SelectedAccountId > 0)); } }
        private void OnCancel()
        {
            if (SelectedPayment != null && SelectedPayment.Id > 0)
            {
                SelectedPayment = _dataAccess.Get(SelectedPayment.Id);
            }
            else
            {
                SelectedPayment = null;
                SelectedAccountId = 0;
            }
            SelectedItem = null;
        }
        #endregion
        #region CreateCmd
        public ICommand CreateCmd { get { return new RelayCommand(OnCreate, () => (SelectedAccountId > 0)); } }
        private void OnCreate()
        {
            PaymentItem item = new PaymentItem() { Enabled = true, PeriodFrom = DateTime.Today.AddMonths(-1).AddDays(1 - DateTime.Today.Day), PeriodTo = DateTime.Today.AddDays(-DateTime.Today.Day) };
            ItemsList.Add(item);
            SelectedItem = item;
        }
        #endregion
        #region DeleteCmd
        public RelayCommand<object> DeleteCmd { get { return new RelayCommand<object>(OnDelete, obj => (obj != null && ItemsList.Count > 0), false); } }
        private void OnDelete(object obj)
        {
            PaymentItem item = (PaymentItem)obj;
            ItemsList.Remove(item);
        }
        #endregion
        #region DebtCmd
        public RelayCommand<object> DebtCmd { get { return new RelayCommand<object>(OnDebtCmd, obj => (obj != null && !_importInProcess), false); } }
        private async void OnDebtCmd(object obj)
        {
            var account = obj as Account;
            if (null == account)
            {
                return;
            }
            var loginViewModel = new LoginViewModel();

            var success = _dialogService.ShowDialog(this, loginViewModel);
            if (success == true)
            {
                _netRepository.Login = loginViewModel.Login;
                _netRepository.Password = loginViewModel.Password;
                _importInProcess = true;
                this.CanClose = false;
                this.Cursor = Cursors.Wait;
                try
                {
                    var debt = await _netRepository.GetDebt(account, DateTime.Today);
                    if (null == debt)
                    {
                        _dialogService.ShowMessageBox(this, "Login/Password is failed", "Warning", System.Windows.MessageBoxButton.OK);
                    }
                    else if (debt.DebtItems.Count() == 0)
                    {
                        _dialogService.ShowMessageBox(this, "The server returned an empty dataset", "There is no data", System.Windows.MessageBoxButton.OK);
                    }
                    else
                    {
                        var debtViewModel = new DebtsViewModel(debt, this._logger);
                        success = _dialogService.ShowDialog(this, debtViewModel);
                        if (success == true)
                        {
                            var newPayment = await _netRepository.CreatePayment(account, debtViewModel.SelectedPayBy, DateTime.Today, _services);
                            if (null != newPayment)
                            {
                                var itemsServIds = ItemsList.Select(x => x.ServiceId).ToList();
                                var newServIds = newPayment.PaymentItems.Select(x => x.ServiceId).ToList();
                                var servIds = itemsServIds.Except(newServIds).ToList();
                                foreach (var id in servIds)
                                {
                                    var debtItem = ItemsList.First(x => x.ServiceId == id);
                                    if (_services.Any(x => x.Id == id))
                                    {
                                        var item = new PaymentItem();
                                        item.Enabled = true;
                                        item.Service = _services.First(x => x.Id == id);
                                        item.ServiceId = item.Service.Id;
                                        item.PeriodFrom = DateTime.Today.AddMonths(-1).AddDays(1 - DateTime.Today.Day);
                                        item.PeriodTo = DateTime.Today.AddDays(-DateTime.Today.Day);
                                        newPayment.PaymentItems.Add(item);
                                    }
                                }

                                foreach (var item in ItemsList.Where(item => item.CurrentIndication.HasValue || item.LastIndication.HasValue || item.Value.HasValue))
                                {
                                    if (newPayment.PaymentItems.Any(x => x.ServiceId == item.ServiceId))
                                    {
                                        var newItem = newPayment.PaymentItems.First(x => x.ServiceId == item.ServiceId);
                                        newItem.CurrentIndication = item.CurrentIndication;
                                        newItem.LastIndication = item.LastIndication;
                                        newItem.Value = item.Value;
                                    }
                                }
                                SelectedPayment = _dataAccess.Get(_dataAccess.Create(newPayment));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessageBox(this, ex.Message, "Error", System.Windows.MessageBoxButton.OK);
                }
                finally
                {
                    _importInProcess = false;
                    this.CanClose = true;
                    this.Cursor = Cursors.Arrow;
                }
            }
        }
        #endregion

    }
}
