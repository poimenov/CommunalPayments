using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.Common.Reports;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace CommunalPayments.WPF.ViewModels
{
    public class PaymentsViewModel : DockWindowViewModel
    {
        private IPayment _dataAccess;
        private List<int> _deletedIds;
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
        public PaymentsViewModel(IPayment dataAccess, IDataAccess<Account> accounts, ILog logger) : base(logger)
        {
            _dataAccess = dataAccess;
            _columns.Add(new KeyValuePair<string, string>("Id", "Id"));
            _columns.Add(new KeyValuePair<string, string>("Name", "Name"));
            _columns.Add(new KeyValuePair<string, string>("Sum", "Sum"));
            _columns.Add(new KeyValuePair<string, string>("Comment", "Comment"));
            Accounts = new ObservableCollection<Account>(accounts.ItemsList);
            _deletedIds = new List<int>();
        }
        #region SaveCmd
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
        private void OnDelete(object obj)
        {
            Payment item = obj as Payment;
            if (item != null && item.Id > 0)
            {
                _deletedIds.Add(item.Id);
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
            if (item != null && item.Id > 0)
            {
                string reportPath;
                PaymentReport.Create(item, ReportFormat.html, Bank, out reportPath);
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
        #endregion
        #region ExportXmlCmd
        public RelayCommand<object> ExportXmlCmd { get { return new RelayCommand<object>(OnExportXml, obj => (obj != null && Payments.Count > 0), false); } }
        private void OnExportXml(object obj)
        {
            Payment item = obj as Payment;
            if (item != null && item.Id > 0)
            {
                string reportPath;
                PaymentReport.Create(item, ReportFormat.xml, Bank, out reportPath);
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
        #endregion
        private BankInfo Bank
        {
            get
            {
                var settings = Properties.Settings.Default;
                return new BankInfo(settings.BankName, settings.BankAccount, settings.EDRPOU);
            }
        }
    }
}
