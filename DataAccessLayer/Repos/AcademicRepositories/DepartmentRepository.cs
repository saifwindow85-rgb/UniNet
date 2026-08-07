using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses.AcademicResponses.DepartmentResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.DepartmentInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.AcademicRepositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbcontext _context;
        private readonly Expression<Func<Department, DepartmentDTO>> ToDTO = d => new DepartmentDTO
        {
            DepartmentId = d.DepartmentId,
            DepartmentName = d.Name,
            Description = d.Description,
            CreatedAt = d.CreatedAt,
            CreatedByUserId = d.CreatedByUserId,
            CreatedByUserName = d.CreatedByUser.UserName,
            UpdatedAt = d.UpdatedAt,
            UpdatedByUserId = d.UpdatedByUserId,
            UpdatedByUserName = d.UpdatedByUser == null ? null : d.UpdatedByUser.UserName,
        };
        private readonly Expression<Func<Department, DepartmentAuthorizationInfo>> ToInfo = d => new DepartmentAuthorizationInfo
        {
            DepartmentId = d.DepartmentId,
            CollegeId = d.CollegeId,
            UniversityId = d.College.UniversityId
        };
        public DepartmentRepository(AppDbcontext context)
        {
            _context = context;
        }

        public void Add(Department department)
        {
            _context.Departments.Add(department);
        }

        public async Task<bool> Delete(int DepartmentId)
        {
            var department = await _context.Departments.FindAsync(DepartmentId);
            if (department == null)
                return false;

            _context.Departments.Remove(department);
            return true;
        }

        public async Task<bool> ExistsById(int departmentId)
        {
            return await _context.Departments.AnyAsync(d => d.DepartmentId == departmentId);
        }

        public async Task<bool> ExistsByName(int collegeId, string name)
        {
            return await _context.Departments.AnyAsync(d=>d.CollegeId == collegeId&& d.Name == name);
        }

        public async Task<PagedResult<DepartmentDTO>> GetAllDepartments(AcademicFilter?filter,int pageNumber, int pageSize)
        {
            if(filter == null)
                filter = new AcademicFilter();
            var query = _context.Departments.AsNoTracking().OrderBy(d=>d.College.UniversityId).ThenBy(c=>c.CollegeId).ThenBy(c=>c.DepartmentId).AsQueryable();

            if(filter.UniversityId.HasValue)
            {
                query = query.Where(d=>d.College.UniversityId == filter.UniversityId);
            }

            if(filter.CollegeId.HasValue)
            {
                query = query.Where(d=>d.CollegeId == filter.CollegeId);
            }

            if(filter.DepartmentId.HasValue)
            {
                query = query.Where(d => d.DepartmentId == filter.DepartmentId);
            }

            if(!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(d => EF.Functions.Like(d.Name, $"%{filter.Search}%"));
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= filter.StartDate);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= filter.EndDate);
            }

            return await query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<DepartmentAuthorizationInfo?> GetDepartmentAuthorizationInfoAsync(int departmentId)
        {
            return await _context.Departments.AsNoTracking().Where(d => d.DepartmentId == departmentId).Select(ToInfo).SingleOrDefaultAsync();
        }


        public async Task<PagedResult<DepartmentDTO>> GetDepartmentsPerCollege(UserScope?scope,AcademicFilter?filter, int pageNumber, int pageSize)
        {
            if (scope == null)
                scope = new UserScope();

            if(filter == null)
                filter = new AcademicFilter();

            var query = _context.Departments.AsNoTracking().OrderBy(d => d.College.UniversityId).ThenBy(c => c.CollegeId).ThenBy(c => c.DepartmentId).AsQueryable();
            if (scope.UniversityId.HasValue)
            {
                query = query.Where(d => d.College.UniversityId == scope.UniversityId);
            }
            else if(filter.UniversityId.HasValue)
            {
                query = query.Where(d => d.College.UniversityId == filter.UniversityId);
            }

            if(scope.CollegeId.HasValue)
            {
                query = query.Where(d=>d.CollegeId == scope.CollegeId);
            }
            else if(filter.CollegeId.HasValue)
            {
                query = query.Where(d => d.CollegeId == filter.CollegeId);
            }

            if(!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(d => EF.Functions.Like(d.Name, $"%{filter.Search}%"));
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt >= filter.StartDate);
            }

            if(filter.EndDate.HasValue)
            {
                query = query.Where(d=>d.CreatedAt <= filter.EndDate);
            }

            return await query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<DepartmentDTO?> GetDTOById(int departmentId)
        {
            return await _context.Departments.Select(ToDTO).SingleOrDefaultAsync(d=>d.DepartmentId == departmentId);
        }

        public async Task<Department?> GetEntityById(int departmentId)
        {
            return await _context.Departments.FindAsync(departmentId);
        }
    }
}
