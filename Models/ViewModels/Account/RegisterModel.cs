using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchedulerApi.Models.ViewModels.Account;

public class RegisterModel
{
    [Required] public string? PhoneNumber { get; set; }
    [Required] public string? UserName { get; set; }
    [Required] public string? Id { get; set; }

    [Required]
    [PasswordPropertyText]
    public string? Password { get; set; }

    [Required] 
    [PasswordPropertyText] 
    [Compare("Password", ErrorMessage = "Passwords don't match.")] 
    public string? ConfirmPassword { get; set; }
}