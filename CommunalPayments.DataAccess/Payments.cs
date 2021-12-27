using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunalPayments.DataAccess
{
    public class Payments : IPayment
    {
        public IEnumerable<Payment> ItemsList
        {
            get
            {
                using (var db = new DataModel())
                {
                    if (db.Payments.Count() == 0)
                    {
                        return new List<Payment>();
                    }
                    return db.Payments.Include(x => x.PaymentItems).ThenInclude(x => x.Service).OrderByDescending(x => x.PaymentDate).ToList();
                }
            }
        }

        public void Create(IEnumerable<Payment> payments)
        {
            if (null == payments || payments.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var payment in payments)
                {
                    db.Payments.Add(Clone(payment));
                }
                db.SaveChanges();
            }
        }

        public int Create(Payment payment)
        {
            using (var db = new DataModel())
            {
                var entity = Clone(payment);
                db.Payments.Add(entity);
                db.SaveChanges();
                return entity.Id;
            }
        }

        public void Delete(IEnumerable<int> paymentIds)
        {
            if (null == paymentIds || paymentIds.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var id in paymentIds)
                {
                    var payment = db.Payments.First(x => x.Id == id);
                    db.Payments.Remove(payment);
                }
                db.SaveChanges();
            }
        }

        public Payment Get(int paymentId)
        {
            using (var db = new DataModel())
            {
                return db.Payments.Include(x => x.Account)
                    .Include(x => x.Account.Person)
                    .Include(x => x.PaymentItems).ThenInclude(x => x.Service).First(x => x.Id == paymentId);
            }
        }

        public IEnumerable<Payment> GetPaymentsByAccountId(int accountId)
        {
            using (var db = new DataModel())
            {
                var query = db.Payments.Where(x => x.AccountId == accountId).Select(x => x);
                if (query.Count() > 0)
                {
                    return query.Include(x => x.Account.Person)
                                .Include(x => x.PaymentItems)
                                .ThenInclude(x => x.Service)
                                .OrderByDescending(x => x.PaymentDate)
                                .ToList();
                }
                else
                {
                    return Enumerable.Empty<Payment>();
                }
            }
        }

        public IEnumerable<Payment> GetUnpaidPaymentsByPersonId(int personId)
        {
            using (var db = new DataModel())
            {
                var accontIds = db.Accounts.Where(x => x.PersonId == personId).Select(x => x.Id).ToList();
                var query = db.Payments.Where(x => !x.BillId.HasValue && x.ErcId > 0 && x.Enabled && accontIds.Contains(x.AccountId));
                if (query.Count() > 0)
                {
                    return query.Include(x => x.Account.Person)
                                .Include(x => x.PaymentItems)
                                .ThenInclude(x => x.Service)
                                .OrderByDescending(x => x.PaymentDate)
                                .ToList();
                }
                else
                {
                    return Enumerable.Empty<Payment>();
                }
            }
        }

        public Payment LastPayment(int accountId)
        {
            using (var db = new DataModel())
            {
                if (!db.Payments.Any(x => x.AccountId == accountId))
                {
                    return null;
                }
                var retVal = db.Payments.Where(x => x.AccountId == accountId)
                                        .Include(x => x.PaymentItems)
                                        .ThenInclude(x => x.Service)
                                        .OrderByDescending(x => x.PaymentDate)
                                        .FirstOrDefault();
                foreach(var service in db.Services)
                {
                    var paymentItems = db.PaymentItems.Where(p => p.CurrentIndication.HasValue && 
                                                                  p.ServiceId == service.Id && 
                                                                  p.Payment.AccountId == accountId)
                                                      .OrderByDescending(p=>p.Payment.PaymentDate);
                    if(paymentItems.Count() > 0)
                    {
                        if(retVal.PaymentItems.Any(p=>p.ServiceId == service.Id))
                        {
                            //обновить на случай, если показания счётчика в прошлый платёж не передавались
                            retVal.PaymentItems.First(p => p.ServiceId == service.Id).CurrentIndication = paymentItems.First().CurrentIndication;
                        }
                        else
                        {
                            //добавить строку если по ней в прошлый раз не платили
                            retVal.PaymentItems.Add(paymentItems.First());
                        }
                    }
                }
                return retVal;
            }
        }

        public void Update(IEnumerable<Payment> payments)
        {
            if (null == payments || payments.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var payment in payments)
                {
                    var entity = db.Payments.Include(x => x.PaymentItems).First(x => x.Id == payment.Id);
                    entity.AccountId = payment.AccountId;
                    entity.Comment = payment.Comment;
                    entity.Enabled = payment.Enabled;
                    entity.PaymentDate = payment.PaymentDate;
                    entity.ErcId = payment.ErcId;
                    entity.BillId = payment.BillId;
                    //payment item ids in payment
                    var ids = payment.PaymentItems.Where(x => x.Id > 0).Select(x => x.Id).ToList();
                    //payment item ids to delete
                    var deletedIds = entity.PaymentItems.Where(x => !ids.Contains(x.Id)).Select(x => x.Id).ToList();
                    foreach (var id in deletedIds)
                    {
                        //delete payment item
                        db.PaymentItems.Remove(entity.PaymentItems.First(x => x.Id == id));
                    }
                    //add new items
                    foreach (var paymentItem in payment.PaymentItems.Where(x => x.Id == 0))
                    {
                        var item = new PaymentItem();
                        item.Amount = paymentItem.Amount;
                        item.CurrentIndication = paymentItem.CurrentIndication;
                        item.LastIndication = paymentItem.LastIndication;
                        item.PeriodFrom = paymentItem.PeriodFrom;
                        item.PeriodTo = paymentItem.PeriodTo;
                        item.ServiceId = paymentItem.ServiceId;
                        item.Value = paymentItem.Value;
                        item.Enabled = payment.Enabled;
                        entity.PaymentItems.Add(item);
                    }
                    //update other payment items
                    foreach (var id in ids)
                    {
                        var item = db.PaymentItems.First(x => x.Id == id);
                        var upd = payment.PaymentItems.First(x => x.Id == id);
                        item.Amount = upd.Amount;
                        item.CurrentIndication = upd.CurrentIndication;
                        item.LastIndication = upd.LastIndication;
                        item.PeriodFrom = upd.PeriodFrom;
                        item.PeriodTo = upd.PeriodTo;
                        item.ServiceId = upd.ServiceId;
                        item.Value = upd.Value;
                        item.Enabled = upd.Enabled;
                    }
                }
                db.SaveChanges();
            }
        }

        private Payment Clone(Payment payment)
        {
            var retVal = new Payment();
            retVal.AccountId = payment.AccountId;
            retVal.Comment = payment.Comment;
            retVal.Enabled = payment.Enabled;
            retVal.PaymentDate = payment.PaymentDate;
            retVal.PaymentDate = DateTime.Now;
            retVal.ErcId = payment.ErcId;
            retVal.BillId = payment.BillId;
            retVal.Commission = payment.Commission;
            retVal.Bbl = payment.Bbl;
            foreach (var paymentItem in payment.PaymentItems)
            {
                var item = new PaymentItem();
                item.Amount = paymentItem.Amount;
                item.CurrentIndication = paymentItem.CurrentIndication;
                item.LastIndication = paymentItem.LastIndication;
                item.PeriodFrom = paymentItem.PeriodFrom;
                item.PeriodTo = paymentItem.PeriodTo;
                item.ServiceId = paymentItem.ServiceId;
                item.Value = paymentItem.Value;
                item.Enabled = payment.Enabled;
                item.Options = paymentItem.Options;
                retVal.PaymentItems.Add(item);
            }
            return retVal;
        }
    }
}
