using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CommunalPayments.WPF.ViewModels
{
    public enum ColumnType
    {
        TextColumn,
        CheckBoxColumn,
        HyperlinkColumn
    }
    public class ColDescript
    {
        private ResourceManager _resourceManager;
        public ColDescript(string propertyName, string bindingPath)
        {
            PropertyName = propertyName;
            BindingPath = bindingPath;
            ColumnType = ColumnType.TextColumn;
            StringFormat = null;
        }
        public ColDescript(string propertyName, string bindingPath, ColumnType columnType)
        {
            PropertyName = propertyName;
            BindingPath = bindingPath;
            ColumnType = columnType;
            StringFormat = null;
        }
        public ColDescript(string propertyName, string bindingPath, ColumnType columnType, string stringFormat)
        {
            PropertyName = propertyName;
            BindingPath = bindingPath;
            ColumnType = columnType;
            StringFormat = stringFormat;
        }
        public string PropertyName { get; }
        public string BindingPath { get; }
        public ColumnType ColumnType { get; }
        public string StringFormat { get; }
        public string GetDisplayName(Type sourceType)
        {
            var property = sourceType.GetProperty(this.PropertyName);
            var displayAttributes = property.GetCustomAttributes(typeof(DisplayAttribute), true);
            if (displayAttributes.Any())
            {
                var displayAttribute = displayAttributes.First() as DisplayAttribute;
                if (null == _resourceManager)
                {
                    _resourceManager = new ResourceManager(displayAttribute.ResourceType.FullName, sourceType.Assembly);
                }
                return _resourceManager.GetString(displayAttribute.Name);
            }
            else
            {
                return this.PropertyName;
            }
        }
    }
    public abstract class DockWindowViewModel : ViewModelBase
    {
        protected ILog _logger;
        protected List<ColDescript> _columns;
        protected abstract Type GridItemType
        {
            get;
        }

        public DockWindowViewModel(ILog logger)
        {
            this.CanClose = true;
            this.IsClosed = false;
            this._logger = logger;
            this._columns = new List<ColDescript>();
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

        private Cursor _cursor = Cursors.Arrow;
        public Cursor Cursor
        {
            get { return _cursor; }
            set
            {
                if (_cursor != value)
                {
                    _cursor = value;
                    RaisePropertyChanged(nameof(Cursor));
                }
            }
        }

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
            if (_columns.Any(x => x.PropertyName == e.PropertyName))
            {
                var colDesc = _columns.First(x => x.PropertyName == e.PropertyName);
                DataGridBoundColumn column;
                switch (colDesc.ColumnType)
                {
                    case ColumnType.CheckBoxColumn:
                        column = new DataGridCheckBoxColumn();
                        break;
                    default:
                        column = new DataGridTextColumn();
                        break;

                }

                column.Binding = new Binding(colDesc.BindingPath);
                if (!string.IsNullOrEmpty(colDesc.StringFormat))
                {
                    column.Binding.StringFormat = colDesc.StringFormat;
                }
                else if (e.PropertyType == typeof(DateTime))
                {
                    //default date format
                    column.Binding.StringFormat = "dd MMM yyyy";
                }
                column.Header = colDesc.PropertyName;
                e.Column = column;
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region AutoGeneratedColumnCmd
        public RelayCommand<object> AutoGeneratedColumnCmd { get { return new RelayCommand<object>(OnAutoGeneratedColumn, obj => (obj != null), false); } }
        private void OnAutoGeneratedColumn(object obj)
        {
            ObservableCollection<DataGridColumn> columns = obj as ObservableCollection<DataGridColumn>;
            if (null != columns)
            {
                foreach (var colDesc in _columns)
                {
                    var column = columns.First(x => (string)x.Header == colDesc.PropertyName);
                    column.DisplayIndex = _columns.IndexOf(colDesc);
                    column.Header = colDesc.GetDisplayName(GridItemType);
                }
            }
        } 
        #endregion
    }
}
