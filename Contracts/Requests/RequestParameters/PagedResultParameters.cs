using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.RequestParameters
{
    public class PagedResultParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageSize  Format!/PageSize Must Be > 0")]
        public int PageSize { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "PageNumber  Format!/PageNumber Must Be > 0")]
        public int PageNumber { get; set; }
    }
}
