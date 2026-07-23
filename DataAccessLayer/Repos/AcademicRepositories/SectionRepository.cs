using Contracts.Responses.AcademicResponses.SectionResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.AcademicRepositories
{
    public class SectionRepository : ISectionRepository
    {
        private AppDbcontext _context;
        private readonly Expression<Func<Section, SectionDTO>> ToDTO = s => new SectionDTO
        {
            SectionId = s.SectionId,
            SectionName = s.Name,
            BatchId = s.BatchId,
            BatchName = s.Batch.Name,
            CreatedAt = s.CreatedAt,
            CreatedByUserId = s.CreatedByUserId,
            CreatedByUserName = s.CreatedByUser.UserName,
            UpdatedAt = s.UpdatedAt,
            UpdatedByUserId = s.UpdatedByUserId,
            UpdatedByUserName = s.UpdatedByUser == null ? null : s.UpdatedByUser.UserName,
        };

        public SectionRepository(AppDbcontext context)
        {
            _context = context;
        }

        public void Add(Section section)
        {
            _context.Sections.Add(section);
        }

        public async Task<bool> Delete(int sectionId)
        {
            var section = await _context.Sections.FindAsync(sectionId);
            if (section == null)
                return false;

            _context.Sections.Remove(section);
            return true;
        }

        public async Task<bool> ExistsById(int sectionId)
        {
           return await _context.Sections.AnyAsync(s=>s.SectionId ==  sectionId);
        }

        public async Task<bool> ExistsByName(int batchId, string name)
        {
            return await _context.Sections.Where(s => s.BatchId == batchId).AnyAsync(s => s.Name == name);
        }

        public async Task<PagedResult<SectionDTO>> GetAllSections(int pageNumber, int pageSize)
        {
            return await _context.Sections.AsNoTracking().OrderBy(s => s.BatchId).Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<SectionDTO?> GetDTOById(int sectionId)
        {
            return await _context.Sections.AsNoTracking().Select(ToDTO).SingleOrDefaultAsync(s => s.SectionId == sectionId);
        }

        public async Task<SectionDTO?> GetDTOByName(int batchId,string name)
        {
            return await  _context.Sections.AsNoTracking().Where(s=>s.BatchId == batchId && s.Name == name).Select(ToDTO).SingleOrDefaultAsync();
        }

        public async Task<Section?> GetEntityById(int sectionId)
        {
            return await _context.Sections.FindAsync(sectionId);
        }

        public async Task<Section?> GetEntityByName(int batchId,string name)
        {
            return await _context.Sections.Where(s => s.BatchId == batchId && s.Name == name).SingleOrDefaultAsync();
        }

        public async Task<PagedResult<SectionDTO>> GetSectionsPerBatch(int batchId, int pageNumber, int pageSize)
        {
           return await _context.Sections.AsNoTracking().Where(s=>s.BatchId == batchId).Select(ToDTO).ToPagedResultAsync(pageNumber, pageSize);
        }
    }
}
