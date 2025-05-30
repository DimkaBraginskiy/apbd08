﻿namespace apbd08.Model.DTOs;

public class ClientTripDto
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }

    public List<CountryResponseDto> Countries { get; set; } = new List<CountryResponseDto>();
}