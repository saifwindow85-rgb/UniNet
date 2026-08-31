using Contracts.Exceptions;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Repos.AcademicRepositories;
using DataAccessLayer.Repos.EmployeeRepository;
using DataAccessLayer.Repos.IdentityRepositories;
using DataAccessLayer.Repos.StudentRepositories;
using DataAccessLayer.Repos.StudentRepository;
using DataAccessLayer.Repos.StudyRepository;
using Domain.Interfaces.AcademicStructureInterfaces.BatchInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.DepartmentInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.UniversityInterfaces;
using Domain.Interfaces.EmployeeInterfaces;
using Domain.Interfaces.ContentInterfaces;
using Domain.Interfaces.ImageInterfaces;
using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserRoleInterfaces;
using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.LoginInterfaces.TokenInterfaces;
using Domain.Interfaces.StudentInterfaces;
using Domain.Interfaces.StudentStatusInterfaces;
using Domain.Interfaces.StudyInterfaces.SubjectInterfaces;
using Domain.Interfaces.StudyInterfaces.SemesterInterfaces;
using Domain.Interfaces.StudyInterfaces.SectionSubjectInterfaces;
using Domain.Interfaces.StudyInterfaces.StudentResultInterfaces;
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

        public IBatchRepository BatchRepository {  get; private set; }

        public ISectionRepository SectionRepository { get; private set; }

        public IEmployeeRepository EmployeeRepository {  get; private set; }

        public IStatusRepository StatusRepository {  get; private set; }

        public IStudentRepository StudentRepository { get; private set; }

        public ISubjectRepository SubjectRepository { get; private set;  }
        public ISemesterRepository SemesterRepository { get; private set; }
        public ISectionSubjectRepository SectionSubjectRepository { get; private set; }
        public IStudentResultRepository StudentResultRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }
        public IContentRepository ContentRepository { get; private set; }

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
            BatchRepository = new BatchRepository(context);
            SectionRepository = new SectionRepository(context);
            EmployeeRepository = new EmployeeRepository.EmployeeRepository(context);
            StatusRepository = new StudentStatusRepository(context);
            StudentRepository = new StudentRepository.StudentRepository(context);
            SubjectRepository = new SubjectRepository(context);
            SemesterRepository = new SemesterRepository(context);
            SectionSubjectRepository = new SectionSubjectRepository(context);
            StudentResultRepository = new StudentResultRepository(context);
            ImageRepository = new ImageRepository.ImageRepository(context);
            ContentRepository = new ContentRepository.ContentRepository(context);
        }
        public async Task<int> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            // شرط "when" لا يلتقط الاستثناء إلا حين يتحقق فعلاً؛ فشل الشرط يجعل .NET
            // يتجاهل هذا الـ catch تماماً ويستمر بالبحث للأعلى — بلا حاجة لـ throw يدوي،
            // وبلا احتمال سقوط صامت كما كان يحدث سابقاً.
            // قبل مُرشِّحات SqlException: DbUpdateConcurrencyException يرث DbUpdateException
            // لكن استثناءه الداخلي ليس SqlException، فلا يطابق أيًّا منها ويتسرّب كـ 500.
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrentModificationException(
                    "This resource was modified by another request. Reload it and try again.", ex);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
            {
                // 547 ليس خطأ حذف بالضرورة: SQL Server يُطلقه أيضًا على INSERT/UPDATE
                // يخالف CHECK أو مفتاحًا أجنبيًا. التمييز الموثوق ليس بنصّ الرسالة
                // (يتغيّر باللغة) بل بحالة الكيانات المتأثرة: وجود كيان محذوف يعني قيد حذف.
                if (ex.Entries.Any(e => e.State == EntityState.Deleted))
                {
                    throw new DeleteRestrictedException(
                        "Cannot delete this resource because it has related resources.", ex);
                }

                throw new ConstraintViolationException(
                    "The operation violates a data constraint. Check that every referenced record exists and that the values are consistent.", ex);
            }
            // 2601/2627 = فهرس فريد. يصطدم بها مسار استبدال صورة المحتوى تحديدًا
            // (IX_Images_ContentItemId فريد)، وبلا هذا الفرع تخرج كـ 500 عارٍ.
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                throw new DuplicateResourceException(
                    "A record with the same unique value already exists.", ex);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
    }
