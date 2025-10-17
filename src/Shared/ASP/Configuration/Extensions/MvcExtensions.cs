namespace AlguienDijoChamba.Api.Shared.ASP.Configuration.Extensions;

public static class MvcExtensions
{
    public static IServiceCollection AddKebabCaseRouting(this IServiceCollection services)
    {
        services.AddControllers(options => { options.Conventions.Add(new KebabCaseRouteNamingConvention()); });
        return services;
    }
}