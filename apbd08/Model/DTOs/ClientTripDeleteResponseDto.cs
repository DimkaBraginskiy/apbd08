namespace apbd08.Model.DTOs;

public class ClientTripDeleteResponseDto
{
    public string Message { get; set; }
    public int IdClient { get; set; }
    public int IdTrip { get; set; }
    public DateTime RemovedAt { get; set; }
}