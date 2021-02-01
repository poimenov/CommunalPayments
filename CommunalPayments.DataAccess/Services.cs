using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.DataAccess.Database;
using System.Collections.Generic;
using System.Linq;

namespace CommunalPayments.DataAccess
{
    public class Services : IDataAccess<Service>
    {
        public IEnumerable<Service> ItemsList
        {
            get
            {
                using (var db = new DataModel())
                {
                    return db.Services.ToList();
                }
            }
        }

        public void Create(IEnumerable<Service> items)
        {
            if (null == items || items.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var item in items)
                {
                    if (!db.Services.Any(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                    {
                        db.Services.Add(item);
                    }
                }
                db.SaveChanges();
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
                    if (db.Services.Any(x => x.Id == id))
                    {
                        var service = db.Services.First(x => x.Id == id);
                        db.Services.Remove(service);
                    }
                }
                db.SaveChanges();
            }
        }

        public Service Get(int itemId)
        {
            using (var db = new DataModel())
            {
                return db.Services.FirstOrDefault(x => x.Id == itemId);
            }
        }

        public void Update(IEnumerable<Service> items)
        {
            if (null == items || items.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var item in items)
                {
                    if (db.Services.Any(x => x.Id == item.Id))
                    {
                        var entity = db.Services.First(x => x.Id == item.Id);
                        entity.Name = item.Name;
                        entity.Enabled = item.Enabled;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
