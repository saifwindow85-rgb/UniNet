using Contracts.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Extensions
{
    public static class PagedResultExtensions
    {

        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalRecords = await query.CountAsync();
            int Totalpages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<T>
            {
                Data = data,
                TotalRecords = totalRecords,
                TotalPages = Totalpages,
                CurrentPage = pageNumber
            };
        }

    }
}
