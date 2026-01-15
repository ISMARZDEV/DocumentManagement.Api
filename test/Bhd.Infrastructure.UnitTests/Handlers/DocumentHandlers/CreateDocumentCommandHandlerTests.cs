using Bhd.Application.Commands.DocumentCommands;
using Bhd.Application.Exceptions;
using Bhd.Domain.Constants;
using Bhd.Domain.Enums;
using Bhd.Infrastructure.Handlers.DocumentHandlers;
using Bhd.Infrastructure.Jobs;
using Bhd.Infrastructure.UnitTests.Fixtures;
using Bhd.Infrastructure.UnitTests.TestData;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bhd.Infrastructure.UnitTests.Handlers.DocumentHandlers;

public class CreateDocumentCommandHandlerTests : IClassFixture<ApplicationDbContextFixture>
{
    private readonly ApplicationDbContextFixture _fixture;
    private readonly Mock<ILogger<CreateDocumentCommandHandler>> _mockLogger;
    private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;

    public CreateDocumentCommandHandlerTests(ApplicationDbContextFixture fixture)
    {
        _fixture = fixture;
        _mockLogger = new Mock<ILogger<CreateDocumentCommandHandler>>();
        _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();

        _mockBackgroundJobClient
            .Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()))
            .Returns("job-12345");

        _fixture.ClearDatabase();
    }

    #region TC-01: Validación de Roles y CustomerId

    // TC-1-01 - Cliente sin CustomerId

    [Fact]
    public async Task Should_CreateDocument_WhenUserIsClient_WithoutCustomerId()
    {
        // Arrange
        var command = TestDataBuilder.CreateCommandForClient();
        var handler = CreateHandler();

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.DocumentId.Should().NotBeEmpty();
        result.Status.Should().Be(DocumentStatus.RECEIVED.ToString());

        var document = await _fixture.Context.Documents.FindAsync(result.DocumentId);
        document.Should().NotBeNull();
        document!.CustomerId.Should().Be(command.UserId, "Para clientes, CustomerId debe ser igual a UserId");
    }

    // TC-1-02 - Operador sin CustomerId

    [Fact]
    public async Task Should_ThrowBadRequestException_WhenUserIsOperatorWithoutCustomerId()
    {
        // Arrange
        var command = TestDataBuilder.CreateCommandForOperatorWithoutCustomerId();
        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*CustomerId es obligatorio*");
    }

    // TC-1-03 - Operador con CustomerId

    [Fact]
    public async Task Should_CreateDocument_WhenUserIsOperatorWithCustomerId()
    {
        // Arrange
        var command = TestDataBuilder.CreateCommandForOperator();
        var handler = CreateHandler();

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        var document = await _fixture.Context.Documents.FindAsync(result.DocumentId);
        document.Should().NotBeNull();
        document!.CustomerId.Should().Be(command.CustomerId, "Para operadores, debe usarse el CustomerId proporcionado");
    }

    //TC-1-04 - Rol Inválido (Seguridad)

    [Fact]
    public async Task Should_ThrowUnauthorizedAccessException_WhenUserRoleIsInvalid()
    {
        // Arrange
        var command = TestDataBuilder.CreateCommandWithInvalidRole();
        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*no reconocido*");
    }

    #endregion

    #region TC-02: Procesamiento de Archivos

    // TC-2-01 - Guardar en Staging

    [Fact]
    public async Task Should_SaveFileToStaging_AndReturnValidPath()
    {
        // Arrange
        var command = TestDataBuilder.CreateValidCommand();
        var handler = CreateHandler();

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        var document = await _fixture.Context.Documents.FindAsync(result.DocumentId);
        document.Should().NotBeNull();
        document!.Size.Should().BeGreaterThan(0, "El archivo debe tener un tamaño válido");
    }

    // TC-2-02 - Base64 Inválido

    [Fact]
    public async Task Should_ThrowBadRequestException_WhenBase64IsInvalid()
    {
        // Arrange
        var command = TestDataBuilder.CreateCommandWithInvalidBase64();
        var handler = CreateHandler();

        // Act
        Func<Task> act = async () => await handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("*Base64 válido*");
    }

    // TC-2-03 - Cálculo de Tamaño

    [Fact]
    public async Task Should_CalculateFileSize_Correctly()
    {
        // Arrange
        var expectedSizeInBytes = 5 * 1024 * 1024; // 5 MB
        var command = TestDataBuilder.CreateValidCommand();
        command.EncodedFile = TestDataBuilder.CreateBase64File(expectedSizeInBytes);
        var handler = CreateHandler();

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var document = await _fixture.Context.Documents.FindAsync(result.DocumentId);
        document.Should().NotBeNull();
        document!.Size.Should().Be(expectedSizeInBytes, "El tamaño del archivo debe calcularse correctamente");
    }

    #endregion

    #region TC-02: Creación de Entidad Document

    // TC-3-01 - Propiedades Iniciales

    [Fact]
    public async Task Should_CreateDocument_WithCorrectInitialProperties()
    {
        // Arrange
        var command = TestDataBuilder.CreateValidCommand();
        var handler = CreateHandler();

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var document = await _fixture.Context.Documents.FindAsync(result.DocumentId);
        document.Should().NotBeNull();
        document!.Status.Should().Be(DocumentStatus.RECEIVED);
        document.FileUrl.Should().BeNull("FileUrl debe ser null hasta que el job procese el documento");
        document.Filename.Should().Be(command.Filename);
        document.ContentType.Should().Be(command.ContentType);
        document.DocumentType.Should().Be(command.DocumentType);
        document.Channel.Should().Be(command.Channel);
        document.UserId.Should().Be(command.UserId);
    }

    // TC-3-02 - Fecha UTC

    [Fact]
    public async Task Should_SetCreatedAtToUtcNow()
    {
        // Arrange
        var command = TestDataBuilder.CreateValidCommand();
        var handler = CreateHandler();
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var result = await handler.HandleAsync(command);
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        var document = await _fixture.Context.Documents.FindAsync(result.DocumentId);
        document.Should().NotBeNull();
        document!.CreatedAt.Should().BeAfter(beforeCreation);
        document.CreatedAt.Should().BeBefore(afterCreation);
    }

    // TC-3-03 - IDs Únicos

    [Fact]
    public async Task Should_AssignUniqueGuids()
    {
        // Arrange
        var command1 = TestDataBuilder.CreateValidCommand();
        var command2 = TestDataBuilder.CreateValidCommand();
        var handler = CreateHandler();

        // Act
        var result1 = await handler.HandleAsync(command1);
        var result2 = await handler.HandleAsync(command2);

        // Assert
        result1.DocumentId.Should().NotBe(result2.DocumentId, "Cada documento debe tener un Id único");
    }

    #endregion

    #region TC-04: Integración con Hangfire y CorrelationId

    // TC-4-01 - Encolar Job y CorrelationId Auto

    [Fact]
    public async Task Should_EnqueueBackgroundJob_AndUpdateCorrelationId_WhenCorrelationIdIsEmpty()
    {
        // Arrange
        var command = TestDataBuilder.CreateCommandWithoutCorrelationId();
        var expectedJobId = "job-abc-123";
        
        _mockBackgroundJobClient
            .Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()))
            .Returns(expectedJobId);

        var handler = CreateHandler();

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.JobId.Should().Be(expectedJobId);
        
        var document = await _fixture.Context.Documents.FindAsync(result.DocumentId);
        document.Should().NotBeNull();
        document!.CorrelationId.Should().Be(expectedJobId, "Si no hay CorrelationId, debe asignarse el JobId");

        _mockBackgroundJobClient.Verify(
            x => x.Create(
                It.Is<Job>(job => 
                    job.Type == typeof(DocumentUploadJob) && 
                    job.Method.Name == "ProcessAsync"),
                It.IsAny<EnqueuedState>()),
            Times.Once);
    }

    // TC-4-02 - Preservar CorrelationId

    [Fact]
    public async Task Should_PreserveCorrelationId_WhenProvided()
    {
        // Arrange
        var command = TestDataBuilder.CreateValidCommand();
        var originalCorrelationId = command.CorrelationId;
        var handler = CreateHandler();

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var document = await _fixture.Context.Documents.FindAsync(result.DocumentId);
        document.Should().NotBeNull();
        document!.CorrelationId.Should().Be(originalCorrelationId, "El CorrelationId proporcionado debe preservarse");
    }

    #endregion

    #region Helper Methods

    private CreateDocumentCommandHandler CreateHandler()
    {
        return new CreateDocumentCommandHandler(
            _fixture.Context,
            _mockLogger.Object,
            _mockBackgroundJobClient.Object);
    }

    #endregion
}
