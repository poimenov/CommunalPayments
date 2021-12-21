using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.Common.Reports;
using HtmlAgilityPack;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CommunalPayments.Erc.Repository
{
    public class NetRepository : INetRepository
    {
        private readonly ILog _logger;
        private const string baseUrl = "https://erc.megabank.ua";
        private const string loginUrl = "/ru/node?destination=node";
        private const string accountsUrl = "/ru/service/publicutilities";
        private const string billsUrl = "/ru/cabinet/resp/billlist/5?order=asc";
        private const string orderUrl = "/ru/service/resp/pays/orderedit/";
        private const string paysOrderUrl = "/ru/service/resp/pays/order";
        //{0} - квартира
        //{2} - ГодМесяц
        private const string debtUrl = "/ru/service/resp/debt/{0}/{1}?order=asc";
        //{0} - квартира
        //{1} - оплата по графе начислено(0)/долг(1)/остаток долга(2)
        //{2} - ГодМесяц
        //пример:
        //https://erc.megabank.ua/ru/service/publicutilities/paysdebt/2/2/202104
        private const string createPaymentUrl = "/ru/service/publicutilities/paysdebt/{0}/{1}/{2}";
        private const string deletePaymentUrl = "/ru/cabinet/resp/orderdelete";
        private const string checkOutUrl = "/ru/checkout/resp/list";
        private const string bankTransfer = "/ru/checkout/banktransfer";
        private static readonly HttpClient client;
        public string Login { set; get; }
        public string Password { set; get; }
        static NetRepository()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
        }
        public NetRepository(ILog logger)
        {
            _logger = logger;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
        public async IAsyncEnumerable<(Common.Bill, int, int)> GetBills(IEnumerable<Account> accounts, IEnumerable<Service> services)
        {
            var retVal = new List<Common.Bill>();

            if (await Login2Site())
            {
                var bills = await GetBillsList(accounts);
                int i = 0;
                var count = bills.SelectMany(b => b.Payments, (b, p) => new { b, p }).Count();
                foreach (var bill in bills.OrderBy(b => b.ErcId))
                {
                    foreach (var payment in bill.Payments.OrderBy(b => b.ErcId))
                    {
                        payment.PaymentItems.AddRange(await GetPaymentItems(payment.ErcId, services));
                        i++;
                    }
                    yield return (bill, count, i);
                }
            }
        }
        public async Task<Debt> GetDebt(Account account, DateTime date)
        {
            var retVal = new Debt(account, date);
            try
            {
                if (!await Login2Site())
                {
                    return retVal;
                }
                retVal.DebtItems = await GetDebts(account.InternalId, retVal.YearMonths);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw ex;
            }
            return retVal;
        }
        public async Task<Payment> CreatePayment(Account account, PayBy payBy, DateTime date, IEnumerable<Service> services)
        {
            var retVal = new Payment();
            retVal.Account = account;
            retVal.AccountId = account.Id;
            retVal.Enabled = true;
            try
            {
                if (!await Login2Site())
                {
                    return null;
                }
                using (HttpResponseMessage response = await client.GetAsync(string.Format(createPaymentUrl, account.InternalId, (int)payBy, date.AddMonths(-1).ToString("yyyyMM"))))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var url = response.RequestMessage.RequestUri.ToString();
                        if (!url.Contains("/ru/service/publicutilities/orderedit"))
                        {
                            return null;
                        }
                        retVal.ErcId = Convert.ToInt64(url.Split('/').Last());
                        var page = await response.Content.ReadAsStringAsync();
                        var doc = new HtmlDocument();
                        doc.LoadHtml(page);
                        var json = doc.DocumentNode.SelectSingleNode("//script[text()[contains(.,'\"bbl\":')]]").InnerText.Replace("jQuery.extend(Drupal.settings, ", "").Replace(");", "");
                        using (var reader = new JsonTextReader(new StringReader(json)))
                        {
                            while (reader.Read())
                            {
                                if (reader.Value != null && Convert.ToString(reader.Value) == "bbl")
                                {
                                    reader.Read();
                                    retVal.Bbl = Convert.ToString(reader.Value);
                                    break;
                                }
                            }
                        }
                        retVal.PaymentItems.AddRange(await GetPaymentItems(retVal.ErcId, services));
                        retVal.PaymentItems.ForEach(item => item.Enabled = true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw ex;
            }
            return retVal;
        }
        public async Task<bool> UpdatePayment(Payment payment)
        {
            bool retVal = false;
            if (payment.ErcId > 0 && !String.IsNullOrEmpty(payment.Bbl) && null != payment.Account && payment.Account.Number > 0)
            {
                try
                {
                    if (!await Login2Site())
                    {
                        return retVal;
                    }
                    foreach (var item in payment.PaymentItems.Where(p => string.IsNullOrEmpty(p.Options)))
                    {
                        item.Options = GetTime();
                    }
                    var orderItems = new List<OrderItem>();
                    foreach (var item in payment.PaymentItems.OrderBy(p => p.ServiceId))
                    {
                        orderItems.Add(new OrderItem()
                        {
                            Service = item.Service.ErcId,
                            Amount = item.Amount,
                            CurCounter = item.CurrentIndication.HasValue ? item.CurrentIndication.Value : 0m,
                            PrevCounter = item.LastIndication.HasValue ? item.LastIndication.Value : 0m,
                            Diff = item.Value.HasValue ? item.Value.Value : 0m,
                            Month1 = item.PeriodFrom.ToString("yyyyMM"),
                            Month2 = item.PeriodTo.ToString("yyyyMM"),
                            Options = item.Options
                        });
                    }

                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("sorder", JsonConvert.SerializeObject(orderItems).ToLower()));
                    postData.Add(new KeyValuePair<string, string>("cdf", payment.Account.Number.ToString()));
                    postData.Add(new KeyValuePair<string, string>("idf", "0"));
                    postData.Add(new KeyValuePair<string, string>("ido", payment.ErcId.ToString()));
                    postData.Add(new KeyValuePair<string, string>("bbl", payment.Bbl));

                    using (var content = new FormUrlEncodedContent(postData))
                    {
                        using (var response = await client.PostAsync(paysOrderUrl, content))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    throw ex;
                }
            }
            return retVal;
        }
        public async Task<bool> DeletePayment(Payment payment)
        {
            bool retVal = false;
            if (payment.ErcId > 0 && !String.IsNullOrEmpty(payment.Bbl))
            {
                try
                {
                    if (!await Login2Site())
                    {
                        return retVal;
                    }
                    var ids = new long[] { payment.ErcId };
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("sorder", JsonConvert.SerializeObject(ids)));
                    using (var content = new FormUrlEncodedContent(postData))
                    {
                        using (var response = await client.PostAsync(deletePaymentUrl, content))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    throw ex;
                }
            }
            return retVal;
        }
        public async Task<Common.Bill> CreateInvoice(PaymentMode mode, IEnumerable<long> paymentErcIds)
        {
            Common.Bill retVal = null;
            try
            {
                if (!await Login2Site())
                {
                    return null;
                }

                JsonSerializer serializer = new JsonSerializer();
                StringBuilder sBuilder = new StringBuilder();
                using (StringWriter sWriter = new StringWriter(sBuilder))
                {
                    using (JsonTextWriter jtWriter = new JsonTextWriter(sWriter))
                    {
                        serializer.Serialize(jtWriter, paymentErcIds.Select(x => x.ToString()));
                    }
                }

                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("sorder", sBuilder.ToString()));                
                string url = null;
                using (var content = new FormUrlEncodedContent(postData))
                {
                    using (var response = await client.PostAsync(checkOutUrl, content))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {

                            var contentString = await response.Content.ReadAsStringAsync();
                            using (var reader = new JsonTextReader(new StringReader(contentString)))
                            {
                                while (reader.Read())
                                {
                                    if (reader.Value != null && Convert.ToString(reader.Value) == "url")
                                    {
                                        reader.Read();
                                        url = Convert.ToString(reader.Value);
                                    }
                                }
                            }
                        }
                    }
                }

                string u = null;
                if (null != url)
                {
                    using (var response = await client.GetAsync(url))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            u = HttpUtility.ParseQueryString(url).Get("u");
                        }
                    }
                }

                if (null != u)
                {
                    switch (mode)
                    {
                        case PaymentMode.BankTransfer:
                            retVal = await CreateInvoiceByBankTransfer(u, paymentErcIds);
                            break;
                        case PaymentMode.BankCard:
                            throw new NotImplementedException();
                        default:
                            throw new NotImplementedException();
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw ex;
            }
            return retVal;
        }
        public string GetReportPath(PaymentMode mode, long ercId)
        {
            return Path.Combine(PaymentReport.AppDataPath, "Reports", $"{mode}_{ercId}.htm");
        }
        #region Private methods
        private DateTime PeriodFrom(string s)
        {
            DateTime retVal = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(s) && s.Length == 6)
            {
                int year = Convert.ToInt32(s.Substring(0, 4));
                int month = Convert.ToInt32(s.Substring(4, 2));
                retVal = new DateTime(year, month, 1);
            }
            return retVal;
        }
        private DateTime PeriodTo(DateTime periodFrom, string s)
        {
            DateTime retVal = periodFrom.AddMonths(1);
            if (!string.IsNullOrWhiteSpace(s) && s.Length == 6)
            {
                int year = Convert.ToInt32(s.Substring(0, 4));
                int month = Convert.ToInt32(s.Substring(4, 2));
                retVal = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
            }
            return retVal;
        }
        private async Task<bool> Login2Site()
        {
            bool retVal = false;
            if (string.IsNullOrWhiteSpace(this.Login) || string.IsNullOrWhiteSpace(this.Password))
            {
                return retVal;
            }

            var loginPage = await client.GetStringAsync("/ru");
            var doc = new HtmlDocument();
            doc.LoadHtml(loginPage);
            var formBuildIdInput = doc.DocumentNode.SelectSingleNode("//input[@name=\"form_build_id\"]");
            if (formBuildIdInput == null)
            {
                return true;
            }

            var formBuildId = formBuildIdInput.Attributes["value"].Value;
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("name", this.Login));
            postData.Add(new KeyValuePair<string, string>("pass", this.Password));
            postData.Add(new KeyValuePair<string, string>("op", "Войти"));
            postData.Add(new KeyValuePair<string, string>("form_build_id", formBuildId));
            postData.Add(new KeyValuePair<string, string>("form_id", "user_login_block"));

            using (var content = new FormUrlEncodedContent(postData))
            {
                using (var response = await client.PostAsync(loginUrl, content))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var strContent = await response.Content.ReadAsStringAsync();
                        if (strContent.Contains("Извините, это имя пользователя или пароль неверны"))
                        {
                            return false;
                        }
                        retVal = true;
                    }
                    else
                    {
                        _logger.Info(string.Format("Post request to {0} returns status {1} at {2}", string.Concat(baseUrl, loginUrl), response.StatusCode, DateTime.Now));
                    }
                }
            }

            return retVal;
        }
        public async Task<IEnumerable<Account>> GetAccounts(int userId)
        {
            List<Account> retVal = new List<Account>();
            try
            {
                if (!await Login2Site())
                {
                    return retVal;
                }
                var page = await client.GetStringAsync(accountsUrl);

                var doc = new HtmlDocument();
                doc.LoadHtml(page);
                foreach (var div in doc.DocumentNode.SelectNodes("//div[@class=\"block_lic\"]"))
                {
                    var account = new Account() { PersonId = userId, Enabled = true, Payments = new List<Payment>() };
                    account.Number = Convert.ToInt64(div.SelectSingleNode("h3[@class=\"title\"]/child::text()[1]").InnerText);
                    account.Key = div.SelectSingleNode("a[text()=\"Оплатить\"]").Attributes["href"].Value.Substring(29);
                    account.InternalId = Convert.ToInt32(div.SelectSingleNode("a[text()=\"Данные о задолженности\"]").Attributes["href"].Value.Substring(33));
                    account.City = div.SelectSingleNode("p[@class=\"addr_lic\"]/child::text()[1]").InnerText;
                    account.Street = div.SelectSingleNode("p[@class=\"addr_lic\"]/child::text()[2]").InnerText;
                    account.Building = div.SelectSingleNode("p[@class=\"addr_lic\"]/em[1]").InnerText;
                    account.Apartment = div.SelectSingleNode("p[@class=\"addr_lic\"]/em[2]").InnerText;
                    retVal.Add(account);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw ex;
            }
            return retVal;
        }
        private async Task<List<Common.Bill>> GetBillsList(IEnumerable<Account> accounts)
        {
            List<Common.Bill> retVal = new List<Common.Bill>();
            var billsJson = await client.GetStringAsync(billsUrl);
            IList<Bill> bills = new List<Bill>();
            using (JsonTextReader reader = new JsonTextReader(new StringReader(billsJson)))
            {
                if (reader.Read())
                {
                    JsonSerializer serializer = new JsonSerializer();
                    bills = serializer.Deserialize<IList<Bill>>(reader);
                }
            }

            CultureInfo culture = new CultureInfo("ru-ru");
            foreach (var bill in bills)
            {
                var item = new Common.Bill()
                {
                    Enabled = false,
                    ErcId = bill.IdBill,
                    ModeId = bill.IdPayMode,
                    Mode = new PayMode() { Id = bill.IdPayMode, Name = bill.PayModeName },
                    StatusId = bill.Status,
                    Status = new PayStatus() { Id = bill.Status, Name = bill.StatusName },
                    CreateDate = DateTime.Now
                };
                DateTime dt;
                // example DateTime format: "15.09.2020 16:04:46"
                if (DateTime.TryParse(bill.DtCreate, culture, DateTimeStyles.AllowWhiteSpaces, out dt))
                {
                    item.CreateDate = dt;
                }
                if (null != bill.OrderList)
                {
                    foreach (var order in bill.OrderList)
                    {
                        var payment = new Payment()
                        {
                            AccountId = accounts.First(x => x.Number == order.CdFlat).Id,
                            Enabled = false,
                            ErcId = order.IdOrder,
                            Comment = order.Comment,
                            Commission = order.Commission,
                            PaymentDate = DateTime.Now
                        };
                        if (DateTime.TryParse(order.DtCreate, culture, DateTimeStyles.AllowWhiteSpaces, out dt))
                        {
                            payment.PaymentDate = dt;
                        }
                        item.Payments.Add(payment);
                    }
                }
                retVal.Add(item);
            }
            return retVal;
        }
        private async Task<List<PaymentItem>> GetPaymentItems(long idOrder, IEnumerable<Service> services)
        {
            List<PaymentItem> retVal = new List<PaymentItem>();
            var orderJson = await client.GetStringAsync(orderUrl + idOrder.ToString());
            IList<OrderItem> orderItems = new List<OrderItem>();
            using (JsonTextReader reader = new JsonTextReader(new StringReader(orderJson)))
            {
                if (reader.Read())
                {
                    JsonSerializer serializer = new JsonSerializer();
                    orderItems = serializer.Deserialize<IList<OrderItem>>(reader);
                }
            }
            foreach (var orderItem in orderItems)
            {
                var paymentItem = new PaymentItem()
                {
                    Amount = orderItem.Amount,
                    CurrentIndication = orderItem.CurCounter,
                    LastIndication = orderItem.PrevCounter,
                    Value = orderItem.Diff,
                    Options = orderItem.Options,
                    ServiceId = services.First(x => x.ErcId == orderItem.Service).Id,
                    PeriodFrom = PeriodFrom(orderItem.Month1)
                };
                paymentItem.PeriodTo = PeriodTo(paymentItem.PeriodFrom, orderItem.Month2);
                retVal.Add(paymentItem);
            }
            return retVal;
        }
        private async Task<IList<DebtItem>> GetDebts(int internalId, string yearM)
        {
            IList<DebtItem> retVal = new List<DebtItem>();
            var debtJson = await client.GetStringAsync(string.Format(debtUrl, internalId, yearM));
            debtJson = debtJson.Replace("\"\\u0026#1053;\\/\\u0026#1044;\"", "null");
            using (JsonTextReader reader = new JsonTextReader(new StringReader(debtJson)))
            {
                if (reader.Read())
                {
                    JsonSerializer serializer = new JsonSerializer();
                    retVal = serializer.Deserialize<IList<DebtItem>>(reader);
                }
            }
            return retVal;
        }
        private string GetTime()
        {
            var time = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1));
            return Convert.ToString(Math.Round(time.TotalMilliseconds));
        }
        private async Task<Common.Bill> CreateInvoiceByBankTransfer(string u, IEnumerable<long> paymentErcIds)
        {
            var retVal = new Common.Bill();
            retVal.Enabled = true;
            retVal.ModeId = (int)PaymentMode.BankTransfer;
            retVal.StatusId = (int)PaymentStatus.Created;
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("orderlist", string.Join(',', paymentErcIds.ToArray()) + ","));
            postData.Add(new KeyValuePair<string, string>("u", u));
            postData.Add(new KeyValuePair<string, string>("p", "0"));
            using (var content = new FormUrlEncodedContent(postData))
            {
                using (var response = await client.PostAsync(bankTransfer, content))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var page = await response.Content.ReadAsStringAsync();
                        var doc = new HtmlDocument();
                        doc.LoadHtml(page);
                        var purposeOfPayment = doc.DocumentNode.SelectSingleNode("//div[@class=\"banktransferinfo_line\"]/span/textarea").InnerText;
                        retVal.ErcId = Convert.ToInt64(purposeOfPayment.Split(';')[1]);
                        retVal.CreateDate = DateTime.Now;
                        var spans = doc.DocumentNode.SelectNodes("//div[@class=\"pay_conf_order_header\"]/span[@style=\"color:rgb(240, 80, 51);\"]");
                        int i = 0;
                        foreach (var span in spans)
                        {
                            var payment = new Common.Payment();
                            payment.ErcId = paymentErcIds.ElementAt(i);
                            payment.Commission = 0m;
                            decimal commission;
                            var style = NumberStyles.AllowDecimalPoint;
                            var culture = CultureInfo.CreateSpecificCulture("en-US");
                            if (decimal.TryParse(span.InnerText.Replace(" грн.", string.Empty), style, culture, out commission))
                            {
                                payment.Commission = commission;
                            }
                            retVal.Payments.Add(payment);
                            i++;
                        }
                        //сохранить страницу на диске и открыть в браузере.
                        using (var writer = File.CreateText(GetReportPath(PaymentMode.BankTransfer, retVal.ErcId)))
                        {
                            writer.Write(page);
                        }
                    }
                }
            }
            return retVal;
        }

        #endregion

    }
}
