using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository
{
    //Chứa các tiêu chí tìm kiếm sản phẩm
    public  class ProductFindCreterias :PagingCreterias
    {
        //Sản phẩm có thể tìm theo khoảng giá , theo Id, Name.

        public double MinPrice { get; set; } = double.MinValue;
        public double MaxPrice { get; set; } = double.MaxValue;

        //Danh sách Id
        public IEnumerable <Guid> Ids { get; set; } = Enumerable.Empty<Guid>();
        public string Name { get; set; } = string.Empty;
        public static ProductFindCreterias Empty => new()
        {
        };
    }
}
