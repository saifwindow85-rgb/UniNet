using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SectionSubjectRequests;
using Contracts.Responses.StudyResponses.SectionSubjectResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Study;
using Domain.Interfaces.StudyInterfaces.SectionSubjectInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.StudyRepository
{
    public class SectionSubjectRepository : ISectionSubjectRepository
    {
        private readonly AppDbcontext _context;

        private readonly Expression<Func<SectionSubject, SectionSubjectDTO>> ToDto = ss => new SectionSubjectDTO
        {
            SectionSubjectId = ss.SectionSubjectId,
            SectionId = ss.SectionId,
            SectionName = ss.Section.Name,
            SubjectId = ss.SubjectId,
            SubjectName = ss.Subject.Name,
            SubjectCode = ss.Subject.Code,
            SemesterId = ss.SemesterId,
            SemesterName = ss.Semester.Name,
            LecturerName = ss.LecturerName,
        };

        private readonly Expression<Func<SectionSubject, SectionSubjectAuthorizationInfo>> ToInfo = ss => new SectionSubjectAuthorizationInfo
        {
            UniversityId = ss.Section.Batch.Department.College.UniversityId,
            CollegeId = ss.Section.Batch.Department.CollegeId,
            DepartmentId = ss.Section.Batch.DepartmentId,
            BatchId = ss.Section.BatchId,
            SectionId = ss.SectionId,
        };

        private readonly Expression<Func<SectionSubject, DetaieldSectionSubjectDTO>> ToDetaieldDto = ss => new DetaieldSectionSubjectDTO
        {
            SectionSubjectId = ss.SectionSubjectId,
            SectionId = ss.SectionId,
            SectionName = ss.Section.Name,
            SubjectId = ss.SubjectId,
            SubjectName = ss.Subject.Name,
            SubjectCode = ss.Subject.Code,
            SemesterId = ss.SemesterId,
            SemesterName = ss.Semester.Name,
            DepartmentId = ss.Section.Batch.DepartmentId,
            DepartmentName = ss.Section.Batch.Department.Name,
            BatchId = ss.Section.BatchId,
            BatchName = ss.Section.Batch.Name,
            LecturerName = ss.LecturerName,
            CreatedAt = ss.CreatedAt,
            CreatedByUserId = ss.CreatedByUserId,
            CreatedByUserName = ss.CreatedByUser.UserName,
            UpdatedAt = ss.UpdatedAt,
            UpdatedByUserId = ss.UpdatedByUserId,
            UpdatedByUserName = ss.UpdatedByUser == null ? null : ss.UpdatedByUser.UserName,
        };

        public SectionSubjectRepository(AppDbcontext context)
        {
            _context = context;
        }

        public void Add(SectionSubject sectionSubject)
        {
            _context.SectionSubjects.Add(sectionSubject);
        }

        public bool Delete(SectionSubject sectionSubject)
        {
            if (sectionSubject == null)
                return false;
            _context.SectionSubjects.Remove(sectionSubject);
            return true;
        }

        public async Task<PagedResult<SectionSubjectDTO>> GetAll(SectionSubjectFilterDTO? filter, int pageNumber, int pageSize)
        {
            if (filter == null)
                filter = new SectionSubjectFilterDTO();

            var query = _context.SectionSubjects.AsNoTracking()
                .OrderBy(ss => ss.SemesterId).ThenBy(ss => ss.SectionId).ThenBy(ss => ss.SubjectId).AsQueryable();

            query = ApplyFilter(query, filter);

            return await query.Select(ToDto).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<SectionSubjectDTO>> GetSectionSubjectsPerScope(UserScope? scope, SectionSubjectFilterDTO? filter, int pageNumber, int pageSize)
        {
            if (filter == null)
                filter = new SectionSubjectFilterDTO();

            if (scope == null)
                scope = new UserScope();

            var query = _context.SectionSubjects.AsNoTracking()
                .OrderBy(ss => ss.SemesterId).ThenBy(ss => ss.SectionId).ThenBy(ss => ss.SubjectId).AsQueryable();

            query = ApplyFilter(query, filter);

            if (!scope.IsGlobal)
            {
                if (scope.UniversityId.HasValue)
                {
                    query = query.Where(ss => ss.Section.Batch.Department.College.UniversityId == scope.UniversityId);
                }

                if (scope.CollegeId.HasValue)
                {
                    query = query.Where(ss => ss.Section.Batch.Department.CollegeId == scope.CollegeId);
                }

                if (scope.DepartmentId.HasValue)
                {
                    query = query.Where(ss => ss.Section.Batch.DepartmentId == scope.DepartmentId);
                }

                if (scope.BatchId.HasValue)
                {
                    query = query.Where(ss => ss.Section.BatchId == scope.BatchId);
                }
            }

            return await query.Select(ToDto).ToPagedResultAsync(pageNumber, pageSize);
        }

        private static IQueryable<SectionSubject> ApplyFilter(IQueryable<SectionSubject> query, SectionSubjectFilterDTO filter)
        {
            if (filter.SectionId.HasValue)
            {
                query = query.Where(ss => ss.SectionId == filter.SectionId);
            }

            if (filter.SubjectId.HasValue)
            {
                query = query.Where(ss => ss.SubjectId == filter.SubjectId);
            }

            if (filter.SemesterId.HasValue)
            {
                query = query.Where(ss => ss.SemesterId == filter.SemesterId);
            }

            if (!string.IsNullOrEmpty(filter.LecturerName))
            {
                query = query.Where(ss => EF.Functions.Like(ss.LecturerName, $"%{filter.LecturerName}%"));
            }

            return query;
        }

        public async Task<SectionSubjectDTO?> GetDTOById(int sectionSubjectId)
        {
            return await _context.SectionSubjects.Where(ss => ss.SectionSubjectId == sectionSubjectId).Select(ToDto).SingleOrDefaultAsync();
        }

        public async Task<DetaieldSectionSubjectDTO?> GetDetaieldSectionSubjectDTOById(int sectionSubjectId)
        {
            return await _context.SectionSubjects.Where(ss => ss.SectionSubjectId == sectionSubjectId).Select(ToDetaieldDto).SingleOrDefaultAsync();
        }

        public async Task<SectionSubject?> GetEntityById(int sectionSubjectId)
        {
            return await _context.SectionSubjects.FindAsync(sectionSubjectId);
        }

        public async Task<SectionSubjectAuthorizationInfo?> GetSectionSubjectAuthorizationInfoAsync(int sectionSubjectId)
        {
            return await _context.SectionSubjects.Where(ss => ss.SectionSubjectId == sectionSubjectId).Select(ToInfo).SingleOrDefaultAsync();
        }

        public async Task<bool> IsExistsById(int sectionSubjectId)
        {
            return await _context.SectionSubjects.AnyAsync(ss => ss.SectionSubjectId == sectionSubjectId);
        }

        public async Task<bool> IsAlreadyAssigned(int sectionId, int subjectId, int semesterId)
        {
            return await _context.SectionSubjects.AnyAsync(ss => ss.SectionId == sectionId && ss.SubjectId == subjectId && ss.SemesterId == semesterId);
        }
    }
}
