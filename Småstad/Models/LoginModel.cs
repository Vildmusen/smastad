using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Småstad.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Fyll i ditt användarnamn.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fyll i ditt lösenord.")]
        [UIHint("password")]
        public string Password { get; set; }

        public string ReturnUrl = "/";
    }
}
