using Bhd.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Bhd.Application.DTOs.DocumentDTOs
{
    public class CreateDocumentRequestDto
    {
        /// <summary>
        /// Archivo a cargar
        /// </summary>
        public IFormFile File { get; set; } = null!;

        /// <summary>
        /// Tipo de documento
        /// </summary>
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// Canal de origen
        /// </summary>
        public DocumentChannel Channel { get; set; }

        /// <summary>
        /// ID del cliente (obligatorio para Operadores/Admins, opcional para Clientes)
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// ID de correlación para trazabilidad
        /// </summary>
        public string? CorrelationId { get; set; }
    }
}