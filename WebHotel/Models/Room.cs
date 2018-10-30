using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebHotel.Models
{
    public class Room
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Room level required")]
        [RegularExpression(@"^[G123]$", ErrorMessage = "Exactly one character allowed of either 'G' '1' '2' or '3'.")]
        public string Level { get; set; }
        
        [Range(1,3)]
        public int BedCount { get; set; }

        [Range(50.00, 300.00)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public ICollection<Booking> TheBookings { get; set; }
    }
}
