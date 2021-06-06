using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Fonet;

namespace CommunalPayments.Common.Reports
{
    public static class PaymentReport
    {
        public static string AppDataPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "poimenov", "CommunalPayments");
            }
        }
        public static void Create(Payment payment, ReportFormat format, BankInfo bank, out string reportPath)
        {
            var currAssembly = Assembly.GetExecutingAssembly();
            var fileName = string.Format("{0}.{1}", payment.Id, format);
            reportPath = Path.Combine(AppDataPath, "Reports", fileName);
            if (!Directory.Exists(Path.GetDirectoryName(reportPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
            }
            if (File.Exists(reportPath))
            {
                File.Delete(reportPath);
            }
            var args = new XsltArgumentList();
            args.AddParam("bankName", "", bank.Name);
            args.AddParam("bankAccountNumber", "", bank.AccountNumber);
            args.AddParam("bankEdrpou", "", bank.Edrpou);
            using (var writer = XmlWriter.Create(reportPath))
            {
                GetTransform(format).Transform(GetDocument(payment), args, writer);
            }
            if(format == ReportFormat.pdf && File.Exists(reportPath))
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

        private static void Driver_OnWarning(object driver, FonetEventArgs e)
        {
            Debug.WriteLine(string.Format("driver warning: {0}", e.GetMessage()));
        }

        private static void Driver_OnInfo(object driver, FonetEventArgs e)
        {
            Debug.WriteLine(string.Format("driver info: {0}", e.GetMessage()));
        }

        private static void Driver_OnError(object driver, FonetEventArgs e)
        {
            Debug.WriteLine(string.Format("driver error: {0}", e.GetMessage()));
        }

        public static string Create(Payment payment, ReportFormat format)
        {
            StringBuilder sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                GetTransform(format).Transform(GetDocument(payment), null, writer);
            }
            return sb.ToString();
        }
        private static XmlDocument GetDocument(Payment payment)
        {
            var ser = new XmlSerializer(typeof(Payment));
            StringBuilder sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                ser.Serialize(writer, payment);
            }
            var retVal = new XmlDocument();
            retVal.LoadXml(sb.ToString());
            return retVal;
        }
        private static XslCompiledTransform GetTransform(ReportFormat format)
        {
            XslCompiledTransform retVal = new XslCompiledTransform();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("CommunalPayments.Common.Reports.Payment.{0}.xslt", format);
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = XmlReader.Create(stream))
                {
                    retVal.Load(reader, null, null);
                }
            }
            return retVal;
        }
    }
    public enum ReportFormat
    {
        html,
        xml,
        pdf
    }
    public struct BankInfo
    {
        public BankInfo(string name, string accountNumber, string edrpou)
        {
            this.Name = name;
            this.AccountNumber = accountNumber;
            this.Edrpou = edrpou;
        }
        public string Name { get; }
        public string AccountNumber { get; }
        public string Edrpou { get; }
    }
}
