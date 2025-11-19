
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AlguienDijoChamba.Api.Shared.Infrastructure.Services;

/// <summary>
/// Define el contrato para los servicios de almacenamiento de archivos.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Sube el archivo proporcionado a la ubicación de almacenamiento.
    /// </summary>
    /// <param name="file">El archivo (IFormFile) a subir.</param>
    /// <param name="userId">Un identificador para asociar el archivo (usado para el nombre o la ruta).</param>
    /// <param name="cancellationToken">Token para monitorear la cancelación de la operación.</param>
    /// <returns>La URL pública del archivo subido.</returns>
    Task<string> UploadFileAsync(IFormFile file, string userId, CancellationToken cancellationToken);
}