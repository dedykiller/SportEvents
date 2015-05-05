using SportEvents.Controllers.Utility;
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

        //[DisplayName("E-mail(login)")]
        [Display(Name = "emailLog", ResourceType = typeof(SportEvents.Languages.Resources))] 
        //[Required(ErrorMessage = "Vyplňte prosím email sloužící jako login.")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "pleaseFillInEmail", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$",
            ErrorMessageResourceName = "invalidEmail", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        public string Email { get; set; }

        [Display(Name = "password", ResourceType = typeof(SportEvents.Languages.Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "fillPassword", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        [RegularExpression("^[a-zA-Z0-9]{8,}$", ErrorMessageResourceName = "allowedCharacters", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        [DataType(DataType.Password)]
        public string Password { get; set; }
            
        [NotMapped]
        [Display(Name = "passwordConfirm", ResourceType = typeof(SportEvents.Languages.Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "completePassword", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        [DataType(DataType.Password)]
        [CompareAttribute("Password", ErrorMessageResourceName = "badPassword", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        public string PasswordComparison { get; set; }

        [Display(Name = "name", ResourceType = typeof(SportEvents.Languages.Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "fillName", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        public string FirstName { get; set; }

        [Display(Name = "surname", ResourceType = typeof(SportEvents.Languages.Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "fillSurname", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))]
        public string Surname { get; set; }

        [Display(Name = "phone", ResourceType = typeof(SportEvents.Languages.Resources))]
        [RegularExpression("^[+]?[()/0-9. -]{9,}$", ErrorMessageResourceName = "invalidePhone", ErrorMessageResourceType = typeof(SportEvents.Languages.Resources))] 
        public string Telephone { get; set; }

        [DisplayName("Datum a čas registrace")]
        public DateTime RegistrationTime { get; set; }        
        [NotMapped]
        public List<Event> EventsParticipationYes { get; set; }
        [NotMapped]
        public decimal sum { get; set; }

        [DisplayName("Skupiny")]
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<UsersInEvent> UserInEvents { get; set; }
        public virtual ICollection<TypeOfPaymentForUserInPeriod> TypeOfPaymentsForUsersInPeriods { get; set; }

        public User()
        {
            Groups = new HashSet<Group>();

        }
    }
}