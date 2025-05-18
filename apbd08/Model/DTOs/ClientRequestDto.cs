using System.ComponentModel.DataAnnotations;

namespace apbd08.Model.DTOs;

public class ClientRequestDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [Phone]
    public string Telephone { get; set; }
    [Required]
    public string Pesel { get; set; }
}