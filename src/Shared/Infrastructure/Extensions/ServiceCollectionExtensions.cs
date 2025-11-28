using AlguienDijoChamba.Api.Shared.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AlguienDijoChamba.Api.Shared.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // 🚀 REGISTRO DE TU SERVICIO DE ALMACENAMIENTO DE ARCHIVOS
        // IFileStorageService es la interfaz, FileStorageService es la implementación
        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}