using System.ComponentModel.DataAnnotations;
using KadreeBank.API.Models.Enums;

namespace KadreeBank.API.DTOs.Customers;

public sealed record CreateCustomerRequestDto
{
    [Required(ErrorMessage = "El número de documento es obligatorio.")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "El número de documento debe tener entre 5 y 20 caracteres.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "El número de documento solo puede contener números.")]
    public string DocumentNumber { get; init; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "El tipo de cliente es obligatorio.")]
    [EnumDataType(typeof(CustomerType), ErrorMessage = "El tipo de cliente no es válido.")]
    public CustomerType? Type { get; init; }

    [Required(ErrorMessage = "El PIN es obligatorio.")]
    [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "El PIN debe tener exactamente 4 dígitos.")]
    public string Pin { get; init; } = string.Empty;
}
