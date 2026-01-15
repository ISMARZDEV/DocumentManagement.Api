using Bhd.Domain.Constants;

namespace Bhd.Application.Validators.DocumentValidator
{
    public static class DocumentSearchCriteriaValidator
    {
        public static void Validate(DocumentSearchCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria), "Los criterios de búsqueda no pueden ser nulos");
            
            if (criteria.Page < PaginationConstants.DEFAULT_PAGE)
                throw new ArgumentException(
                    $"El número de página debe ser mayor o igual a {PaginationConstants.DEFAULT_PAGE}", 
                    nameof(criteria.Page));
            
            if (criteria.PageSize < 1)
                throw new ArgumentException(
                    "El tamaño de página debe ser mayor a 0", 
                    nameof(criteria.PageSize));
            
            if (criteria.PageSize > PaginationConstants.MAX_PAGE_SIZE)
                throw new ArgumentException(
                    $"El tamaño de página no puede exceder {PaginationConstants.MAX_PAGE_SIZE}", 
                    nameof(criteria.PageSize));

            if (!System.Enum.IsDefined(typeof(Domain.Enums.DocumentSortBy), criteria.SortBy))
                throw new ArgumentException(
                    "El campo sortBy es obligatorio.",
                    nameof(criteria.SortBy));
        }
    }
}