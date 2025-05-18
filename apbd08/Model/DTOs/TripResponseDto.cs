namespace apbd08.Model.DTOs;

public class TripResponseDto
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    
    public ICollection<CountryResponseDto> Countries { get; set; }
}