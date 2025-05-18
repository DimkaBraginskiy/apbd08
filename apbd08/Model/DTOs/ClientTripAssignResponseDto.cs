namespace apbd08.Model.DTOs;

public class ClientTripAssignResponseDto
{
    public int IdClient { get; set; }
    public int IdTrip { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string Message { get; set; }
}