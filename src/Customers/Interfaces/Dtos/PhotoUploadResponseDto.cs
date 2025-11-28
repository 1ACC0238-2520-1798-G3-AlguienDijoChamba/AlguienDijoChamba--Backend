namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

/// <summary>
/// DTO utilizado para devolver la URL pública de la foto subida.
/// </summary>
public class PhotoUploadResponseDto
{
    public string PhotoUrl { get; set; }
    
    // Constructor (opcional, pero útil para crear el objeto fácilmente)
    public PhotoUploadResponseDto(string photoUrl)
    {
        PhotoUrl = photoUrl;
    }
}