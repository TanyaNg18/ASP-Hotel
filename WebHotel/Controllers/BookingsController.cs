using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using WebHotel.Models;
using WebHotel.Models.BookingViewModel;

namespace WebHotel.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles ="Customer, Admin")]
        // GET: Bookings
        public async Task<IActionResult> Index(string sortOrder)
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Booking.ToListAsync());
            }
            else
            {
                if (String.IsNullOrEmpty(sortOrder))
                {
                    sortOrder = "checkIn_asc";
                }

                var _bookings = (IQueryable<Booking>)_context.Booking.Include(b => b.TheCustomer).Include(b => b.TheRoom).Where(e => e.CustomerEmail == User.Identity.Name);

                switch (sortOrder)
                {
                    case "checkIn_asc":
                        _bookings = _bookings.OrderBy(b => b.CheckIn);
                        break;
                    case "checkIn_desc":
                        _bookings = _bookings.OrderByDescending(b => b.CheckIn);
                        break;
                    case "cost_asc":
                        _bookings = _bookings.OrderBy(b => b.Cost);
                        break;
                    case "cost_desc":
                        _bookings = _bookings.OrderByDescending(b => b.Cost);
                        break;
                }

                ViewData["NextCheckInBooking"] = sortOrder != "checkIn_asc" ? "checkIn_asc" : "checkIn_desc";
                ViewData["NextCostBooking"] = sortOrder != "cost_asc" ? "cost_asc" : "cost_desc";

                return View(await _bookings.AsNoTracking().ToListAsync());
            }
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [Authorize(Roles = "Customer, Admin")]
        // GET: Bookings/Create
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "ID");
                ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email");
                return View("~/Views/Bookings/AdminCreate.cshtml");
            }
            else
            {
                ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "ID");
                string _email = User.FindFirst(ClaimTypes.Name).Value;
                return View();
            }
        }

        [Authorize(Roles = "Customer, Admin")]
        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MakeBooking basicBooking)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    var _adminBooking = new Booking
                    {
                        RoomID = basicBooking.RoomID,
                        CustomerEmail = basicBooking.CustomerEmail,
                        CheckIn = basicBooking.CheckIn,
                        CheckOut = basicBooking.CheckOut

                    };

                    TimeSpan timeDiffAdmin = _adminBooking.CheckOut - _adminBooking.CheckIn;

                    decimal nightsAdmin = Convert.ToDecimal(timeDiffAdmin.TotalDays);

                    var theRoomAdmin = await _context.Room.FindAsync(basicBooking.RoomID);

                    _adminBooking.Cost = theRoomAdmin.Price * nightsAdmin;

                    _context.Add(_adminBooking);
                    await _context.SaveChangesAsync();
                    return View("~/Views/Bookings/Confirmation.cshtml", _adminBooking);
                }
                else
                {
                    var _booking = new Booking
                    {
                        RoomID = basicBooking.RoomID,
                        CustomerEmail = User.Identity.Name,
                        CheckIn = basicBooking.CheckIn,
                        CheckOut = basicBooking.CheckOut

                    };

                    TimeSpan timeDiff = _booking.CheckOut - _booking.CheckIn;

                    decimal nights = Convert.ToDecimal(timeDiff.TotalDays);

                    var theRoom = await _context.Room.FindAsync(basicBooking.RoomID);

                    _booking.Cost = theRoom.Price * nights;

                    _context.Add(_booking);
                    await _context.SaveChangesAsync();
                    return View("~/Views/Bookings/Confirmation.cshtml", _booking);
                }
            }
            
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "ID", basicBooking.RoomID);
            return View(basicBooking);
        }

        [Authorize(Roles = "Admin")]
        // GET: Bookings/Create
        public IActionResult AdminCreate()
        {
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email");
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "ID");
            string _email = User.FindFirst(ClaimTypes.Name).Value;
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCreate(MakeBooking basicBooking)
        {
            if (ModelState.IsValid)
            {
                var _booking = new Booking
                {
                    RoomID = basicBooking.RoomID,
                    CustomerEmail = basicBooking.CustomerEmail,
                    CheckIn = basicBooking.CheckIn,
                    CheckOut = basicBooking.CheckOut

                };

                TimeSpan timeDiff = _booking.CheckOut - _booking.CheckIn;

                decimal nights = Convert.ToDecimal(timeDiff.TotalDays);

                var theRoom = await _context.Room.FindAsync(basicBooking.RoomID);

                _booking.Cost = theRoom.Price * nights;

                _context.Add(_booking);
                await _context.SaveChangesAsync();
                return View("~/Views/Bookings/Confirmation.cshtml", _booking);
            }

            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", basicBooking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "ID", basicBooking.RoomID);
            return View(basicBooking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "Level", booking.RoomID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (id != booking.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Set<Customer>(), "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Set<Room>(), "ID", "Level", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.ID == id);
        }
    }
}
