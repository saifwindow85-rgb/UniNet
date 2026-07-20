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

        public async Task<PagedResult<DepartmentDTO>> GetAllDepartments(int pageNumber, int pageSize)
        {
           return await _context.Departments.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<DepartmentDTO>> GetDepartmentsPerCollege(int collegeId, int pageNumber, int pageSize)
        {
            return await _context.Departments.Where(d=>d.CollegeId == collegeId)
                .Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<DepartmentDTO?> GetDTOById(int departmentId)
        {
            return await _context.Departments.Select(ToDTO).SingleOrDefaultAsync(d=>d.DepartmentId == departmentId);
        }

        public async Task<DepartmentDTO?> GetDTOByName(int collegeId, string name)
        {
            return await _context.Departments.Where(d => d.CollegeId == collegeId).Select(ToDTO).SingleOrDefaultAsync(d => d.DepartmentName == name);
        }

        public async Task<Department?> GetEntityById(int departmentId)
        {
            return await _context.Departments.FindAsync(departmentId);
        }

        public async Task<Department?> GetEntityByName(int collegeId, string name)
        {
            return await _context.Departments.SingleOrDefaultAsync(d=>d.CollegeId == collegeId && d.Name == name);
        }
    }
}
