using System;
using Microsoft.AspNetCore.Identity;
using TokenService.Models;

namespace TokenService.Data
{
    public class IdentityInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_context.Database.EnsureCreated())
            {
                if (!_roleManager.RoleExistsAsync(Roles.ROLE_API_EXAMPLE).Result)
                {
                    var resultado = _roleManager.CreateAsync(
                            new IdentityRole(Roles.ROLE_API_EXAMPLE)).Result;
                    if (!resultado.Succeeded)
                    {
                        throw new Exception(
                            $"Error while creating role {Roles.ROLE_API_EXAMPLE}.");
                    }
                }

                CreateUser(
                    new ApplicationUser()
                    {
                        UserName = "admin_apiexample",
                        Email = "admin-apiexample@test.com",
                        EmailConfirmed = true
                    }, "AdminAPIExample01!", Roles.ROLE_API_EXAMPLE);

                CreateUser(
                    new ApplicationUser()
                    {
                        UserName = "invalidusr_apiexample",
                        Email = "invalidusr-apiexample@test.com",
                        EmailConfirmed = true
                    }, "UsrInvAPIExample01!");
            }
        }

        private void CreateUser(
            ApplicationUser user,
            string password,
            string initialRole = null)
        {
            if (_userManager.FindByNameAsync(user.UserName).Result == null)
            {
                var result = _userManager
                    .CreateAsync(user, password).Result;

                if (result.Succeeded &&
                    !String.IsNullOrWhiteSpace(initialRole))
                {
                    _userManager.AddToRoleAsync(user, initialRole).Wait();
                }
            }
        }
    }

}
