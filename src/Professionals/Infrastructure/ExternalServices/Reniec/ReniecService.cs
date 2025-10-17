using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AlguienDijoChamba.Api.Professionals.Domain;

namespace AlguienDijoChamba.Api.Professionals.Infrastructure.ExternalServices.Reniec;
public class ReniecService(IHttpClientFactory httpClientFactory) : IReniecService
{
    public async Task<ReniecInfo?> GetReniecInfoByDni(string dni, CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClient("ReniecApiClient");
        try
        {
            var response = await httpClient.GetFromJsonAsync<ReniecResponse>($"dni?numero={dni}", cancellationToken);
            if (response is null) return null;
            return new ReniecInfo
            {
                Nombres = response.FirstName,
                Apellidos = $"{response.FirstLastName} {response.SecondLastName}".Trim()
            };
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error al consultar la API de RENIEC: {e.Message}");
            return null;
        }
    }
}

internal class ReniecResponse
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("first_last_name")]
    public string FirstLastName { get; set; } = string.Empty;

    [JsonPropertyName("second_last_name")]
    public string SecondLastName { get; set; } = string.Empty;
}