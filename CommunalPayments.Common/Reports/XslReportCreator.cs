using CommunalPayments.Common.Interfaces;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace CommunalPayments.Common.Reports
{
    internal class XslReportCreator : ReportCreator
    {
        private ReportFormat _format;
        public XslReportCreator(ReportFormat format)
        {
            _format = format;
        }
        public override void Create(Payment payment, BankInfo bank, string appDataPath, out string reportPath)
        {
            var currAssembly = Assembly.GetExecutingAssembly();
            var fileName = string.Format("{0}.{1}", payment.Id, _format);
            reportPath = Path.Combine(appDataPath, "Reports", fileName);
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
                GetTransform(_format).Transform(GetDocument(payment), args, writer);
            }
        }
        private XmlDocument GetDocument(Payment payment)
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
        private XslCompiledTransform GetTransform(ReportFormat format)
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
}
