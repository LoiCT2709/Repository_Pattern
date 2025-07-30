using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository
{
    public class PagingCreterias
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = int.MaxValue;

    }
}
