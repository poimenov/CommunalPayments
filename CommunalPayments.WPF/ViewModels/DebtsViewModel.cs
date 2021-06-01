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
        private ResourceManager _resourceManager;
        private List<KeyValuePair<string, string>> _columns;

        public DebtsViewModel(Debt debt, ILog logger)
        {
            this.Debt = debt;
            this._logger = logger;
            _columns = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ServiceName", "ServiceName"),
                new KeyValuePair<string, string>("Credited", "Credited"),
                new KeyValuePair<string, string>("Recalc", "Recalc"),
                new KeyValuePair<string, string>("Pays", "Pays"),
                new KeyValuePair<string, string>("Penalty", "Penalty"),
                new KeyValuePair<string, string>("Subs", "Subs"),
                new KeyValuePair<string, string>("Saldo", "Saldo"),
                new KeyValuePair<string, string>("Paysnew", "Paysnew"),
                new KeyValuePair<string, string>("FirmName", "FirmName")
            };
        }
        private string GetDisplayName(string propertyName)
        {
            try
            {
                var GetGridItemType = typeof(DebtItem);
                var property = GetGridItemType.GetProperty(propertyName);
                var displayAttributes = property.GetCustomAttributes(typeof(DisplayAttribute), true);
                if (displayAttributes.Any())
                {
                    var displayAttribute = displayAttributes.First() as DisplayAttribute;
                    if (null == _resourceManager)
                    {
                        _resourceManager = new ResourceManager(displayAttribute.ResourceType.FullName, GetGridItemType.Assembly);
                    }
                    return _resourceManager.GetString(displayAttribute.Name);
                }
                else
                {
                    return propertyName;
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error(string.Format("Exception in DebtsViewModel.GetDisplayName(propertyName=\"{0}\")", propertyName), ex);
                return propertyName;
            }
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
            if(null == e)
            {
                return;
            }
            if (_columns.Any(x => x.Key == e.PropertyName))
            {
                var kvp = _columns.First(x => x.Key == e.PropertyName);
                var col = new DataGridTextColumn
                {
                    Binding = new Binding(kvp.Value),
                    Header = GetDisplayName(e.PropertyName)
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
