using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vyplňte prosím e-mail sloužící jako login.")]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$",
            ErrorMessage = "Neplatný formát e-mailu.")]
        [DisplayName("E-mail(login)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vyplňte prosím heslo.")]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [DisplayName("Heslo")]
        [RegularExpression("^[a-zA-Z0-9]{8,10}$", ErrorMessage = "Délka hesla musí být minimálně 8 znaků")]
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Vyplňte prosím potvrzovací heslo.")]
        [DataType(DataType.Password)]
        [CompareAttribute("Password", ErrorMessage = "Hesla se neshodují.")]
        [MinLength(8)]
        [DisplayName("Potvrzení hesla")]
        [RegularExpression("^[a-zA-Z0-9]{8,10}$", ErrorMessage = "Délka hesla musí být minimálně 8 znaků")]
        public string PasswordComparison { get; set; }

        [Required(ErrorMessage = "Vyplňte prosím křestní jméno.")]
        [DisplayName("Jméno")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Vyplňte prosím příjmení.")]
        [DisplayName("Příjmení")]
        public string Surname { get; set; }


        [RegularExpression("^[+]?[()/0-9. -]{9,}$", ErrorMessage = "Neplatné telefonní číslo.")]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Telefon")]
        public string Telephone { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Datum registrace")]
        public DateTime RegistrationTime { get; set; }
    }
}