using Contracts.Responses.AcademicResponses.StudentResponses;
using Contracts.Results;
using Domain.Entities.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.StudentInterfaces.StatusInterfaces
{
    public interface IStatusRepository
    {
        public Task<PagedResult<StudentStatusDTO>> GetStatuses(int pageNumber, int pageSize);
        public Task<StudentStatusDTO?> GetDTOById(int statusId);
        public Task<StudentStatus?>GetEntityById(int statusId);
        public Task<StudentStatusDTO?> GetDTOByName(string name);
        public void Add(StudentStatus status);
        public Task<bool> IsExistsByName(string name);

    }
}
