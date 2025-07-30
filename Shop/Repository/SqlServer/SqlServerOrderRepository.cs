using Microsoft.Data.SqlClient;
using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository.SqlServer
{
    internal class SqlServerOrderRepository : IOrderRepository
    {

        private const string INSERT_COMMAND = "INSERT INTO Orders VALUES (@OrderId, @CustomerId, @OrderReference)";
        private const string FIND_BY_ID_QUERY = "SELECT CustomerId, OrderReference FROM Orders WHERE OrderId = @OrderId";
        private const string FIND_BY_REFERENCE_QUERY = "SELECT OrderId, CustomerId FROM Orders WHERE OrderReference = @OrderReference";
        private const string SELECT = "SELECT ";
        private const string FIND_ALL = "OrderId, CustomerId, OrderReference FROM Orders WHERE (1 = 1)";
        private const string FIND_ITEMS = "SELECT OrderItemId, ProductId, Quantity, Price FROM OrderItems WHERE OrderId = @OrderId";
        private const string INSERT_ITEM_COMMAND = "INSERT INTO OrderItems VALUES (@OrderItemId, @OrderId, @ProductId, @Quantity, @Price)";
        private const string DELETE_ALL = "DELETE FROM OrderItems; DELETE FROM Orders;";

        private readonly SqlConnection connection;
        private readonly SqlTransaction? transaction;

        public SqlServerOrderRepository(SqlConnection connection, SqlTransaction? transaction)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.transaction = transaction;
        }

        public Order? Add(Order order)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = INSERT_COMMAND;
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            cmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.UniqueIdentifier)).Value = order.Id;
            cmd.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.UniqueIdentifier)).Value = order.CustomerId;
            cmd.Parameters.Add(new SqlParameter("@OrderReference", SqlDbType.NVarChar, 20)).Value = order.OrderReference;

            if (cmd.ExecuteNonQuery() > 0)
            {
                cmd.CommandText = INSERT_ITEM_COMMAND;
                cmd.Parameters.Clear();

                cmd.Parameters.Add(new SqlParameter("@OrderItemId", SqlDbType.UniqueIdentifier));
                cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.UniqueIdentifier));
                cmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@Price", SqlDbType.Float));

                cmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.UniqueIdentifier)).Value = order.Id;

                foreach (var item in order.Items)
                {
                    cmd.Parameters["@OrderItemId"].Value = item.Id;
                    cmd.Parameters["@ProductId"].Value = item.ProductId;
                    cmd.Parameters["@Quantity"].Value = item.Quantity;
                    cmd.Parameters["@Price"].Value = item.Price;

                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        throw new Exception("Error inserting OrderItems");
                    }
                }

                return order;
            }
            else
            {
                return null;
            }
        }

        public int DeleteAll()
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = DELETE_ALL;
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            return cmd.ExecuteNonQuery();
        }

        public IEnumerable<Order> Find(OrderFindCreterias creterias, OrderSortBy sortBy = OrderSortBy.ReferenceAscending)
        {
            var cmd = connection.CreateCommand();
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            var sql = new StringBuilder(SELECT);
            if (creterias.Take > 0)
            {
                sql.Append("TOP ");
                sql.Append(creterias.Take);
                sql.Append(' ');
            }
            sql.Append(FIND_ALL);

            if (creterias.Ids.Any())
            {
                sql.Append(" AND OrderId IN (");
                sql.Append(string.Join(',', creterias.Ids.Select(id => $"'{id}'")));
                sql.Append(')');
            }

            if (creterias.CustomerIds.Any())
            {
                sql.Append(" AND CustomerId IN (");
                sql.Append(string.Join(',', creterias.CustomerIds.Select(id => $"'{id}'")));
                sql.Append(')');
            }

            if (sortBy == OrderSortBy.ReferenceAscending)
            {
                sql.Append(" ORDER BY OrderReference");
            }
            else
            {
                sql.Append(" ORDER BY OrderReference DESC");
            }

            if (creterias.Skip > 0)
            {
                sql.Append(" OFFSET ");
                sql.Append(creterias.Skip);
                sql.Append(" ROWS");
            }

            cmd.CommandText = sql.ToString();
            using var reader = cmd.ExecuteReader();
            var orders = new List<Order>();

            if (reader != null)
            {
                while (reader.Read())
                {
                    orders.Add(new Order()
                    {
                        Id = reader.GetGuid(0),
                        CustomerId = reader.GetGuid(1),
                        OrderReference = reader.GetString(2)
                    });
                }

                reader.Close();
            }

            foreach (var order in orders)
            {
                LoadOrderItems(order);
            }

            return orders;
        }

        private void LoadOrderItems(Order order)
        {
            var cmd = connection.CreateCommand();
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            cmd.CommandText = FIND_ITEMS;
            cmd.Parameters.Add(new SqlParameter("@OrderId", System.Data.SqlDbType.UniqueIdentifier)).Value = order.Id;

            using var reader = cmd.ExecuteReader();
            if (reader != null)
            {
                while (reader.Read())
                {
                    order.Items.Add(new OrderItem()
                    {
                        Id = reader.GetGuid(0),
                        ProductId = reader.GetGuid(1),
                        Quantity = reader.GetInt32(2),
                        Price = reader.GetDouble(3),
                    });
                }

                reader.Close();
            }
        }

        public Order? FindById(Guid id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = FIND_BY_ID_QUERY;
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            cmd.Parameters.Add(new SqlParameter("@OrderId", System.Data.SqlDbType.UniqueIdentifier)).Value = id;

            var reader = cmd.ExecuteReader();
            if (reader != null && reader.Read())
            {
                return new Order()
                {
                    Id = id,
                    CustomerId = reader.GetGuid(0),
                    OrderReference = reader.GetString(1)
                };
            }
            else
            {
                return null;
            }
        }

        public Order? FindByReference(string reference)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = FIND_BY_REFERENCE_QUERY;
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            cmd.Parameters.Add(new SqlParameter("@OrderReference", System.Data.SqlDbType.NVarChar, 20)).Value = reference;

            var reader = cmd.ExecuteReader();
            if (reader != null && reader.Read())
            {
                return new Order()
                {
                    Id = reader.GetGuid(0),
                    CustomerId = reader.GetGuid(1),
                    OrderReference = reference
                };
            }
            else
            {
                return null;
            }
        }
    }
}
