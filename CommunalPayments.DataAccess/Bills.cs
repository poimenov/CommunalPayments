using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.DataAccess.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunalPayments.DataAccess
{
    public class Bills : IDataAccess<Bill>
    {
        public IEnumerable<Bill> ItemsList
        {
            get
            {
                using (var db = new DataModel())
                {
                    return db.Bills.ToList();
                }
            }
        }

        public void Create(IEnumerable<Bill> items)
        {
            if (null == items || items.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            if (item.ErcId > 0 && !db.Bills.Any(x => x.ErcId == item.ErcId))
                            {
                                if(!db.PayModes.Any(x=>x.Id == item.ModeId))
                                {
                                    //надо бы перечислить все способы в миграции
                                    //но этот код оставить на случай появления новых
                                    db.PayModes.Add(new PayMode { Id = item.ModeId, Name = string.Format("Оплата способом № {0}", item.ModeId) });
                                    db.SaveChanges();
                                }
                                if (!db.PayStatuses.Any(x => x.Id == item.StatusId))
                                {
                                    //надо бы перечислить все способы в миграции
                                    //но этот код оставить на случай появления новых
                                    db.PayStatuses.Add(new PayStatus { Id = item.StatusId, Name = string.Format("Статус № {0}", item.StatusId) });
                                    db.SaveChanges();
                                }
                                var bill = new Bill()
                                {
                                    ErcId = item.ErcId,
                                    CreateDate = item.CreateDate,
                                    Enabled = item.Enabled,
                                    ModeId = item.ModeId,
                                    StatusId = item.StatusId
                                };
                                db.Bills.Add(bill);
                                db.SaveChanges();
                                if (null != item.Payments)
                                {
                                    foreach (var pay in item.Payments)
                                    {
                                        if (pay.ErcId > 0 && !db.Payments.Any(x => x.ErcId == pay.ErcId))
                                        {
                                            var payment = new Payment()
                                            {
                                                ErcId = pay.ErcId,
                                                AccountId = pay.AccountId,
                                                BillId = bill.Id,
                                                Enabled = false,
                                                Comment = pay.Comment,
                                                PaymentDate = pay.PaymentDate
                                            };
                                            db.Payments.Add(payment);
                                            db.SaveChanges();
                                            if (null != pay.PaymentItems)
                                            {
                                                foreach (var payItem in pay.PaymentItems)
                                                {
                                                    var paymentItem = new PaymentItem()
                                                    {
                                                        Amount = payItem.Amount,
                                                        CurrentIndication = payItem.CurrentIndication,
                                                        LastIndication = payItem.LastIndication,
                                                        Value = payItem.Value,
                                                        ServiceId = payItem.ServiceId,
                                                        Enabled = false,
                                                        PeriodFrom = payItem.PeriodFrom,
                                                        PeriodTo = payItem.PeriodTo,
                                                        PaymentId = payment.Id
                                                    };
                                                    db.PaymentItems.Add(paymentItem);
                                                }
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Delete(IEnumerable<int> itemIds)
        {
            if (null == itemIds || itemIds.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var id in itemIds)
                {
                    if (db.Bills.Any(x => x.Id == id))
                    {
                        var bill = db.Bills.First(x => x.Id == id);
                        if (bill.Enabled)
                        {
                            db.Bills.Remove(bill);
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public Bill Get(int itemId)
        {
            using (var db = new DataModel())
            {
                return db.Bills.FirstOrDefault(x => x.Id == itemId);
            }
        }

        public void Update(IEnumerable<Bill> items)
        {
            if (null == items || items.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var item in items)
                {
                    if (db.Bills.Any(x => x.Id == item.Id))
                    {
                        var entity = db.Bills.First(x => x.Id == item.Id);
                        entity.CreateDate = item.CreateDate;
                        entity.Enabled = item.Enabled;
                        entity.ModeId = item.ModeId;
                        entity.StatusId = item.StatusId;
                        entity.ErcId = item.ErcId;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
