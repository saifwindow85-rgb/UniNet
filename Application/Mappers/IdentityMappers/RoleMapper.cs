using Contracts.Responses.IdentityResponses.RoleResponses;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers.IdentityMappers
{
    public static  class RoleMapper
    {
        public static RoleDTO ToDTO(this Role role)
        {
            return new RoleDTO
            {
                RoleId = role.RoleId,
                RoleName = role.Name
            };
        }
    }
}
