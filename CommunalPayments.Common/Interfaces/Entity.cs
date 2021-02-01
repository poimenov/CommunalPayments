namespace CommunalPayments.Common.Interfaces
{
    public abstract class Entity 
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual bool Enabled { get; set; }
    }
}
