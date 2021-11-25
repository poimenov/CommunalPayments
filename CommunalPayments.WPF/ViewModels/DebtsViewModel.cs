using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using MvvmDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CommunalPayments.WPF.ViewModels
{
    public class DebtsViewModel : ViewModelBase, IModalDialogViewModel
    {
        private bool? _dialogResult;
        private ILog _logger;
        private List<ColDescript> _columns;

        public DebtsViewModel(Debt debt, ILog logger)
        {
            this.Debt = debt;
            this._logger = logger;
            _columns = new List<ColDescript>
            {
                new ColDescript("ServiceName", "ServiceName"),
                new ColDescript("Credited", "Credited"),
                new ColDescript("Recalc", "Recalc"),
                new ColDescript("Pays", "Pays"),
                new ColDescript("Penalty", "Penalty"),
                new ColDescript("Subs", "Subs"),
                new ColDescript("Saldo", "Saldo"),
                new ColDescript("Paysnew", "Paysnew"),
                new ColDescript("FirmName", "FirmName")
            };
        }
        #region Debt
        private Debt _debt;
        public Debt Debt
        {
            get
            {
                return _debt;
            }
            private set
            {
                if (_debt != value)
                {
                    _debt = value;
                    RaisePropertyChanged(() => Debt);
                }
            }
        }
        #endregion
        public PayBy SelectedPayBy { get; private set; }
        public bool? DialogResult
        {
            get => _dialogResult;
            private set => Set(nameof(DialogResult), ref _dialogResult, value);
        }
        #region PayByCredit
        public ICommand PayByCredit { get { return new RelayCommand(OnPayByCredit, () => (this.Debt.DebtItems.Count() > 0)); } }
        private void OnPayByCredit()
        {
            SelectedPayBy = PayBy.Credit;
            DialogResult = true;
        }
        #endregion
        #region PayBySaldo
        public ICommand PayBySaldo { get { return new RelayCommand(OnPayBySaldo, () => (this.Debt.DebtItems.Count() > 0)); } }
        private void OnPayBySaldo()
        {
            SelectedPayBy = PayBy.Saldo;
            DialogResult = true;
        }
        #endregion

        #region AutoGeneratingColumnCmd
        public RelayCommand<object> AutoGeneratingColumnCmd { get { return new RelayCommand<object>(OnAutoGeneratingColumn); } }
        private void OnAutoGeneratingColumn(object obj)
        {
            var e = obj as DataGridAutoGeneratingColumnEventArgs;
            if (null == e)
            {
                return;
            }
            if (_columns.Any(x => x.PropertyName == e.PropertyName))
            {
                var colDesc = _columns.First(x => x.PropertyName == e.PropertyName);
                var col = new DataGridTextColumn
                {
                    Binding = new Binding(colDesc.BindingPath),
                    Header = colDesc.GetDisplayName(typeof(DebtItem)),
                    DisplayIndex = _columns.IndexOf(colDesc)
                };
                e.Column = col;
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion
    }
}
