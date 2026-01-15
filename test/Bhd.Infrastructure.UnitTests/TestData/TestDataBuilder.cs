using Bhd.Application.Commands.DocumentCommands;
using Bhd.Domain.Constants;
using Bhd.Domain.Enums;

namespace Bhd.Infrastructure.UnitTests.TestData;

public class TestDataBuilder
{
    public static CreateDocumentCommand CreateValidCommand(string userRole = Roles.Client)
    {
        var base64File = Convert.ToBase64String(new byte[] { 0x25, 0x50, 0x44, 0x46 });
        return new CreateDocumentCommand
        {
            Filename = "test-document.pdf",
            EncodedFile = base64File,
            ContentType = "application/pdf",
            DocumentType = DocumentType.KYC,
            Channel = DocumentChannel.DIGITAL,
            CustomerId = userRole == Roles.Client ? null : Guid.NewGuid(),
            CorrelationId = "CORR-12345",
            UserId = Guid.NewGuid(),
            UserRole = userRole
        };
    }

    public static string CreateBase64File(int sizeInBytes)
    {
        var bytes = new byte[sizeInBytes];
        new Random().NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public static CreateDocumentCommand CreateCommandWithInvalidBase64()
    {
        var command = CreateValidCommand();
        command.EncodedFile = "InvalidBase64!!!@@@###";
        return command;
    }

    public static CreateDocumentCommand CreateCommandForClient()
    {
        var command = CreateValidCommand(Roles.Client);
        command.CustomerId = null;
        return command;
    }

    public static CreateDocumentCommand CreateCommandForOperator()
    {
        var command = CreateValidCommand(Roles.Operator);
        command.CustomerId = Guid.NewGuid();
        return command;
    }

    public static CreateDocumentCommand CreateCommandForOperatorWithoutCustomerId()
    {
        var command = CreateValidCommand(Roles.Operator);
        command.CustomerId = null;
        return command;
    }

    public static CreateDocumentCommand CreateCommandWithInvalidRole()
    {
        var command = CreateValidCommand();
        command.UserRole = "RolInexistente";
        return command;
    }

    public static CreateDocumentCommand CreateCommandWithoutCorrelationId()
    {
        var command = CreateValidCommand();
        command.CorrelationId = null;
        return command;
    }
}
