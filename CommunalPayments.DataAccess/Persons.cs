using CommunalPayments.Common;
using CommunalPayments.Common.Interfaces;
using CommunalPayments.DataAccess.Database;
using System.Collections.Generic;
using System.Linq;

namespace CommunalPayments.DataAccess
{
    public class Persons : IDataAccess<Person>
    {
        public IEnumerable<Person> ItemsList
        {
            get
            {
                using (var db = new DataModel())
                {
                    return db.Persons.ToList();
                }
            }
        }

        public void Create(IEnumerable<Person> persons)
        {
            if (null == persons || persons.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var person in persons)
                {
                    if (!db.Persons.Any(x => x.LastName == person.LastName && x.FirstName == person.FirstName && x.SurName == person.SurName))
                    {
                        db.Persons.Add(person);
                        db.SaveChanges();
                    }
                }                
            }
        }

        public void Delete(IEnumerable<int> personIds)
        {
            if (null == personIds || personIds.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var id in personIds)
                {
                    if (db.Persons.Any(x => x.Id == id))
                    {
                        var person = db.Persons.First(x => x.Id == id);
                        db.Persons.Remove(person);
                    }
                }
                db.SaveChanges();
            }
        }

        public Person Get(int personId)
        {
            using (var db = new DataModel())
            {
                return db.Persons.FirstOrDefault(x => x.Id == personId);
            }
        }

        public void Update(IEnumerable<Person> persons)
        {
            if (null == persons || persons.Count() == 0)
                return;
            using (var db = new DataModel())
            {
                foreach (var person in persons)
                {
                    if (db.Persons.Any(x => x.Id == person.Id))
                    {
                        var entity = db.Persons.First(x => x.Id == person.Id);
                        entity.FirstName = person.FirstName;
                        entity.LastName = person.LastName;
                        entity.SurName = person.SurName;
                        entity.Enabled = person.Enabled;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
