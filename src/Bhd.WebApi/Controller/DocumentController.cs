using Bhd.Application.Commands.DocumentCommands;
using Bhd.Application.DTOs.DocumentDTOs;
using Bhd.Application.Interfaces;
using Bhd.Domain.Constants;
using Bhd.Domain.Enums;
using Bhd.Infrastructure.Handlers.DocumentHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Bhd.WebApi.Controller
{
    [ApiController]
    [Route("api/bhd/mgmt/1/documents")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly CreateDocumentCommandHandler _createDocumentHandler;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(
            IDocumentService documentService,
            CreateDocumentCommandHandler createDocumentHandler,
            ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _createDocumentHandler = createDocumentHandler;
            _logger = logger;
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
        /// <param name="sortBy">[Obligatorio] Campo por el que ordenar (UPLOAD_DATE, FILENAME, STATUS, CHANNEL)</param>
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
            [FromQuery][Required] DocumentSortBy sortBy,
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

        #region UploadDocumentAsync
        /// <summary>
        /// Carga asincrona de un documento se guarda en staging, se persiste en BD y se encola para procesamiento.
        /// </summary>
        /// <param name="request">Datos del documento a cargar (multipart/form-data)</param>
        /// <response code="202">Documento aceptado y encolado para procesamiento</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>ID del documento y del job de procesamiento</returns>
        #endregion
        [HttpPost("actions/upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadDocumentAsync([FromForm] CreateDocumentRequestDto request)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                if (string.IsNullOrEmpty(userRole))
                {
                    return Unauthorized(new { message = "Rol del usuario no encontrado." });
                }

                _logger.LogInformation(
                    "Iniciando carga de documento. Usuario: {UserId}, Rol: {Role}, Filename: {Filename}",
                    currentUserId, userRole, request.File.FileName);

                string encodedFile;
                using (var stream = request.File.OpenReadStream())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    encodedFile = Convert.ToBase64String(fileBytes);
                }
                var command = new CreateDocumentCommand
                {
                    Filename = request.File.FileName,
                    EncodedFile = encodedFile,
                    ContentType = request.File.ContentType,
                    DocumentType = request.DocumentType,
                    Channel = request.Channel,
                    CustomerId = request.CustomerId,
                    CorrelationId = request.CorrelationId,
                    UserId = Guid.Parse(currentUserId),
                    UserRole = userRole
                };

                var result = await _createDocumentHandler.HandleAsync(command);

                _logger.LogInformation(
                    "Documento creado exitosamente. DocumentId: {DocumentId}, JobId: {JobId}",
                    result.DocumentId, result.JobId);

                return Accepted(result);
            }
            catch (Bhd.Application.Exceptions.BadRequestException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear documento");
                return BadRequest(new { message = ex.Message });
            }
            catch (Bhd.Application.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear documento");
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Acceso no autorizado al crear documento");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear documento");
                return StatusCode(500, new { message = "Error interno del servidor al procesar la solicitud." });
            }
        }
    }
}
