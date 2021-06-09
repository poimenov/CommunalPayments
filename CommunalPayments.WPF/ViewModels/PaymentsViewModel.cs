using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.Common.Reports;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace CommunalPayments.WPF.ViewModels
{
    public class PaymentsViewModel : DockWindowViewModel
    {
        private readonly IPayment _dataAccess;
        private readonly IDialogService _dialogService;
        private readonly INetRepository _netRepository;
        protected override Type GetGridItemType
        {
            get
            {
                return typeof(Payment);
            }
        }
        public override string ItemTypeName { get => App.ResGlobal.GetString("Payment"); }
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
        #region SelectedItem
        private Payment _selectedItem;
        public Payment SelectedItem
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
                }
            }
        }
        #endregion
        public ObservableCollection<Payment> Payments { get; private set; }
        public ObservableCollection<Account> Accounts { get; private set; }
        public PaymentsViewModel(IDialogService dialogService, IPayment dataAccess, IDataAccess<Account> accounts, INetRepository netRepository, ILog logger) : base(logger)
        {
            _dataAccess = dataAccess;
            _dialogService = dialogService;
            _netRepository = netRepository;
            _columns.Add(new KeyValuePair<string, string>("Id", "Id"));
            _columns.Add(new KeyValuePair<string, string>("Name", "Name"));
            _columns.Add(new KeyValuePair<string, string>("Sum", "Sum"));
            _columns.Add(new KeyValuePair<string, string>("Comment", "Comment"));
            Accounts = new ObservableCollection<Account>(accounts.ItemsList);
        }
        #region EditCmd
        public RelayCommand<object> EditCmd { get { return new RelayCommand<object>(OnEdit, obj => (obj != null && SelectedAccount != null && Accounts.Count > 0), false); } }
        private void OnEdit(object obj)
        {
            Payment item = obj as Payment;
            if (item != null)
            {
                var paymentDetail = App.Locator.PaymentDetail;
                this.DockManager.AddDocument(paymentDetail, string.Format(App.ResGlobal.GetString("PaymentTitleEdit"), item.Id));
                paymentDetail.SelectedAccountId = item.AccountId;
                paymentDetail.SelectedPayment = item;
            }

        }
        #endregion
        #region CreateCmd
        public ICommand CreateCmd { get { return new RelayCommand(OnCreate, () => (SelectedAccount != null && Accounts.Count > 0), false); } }
        private void OnCreate()
        {
            var paymentDetail = App.Locator.PaymentDetail;
            this.DockManager.AddDocument(paymentDetail, App.ResGlobal.GetString("PaymentTitleCreate"));
            paymentDetail.SelectedAccountId = SelectedAccount.Id;
        }
        #endregion
        #region DeleteCmd
        public RelayCommand<object> DeleteCmd { get { return new RelayCommand<object>(OnDelete, obj => (obj != null && ((Entity)obj).Enabled && Payments.Count > 0), false); } }
        private async void OnDelete(object obj)
        {
            Payment item = obj as Payment;
            var _deletedIds = new List<int>();
            _deletedIds.Add(item.Id);
            if (item != null && item.Id > 0 && item.Enabled)
            {
                MessageBoxSettings settings = new MessageBoxSettings();
                settings.Caption = App.ResGlobal.GetString("DeletePaymentCaption");
                settings.MessageBoxText = App.ResGlobal.GetString("DeletePaymentMessage");
                settings.Button = System.Windows.MessageBoxButton.YesNo;
                settings.Icon = System.Windows.MessageBoxImage.Question;
                if (System.Windows.MessageBoxResult.Yes == _dialogService.ShowMessageBox(this, settings))
                {
                    var isOk = true;
                    if (item.ErcId > 0 && !string.IsNullOrEmpty(item.Bbl))
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
                        isOk = await _netRepository.DeletePayment(item);
                        if (!isOk)
                        {
                            _dialogService.ShowMessageBox(this, App.ResGlobal.GetString("PaymentNotDeleted"), App.ResGlobal.GetString("InfoTitle"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                        }
                    }
                    if (isOk)
                    {
                        _dataAccess.Delete(_deletedIds);
                        Payments = new ObservableCollection<Payment>(_dataAccess.GetPayments(null, SelectedAccount.Id));
                        RaisePropertyChanged(() => Payments);
                    }
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
                Payments = new ObservableCollection<Payment>(_dataAccess.GetPayments(null, item.Id));
                RaisePropertyChanged(() => Payments);
            }
        }
        #endregion
        #region ExportHtmlCmd
        public RelayCommand<object> ExportHtmlCmd { get { return new RelayCommand<object>(OnExportHtml, obj => (obj != null && Payments.Count > 0), false); } }
        private void OnExportHtml(object obj)
        {
            Payment item = obj as Payment;
            ShowReport(item, ReportFormat.html);
        }
        #endregion
        #region ExportXmlCmd
        public RelayCommand<object> ExportXmlCmd { get { return new RelayCommand<object>(OnExportXml, obj => (obj != null && Payments.Count > 0), false); } }
        private void OnExportXml(object obj)
        {
            Payment item = obj as Payment;
            ShowReport(item, ReportFormat.xml);
        }
        #endregion
        #region ExportPdfCmd
        public RelayCommand<object> ExportPdfCmd { get { return new RelayCommand<object>(OnExportPdf, obj => (obj != null && Payments.Count > 0), false); } }
        private void OnExportPdf(object obj)
        {
            Payment item = obj as Payment;
            ShowReport(item, ReportFormat.pdf);
        }
        #endregion
        private BankInfo Bank
        {
            get
            {
                var settings = Properties.Settings.Default;
                return new BankInfo(settings.BankName, settings.BankAccount, settings.EDRPOU);
            }
        }
        private void ShowReport(Payment payment, ReportFormat format)
        {
            if (payment != null && payment.Id > 0)
            {
                string reportPath;
                PaymentReport.Create(payment, format, Bank, out reportPath);
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"/c {reportPath}"
                };
                Process.Start(psi);
            }
        }
    }
}
