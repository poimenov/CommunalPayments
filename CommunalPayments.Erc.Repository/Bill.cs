using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunalPayments.Erc.Repository
{
    internal class Bill
    {
        public int IdBill { get; set; }
        public int IdPayMode { get; set; }
        public string PayModeName { get; set; }
        public decimal Total { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string DtCreate { get; set; }
        public IEnumerable<Order> OrderList { get; set; }
    }
    internal class Order
    {
        public int IdOrder { get; set; }
        public int CdFlat { get; set; }
        public string PayType { get; set; }
        public decimal Total { get; set; }
        public decimal Commission { get; set; }
        public string DtCreate { get; set; }
        public int IdPayType { get; set; }
        public string Comment { get; set; }
        public string Descript { get; set; }
        public string Btn { get; set; }
    }
    internal class OrderItem
    {
        public int Service { get; set; }
		public string Month1 { get; set; }
		public string Month2 { get; set; }
		public decimal Amount { get; set; }
		public decimal? PrevCounter { get; set; }
		public decimal? CurCounter { get; set; }
		public decimal? Diff { get; set; }
		public string Options { get; set; }
    }
}
