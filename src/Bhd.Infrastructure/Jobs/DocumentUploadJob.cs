using Bhd.Domain.Entities;
using Bhd.Domain.Enums;
using Bhd.Infrastructure.Persistance.Contexts;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bhd.Infrastructure.Jobs;

/// <summary>
/// Job de Hangfire para procesar documentos de forma asíncrona.
/// Se ejecuta en background después de que el documento ha sido recibido y guardado en staging.
/// </summary>
public class DocumentUploadJob
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DocumentUploadJob> _logger;

    public DocumentUploadJob(
        ApplicationDbContext context,
        ILogger<DocumentUploadJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 60, 120,  180, 300, 600 })]
    public async Task ProcessAsync(Guid documentId, string stagingPath)
    {
        _logger.LogInformation(
            "Iniciando procesamiento del documento {DocumentId} desde staging {StagingPath}",
            documentId, stagingPath);

        try
        {
            // DEMO: Delay para ver el archivo en staging (comentar en producción)
            await Task.Delay(30000); // 30 segundos para que veas el archivo en /temp

            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
            {
                _logger.LogError("Documento {DocumentId} no encontrado en la base de datos", documentId);
                throw new InvalidOperationException($"Documento {documentId} no encontrado.");
            }

            // DEMO: Simular fallo si el nombre es "fail.pdf"
            if (document.Filename.ToLower() == "fail.pdf")
            {
                _logger.LogWarning("DEMO: Simulanción de fallo 'FAILED' intencional para documento {DocumentId}", documentId);
                throw new InvalidOperationException("DEMO: Archivo rechazado para pruebas de error 'FAILED'");
            }

            if (!File.Exists(stagingPath))
            {
                _logger.LogError(
                    "Archivo de staging no encontrado: {StagingPath} para documento {DocumentId}",
                    stagingPath, documentId);
                
                document.Status = DocumentStatus.FAILED;
                await _context.SaveChangesAsync();
                return;
            }

            // Almacenamiento permanente: Lógica para subir a un servicio externo Azure Blob Storage, AWS S3, etc...
            var permanentPath = await MoveToPermenentStorageAsync(stagingPath, document);

            document.FileUrl = permanentPath;
            document.Status = DocumentStatus.SENT;
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Documento {DocumentId} procesado exitosamente. FileUrl: {FileUrl}, Status: {Status}",
                documentId, permanentPath, document.Status);

            CleanupStagingFile(stagingPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error al procesar el documento {DocumentId} desde staging {StagingPath}",
                documentId, stagingPath);

            try
            {
                var document = await _context.Documents.FindAsync(documentId);
                if (document != null)
                {
                    document.Status = DocumentStatus.FAILED;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception updateEx)
            {
                _logger.LogError(updateEx, 
                    "Error adicional al actualizar estado FAILED del documento {DocumentId}",
                    documentId);
            }

            throw;
        }
    }

    private async Task<string> MoveToPermenentStorageAsync(string stagingPath, Document document)
    {
        var permanentDirectory = Path.Combine(
            Directory.GetCurrentDirectory(), 
            "DocumentStorage", 
            document.CreatedAt.Year.ToString(),
            document.CreatedAt.Month.ToString("D2"));

        Directory.CreateDirectory(permanentDirectory);

        var permanentFilename = $"{document.Id}_{Path.GetFileName(document.Filename)}";
        var permanentPath = Path.Combine(permanentDirectory, permanentFilename);

        await Task.Run(() => File.Copy(stagingPath, permanentPath, overwrite: true));

        _logger.LogInformation(
            "Archivo movido de staging a almacenamiento permanente: {PermanentPath}",
            permanentPath);

        return permanentPath;
    }

    private void CleanupStagingFile(string stagingPath)
    {
        try
        {
            if (File.Exists(stagingPath))
            {
                File.Delete(stagingPath);
                _logger.LogInformation("Archivo de staging eliminado: {StagingPath}", stagingPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, 
                "No se pudo eliminar el archivo de staging: {StagingPath}. Se limpiará manualmente.",
                stagingPath);
        }
    }
}
