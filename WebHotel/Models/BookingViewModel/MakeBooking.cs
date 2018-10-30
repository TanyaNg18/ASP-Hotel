using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models.BookingViewModel
{
    public class MakeBooking
    {
        [Range(1,16)]
        public int RoomID { get; set; }

        public string CustomerEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }


    }
}
