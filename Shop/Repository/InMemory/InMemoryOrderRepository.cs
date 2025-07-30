using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository.InMemory
{
    internal class InMemoryOrderRepository:IOrderRepository
    {
        private readonly List<Order> orders = [];
        public Order? FindById(Guid id)
        {
            return orders.Where(o => o.Id == id).FirstOrDefault(); 
        }

        public Order? FindByReference(string reference)
        {
            return orders.Where(o => o.OrderReference == reference).FirstOrDefault();
        }

        public IEnumerable<Order> Find(OrderFindCreterias orderFindCreterias, OrderSortBy sortBy = OrderSortBy.ReferenceAscending)
        {
            var query = from o in orders select o;

            //Truy van dua tren id
            if(orderFindCreterias.Ids.Any())
            {
                query = query.Where(o => orderFindCreterias.Ids.Contains(o.Id));
            }

            //Truy van dua tren customerid
            if(orderFindCreterias.CustomerIds.Any())
            {
                query = query.Where(o => orderFindCreterias.CustomerIds.Contains(o.CustomerId));
            }

            //paging
            if(orderFindCreterias.Skip > 0)
            {
                query = query.Skip(orderFindCreterias.Skip);

            }
            if (orderFindCreterias.Take > 0 && orderFindCreterias.Take != int.MaxValue)
            {
                query = query.Take(orderFindCreterias.Take);
            }

            //Sort:
            if(sortBy == OrderSortBy.ReferenceAscending)
            {
                query = query.OrderBy(o => o.OrderReference);
            }
            else
            {
                query = query.OrderByDescending(o => o.OrderReference);
            }

            return query;

        }

        public Order? Add(Order order)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));

            if (orders.Any(o => o.OrderReference == order.OrderReference))
            {
                throw new ArgumentException("Duplicated order reference");
            }
            orders.Add(order);
            return order;
        }

        public int DeleteAll()
        {
            var c = orders.Count();
            orders.Clear();
            return c;
        }


       
    }
}
