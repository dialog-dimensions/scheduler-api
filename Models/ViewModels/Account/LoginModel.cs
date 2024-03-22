using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchedulerApi.Models.ViewModels.Account;

public class LoginModel
{
    [Required] public string? Id { get; set; }
    
    [Required] 
    [PasswordPropertyText] 
    public string? Password { get; set; }
}
