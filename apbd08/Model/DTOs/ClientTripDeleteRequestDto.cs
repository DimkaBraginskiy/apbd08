using System.ComponentModel.DataAnnotations;

namespace apbd08.Model.DTOs;

public class ClientTripDeleteRequestDto
{
    [Required]
    public int ClientId { get; set; }
    [Required]
    public int TripId { get; set; }
}