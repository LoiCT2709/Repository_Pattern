using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository
{
    public  class OrderFindCreterias:PagingCreterias
    {
        public IEnumerable<Guid>Ids { get; set; } = Enumerable.Empty<Guid>();
        public IEnumerable<Guid> CustomerIds { get; set;} = Enumerable.Empty<Guid>();
    }
}
