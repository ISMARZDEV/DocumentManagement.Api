using Bhd.Domain.Constants;
using Bhd.Domain.Entities;
using Bhd.Domain.Enums;
using Bhd.Domain.Interfaces;
using Bhd.Infrastructure.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Bhd.Infrastructure.Persistance.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Document> Items, int TotalCount)> GetDocumentsAsync(DocumentSearchCriteria criteria)
        {
            var query = _context.Documents.AsNoTracking().AsQueryable();

            if (criteria.UserId.HasValue)
                query = query.Where(d => d.UserId == criteria.UserId.Value);

            if (criteria.CustomerId.HasValue)
                query = query.Where(d => d.CustomerId == criteria.CustomerId.Value);

            if (criteria.UploadDateStart.HasValue)
                query = query.Where(d => d.CreatedAt >= criteria.UploadDateStart.Value);

            if (criteria.UploadDateEnd.HasValue)
                query = query.Where(d => d.CreatedAt <= criteria.UploadDateEnd.Value);

            if (!string.IsNullOrEmpty(criteria.Filename))
                query = query.Where(d => d.Filename != null && d.Filename.Contains(criteria.Filename));

            if (!string.IsNullOrEmpty(criteria.ContentType))
                query = query.Where(d => d.ContentType == criteria.ContentType);

            if (criteria.DocumentType.HasValue)
                query = query.Where(d => d.DocumentType == criteria.DocumentType.Value);

            if (criteria.Status.HasValue)
                query = query.Where(d => d.Status == criteria.Status.Value);

            if (criteria.Channel.HasValue)
                query = query.Where(d => d.Channel == criteria.Channel.Value);

            var totalCount = await query.CountAsync();

            bool isAsc = criteria.SortDirection != SortDirection.DESC;

            query = criteria.SortBy switch
            {

                DocumentSortBy.FILENAME => isAsc
                    ? query.OrderBy(d => d.Filename)
                    : query.OrderByDescending(d => d.Filename),

                DocumentSortBy.DOCUMENT_TYPE => isAsc
                    ? query.OrderBy(d => d.DocumentType)
                    : query.OrderByDescending(d => d.DocumentType),

                DocumentSortBy.STATUS => isAsc
                    ? query.OrderBy(d => d.Status)
                    : query.OrderByDescending(d => d.Status),

                _ => isAsc
                    ? query.OrderBy(d => d.CreatedAt)
                    : query.OrderByDescending(d => d.CreatedAt)
            };

            var items = await query
                .Skip((criteria.Page - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}