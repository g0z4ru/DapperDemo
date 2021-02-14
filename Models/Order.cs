namespace DapperDemo.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CID { get; set; }
        public string OrderName { get; set; }

        public OrderDetails OrderDetails { get; set; }
    }
}