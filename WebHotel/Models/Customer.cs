using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebHotel.Models
{
    public class Customer
    {
        [Key, Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Surname required")]
        [RegularExpression(@"^[a-zA-Z-']{2,20}$", ErrorMessage ="Name must be a min. 2 characters and max. 20 characters. Can only consist of English letters, hypen and apostrophe")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Given name required")]
        [RegularExpression(@"^[a-zA-Z-']{2,20}$", ErrorMessage = "Name must be a min. 2 characters and max. 20 characters. Can only consist of English letters, hypen and apostrophe")]
        public string GivenName { get; set; }

        [Required(ErrorMessage = "Postcode required")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Must be exactly 4 digits")]
        public string Postcode { get; set; }

        public ICollection<Booking> TheBookings { get; set; }
    }
}
