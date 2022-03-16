using Alura_AspNet_Identity.Identity;
using Alura_AspNet_Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;


[assembly: OwinStartup(typeof(Alura_AspNet_Identity.Startup))]


namespace Alura_AspNet_Identity
{


    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.CreatePerOwinContext<DbContext>(() =>
            new IdentityDbContext<UsuarioAplicacao>("DefaultConnection"));

            builder.CreatePerOwinContext<IUserStore<UsuarioAplicacao>>(
            (opcoes, contextoOwin) =>
{
                var dbContext = contextoOwin.Get<DbContext>();
                return new UserStore<UsuarioAplicacao>(dbContext);
            });

            builder.CreatePerOwinContext<UserManager<UsuarioAplicacao>>(
            (opcoes, contextoOwin) =>
            {
                var userStore = contextoOwin.Get<IUserStore<UsuarioAplicacao>>();
                var userManager = new UserManager<UsuarioAplicacao>(userStore);

                var userValidator = new UserValidator<UsuarioAplicacao>(userManager);
                userValidator.RequireUniqueEmail = true;

                userManager.UserValidator = userValidator;
                userManager.PasswordValidator = new UserPasswordValidator()
                {
                    SizeRequired = 6,
                    SpecialCharRequired = false,
                    DigitsRequired = true,
                };

                // Number of signin attempts before the user is locked out
                userManager.MaxFailedAccessAttemptsBeforeLockout = 5;
                // How long the user is locked out before he can try to sign in again
                userManager.DefaultAccountLockoutTimeSpan= TimeSpan.FromMinutes(5);
                // All users should be locked out by default in case of wrong attempts
                userManager.UserLockoutEnabledByDefault = true;

                return userManager;  
                });

            builder.CreatePerOwinContext<SignInManager<UsuarioAplicacao, string>>(
            (opcoes, contextoOwin) =>
            {
                var userManager = contextoOwin.Get<UserManager<UsuarioAplicacao>>();
                var signInManager =
                    new SignInManager<UsuarioAplicacao, string>(userManager,
                                                                contextoOwin.Authentication);

                return signInManager;
            });

            builder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            }) ;

            using (var dbContext = new IdentityDbContext<UsuarioAplicacao>("DefaultConnection"))
            {
                CreateIdentityRoles(dbContext);
                CreateSuperUser(dbContext);
            }
                
            


        }


        // This function is responsible for creating the Identity roles:

        private void CreateIdentityRoles(IdentityDbContext<UsuarioAplicacao> dbContext)
        {
            using (var roleStore = new RoleStore<IdentityRole>(dbContext))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {
                if (!roleManager.RoleExists(RolesNames.SUPERUSER))
                    roleManager.Create(new IdentityRole(RolesNames.SUPERUSER));
                if (!roleManager.RoleExists(RolesNames.MANAGER))
                    roleManager.Create(new IdentityRole(RolesNames.MANAGER));
                if (!roleManager.RoleExists(RolesNames.USER))
                    roleManager.Create(new IdentityRole(RolesNames.USER));
            }
        }

        // This method creates the superUser the first time the application runs:
        // The key is stored in the web.config

        private void CreateSuperUser(IdentityDbContext<UsuarioAplicacao> dbContext)
        {
            using (var userStore = new UserStore<UsuarioAplicacao>(dbContext))
            using (var userManager = new UserManager<UsuarioAplicacao>(userStore))
            {
                var superUserEmail = ConfigurationManager.AppSettings["admin:email"];

                var superUser = userManager.FindByEmail(superUserEmail);
                if (superUser != null)
                    return;

                superUser = new UsuarioAplicacao();
                superUser.Email = superUserEmail;
                superUser.UserName = ConfigurationManager.AppSettings["admin:user_name"];
                userManager.Create(
                    superUser,
                    ConfigurationManager.AppSettings["admin:password"]);

                userManager.AddToRole(superUser.Id, RolesNames.SUPERUSER);
            }
                
        }
        
    }
}