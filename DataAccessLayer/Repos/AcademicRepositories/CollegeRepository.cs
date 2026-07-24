using Contracts.Responses.CollegeResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.AcademicRepositories
{
    public class CollegeRepository : ICollegeRepository
    {
        private Expression<Func<College, CollegeDTO>> ToDTO = c => new CollegeDTO
        {
            CollegeId = c.CollegeId,
            CollegeName = c.Name,
            Description = c.Description,
            UniversityId = c.UniversityId,
            UniversityName = c.University.Name,
            CreatedAt = c.CreatedAt,
            CreatedByUserId = c.CreatedByUserId,
            CreatedByUserName = c.CreatedByUser.UserName,
            UpdatedAt = c.UpdatedAt,
            UpdatedByUserId = c.UpdatedByUserId,
            UpdatedByUserName = c.UpdatedByUser == null ? null : c.UpdatedByUser.UserName,
        };

        private readonly Expression<Func<College, CollegeAuthorizationInfo>> ToInfo = c => new CollegeAuthorizationInfo
        {
            CollegeId = c.CollegeId,
            UniversityId = c.UniversityId,
        };
        private readonly AppDbcontext _context;
        public CollegeRepository(AppDbcontext context)
        {
            _context = context;
        }
        public void Add(College college)
        {
            _context.Colleges.Add(college);
        }

        public async Task<bool> Delete(int collegeId)
        {
         
            var college = await _context.Colleges.FindAsync(collegeId);
            if (college == null)
                return false;
            _context.Colleges.Remove(college);
            return true;
        }

        public async Task<PagedResult<CollegeDTO>> GetAllColleges(int pageNumber, int pageSize)
        {
            return await _context.Colleges.AsNoTracking().Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<CollegeDTO>> GetAllCollegesPerUniversity(int universityId, int pageNumber, int pageSize)
        {
            return await _context.Colleges.AsNoTracking().Where(c=>c.UniversityId == universityId)
                .OrderBy(c=>c.CollegeId).Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<CollegeDTO?> GetCollegeDTOById(int collegeId)
        {
            return await _context.Colleges.AsNoTracking().Select(ToDTO).SingleOrDefaultAsync(c => c.CollegeId == collegeId);
        }

        public async Task<College?> GetCollegeEntityById(int collegeId)
        {
            return await _context.Colleges.FindAsync(collegeId);
        }

        public async Task<bool> IsCollegeExists(int collegeId)
        {
            return await _context.Colleges.AnyAsync(c => c.CollegeId == collegeId);
        }

        public async Task<bool> IsCollegeExists(int universityId,string collegeName)
        {
            return await _context.Colleges.AnyAsync(c => c.Name == collegeName && c.UniversityId == universityId);
        }

        public async Task<CollegeDTO?> GetCollegeDTOByName(int universityId,string collegeName)
        {
            return await _context.Colleges.AsNoTracking().Where(c=>c.UniversityId == universityId && c.Name == collegeName)
                .Select(ToDTO).SingleOrDefaultAsync();
        }

        public async Task<College?> GetCollegeEntityByName(int universityId, string collegeName)
        {
            return await _context.Colleges.Where(c=>c.UniversityId == universityId && c.Name == collegeName).SingleOrDefaultAsync();
        }

        public async Task<CollegeAuthorizationInfo?> GetCollegeAuthorizationInfo(int collegeId)
        {
            return await _context.Colleges.AsNoTracking().Select(ToInfo).SingleOrDefaultAsync(c => c.CollegeId == collegeId);
        }
    }
}
