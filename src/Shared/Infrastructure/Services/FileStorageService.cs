using Microsoft.AspNetCore.Hosting; // Usaremos IWebHostEnvironment para mayor compatibilidad
using Microsoft.AspNetCore.Http; 
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// Asumiendo que IFileStorageService está definido en el mismo namespace o importado.
// Usamos IWebHostEnvironment ya que es más común en ASP.NET Core API que IHostEnvironment para rutas web.
namespace AlguienDijoChamba.Api.Shared.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    // Inyectamos IWebHostEnvironment para obtener la ruta física de la aplicación
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env) 
    {
        _env = env;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string userId, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("El archivo es nulo o está vacío.");
        }
        
        // --- 1. MODIFICACIÓN: Definir la RUTA FÍSICA con la subcarpeta 'profile-photos' ---
        var relativePath = Path.Combine("uploads", "profile-photos");
        var uploadsPath = Path.Combine(_env.ContentRootPath, relativePath);

        // 2. Crear la carpeta si no existe (creará la ruta completa: uploads/profile-photos)
        Directory.CreateDirectory(uploadsPath); 
        
        // 3. Generar nombre único y la ruta completa del archivo
        var fileExtension = Path.GetExtension(file.FileName);
        // Aseguramos que el userId sea usado y que el nombre sea único con Guid
        var fileName = $"{userId.ToLower()}-{Guid.NewGuid()}{fileExtension}"; 
        var filePath = Path.Combine(uploadsPath, fileName);

        // 4. Guardar el archivo físicamente en el disco
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken); 
        }

        // 5. Devolver la URL de acceso público que coincide con la configuración de Program.cs
        // NOTA: La URL pública *solo* usa /uploads/ porque así se configuró el RequestPath en Program.cs
        var localAccessUrl = $"http://0.0.0.0:5000/uploads/{fileName}"; 

        return localAccessUrl;
    }
}