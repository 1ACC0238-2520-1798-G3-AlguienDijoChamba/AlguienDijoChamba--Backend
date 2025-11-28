using Microsoft.AspNetCore.Http; 

namespace AlguienDijoChamba.Api.Customers.Interfaces.Dtos;

// DTO para recibir solo la foto (multipart/form-data)
public class CustomerPhotoUploadRequest
{
    public IFormFile PhotoFile { get; set; } 
}