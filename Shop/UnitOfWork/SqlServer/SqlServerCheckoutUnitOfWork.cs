using Microsoft.Data.SqlClient;
using Shop.Entities;
using Shop.Repository.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.UnitOfWork.SqlServer
{
    internal class SqlServerCheckoutUnitOfWork : ICheckoutUnitOfWork
    {

        private readonly SqlConnection connection;
        private readonly SqlTransaction transaction;
        private SqlServerOrderRepository? orderRepository;
        private SqlServerProductRepository? productRepository;

        public SqlServerCheckoutUnitOfWork(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            transaction = connection.BeginTransaction();
        }

        public SqlServerOrderRepository OrderRepository
        {
            get
            {
                orderRepository ??= new SqlServerOrderRepository(connection, transaction);

                return orderRepository;
            }
        }

        public SqlServerProductRepository ProductRepository
        {
            get
            {
                productRepository ??= new SqlServerProductRepository(connection, transaction);

                return productRepository;
            }
        }

        public void CreateOrder(Order order)
        {
            foreach (var item in order.Items)
            {
                var product = ProductRepository.FindById(item.ProductId) ?? throw new Exception($"Product not found: {item.ProductId}");

                if (product.Quantity - item.Quantity < 0)
                {
                    throw new Exception("product.Quantity < item.Quantity");
                }
                product.Quantity -= item.Quantity;

                ProductRepository.Update(product);
            }

            OrderRepository.Add(order);
        }

        public void SaveChanges()
        {
            transaction.Commit();
        }
    }
}
