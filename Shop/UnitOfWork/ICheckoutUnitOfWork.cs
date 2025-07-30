using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.UnitOfWork
{
    internal interface ICheckoutUnitOfWork
    {
        void CreateOrder(Order order);
        void SaveChanges();
    }
}
