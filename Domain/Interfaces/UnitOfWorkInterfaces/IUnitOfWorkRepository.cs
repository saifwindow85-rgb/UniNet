using Contracts.Results;
using Domain.Interfaces.AcademicStructureInterfaces.BatchInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.DepartmentInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.UniversityInterfaces;
using Domain.Interfaces.EmployeeInterfaces;
using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserRoleInterfaces;
using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.LoginInterfaces.TokenInterfaces;
using Domain.Interfaces.StudentInterfaces;
using Domain.Interfaces.StudentStatusInterfaces;
using Domain.Interfaces.StudyInterfaces.SubjectInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWorkRepository : IDisposable
    {
        public IUserRepository UserRepository { get; }
        public ILoginRepository LoginRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }
        public IUniversityRepository UniversityRepository { get; }
        public ICollegeRepository CollegeRepository { get; }
        public IDepartmentRepository DepartmentRepository { get; }
        public IBatchRepository BatchRepository { get; }
        public ISectionRepository SectionRepository { get; }
        public IEmployeeRepository EmployeeRepository { get; }
        public IStatusRepository StatusRepository { get; }
        public IStudentRepository StudentRepository { get; }
        public ISubjectRepository SubjectRepository { get; }
        public Task<int> CompleteAsync();
        public Task BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public Task RollbackTransactionAsync();
    }
}
