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
        [Required(ErrorMessage = "This field is required.")]     
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        [CompareAttribute("Password", ErrorMessage = "Password don't match.")]
        public string PasswordComparison { get; set; }
        [Required(ErrorMessage = "This field is required.")] 
        public string FirstName { get; set; }
        [Required(ErrorMessage = "This field is required.")] 
        public string Surname { get; set; }
        
        // TODO : dopln si debile kontrolu spravnosti telefonu a emailu
        public string Telephone { get; set; }
        public DateTime RegistrationTime { get; set; }
    }
}