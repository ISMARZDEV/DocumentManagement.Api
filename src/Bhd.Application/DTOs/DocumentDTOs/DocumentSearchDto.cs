using Bhd.Domain.Constants;
using Bhd.Domain.Enums;

namespace Bhd.Application.DTOs.DocumentDTOs
{
    public class DocumentSearchDto
    {
        /// <summary>
        /// Número de página (por defecto 1)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Cantidad de registros por página (por defecto 10)
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// ID del usuario actual (del JWT)
        /// </summary>
        public Guid? CurrentUserId { get; set; }

        /// <summary>
        /// Indica si el usuario actual es administrador
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// ID del usuario (filtro de búsqueda)
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// ID del cliente
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Fecha de inicio para filtrar documentos
        /// </summary>
        public DateTime? UploadDateStart { get; set; }

        /// <summary>
        /// Fecha de fin para filtrar documentos
        /// </summary>
        public DateTime? UploadDateEnd { get; set; }

        /// <summary>
        /// Nombre del archivo a buscar
        /// </summary>
        public string? Filename { get; set; }

        /// <summary>
        /// Tipo de contenido (MIME type)
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Tipo de documento
        /// </summary>
        public DocumentType? DocumentType { get; set; }

        /// <summary>
        /// Estado del documento
        /// </summary>
        public DocumentStatus? Status { get; set; }

        /// <summary>
        /// Canal de distribución
        /// </summary>
        public DocumentChannel? Channel { get; set; }

        /// <summary>
        /// Campo por el que ordenar
        /// </summary>
        public DocumentSortBy SortBy { get; set; } = DocumentSortBy.UPLOAD_DATE;

        /// <summary>
        /// Dirección del ordenamiento
        /// </summary>
        public SortDirection? SortDirection { get; set; }

        /// <summary>
        /// Constructor que inicializa los valores por defecto de paginación
        /// </summary>
        public DocumentSearchDto()
        {
            Page = PaginationConstants.DEFAULT_PAGE;
            PageSize = PaginationConstants.DEFAULT_PAGE_SIZE;
        }
    }
}