using CommunalPayments.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunalPayments.DataAccess.Database
{
    public class DataModel : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=CommunalPayments.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rate>().Property(x => x.Value).HasPrecision(12, 5);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Rate> Rates { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<PayStatus> PayStatuses { get; set; }
        public virtual DbSet<PayMode> PayModes { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentItem> PaymentItems { get; set; }
    }
}
