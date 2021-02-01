using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

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
        xml
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
