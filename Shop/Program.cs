using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shop.Entities;
using Shop.Repository.InMemory;
using Shop.Repository.SqlServer;
using Shop.Repository;
using Shop.UnitOfWork.SqlServer;

namespace Shop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true)
                .AddJsonFile("connectionStrings.json", optional: true)
                .Build();

            string connectionString = config.GetConnectionString("ShopDatabase") ?? string.Empty; // get from ConnectionStrings

            if (connectionString.Length > 0)
            {
                var sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                var trans = sqlConnection.BeginTransaction();

                var orderRepository = new SqlServerOrderRepository(sqlConnection, trans);
                var productRepository = new SqlServerProductRepository(sqlConnection, trans);

                orderRepository.DeleteAll();
                productRepository.DeleteAll();

                InsertSampleProducts(productRepository);
                QueryProducts(productRepository);
                trans.Commit();

                CreateOrders(sqlConnection);
                QueryOrders(orderRepository);

            }
            else
            {
                var orderRepository = new InMemoryOrderRepository();
                var productRepository = new InMemoryProductRepository();

                InsertSampleProducts(productRepository);
                QueryProducts(productRepository);
            }
        }

        private static void CreateOrders(SqlConnection sqlConnection)
        {
            var uow = new SqlServerCheckoutUnitOfWork(sqlConnection);
            var orderId = Guid.NewGuid();

            uow.CreateOrder(new Order()
            {
                OrderReference = "00001",
                CustomerId = Guid.Empty,
                Id = orderId,
                Items =
                [
                    new OrderItem()
                    {
                        Id = new Guid("00000000-0000-0000-0001-000000000001"),
                        OrderId = orderId,
                        Price = 999,
                        ProductId = new Guid("00000000-0000-0000-0000-000000000001"),
                        Quantity = 1
                    },
                    new OrderItem()
                    {
                        Id = new Guid("00000000-0000-0000-0001-000000000002"),
                        OrderId = orderId,
                        Price = 999,
                        ProductId = new Guid("00000000-0000-0000-0000-000000000002"),
                        Quantity = 1
                    }
                ]
            });

            uow.SaveChanges();
        }

        private static void QueryOrders(IOrderRepository orderRepository)
        {
        }

        private static void QueryProducts(IProductRepository productRepository)
        {
            Console.WriteLine("Product.Price >= 1000");
            var products = productRepository.Find(new ProductFindCreterias()
            {
                MinPrice = 1000,
            });
            PrintProducts(products);

            Console.WriteLine("Product.Price <= 1000");
            products = productRepository.Find(new ProductFindCreterias()
            {
                MaxPrice = 1000,
            });
            PrintProducts(products);

            Console.WriteLine("Product.Name contains iPad");
            products = productRepository.Find(new ProductFindCreterias()
            {
                Name = "iPad",
            });
            PrintProducts(products);
        }

        private static void PrintProducts(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id,20} {product.Name,40} {product.Price,10}");
            }
        }

        private static void InsertSampleProducts(IProductRepository productRepository)
        {
            productRepository.Add(new Product()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Name = "Apple iPhone",
                Price = 999,
                Quantity = 70,
            });

            productRepository.Add(new Product()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                Name = "Apple iPad",
                Price = 799,
                Quantity = 10,
            });

            productRepository.Add(new Product()
            {
                Id = new Guid("00000000-0000-0000-0000-000000000003"),
                Name = "Apple Macbook",
                Price = 1399,
                Quantity = 20,
            });
        }
    }
}
