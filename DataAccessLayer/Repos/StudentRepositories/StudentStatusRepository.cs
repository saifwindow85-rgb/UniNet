using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.StudentInterfaces;
using Domain.Entities.Students;
using Domain.Interfaces.StudentInterfaces.StatusInterfaces;
using Contracts.Responses.AcademicResponses.StudentResponses;
using DataAccessLayer.Dbcontext;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Contracts.Results;
using DataAccessLayer.Extensions;

namespace DataAccessLayer.Repos.StudentRepositories
{
    public class StudentStatusRepository : IStatusRepository
    {
        private readonly AppDbcontext _context;
        public StudentStatusRepository(AppDbcontext context)
        {
            _context = context;
        }
        private readonly Expression<Func<StudentStatus, StudentStatusDTO>> ToDTO = s => new StudentStatusDTO
        {
            StatusId = s.StatusId,
            StatusName = s.Name,
            Description = s.Description,
        };
        public void Add(StudentStatus status)
        {
            _context.StudentStatuses.Add(status);
        }

        public async Task<StudentStatusDTO?> GetDTOById(int statusId)
        {
            return await _context.StudentStatuses.Where(s=>s.StatusId == statusId).Select(ToDTO).SingleOrDefaultAsync();
        }

        public async Task<StudentStatusDTO?> GetDTOByName(string name)
        {
            return await _context.StudentStatuses.Where(s => s.Name == name).Select(ToDTO).SingleOrDefaultAsync();
        }

        public async Task<StudentStatus?> GetEntityById(int statusId)
        {
            return await _context.StudentStatuses.FindAsync(statusId);
        }

        public async Task<PagedResult<StudentStatusDTO>> GetStatuses(int pageNumber, int pageSize)
        {
            return await _context.StudentStatuses.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<bool> IsExistsByName(string name)
        {
            return await _context.StudentStatuses.AnyAsync(s=>s.Name == name);
        }
    }
}
