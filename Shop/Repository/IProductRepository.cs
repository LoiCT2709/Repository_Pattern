using Shop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository
{
    internal interface IProductRepository
    {
        Product? FindById(Guid id);
        IEnumerable<Product> Find(ProductFindCreterias productFindCreterias, ProductSortBy productSortBy = ProductSortBy.NameAscending);

        //Thêm xóa sửa:
        Product? Add (Product product);
        int DeleteAll();
        int Update (Product product);

    }
}
