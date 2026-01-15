using Bhd.Application.DTOs.DocumentDTOs;
using FluentValidation;

namespace Bhd.Application.Validators.DocumentValidator;

public class CreateDocumentRequestValidator : AbstractValidator<CreateDocumentRequestDto>
{
    private const long MAX_FILE_SIZE_BYTES = 10 * 1024 * 1024; // 10 MB
    private const int MAX_CORRELATION_ID_LENGTH = 100;

    private static readonly string[] AllowedContentTypes = new[]
    {
        "application/pdf",
        "image/jpeg",
        "image/jpg",
        "image/png",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",   // .docx
        "application/msword",   // .doc
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xlsx
        "application/vnd.ms-excel" // .xls
    };

    public CreateDocumentRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("El archivo es obligatorio.")
            .Must(x => x.Length > 0)
            .WithMessage("El archivo no puede estar vacío.")
            .Must(x => x.Length <= MAX_FILE_SIZE_BYTES)
            .WithMessage($"El archivo no puede exceder {MAX_FILE_SIZE_BYTES / 1024 / 1024} MB.")
            .Must(x => BeAllowedContentType(x.ContentType))
            .WithMessage($"El tipo de contenido no es permitido. Tipos permitidos: {string.Join(", ", AllowedContentTypes)}")
            .Must(x => BeValidFilename(x.FileName))
            .WithMessage("El nombre del archivo contiene caracteres no permitidos");

        RuleFor(x => x.DocumentType)
            .IsInEnum()
            .WithMessage("El tipo de documento no es válido");

        RuleFor(x => x.Channel)
            .IsInEnum()
            .WithMessage("El canal no es válido");

        RuleFor(x => x.CorrelationId)
            .MaximumLength(MAX_CORRELATION_ID_LENGTH)
            .When(x => !string.IsNullOrEmpty(x.CorrelationId))
            .WithMessage($"El ID de correlación no puede exceder {MAX_CORRELATION_ID_LENGTH} caracteres");
    }

    private bool BeValidFilename(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return false;

        char[] invalidChars = Path.GetInvalidFileNameChars();
        return !filename.Any(c => invalidChars.Contains(c));
    }

    private bool BeAllowedContentType(string contentType)
    {
        return AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
    }
}
