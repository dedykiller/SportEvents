using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vyplňte prosím email sloužící jako login.")]     
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Vyplňte prosím heslo.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
       // TODO : doplnit validaci emailu
        [NotMapped]        
        [Required(ErrorMessage = "Vyplňte prosím potvrzovací heslo.")]
        [DataType(DataType.Password)]
        [CompareAttribute("Password", ErrorMessage = "Hesla se neshodují.")]
        public string PasswordComparison { get; set; }
        
        [Required(ErrorMessage = "Vyplňte prosím křestní jméno.")] 
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Vyplňte prosím příjmení.")] 
        public string Surname { get; set; }
        
        
        [RegularExpression ("^[+]?[()/0-9. -]{9,}$", ErrorMessage = "Neplatné telefonní číslo")] // TODO : upravit validaci telefonu
        public string Telephone { get; set; }
        
        public DateTime RegistrationTime { get; set; }
    }
}