using CommonServiceLocator;
using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.DataAccess;
using CommunalPayments.DataAccess.Database;
using CommunalPayments.Erc.Repository;
using CommunalPayments.WPF.ViewModels;
using CommunalPayments.WPF.ViewModels.Dictionary;
using GalaSoft.MvvmLight.Ioc;
using log4net;
using MvvmDialogs;
using System;
using System.Reflection;

namespace CommunalPayments.WPF
{
    public class ViewModelLocator
    {
        private ServicesViewModel _services;
        private RatesViewModel _rates;
        private PersonsViewModel _persons;
        private AccountsViewModel _accounts;
        private ChartViewModel _chart;
        private CircleChartViewModel _circleChart;
        static ViewModelLocator()
        {
            //if (ViewModelBase.IsInDesignModeStatic)
            //{
            //    SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            //}
            //else
            //{
            //    SimpleIoc.Default.Register<IDataService, DataService>();
            //}
            log4net.Config.XmlConfigurator.Configure();
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<ILog>(() => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType));
            SimpleIoc.Default.Register<IDialogService>(()=>new DialogService());
            SimpleIoc.Default.Register<IDataAccess<Service>, Services>();
            SimpleIoc.Default.Register<IDataAccess<Bill>, Bills>();
            SimpleIoc.Default.Register<IDataAccess<Rate>, Rates>();
            SimpleIoc.Default.Register<IDataAccess<Person>, Persons>();
            SimpleIoc.Default.Register<IPayment, Payments>();
            SimpleIoc.Default.Register<IDataAccess<Account>, Accounts>();
            SimpleIoc.Default.Register<INetRepository, NetRepository>();
            SimpleIoc.Default.Register<IImporter, Importer>();
            SimpleIoc.Default.Register<DataModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<PaymentsViewModel>();
            SimpleIoc.Default.Register<PaymentDetailViewModel>();
            SimpleIoc.Default.Register<ServicesViewModel>();
            SimpleIoc.Default.Register<RatesViewModel>();
            SimpleIoc.Default.Register<PersonsViewModel>();
            SimpleIoc.Default.Register<AccountsViewModel>();
            SimpleIoc.Default.Register<ChartViewModel>();
            SimpleIoc.Default.Register<CircleChartViewModel>();
        }

        public DataModel DataContext
        {
            get
            {
                return SimpleIoc.Default.GetInstance<DataModel>(Guid.NewGuid().ToString());
            }
        }
        public MainViewModel Main
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MainViewModel>();
            }
        }
        public PaymentsViewModel Payments
        {
            get
            {
                return SimpleIoc.Default.GetInstance<PaymentsViewModel>(Guid.NewGuid().ToString());
            }
        }
        public PaymentDetailViewModel PaymentDetail
        {
            get
            {
                return SimpleIoc.Default.GetInstance<PaymentDetailViewModel>(Guid.NewGuid().ToString());
            }
        }
        public ServicesViewModel Services
        {
            get
            {
                if(_services == null)
                {
                    _services = SimpleIoc.Default.GetInstance<ServicesViewModel>();
                }
                return _services;
            }
        }
        public RatesViewModel Rates
        {
            get
            {
                if(_rates == null)
                {
                    _rates = SimpleIoc.Default.GetInstance<RatesViewModel>();
                }
                return _rates;
            }
        }
        public PersonsViewModel Persons
        {
            get
            {
                if(_persons == null)
                {
                    _persons = SimpleIoc.Default.GetInstance<PersonsViewModel>();
                }
                return _persons;
            }
        }
        public AccountsViewModel Accounts
        {
            get
            {
                if(_accounts == null)
                {
                    _accounts = SimpleIoc.Default.GetInstance<AccountsViewModel>();
                }
                return _accounts;
            }
        }
        public ChartViewModel LineChart
        {
            get
            {
                if(_chart == null)
                {
                    _chart = SimpleIoc.Default.GetInstance<ChartViewModel>();
                }
                return _chart;
            }
        }
        public CircleChartViewModel CircleChart
        {
            get
            {
                if(_circleChart == null)
                {
                    _circleChart = SimpleIoc.Default.GetInstance<CircleChartViewModel>();
                }
                return _circleChart;
            }
        }
        public ILog Logger
        {
            get
            {
                return SimpleIoc.Default.GetInstance<ILog>();
            }
        }
        public IDialogService DialogService
        {
            get
            {
                return SimpleIoc.Default.GetInstance<IDialogService>();
            }
        }
    }
}
