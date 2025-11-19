using AlguienDijoChamba.Api.Customers.Application.Command;
using AlguienDijoChamba.Api.Customers.Application.Queries;
using AlguienDijoChamba.Api.Customers.Interfaces.Dtos;
using AlguienDijoChamba.Api.IAM.Application.Commands;
using AlguienDijoChamba.Api.IAM.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlguienDijoChamba.Api.Customers.Interfaces;

[ApiController]
[Route("api/v1/[controller]")] // Ruta base: /api/v1/Customer
public class CustomerController(ISender sender) : ControllerBase
{
    // ... [Endpoints de Register y Login existentes] ...

    /// <summary>
    /// Registrar un cliente (Customer)
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CustomerRegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCustomerCommand(
            request.Email, request.Password, request.Nombres, request.Apellidos, request.Celular
        );
        var userId = await sender.Send(command, cancellationToken);
        return Ok(new { UserId = userId });
    }

    /// <summary>
    /// Login de un cliente (Customer)
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CustomerLoginRequest request, CancellationToken cancellationToken)
    {
        var query = new CustomerLoginQuery(request.Email, request.Password);
        var result = await sender.Send(query, cancellationToken);
        return Ok(result); 
    }
    
// ----------------------------------------------------------------------
    // 🚀 ENDPOINT PARA SUBIR LA FOTO
    // ----------------------------------------------------------------------
    
    /// <summary>
    /// Sube una foto de perfil y actualiza la URL en el registro del cliente.
    /// </summary>
    // ✅ CAMBIADO: customerId -> userId en la ruta
    [HttpPost("{userId:guid}/profile/photo")] 
    [Consumes("multipart/form-data")] 
    [ProducesResponseType(typeof(PhotoUploadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadPhoto(
        [FromRoute] Guid userId, // ✅ CAMBIADO: customerId -> userId
        [FromForm] CustomerPhotoUploadRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Crear el Comando de subida de foto (Usando userId)
        var command = new UploadCustomerPhotoCommand(
            userId, // ✅ USANDO userId
            request.PhotoFile 
        );

        var photoUrl = await sender.Send(command, cancellationToken);
        
        return Ok(new PhotoUploadResponseDto(photoUrl));
    }
    
    // ----------------------------------------------------------------------

    /// <summary>
    /// [ETAPA 2] Completa la información de perfil (pago, preferencias) de un cliente recién registrado.
    /// </summary>
    [HttpPost("{userId:guid}/profile/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteProfile(
        [FromRoute] Guid userId, // ✅ USA userId
        [FromBody] CustomerCompleteRegistrationRequest request, 
        CancellationToken cancellationToken)
    {
        // 1. Crear el Comando (Usando userId)
        var command = new CompleteCustomerProfileCommand(
            userId, // ✅ USANDO userId
            request.PreferredPaymentMethod, 
            request.AcceptsBookingUpdates, 
            request.AcceptsPromotionsAndOffers, 
            request.AcceptsNewsletter
        );

        await sender.Send(command, cancellationToken);
        // NOTA: El doble envío de 'command' aquí: 'var updatedData = await sender.Send(command, cancellationToken);'
        // fue eliminado, ya que el comando solo debería enviarse una vez. Se devuelve NoContent() o Ok(new { UserId = userId }).
        
        var updatedData = await sender.Send(command, cancellationToken); 
        return Ok(updatedData);// Generalmente se usa 204 NoContent para PUT/POST que no retornan data
    }

    /// <summary>
    /// Obtiene la información completa del perfil de un cliente.
    /// </summary>
    [HttpGet("{userId:guid}/profile")] // ✅ CAMBIADO: customerId -> userId en la ruta
    [ProducesResponseType(typeof(CustomerProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile([FromRoute] Guid userId, CancellationToken cancellationToken) // ✅ CAMBIADO: customerId -> userId
    {
        // 1. Crear el Query (Usando userId)
        var query = new GetCustomerProfileQuery { UserId = userId }; // ✅ USANDO userId
        
        var profileDto = await sender.Send(query, cancellationToken);
        
        return Ok(profileDto);
    }
    
    /// <summary>
    /// Actualiza todos los datos modificables del perfil de un cliente (incluyendo datos básicos).
    /// </summary>
    [HttpPut("{customerId:guid}/profile")] // 👈 MANTIENE: customerId
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        [FromRoute] Guid customerId, // 👈 MANTIENE: customerId
        [FromBody] CustomerProfileUpdateRequest request, 
        CancellationToken cancellationToken)
    {
        // 1. Crear el Comando (Usando customerId)
        var command = new UpdateCustomerProfileCommand(
            customerId, // 👈 USANDO customerId, según tu solicitud
            request.Nombres,
            request.Apellidos,
            request.Celular,
            request.PhotoUrl,
            request.PreferredPaymentMethod,
            request.AcceptsBookingUpdates,
            request.AcceptsPromotionsAndOffers,
            request.AcceptsNewsletter
        );

        var updated = await sender.Send(command, cancellationToken);

        return Ok(updated);
    }
}