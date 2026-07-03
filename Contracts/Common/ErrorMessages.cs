using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Messages
{
    public class ErrorMessages
    {
        public static string NotFound<TEntity>(int Id)
        {
            return $"No {typeof(TEntity).Name} Found With Id = {Id}";
        }
    }
}
