using System.ComponentModel.DataAnnotations;

namespace ASPNETMVCPHONEBOOK.Models.Domain
{
    public class Contact
    {
        [Required]  // required means it cant be null 
        public Guid Id { get; set; }
        [Required, MaxLength(20)]
        public string FirstName { get; set; }
        [Required, MaxLength(20)]
        public string Surname { get; set; }
        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; }


    }
}
