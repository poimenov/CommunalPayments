using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.WPF.ViewModels.Dictionary;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace CommunalPayments.WPF.ViewModels
{
    public class DockManagerViewModel : ViewModelBase
    {
        public ObservableCollection<DockWindowViewModel> Documents { get; private set; }
        public DockWindowViewModel SelectedDocument { get; private set; }
        public object SelectedItem { get; private set; }
        public DockManagerViewModel()
        {
            this.Documents = new ObservableCollection<DockWindowViewModel>();
        }
        public void AddDocument(DockWindowViewModel document, string title)
        {
            document.Title = title;
            document.IsClosed = false;
            document.DockManager = this;
            if (!this.Documents.Contains(document))
            {
                document.PropertyChanged += DockWindowViewModel_PropertyChanged;
                this.Documents.Add(document);
            }
            document.IsSelected = true;
            document.IsActive = true;
        }
        private void DockWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DockWindowViewModel document = sender as DockWindowViewModel;

            switch (e.PropertyName)
            {
                case nameof(DockWindowViewModel.IsClosed):
                    if (document.IsClosed)
                    {
                        this.Documents.Remove(document);
                    }
                    break;
                case nameof(DockWindowViewModel.IsActive):
                    if (document.IsActive && SelectedDocument != document)
                    {
                        SelectedDocument = document;
                        RaisePropertyChanged(() => this.SelectedDocument);
                        SelectedItem = null;
                        RaisePropertyChanged(() => this.SelectedItem);
                    }
                    break;
                case "SelectedItem":
                    if (sender is AccountsViewModel)
                    {
                        SelectedItem = ((AccountsViewModel)sender).SelectedItem;
                    }
                    if (sender is PersonsViewModel)
                    {
                        SelectedItem = ((PersonsViewModel)sender).SelectedItem;
                    }
                    if (sender is RatesViewModel)
                    {
                        SelectedItem = ((RatesViewModel)sender).SelectedItem;
                    }
                    if (sender is ServicesViewModel)
                    {
                        SelectedItem = ((ServicesViewModel)sender).SelectedItem;
                    }
                    if (sender is PaymentDetailViewModel)
                    {
                        var item = ((PaymentDetailViewModel)sender).SelectedItem;
                        if (null == item)
                        {
                            return;
                        }
                        SelectedItem = item;
                    }
                    if (sender is PaymentsViewModel)
                    {
                        SelectedItem = ((PaymentsViewModel)sender).SelectedItem;
                    }
                    RaisePropertyChanged(() => this.SelectedItem);
                    break;
            }
        }
        public RelayCommand<object> PropertyChangedCmd { get { return new RelayCommand<object>(OnPropertyChanged, obj => (obj != null) || (SelectedDocument is PaymentDetailViewModel), false); } }
        private void OnPropertyChanged(object obj)
        {
            var args = obj as PropertyValueChangedEventArgs;
            var ent = SelectedItem as Entity;
            if (null == args || null == ent)
            {
                return;
            }
            if (ent.Id > 0)
            {
                if (SelectedDocument is AccountsViewModel)
                {
                    ((AccountsViewModel)SelectedDocument).ChangedItemId(ent.Id);
                }
                if (SelectedDocument is PersonsViewModel)
                {
                    ((PersonsViewModel)SelectedDocument).ChangedItemId(ent.Id);
                }
                if (SelectedDocument is RatesViewModel)
                {
                    ((RatesViewModel)SelectedDocument).ChangedItemId(ent.Id);
                }
                if (SelectedDocument is ServicesViewModel)
                {
                    ((ServicesViewModel)SelectedDocument).ChangedItemId(ent.Id);
                }
            }
            if (SelectedDocument is PaymentDetailViewModel)
            {
                var item = SelectedItem as PaymentItem;
                var prop = args.OriginalSource as PropertyItem;
                if (null != item && null != prop && (prop.PropertyName == nameof(item.LastIndication) || prop.PropertyName == nameof(item.CurrentIndication) || prop.PropertyName == nameof(item.Value) || prop.PropertyName == nameof(item.Amount)))
                {
                    if (Convert.ToDecimal(args.OldValue) != Convert.ToDecimal(args.NewValue))
                    {
                        ((PaymentDetailViewModel)SelectedDocument).ChangedItem(item);
                    }
                }
            }
        }
        public RelayCommand<object> SelectedItemChangedCmd { get { return new RelayCommand<object>(OnSelectedItemChanged, obj => (obj != null) || (SelectedDocument is PaymentDetailViewModel), false); } }
        private void OnSelectedItemChanged(object obj)
        {
            var args = obj as System.Windows.RoutedPropertyChangedEventArgs<Xceed.Wpf.Toolkit.PropertyGrid.PropertyItemBase>;
            if (null != args && null != args.OldValue)
            {
                ((PaymentDetailViewModel)SelectedDocument).RefreshSelectedPayment();
            }
        }
        public RelayCommand<object> KeyDownCmd { get { return new RelayCommand<object>(OnKeyDownCmd, obj => (obj != null) || (SelectedDocument is PaymentDetailViewModel), false); } }
        private void OnKeyDownCmd(object obj)
        {
            var args = obj as System.Windows.Input.KeyEventArgs;
            if (null != args && args.Key == System.Windows.Input.Key.Enter)
            {
                ((PaymentDetailViewModel)SelectedDocument).RefreshSelectedPayment();
            }
        }

    }


}
