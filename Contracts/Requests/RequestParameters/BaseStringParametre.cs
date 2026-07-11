using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.RequestParameters
{
    public class BaseStringParametre
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(250, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 250 characters.")]
        public string Name { get; set; } = null!;
    }
}
