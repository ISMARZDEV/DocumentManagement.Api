using Bhd.Domain.Enums;

namespace Bhd.Application.DTOs.DocumentDTOs
{
    public class DocumentDto
    {
        /// <summary>
        /// ID único del documento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del archivo
        /// </summary>
        public string Filename { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de contenido (MIME type)
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de documento
        /// </summary>
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// Canal de distribución
        /// </summary>
        public DocumentChannel Channel { get; set; }

        /// <summary>
        /// ID del cliente
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Estado del documento
        /// </summary>
        public DocumentStatus Status { get; set; }

        /// <summary>
        /// URL del archivo
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Tamaño del archivo en bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Fecha de carga del documento
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// ID de correlación
        /// </summary>
        public string? CorrelationId { get; set; }
    }
}