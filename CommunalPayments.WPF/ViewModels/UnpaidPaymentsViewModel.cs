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

namespace CommunalPayments.WPF.ViewModels
{
    public class UnpaidPaymentsViewModel : DockWindowViewModel
    {
        private readonly IPayment _dataAccess;
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
        public UnpaidPaymentsViewModel(IDialogService dialogService, IPayment dataAccess, IDataAccess<Person> persons, INetRepository netRepository, ILog logger) : base(logger)
        {
            _dataAccess = dataAccess;
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
                Payments = new ObservableCollection<CheckedPayment>(_dataAccess.GetUnpaidPaymentsByPersonId(item.Id).OrderBy(p => p.AccountId).Select(p => new CheckedPayment(p)));
                RaisePropertyChanged(() => Payments);
            }
        }
        #endregion
        public ICommand PayCmd { get { return new RelayCommand(OnPay, () => (null != Payments && Payments.Where(x => x.Checked).Count() > 0)); } }
        private async void OnPay()
        { 
            //
            //
        }
    }
}
