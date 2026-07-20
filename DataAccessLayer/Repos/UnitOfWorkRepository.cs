using Contracts.Exceptions;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Repos.AcademicRepositories;
using DataAccessLayer.Repos.IdentityRepositories;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.DepartmentInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.UniversityInterfaces;
using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserRoleInterfaces;
using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.LoginInterfaces.TokenInterfaces;
using Domain.Interfaces.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        private readonly AppDbcontext _context;
        public IUserRepository UserRepository { get; private set; }

        public ILoginRepository LoginRepository { get; private set; }

        public IRefreshTokenRepository RefreshTokenRepository { get; private set; }

        public IRoleRepository RoleRepository { get; private set; }
        public IUserRoleRepository UserRoleRepository { get; private set; }

        public IUniversityRepository UniversityRepository {  get; private set; }

        public ICollegeRepository CollegeRepository {  get; private set; }

        public IDepartmentRepository DepartmentRepository {  get; private set; }

        public UnitOfWorkRepository(AppDbcontext context)
        {
            _context = context;
            UserRepository = new UserRepository(context);
            LoginRepository = new LoginRepository(context);
            RefreshTokenRepository = new RefreshTokenRepository(context);
            RoleRepository = new RoleRepository(context);
            UserRoleRepository = new UserRoleRepository(context);
            UniversityRepository = new UniversityRepository(context);
            CollegeRepository = new CollegeRepository(context);
            DepartmentRepository = new DepartmentRepository(context);
        }
        public async Task<int> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch(DbUpdateException ex)
            {
                if(ex.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 547)
                        throw new DeleteRestrictedException("Cannot delete this resource because it has related resources.");
                }
            }
            throw new Exception();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        }
    }
