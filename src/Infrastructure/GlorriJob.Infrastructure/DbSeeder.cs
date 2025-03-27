using GlorriJob.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Infrastructure
{
	public static class DbSeeder
	{
		public static async Task SeedRolesAndUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
		{

			string adminEmail = "admin@gmail.com";
			string adminPassword = "Admin123";

			if (!await roleManager.RoleExistsAsync("admin"))
			{
				var role = new Role()
				{
					Name = "admin",
					NormalizedName = "ADMIN"
				};
				await roleManager.CreateAsync(role);
			}
			if (!await roleManager.RoleExistsAsync("user"))
			{
				var role = new Role()
				{
					Name = "user",
					NormalizedName = "USER"
				};
				await roleManager.CreateAsync(role);
			}
			var adminUser = await userManager.FindByEmailAsync(adminEmail);
			if (adminUser is null)
			{
				adminUser = new User
				{
					UserName = adminEmail,
					Name = adminEmail,
					Email = adminEmail,
					EmailConfirmed = true
				};

				var result = await userManager.CreateAsync(adminUser, adminPassword);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "user");
					await userManager.AddToRoleAsync(adminUser, "admin");
					Console.WriteLine("Admin user created successfully!");
				}
				else
				{
					Console.WriteLine("Admin user creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
		}
	}
}
