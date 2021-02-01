using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CommunalPayments.DataAccess
{
    public class Accounts : IDataAccess<Account>
    {
        public IEnumerable<Account> ItemsList
        {
            get
            {
                using (var db = new DataModel())
                {
                    return db.Accounts.Include(x => x.Person).ToList();
                }
            }
        }

        public void Create(IEnumerable<Account> accounts)
        {
            if (null == accounts || accounts.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var account in accounts)
                {
                    if (!db.Accounts.Any(x => x.Number == account.Number))
                    {
                        var entity = new Account();
                        entity.Number = account.Number;
                        entity.PersonId = account.PersonId;
                        entity.Street = account.Street;
                        entity.Apartment = account.Apartment;
                        entity.Building = account.Building;
                        entity.City = account.City;
                        entity.InternalId = account.InternalId;
                        entity.Key = account.Key;
                        entity.Enabled = account.Enabled;
                        db.Accounts.Add(entity);
                    }
                }
                db.SaveChanges();
            }
        }

        public void Delete(IEnumerable<int> accountIds)
        {
            if (null == accountIds || accountIds.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var id in accountIds)
                {
                    if (db.Accounts.Any(x => x.Id == id))
                    {
                        var account = db.Accounts.First(x => x.Id == id);
                        db.Accounts.Remove(account);
                    }
                }
                db.SaveChanges();
            }
        }

        public Account Get(int accountId)
        {
            using (var db = new DataModel())
            {
                return db.Accounts.Include(x => x.Person).FirstOrDefault(x => x.Id == accountId);
            }
        }

        public void Update(IEnumerable<Account> accounts)
        {
            if (null == accounts || accounts.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var account in accounts)
                {
                    if (db.Accounts.Any(x => x.Number == account.Number))
                    {
                        var entity = db.Accounts.First(x => x.Id == account.Id);
                        entity.Number = account.Number;
                        entity.PersonId = account.PersonId;
                        entity.Apartment = account.Apartment;
                        entity.Building = account.Building;
                        entity.City = account.City;
                        entity.Street = account.Street;
                        entity.Enabled = account.Enabled;
                        entity.InternalId = account.InternalId;
                        entity.Key = account.Key;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
