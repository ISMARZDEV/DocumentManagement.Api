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
                    Id = Guid.Parse("cc2fef10-2bab-42a6-b665-79248d985265"),
                    UserId = clienteUserId,
                    Filename = "fail.pdf",
                    ContentType = "application/pdf",
                    DocumentType = DocumentType.CONTRACT,
                    Channel = DocumentChannel.DIGITAL,
                    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440011"),
                    FileUrl = null,
                    Size = 89446,
                    Status = DocumentStatus.FAILED,
                    CorrelationId = "80020",
                    CreatedAt = DateTime.Parse("2026-01-15T05:41:20.8335273")
                },
                new Document
                {
                    Id = Guid.Parse("1b036a5c-82d7-46e1-aaf3-bd9b23d792f3"),
                    UserId = clienteUserId,
                    Filename = "test document.docx",
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    DocumentType = DocumentType.SUPPORTING_DOCUMENT,
                    Channel = DocumentChannel.BACKOFFICE,
                    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440011"),
                    FileUrl = "/app/DocumentStorage/2026/01/1b036a5c-82d7-46e1-aaf3-bd9b23d792f3_test document.docx",
                    Size = 2285500,
                    Status = DocumentStatus.SENT,
                    CorrelationId = "80019",
                    CreatedAt = DateTime.Parse("2026-01-15T05:36:48.9631863")
                },
                new Document
                {
                    Id = Guid.Parse("da2154e8-8a0b-478c-bd8d-91cb25f5d9a5"),
                    UserId = clienteUserId,
                    Filename = "test document.xlsx",
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    DocumentType = DocumentType.FORM,
                    Channel = DocumentChannel.DIGITAL,
                    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440011"),
                    FileUrl = "/app/DocumentStorage/2026/01/da2154e8-8a0b-478c-bd8d-91cb25f5d9a5_test document.xlsx",
                    Size = 9479,
                    Status = DocumentStatus.SENT,
                    CorrelationId = "80018",
                    CreatedAt = DateTime.Parse("2026-01-15T05:36:33.0352655")
                },
                new Document
                {
                    Id = Guid.Parse("ebc74bc8-a771-4cbf-b2b6-c821c6a964b3"),
                    UserId = clienteUserId,
                    Filename = "test_document.pdf",
                    ContentType = "application/pdf",
                    DocumentType = DocumentType.OTHER,
                    Channel = DocumentChannel.API_INTEGRATION,
                    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440011"),
                    FileUrl = "/app/DocumentStorage/2026/01/ebc74bc8-a771-4cbf-b2b6-c821c6a964b3_test_document.pdf",
                    Size = 89446,
                    Status = DocumentStatus.SENT,
                    CorrelationId = "80015",
                    CreatedAt = DateTime.Parse("2026-01-15T05:19:40.6844382")
                },
                new Document
                {
                    Id = Guid.Parse("35973e79-8ba6-492c-bb1e-e9ab2fd4d684"),
                    UserId = operadorUserId,
                    Filename = "test_image.jpg",
                    ContentType = "image/jpeg",
                    DocumentType = DocumentType.KYC,
                    Channel = DocumentChannel.BACKOFFICE,
                    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440011"),
                    FileUrl = "/app/DocumentStorage/2026/01/35973e79-8ba6-492c-bb1e-e9ab2fd4d684_test_image.jpg",
                    Size = 180110,
                    Status = DocumentStatus.SENT,
                    CorrelationId = "80016",
                    CreatedAt = DateTime.Parse("2026-01-15T05:32:47.8042926")
                },
                new Document
                {
                    Id = Guid.Parse("d1de9bc0-04ee-4dcc-93f7-6317c59d74bc"),
                    UserId = clienteUserId,
                    Filename = "test_image.png",
                    ContentType = "image/png",
                    DocumentType = DocumentType.CONTRACT,
                    Channel = DocumentChannel.DIGITAL,
                    CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440011"),
                    FileUrl = "/app/DocumentStorage/2026/01/d1de9bc0-04ee-4dcc-93f7-6317c59d74bc_test_image.png",
                    Size = 1255233,
                    Status = DocumentStatus.RECEIVED,
                    CorrelationId = "80017",
                    CreatedAt = DateTime.Parse("2026-01-15T05:33:48.4928443")
                }
            };

            await _context.Documents.AddRangeAsync(documents);
        }
    }
}
