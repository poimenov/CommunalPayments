using CommunalPayments.Common;
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
    public class RatesViewModel : DictionaryWindowViewModel<Rate>
    {
        public RatesViewModel(IDataAccess<Rate> dataAccess, ILog logger) : base(dataAccess, logger)
        {
            _columns.Add(new KeyValuePair<string, string>("Id", "Id"));
            _columns.Add(new KeyValuePair<string, string>("ServiceId", "Service.Name"));
            _columns.Add(new KeyValuePair<string, string>("DateFrom", "DateFrom"));
            _columns.Add(new KeyValuePair<string, string>("VolumeFrom", "VolumeFrom"));
            _columns.Add(new KeyValuePair<string, string>("Value", "Value"));
            _columns.Add(new KeyValuePair<string, string>("Measure", "Measure"));
            _columns.Add(new KeyValuePair<string, string>("Description", "Description"));
        }
        public override string ItemTypeName
        {
            get => App.ResGlobal.GetString("Rate"); }
    }
}
