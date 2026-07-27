using Contracts.Requests.EmployeeRequests;
using Contracts.Responses.EmployeeResponse;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Employees;
using Domain.Entities.Identity;
using Domain.Interfaces.EmployeeInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.EmployeeRepository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbcontext _context;

        private readonly Expression<Func<Employee, EmployeeDTO>> ToDTO = e => new EmployeeDTO
        {
            EmployeeId = e.EmployeeId,
            UserId = e.UserId,
            FullName = e.User.FullName,
            UserName = e.User.UserName,
            Email = e.User.Email,
            PhoneNumber = e.User.PhoneNumber,
            IsActive = e.User.IsActive,
            UniversityId = e.UniversityId,
            UniversityName = e.University.Name,
            CollegeId = e.CollegeId,
            CollegeName = e.College == null ? null : e.College.Name,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department == null ? null : e.Department.Name,

        };

        public EmployeeRepository(AppDbcontext context)
        {
            _context = context;
        }

        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
        }

        public async Task<Employee?> GetById(int employeeId)
        {
            return await _context.Employees.FindAsync(employeeId);
        }

        public Task<EmployeeDTO?> GetDTOById(int employeeId)
        {
            return _context.Employees.AsNoTracking().Select(ToDTO).SingleOrDefaultAsync(e=>e.EmployeeId == employeeId);
        }

        public async Task<PagedResult<EmployeeDTO>> GetEmployees(EmployeeFilter? employeeFilter, EmployeeScope? employeeScope, int pageNumber, int pageSize)
        {
            var query = _context.Employees.AsNoTracking().AsQueryable();

            if (employeeScope.UniversityId.HasValue)
                query = query.Where(e => e.UniversityId == employeeScope.UniversityId.Value);

            if(employeeScope.CollegeId.HasValue)
                query = query.Where(e => e.CollegeId == employeeScope.CollegeId.Value);

            if(employeeScope.DepartmentId.HasValue)
                query = query.Where(e => e.DepartmentId == employeeScope.DepartmentId.Value);

            if(employeeFilter.IsActive.HasValue)
                query = query.Where(e => e.User.IsActive == employeeFilter.IsActive.Value);

            if(!string.IsNullOrEmpty(employeeFilter.Search))
                query = query.Where(e => EF.Functions.Like(e.User.FullName,$"{employeeFilter.Search}"));


            return await query.Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

       
    }
}
