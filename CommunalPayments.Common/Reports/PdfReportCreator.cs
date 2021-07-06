using Fonet;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace CommunalPayments.Common.Reports
{
    internal class PdfReportCreator : XslReportCreator
    {
        public PdfReportCreator(): base(ReportFormat.pdf)
        {
        }
        public override void Create(Payment payment, BankInfo bank, string appDataPath, out string reportPath)
        {
            base.Create(payment, bank, appDataPath, out reportPath);
            if (File.Exists(reportPath))
            {
                var driver = Fonet.FonetDriver.Make();
                driver.Options = new Fonet.Render.Pdf.PdfRendererOptions
                {
                    FontType = Fonet.Render.Pdf.FontType.Subset
                };
                driver.OnError += Driver_OnError;
                driver.OnInfo += Driver_OnInfo;
                driver.OnWarning += Driver_OnWarning;
                XmlDocument document = new XmlDocument();
                document.Load(reportPath);
                using (var stream = File.OpenWrite(reportPath))
                {
                    driver.Render(document, stream);
                }
            }
        }
        private void Driver_OnWarning(object driver, FonetEventArgs e)
        {
            Debug.WriteLine(string.Format("driver warning: {0}", e.GetMessage()));
        }

        private void Driver_OnInfo(object driver, FonetEventArgs e)
        {
            Debug.WriteLine(string.Format("driver info: {0}", e.GetMessage()));
        }

        private void Driver_OnError(object driver, FonetEventArgs e)
        {
            Debug.WriteLine(string.Format("driver error: {0}", e.GetMessage()));
        }
    }
}
