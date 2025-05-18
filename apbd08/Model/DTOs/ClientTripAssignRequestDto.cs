using System.ComponentModel.DataAnnotations;

namespace apbd08.Model.DTOs;

public class ClientTripAssignRequestDto
{
    [Required]
    public int ClientId { get; set; }
    [Required]
    public int TripId { get; set; }
}