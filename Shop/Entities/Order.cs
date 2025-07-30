using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Entities
{
    public  class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public required string OrderReference { get; set; }
        public List<OrderItem> Items { get; set; } = [];
       
    }
}
