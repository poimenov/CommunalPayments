using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CommunalPayments.DataAccess
{
    public class Rates : IDataAccess<Rate>
    {
        public IEnumerable<Rate> ItemsList
        {
            get
            {
                using (var db = new DataModel())
                {
                    return db.Rates.Include(x=>x.Service).ToList();
                }
            }
        }

        public void Create(IEnumerable<Rate> rates)
        {
            if (null == rates || rates.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var rate in rates)
                {
                    db.Rates.Add(rate);
                }
                db.SaveChanges();
            }
        }

        public void Delete(IEnumerable<int> rateIds)
        {
            if (null == rateIds || rateIds.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var id in rateIds)
                {
                    var rate = db.Rates.First(x => x.Id == id);
                    db.Rates.Remove(rate);
                }
                db.SaveChanges();
            }
        }

        public Rate Get(int rateId)
        {
            using (var db = new DataModel())
            {
                return db.Rates.Include(x => x.Service).First(x => x.Id == rateId);
            }
        }

        public void Update(IEnumerable<Rate> rates)
        {
            if (null == rates || rates.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var rate in rates)
                {
                    var entity = db.Rates.First(x => x.Id == rate.Id);
                    entity.ServiceId = rate.ServiceId;
                    entity.Value = rate.Value;
                    entity.VolumeFrom = rate.VolumeFrom;
                    entity.DateFrom = rate.DateFrom;
                    entity.Description = rate.Description;
                    entity.Enabled = rate.Enabled;
                    entity.Measure = rate.Measure;
                }
                db.SaveChanges();
            }
        }
    }
}
