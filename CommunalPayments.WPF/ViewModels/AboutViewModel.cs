using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using System;
using System.Diagnostics;
using System.Windows.Navigation;

namespace CommunalPayments.WPF.ViewModels
{
    class AboutViewModel : ViewModelBase, IModalDialogViewModel
    {
        public bool? DialogResult { get { return false; } }

        public string Content
        {
            get
            {
                return "CommunalPayments" + Environment.NewLine + App.ResGlobal.GetString("About");
            }
        }

        public string VersionText
        {
            get
            {
                var version1 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                // For external assemblies
                // var ver2 = typeof(Assembly1.ClassOfAssembly1).Assembly.GetName().Version;
                // var ver3 = typeof(Assembly2.ClassOfAssembly2).Assembly.GetName().Version;

                return "CommunalPayments v " + version1.ToString();
            }
        }

        public RelayCommand<object> NavigateCmd { get { return new RelayCommand<object>(OnNavigate, obj => (obj != null), false); } }
        private void OnNavigate(object obj)
        {
            RequestNavigateEventArgs e = obj as RequestNavigateEventArgs;
            if (e != null)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"/c start {e.Uri.AbsoluteUri}"
                };
                Process.Start(psi);
                e.Handled = true;
            }
        }
    }
}
