using System.ComponentModel.DataAnnotations;

namespace KadreeBank.API.DTOs.Auth;

// Mismas reglas de formato que CreateCustomerRequestDto.
public sealed record LoginRequestDto
{
    [Required(ErrorMessage = "El número de documento es obligatorio.")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "El número de documento debe tener entre 5 y 20 caracteres.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "El número de documento solo puede contener números.")]
    public string DocumentNumber { get; init; } = string.Empty;

    [Required(ErrorMessage = "El PIN es obligatorio.")]
    [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "El PIN debe tener exactamente 4 dígitos.")]
    public string Pin { get; init; } = string.Empty;
}
