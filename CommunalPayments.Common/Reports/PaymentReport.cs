using CommunalPayments.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommunalPayments.Common.Reports
{
    public static class PaymentReport
    {
        private static readonly Dictionary<ReportFormat, Func<ReportCreator>> _map = new Dictionary<ReportFormat, Func<ReportCreator>>();
        static PaymentReport()
        {
            _map[ReportFormat.html] = () => new XslReportCreator(ReportFormat.html);
            _map[ReportFormat.xml] = () => new XslReportCreator(ReportFormat.xml);
            _map[ReportFormat.pdf] = () => new PdfReportCreator();
        }
        public static string AppDataPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "poimenov", "CommunalPayments");
            }
        }
        public static void Create(Payment payment, ReportFormat format, BankInfo bank, out string reportPath)
        {
            reportPath = string.Empty;
            var creator = GetCreator(format);
            if(null != creator)
            {
                creator().Create(payment, bank, AppDataPath, out reportPath);
            }

        }
        private static Func<ReportCreator> GetCreator(ReportFormat format)
        {
            Func<ReportCreator> retVal;
            _map.TryGetValue(format, out retVal);
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
