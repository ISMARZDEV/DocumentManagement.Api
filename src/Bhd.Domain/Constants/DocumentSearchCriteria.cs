using Bhd.Domain.Enums;

namespace Bhd.Domain.Constants
{
    public class DocumentSearchCriteria
    {
        public Guid? UserId { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime? UploadDateStart { get; set; }
        public DateTime? UploadDateEnd { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
        public DocumentType? DocumentType { get; set; }
        public DocumentStatus? Status { get; set; }
        public DocumentChannel? Channel { get; set; }
        public DocumentSortBy SortBy { get; set; } 
        public SortDirection? SortDirection { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public DocumentSearchCriteria()
        {
            Page = PaginationConstants.DEFAULT_PAGE;
            PageSize = PaginationConstants.DEFAULT_PAGE_SIZE;
        }
    }
}