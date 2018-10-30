using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHotel.Data;
using WebHotel.Models;

namespace WebHotel.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers/MyDetails
        public async Task<IActionResult> MyDetails()
        {
            string _email = User.FindFirst(ClaimTypes.Name).Value;
            var customers = await _context.Customer.FindAsync(_email);

            if (customers == null)
            {
                customers = new Customer { Email = _email };
                return View("~/Views/Customers/MyDetailsCreate.cshtml", customers);
            }
            else
            {
                return View("~/Views/Customers/MyDetailsUpdate.cshtml", customers);
            }
        }

        // POST: Customers/MyDetailsCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyDetailsCreate([Bind("Email,Surname,GivenName,Postcode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return View("~/Views/Customers/MyDetailsSuccess.cshtml", customer);
            }
            return View(customer);
        }

        // POST: Customers/MyDetailsUpdate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyDetailsUpdate([Bind("Email,Surname,GivenName,Postcode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return View("~/Views/Customers/MyDetailsSuccess.cshtml", customer);
            }
            return View(customer);
        }

        private bool CustomerExists(string id)
        {
            return _context.Customer.Any(e => e.Email == id);
        }
    }
}
