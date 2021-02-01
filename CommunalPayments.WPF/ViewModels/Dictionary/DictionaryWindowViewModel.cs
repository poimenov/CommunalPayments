using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CommunalPayments.WPF.ViewModels.Dictionary
{
    public abstract class DictionaryWindowViewModel<T> : DockWindowViewModel where T : Entity, new()
    {
        protected List<int> _deletedIds;
        protected List<int> _updatedIds;
        protected ObservableCollection<T> _items;
        protected IDataAccess<T> _dataAccess;
        protected override Type GetGridItemType
        {
            get
            {
                return typeof(T);
            }
        }
        public DictionaryWindowViewModel(IDataAccess<T> dataAccess, ILog logger):base(logger)
        {
            this._dataAccess = dataAccess;
            this._deletedIds = new List<int>();
            this._updatedIds = new List<int>();            
        }        
        public void ChangedItemId(int id)
        {
            if (id > 0 && !_updatedIds.Contains(id))
            {
                _updatedIds.Add(id);
            }
        }
        #region SelectedItem
        private T _selectedItem;
        public T SelectedItem
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
        public ObservableCollection<T> ItemList
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<T>(_dataAccess.ItemsList);
                }
                return _items;
            }
        }
        #region SaveCmd
        public ICommand SaveCmd { get { return new RelayCommand(OnSave); } }
        private void OnSave()
        {
            _dataAccess.Delete(_deletedIds);
            var newItems = ItemList.Where(x => ((Entity)x).Id == 0).ToList();
            _dataAccess.Create(newItems);
            var updatedItems = ItemList.Where(x => _updatedIds.Contains(((Entity)x).Id)).ToList();
            _dataAccess.Update(updatedItems);
            OnCancel();
        }
        #endregion
        #region CancelCmd
        public ICommand CancelCmd { get { return new RelayCommand(OnCancel); } }
        private void OnCancel()
        {
            _items = new ObservableCollection<T>(_dataAccess.ItemsList);
            this.RaisePropertyChanged(() => this.ItemList);
            SelectedItem = null;
            _deletedIds = new List<int>();
            _updatedIds = new List<int>();
        }
        #endregion
        #region CreateCmd
        public ICommand CreateCmd { get { return new RelayCommand(OnCreate); } }
        private void OnCreate()
        {
            T item = new T() { Enabled = true };
            ItemList.Add(item);
            SelectedItem = item;
        }
        #endregion
        #region DeleteCmd
        public RelayCommand<object> DeleteCmd { get { return new RelayCommand<object>(OnDelete, obj => (obj != null && ItemList.Count > 0 && ((Entity)obj).Enabled), false); } }
        private void OnDelete(object obj)
        {
            Entity item = (Entity)obj;
            if (item.Id > 0)
            {
                _deletedIds.Add(item.Id);
            }
            ItemList.Remove((T)obj);
        }
        #endregion
    }
}
