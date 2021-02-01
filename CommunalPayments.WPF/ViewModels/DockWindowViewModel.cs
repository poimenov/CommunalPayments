using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CommunalPayments.WPF.ViewModels
{
    public abstract class DockWindowViewModel : ViewModelBase
    {
        protected ILog _logger;
        protected List<KeyValuePair<string, string>> _columns;
        private ResourceManager _resourceManager;
        protected string GetDisplayName(string propertyName)
        {
            try
            {
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
                _logger.Error(string.Format("Exception in DictionaryWindowViewModel.GetDisplayName(propertyName=\"{0}\")", propertyName), ex);
                return propertyName;
            }
        }
        protected virtual Type GetGridItemType
        {
            get
            {
                return typeof(Entity);
            }
        }
        public DockWindowViewModel(ILog logger)
        {
            this.CanClose = true;
            this.IsClosed = false;
            this._logger = logger;
            this._columns = new List<KeyValuePair<string, string>>();
        }
        #region Properties

        public DockManagerViewModel DockManager { get; set; }
        public abstract string ItemTypeName { get; }

        #region CloseCommand
        private ICommand _CloseCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                    _CloseCommand = new RelayCommand(() => Close());
                return _CloseCommand;
            }
        }
        #endregion

        #region IsClosed
        private bool _IsClosed;
        public bool IsClosed
        {
            get { return _IsClosed; }
            set
            {
                if (_IsClosed != value)
                {
                    _IsClosed = value;
                    this.RaisePropertyChanged(nameof(IsClosed));
                }
            }
        }
        #endregion

        #region CanClose
        private bool _CanClose;
        public bool CanClose
        {
            get { return _CanClose; }
            set
            {
                if (_CanClose != value)
                {
                    _CanClose = value;
                    RaisePropertyChanged(nameof(CanClose));
                }
            }
        }
        #endregion

        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    RaisePropertyChanged(nameof(Title));
                }
            }
        }
        #endregion

        public ImageSource IconSource
        {
            get;
            protected set;
        }

        #region ContentId

        private string _contentId = null;
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    this.RaisePropertyChanged(() => this.ContentId);
                }
            }
        }

        #endregion

        #region IsSelected

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    this.RaisePropertyChanged(() => this.IsSelected);
                }
            }
        }

        #endregion

        #region IsActive

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    this.RaisePropertyChanged(() => this.IsActive);
                }
            }
        }

        #endregion

        #endregion
        public void Close()
        {
            if (this.CanClose)
            {
                this.IsClosed = true;
            }
        }
        #region AutoGeneratingColumnCmd
        public RelayCommand<object> AutoGeneratingColumnCmd { get { return new RelayCommand<object>(OnAutoGeneratingColumn, obj => (obj != null), false); } }
        private void OnAutoGeneratingColumn(object obj)
        {
            var e = obj as DataGridAutoGeneratingColumnEventArgs;
            if (_columns.Any(x => x.Key == e.PropertyName))
            {
                var kvp = _columns.First(x => x.Key == e.PropertyName);
                var col = new DataGridTextColumn();
                col.Binding = new Binding(kvp.Value);
                if (e.PropertyType == typeof(DateTime))
                {
                    if (e.PropertyName == "PeriodFrom" || e.PropertyName == "PeriodTo")
                    {
                        col.Binding.StringFormat = "MMMM yyyy";
                    }
                    else
                    {
                        col.Binding.StringFormat = "dd MMM yyyy";
                    }
                }
                col.Header = GetDisplayName(e.PropertyName);
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
