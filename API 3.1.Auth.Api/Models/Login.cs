using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Auth.Api.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Введите ваш электронный адресс.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
