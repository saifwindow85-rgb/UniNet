using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.Messages
{
    public class ErrorMessages
    {
        public static string NotFound<TEntity>(int Id)
        {
            return $"No {typeof(TEntity).Name} Found With Id = {Id}";
        }
        public static string NotFound<TEntity>(string name)
        {
            return $"No {typeof(TEntity).Name} Found With Name = {name}";
        }
        public static string NotFound(string EntityName,int Id)
        {
            return $"No {EntityName} Found With Id = {Id}";
        }
        public static string NotFound(string EntityName, string name)
        {
            return $"No {EntityName} Found With Name = {name}";
        }
    }
}
