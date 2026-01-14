using Bhd.Application.DTOs.DocumentDTOs;
using Bhd.Application.Interfaces;
using Bhd.Domain.Constants;
using Bhd.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bhd.WebApi.Controller
{
    [ApiController]
    [Route("api/bhd/mgmt/1/documents")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        #region GetDocumentsAsync
        /// <summary>
        /// Obtiene una lista paginada de documentos con filtros de busqueda y ordenamiento.
        /// </summary>
        /// <param name="userId">ID del usuario (filtro de seguridad)</param>
        /// <param name="customerId">ID del cliente</param>
        /// <param name="filename">Nombre del archivo a buscar</param>
        /// <param name="contentType">Tipo de contenido (MIME type)</param>
        /// <param name="documentType">Tipo de documento</param>
        /// <param name="status">Estado del documento</param>
        /// <param name="channel">Canal de distribución</param>
        /// <param name="uploadDateStart">Fecha de inicio para filtrar</param>
        /// <param name="uploadDateEnd">Fecha de fin para filtrar</param>
        /// <param name="sortBy">Campo por el que ordenar (por defecto UPLOAD_DATE)</param>
        /// <param name="sortDirection">Dirección del ordenamiento (ASC o DESC)</param>
        /// <param name="page">Número de página (por defecto 1)</param>
        /// <param name="pageSize">Cantidad de registros por página (por defecto 10)</param>
        /// <response code="200">Documentos encontrados correctamente</response>
        /// <response code="400">Criterios de búsqueda inválidos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Estás autenticado, pero no tienes permisos</response>
        /// <response code="404">Docomento no encontrado</response>
        /// <response code="500">Error interno del servidor</response>            
        #endregion
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDocumentsAsync(
            [FromQuery] Guid? userId,
            [FromQuery] Guid? customerId,
            [FromQuery] string? filename,
            [FromQuery] string? contentType,
            [FromQuery] DocumentType? documentType,
            [FromQuery] DocumentStatus? status,
            [FromQuery] DocumentChannel? channel,
            [FromQuery] DateTime? uploadDateStart,
            [FromQuery] DateTime? uploadDateEnd,
            [FromQuery] DocumentSortBy sortBy = DocumentSortBy.UPLOAD_DATE,
            [FromQuery] SortDirection? sortDirection = null,
            [FromQuery] int page = PaginationConstants.DEFAULT_PAGE,
            [FromQuery] int pageSize = PaginationConstants.DEFAULT_PAGE_SIZE)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var searchDto = new DocumentSearchDto
            {
                CurrentUserId = !string.IsNullOrEmpty(currentUserId) ? Guid.Parse(currentUserId) : null,
                IsAdmin = isAdmin,
                UserId = userId,
                CustomerId = customerId,
                Filename = filename,
                ContentType = contentType,
                DocumentType = documentType,
                Status = status,
                Channel = channel,
                UploadDateStart = uploadDateStart,
                UploadDateEnd = uploadDateEnd,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                PageSize = pageSize
            };

            var (documents, totalCount) = await _documentService.GetDocumentsAsync(searchDto);

            var response = new
            {
                items = documents,
                totalCount = totalCount,
                page = searchDto.Page,
                pageSize = searchDto.PageSize
            };

            return Ok(response);
        }
    }
}
