using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using GalaSoft.MvvmLight;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using MvvmDialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunalPayments.WPF.ViewModels
{
    public class ChartViewModel : ViewModelBase, IModalDialogViewModel
    {
        private double _axisMax;
        private double _axisMin;
        private readonly IPayment _payments;
        private readonly IDataAccess<Account> _accounts;
        public bool? DialogResult { get { return false; } }
        public ChartViewModel(IPayment payments, IDataAccess<Account> accounts)
        {
            _payments = payments;
            _accounts = accounts;
            var mapper = Mappers.Xy<MeasureModel>().X(model => model.DateTime.Ticks).Y(model => model.Value);
            Charting.For<MeasureModel>(mapper);
            YFormatter = value => value.ToString("C");
            XFormatter = value => new DateTime((long)value).ToString("dd.MMM.yyyy");
            AxisStep = TimeSpan.FromDays(182).Ticks;
            AxisUnit = TimeSpan.TicksPerDay * 182;
            Items = new SeriesCollection();
            var allPayments = _payments.ItemsList.Reverse().ToList();
            if (allPayments.Count() > 0)
            {
                AxisMin = allPayments.Min(p => p.PaymentDate).Ticks;
                AxisMax = allPayments.Max(p => p.PaymentDate).Ticks;
                Items.AddRange(allPayments.GroupBy(p => p.AccountId, p => p, (accId, p) => GetLine(accId, p)).ToList());
            }                        
        }
        public SeriesCollection Items { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                RaisePropertyChanged(() => AxisMax);
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                RaisePropertyChanged(() => AxisMin);
            }
        }

        private LineSeries GetLine(int accountId, IEnumerable<Payment> p)
        {
            LineSeries retVal = new LineSeries();
            retVal.Title = _accounts.Get(accountId).Name;
            retVal.Values = new ChartValues<MeasureModel>(p.Select(c => new MeasureModel { DateTime = c.PaymentDate, Value = Convert.ToDouble(c.Sum) }));
            return retVal;
        }
    }
    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}
