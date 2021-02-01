using CommunalPayments.WPF;
using CommunalPayments.WPF.Resources;
using CommunalPayments.WPF.Themes;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Resources;
using System.Windows;

namespace CommunalPayments
{
    public partial class App : Application
    {
        public static ViewModelLocator Locator { get; set; }
        public static ResourceManager ResGlobal = new ResourceManager("CommunalPayments.WPF.Properties.Resource", typeof(App).Assembly);
        public App()
        {
            Locator = new ViewModelLocator();            
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Locator.Logger.Info("Application Startup");
            LanguageManager.Language = WPF.Properties.Settings.Default.DefaultLanguage;
            ThemeManager.Theme = WPF.Properties.Settings.Default.DefaultTheme;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.SetData("DataDirectory", Common.Reports.PaymentReport.AppDataPath);
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionOccured);
            CreateDatabase();
            Locator.Logger.Info("Starting App");
            LogMachineDetails();
        }
        static void UnhandledExceptionOccured(object sender, UnhandledExceptionEventArgs args)
        {
            var path = Path.Combine(Common.Reports.PaymentReport.AppDataPath, "log.txt");
            Locator.DialogService.ShowMessageBox(Locator.Main, String.Format(ResGlobal.GetString("ErrorMessage"), path, Environment.NewLine), ResGlobal.GetString("ErrorTitle"), MessageBoxButton.OK);
            Exception e = (Exception)args.ExceptionObject;
            Locator.Logger.Fatal("Application has crashed", e);
        }
        private void CreateDatabase()
        {
            var path = Path.Combine(Common.Reports.PaymentReport.AppDataPath, "CommunalPayments.db");
            if(!File.Exists(path))
            {
                try
                {
                    using (var db = Locator.DataContext)
                    {
                        db.Database.Migrate();
                        Locator.Logger.Info("Database created");
                    }
                }
                catch (Exception e)
                {
                    Locator.Logger.Fatal("Вatabase was not created", e);
                }                
            }
        }
        private void LogMachineDetails()
        {
            string text = string.Format("OS/Framework: {0}/{1}",System.Runtime.InteropServices.RuntimeInformation.OSDescription, System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
            Locator.Logger.Info(text);
        }
    }
}
