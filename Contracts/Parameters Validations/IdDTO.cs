using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Parameters_Validations
{
    public class IdDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Id Format!/Id Must Be > 0")]
        public int Id { get; set; }
    }
}
