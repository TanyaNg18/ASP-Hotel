﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebHotel.Models;

namespace GourmetPizzaPrac3.Data
{
    public class SeedRoles
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration Configuration)
        {
            // Get the RoleManager and the UserManager objects
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            // Include your role names here
            string[] roleNames = { "Admin", "Customer" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                // check whether the role already exists
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // creating the roles
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Creating an admin user who will maintain the web app
            // His/her username are read from the configuration file: appsettings.json
            var poweruser = new ApplicationUser
            {
                UserName = Configuration.GetSection("UserSettings")["UserEmail"],
                Email = Configuration.GetSection("UserSettings")["UserEmail"]
            };

            string userPassword = Configuration.GetSection("UserSettings")["UserPassword"];
            var user = await UserManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["UserEmail"]);
            // if this admin user doesn't exist in the database, ​create it in the database;
            // otherwise, do nothing.
            if (user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPassword);
                if (createPowerUser.Succeeded)
                {
                    // here we assign the new user the "Admin" role 
                    await UserManager.AddToRoleAsync(poweruser, "Admin");
                }
            }
        }
    }
}