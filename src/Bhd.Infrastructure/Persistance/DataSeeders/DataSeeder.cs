using Bhd.Application.Interfaces;
using Bhd.Domain.Constants;
using Bhd.Domain.Entities;
using Bhd.Domain.Enums;
using Bhd.Infrastructure.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Bhd.Infrastructure.Persistance.DataSeeders
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public DataSeeder(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            try
            {
                var hasUsers = await _context.Users.AnyAsync();
                if (hasUsers)
                {
                    return;
                }

                await SeedUsersAsync();
                await SeedDocumentsAsync();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error seeding database", ex);
            }
        }

        private async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                return;
            }

            var users = new[]
            {   new User
                {
                    Id = Guid.Parse("aebdcc30-a004-42b7-8c9c-cba97a500780"),
                    Email = "admin@prueba.com",
                    UserName = "admin",
                    Name = "Administrador",
                    PasswordHash = _passwordHasher.HashPassword("Candado6947!"),
                    Role = Roles.Admin,
                    CreatedAt = DateTime.UtcNow,
                },
                new User
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440010"),
                    Email = "operador@prueba.com",
                    UserName = "operador",
                    Name = "María de Jesús",
                    PasswordHash = _passwordHasher.HashPassword("Candado6947!"),
                    Role = Roles.Operator,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440011"),
                    Email = "cliente@prueba.com",
                    UserName = "cliente",
                    Name = "José Pérez",
                    PasswordHash = _passwordHasher.HashPassword("Candado6947!"),
                    Role = Roles.Client,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.Users.AddRangeAsync(users);
        }

        private async Task SeedDocumentsAsync()
        {
            if (await _context.Documents.AnyAsync())
            {
                return;
            }

            var operadorUserId = Guid.Parse("550e8400-e29b-41d4-a716-446655440010");
            var clienteUserId = Guid.Parse("550e8400-e29b-41d4-a716-446655440011");

            var documents = new[]
            {
                new Document
                {
                    Id = Guid.NewGuid(),
                    UserId = operadorUserId,
                    Filename = "kyc_document.pdf",
                    ContentType = "application/pdf",
                    DocumentType = DocumentType.KYC,
                    Channel = DocumentChannel.DIGITAL,
                    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440005"),
                    FileUrl = "https://storage.example.com/docs/kyc_001",
                    Size = 2048576,
                    Status = DocumentStatus.RECEIVED,
                    CorrelationId = "CORR-2026-001-KYC",
                    CreatedAt = DateTime.UtcNow
                },
                new Document
                {
                    Id = Guid.NewGuid(),
                    UserId = clienteUserId,
                    Filename = "contract_template.docx",
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    DocumentType = DocumentType.CONTRACT,
                    Channel = DocumentChannel.BACKOFFICE,
                    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440003"),
                    FileUrl = "https://storage.example.com/docs/contract_001",
                    Size = 1048576,
                    Status = DocumentStatus.SENT,
                    CorrelationId = "CORR-2026-002-CONTRACT",
                    CreatedAt = DateTime.UtcNow
                },
                new Document
                {
                    Id = Guid.NewGuid(),
                    UserId = operadorUserId,
                    Filename = "supporting_docs.zip",
                    ContentType = "application/zip",
                    DocumentType = DocumentType.SUPPORTING_DOCUMENT,
                    Channel = DocumentChannel.API_INTEGRATION,
                    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440004"),
                    FileUrl = "https://storage.example.com/docs/support_001",
                    Size = 5242880,
                    Status = DocumentStatus.FAILED,
                    CorrelationId = "CORR-2026-003-SUPPORT",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            await _context.Documents.AddRangeAsync(documents);
        }
    }
}
