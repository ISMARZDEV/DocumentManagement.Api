using Bhd.Domain.Constants;
using Bhd.Domain.Entities;

namespace Bhd.Domain.Interfaces;

/// <summary>
/// Interfaz para el repositorio de documentos
/// Define las operaciones de acceso a datos para la entidad Document
/// </summary>
public interface IDocumentRepository
{
    /// <summary>
    /// Obtiene una lista paginada de documentos aplicando filtros y ordenamiento
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda que incluyen filtros, ordenamiento y paginación</param>
    /// <returns>Una tupla con la colección de documentos y el total de registros que coinciden con los criterios</returns>
    /// <exception cref="ArgumentNullException">Se lanza si criteria es nulo</exception>
    /// <exception cref="ArgumentException">Se lanza si los parámetros de paginación son inválidos</exception>
    Task<(IEnumerable<Document> Items, int TotalCount)> GetDocumentsAsync(DocumentSearchCriteria criteria);
}
