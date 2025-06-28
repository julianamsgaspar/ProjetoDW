using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using PawBuddy.Models;

namespace PawBuddy.ViewModels;

public class AdminViewModel
{
    public IdentityUser IdentityUser { get; set; }
    public Utilizador Utilizador { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
