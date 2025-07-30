using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Entities
{
    public  class Product
    {

        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required double Price { get; set; }
        public required int Quantity {  get; set; }
    }
}
