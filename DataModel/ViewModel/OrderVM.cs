using DataModel.Models;

namespace DataModel.ViewModel
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails{ get; set; }
    }
}
