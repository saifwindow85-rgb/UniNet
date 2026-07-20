using Contracts.Responses.AcademicResponses.UniversityResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.UniversityInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.AcademicRepositories
{
    public class UniversityRepository : IUniversityRepository
    {
        private readonly AppDbcontext _context;
        private readonly Expression<Func<University, UniversityDTO>> ToDTO = u => new UniversityDTO
        {
            UniversityId = u.UniversityId,
            UniversityName = u.Name,
            Description = u.Description,
            CreatedAt = u.CreatedAt,
            CreatedByUserId = u.CreatedByUserId,
            CreatedByUserName = u.CreatedByUser.UserName,
            UpdatedAt = u.UpdatedAt,
            UpdatedByUserId = u.UpdatedByUserId,
            UpdatedByUserName = u.UpdatedByUser == null ? null : u.UpdatedByUser.UserName,

        };
        public UniversityRepository(AppDbcontext context)
        {
            _context = context;
        }
        public  void Add(University university)
        {
            _context.Universities.Add(university);
        }

        public async Task<bool> Delete(int UniversityId)
        {
            var university = await _context.Universities.FindAsync(UniversityId);
            if (university == null)
                return false;
            _context.Universities.Remove(university);
            return true;
        }

        public async Task<PagedResult<UniversityDTO>> GetAllUniversities(int pageNumber, int pageSize)
        {
            return await _context.Universities.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<UniversityDTO?> GetUniversityDTOById(int universityId)
        {
            return await _context.Universities.Select(ToDTO).SingleOrDefaultAsync(u=>u.UniversityId == universityId);
        }

        public async Task<University?> GetUniversityEntityById(int universityId)
        {
            return await _context.Universities.FindAsync(universityId);
        }

        public async Task<bool> IsUniversityExists(int universityId)
        {
            return await _context.Universities.AnyAsync(u => u.UniversityId == universityId);
        }

        public Task<bool> IsUniversityExists(string universityName)
        {
            return _context.Universities.AnyAsync(u => u.Name == universityName);
        }
    }
}
