using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.Messages
{
    public  class DeleteMessage
    {
        public static string DeletedSuccessfuly<TEntity>(int id)
        {
            return $"{typeof(TEntity).Name} with id  = {id} deleted successfuly";
        }

        public static string DeletionFailed<TEntity>(int id)
        {
            return $"Failed to delete {typeof(TEntity).Name} with id = {id}";
        }
    }
}
