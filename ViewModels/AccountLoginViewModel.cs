using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Alura_AspNet_Identity.ViewModels
{
    public class AccountLoginViewModel
    {
        
        
        /*[Required]
        [EmailAddress]
        public string Email { get; set; }*/

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Continuar logado")]
        public bool KeepLogedIn { get; set; }
    }
}