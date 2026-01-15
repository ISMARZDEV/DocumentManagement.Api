using Bhd.Application.Commands.DocumentCommands;
using Bhd.Application.Exceptions;
using Bhd.Domain.Constants;
using Bhd.Domain.Entities;
using Bhd.Domain.Enums;
using Bhd.Infrastructure.Jobs;
using Bhd.Infrastructure.Persistance.Contexts;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Bhd.Infrastructure.Handlers.DocumentHandlers;

/// <summary>
/// Handler para el comando CreateDocument
/// Implementa el patrón Fire-and-Forget con staging
/// </summary>
public class CreateDocumentCommandHandler
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreateDocumentCommandHandler> _logger;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public CreateDocumentCommandHandler(
        ApplicationDbContext context,
        ILogger<CreateDocumentCommandHandler> logger,
        IBackgroundJobClient backgroundJobClient)
    {
        _context = context;
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<CreateDocumentResult> HandleAsync(CreateDocumentCommand command)
    {
        var effectiveCustomerId = ValidateAndResolveCustomerId(command);

        var stagingPath = await SaveFileToStagingAsync(command.EncodedFile, command.Filename);

        var fileSize = GetFileSize(stagingPath);

        var document = new Document
        {
            Id = Guid.NewGuid(),
            Filename = command.Filename,
            ContentType = command.ContentType,
            Size = fileSize,
            DocumentType = command.DocumentType,
            Channel = command.Channel,
            Status = DocumentStatus.RECEIVED,
            FileUrl = null, // se le asignará cuando se procese el documento
            CorrelationId = command.CorrelationId,
            CustomerId = effectiveCustomerId,
            UserId = command.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Documento {DocumentId} creado exitosamente. Filename: {Filename}, Status: {Status}",
            document.Id, document.Filename, document.Status);

        var jobId = _backgroundJobClient.Enqueue<DocumentUploadJob>(
            job => job.ProcessAsync(document.Id, stagingPath));

        // Si CorrelationId viene vacío, asignar jobId como CorrelationId
        if (string.IsNullOrWhiteSpace(document.CorrelationId))
        {
            document.CorrelationId = jobId;
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation(
            "Job {JobId} encolado para procesar documento {DocumentId}",
            jobId, document.Id);

        return new CreateDocumentResult
        {
            DocumentId = document.Id,
            JobId = jobId,
            Status = document.Status.ToString(),
            Message = $"Documento recibido exitosamente. Job {jobId} encolado para procesamiento."
        };
    }

    private Guid? ValidateAndResolveCustomerId(CreateDocumentCommand command)
    {
        // si es Cliente, se usa su propio UserId como CustomerId
        if (command.UserRole == Roles.Client)
        {
            return command.UserId;
        }

        // si es Operador o Admin, el CustomerId es obligatorio
        if (command.UserRole == Roles.Operator || command.UserRole == Roles.Admin)
        {
            if (!command.CustomerId.HasValue)
            {
                throw new BadRequestException(
                    "CustomerId es obligatorio para usuarios con rol de Operador o Admin.");
            }
            return command.CustomerId.Value;
        }

        throw new UnauthorizedAccessException(
            $"Rol '{command.UserRole}' no reconocido para esta operación.");
    }

    private async Task<string> SaveFileToStagingAsync(string encodedFile, string originalFilename)
    {
        try
        {
            var fileBytes = Convert.FromBase64String(encodedFile);

            var stagingDirectory = Path.GetTempPath();
            var uniqueFilename = $"{Guid.NewGuid()}_{Path.GetFileName(originalFilename)}";
            var stagingPath = Path.Combine(stagingDirectory, uniqueFilename);

            // Guardar el archivo en disco
            await File.WriteAllBytesAsync(stagingPath, fileBytes);

            _logger.LogInformation(
                "Archivo guardado en staging: {StagingPath}, Tamaño: {Size} bytes",
                stagingPath, fileBytes.Length);

            return stagingPath;
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Error al decodificar el archivo Base64");
            throw new BadRequestException("El archivo codificado no tiene un formato Base64 válido.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el archivo en staging");
            throw new InvalidOperationException("Error al guardar el archivo en el sistema de staging.", ex);
        }
    }

    private long GetFileSize(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        return fileInfo.Length;
    }
}
