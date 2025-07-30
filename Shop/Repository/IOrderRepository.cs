using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository
{
    internal interface IOrderRepository
    {
        Order? FindById (Guid id);
        Order? FindByReference (string reference);

        //Danh sach tim kiem tra ve
        IEnumerable<Order> Find(OrderFindCreterias orderFindCreterias, OrderSortBy sortBy = OrderSortBy.ReferenceAscending);
        Order? Add(Order order);
        int DeleteAll();
    }
}
