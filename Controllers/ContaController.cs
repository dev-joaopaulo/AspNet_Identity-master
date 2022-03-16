using Alura_AspNet_Identity.Models;
using Alura_AspNet_Identity.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace Alura_AspNet_Identity.Controllers
{
    public class ContaController : Controller
    {

        private UserManager<UsuarioAplicacao> _userManager;
        public UserManager<UsuarioAplicacao> UserManager
        {
            get
            {
                if (_userManager == null)

                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _userManager = contextOwin.GetUserManager<UserManager<UsuarioAplicacao>>();
                }
                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }
        private SignInManager<UsuarioAplicacao, string> _signInManager;
        public SignInManager<UsuarioAplicacao, string> SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _signInManager = contextOwin.GetUserManager<SignInManager<UsuarioAplicacao, string>>();
                }
                return _signInManager;
            }
            set
            {
                _signInManager = value;
            }
        }
        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                var contextOwin = Request.GetOwinContext();
                return contextOwin.Authentication;
            }
        }

        // GET: Conta
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registrar(ContaRegistrarViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var novoUsuario = new UsuarioAplicacao();

                novoUsuario.Email = modelo.Email;
                novoUsuario.UserName = modelo.UserName;
                novoUsuario.NomeCompleto = modelo.NomeCompleto;

                var checkUser = await UserManager.FindByEmailAsync(modelo.Email);
                var userExist = checkUser != null;

                var resultado = await UserManager.CreateAsync(novoUsuario, modelo.Senha);

                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AdicionaErros(resultado);
                }

            }

            // Alguma coisa deu errada
            return View(modelo);
        }

        public async Task<ActionResult> Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Login(AccountLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Realizar Login pelo Identity

                var user = await UserManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    var signInResult = await SignInManager.PasswordSignInAsync(
                        user.UserName,
                        model.Password,
                        isPersistent: model.KeepLogedIn,
                        shouldLockout: true);

                    switch (signInResult)
                    {
                        case SignInStatus.Success:
                            return RedirectToAction("Index", "Home");
                        case SignInStatus.LockedOut:
                            var correctPassword = await UserManager.CheckPasswordAsync(user, model.Password);
                            if (correctPassword)
                            {
                                ModelState.AddModelError("", "Login attempts exceeded, try again later.");
                                break;
                            }
                            else
                                return InvalidPasswordOrUser();
                        default:
                            return InvalidPasswordOrUser();
                    }
                }
                else if (user == null)
                    return InvalidPasswordOrUser();
            }

            // Algo de errado aconteceu
            return View(model);
        }


        [HttpPost]
        public ActionResult Logoff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private ActionResult InvalidPasswordOrUser()
        {
            ModelState.AddModelError("", "Invalid user or password");
            return View("Login");
        }

        private void AdicionaErros(IdentityResult resultado)
        {
            foreach (var erro in resultado.Errors)
                ModelState.AddModelError("", erro);
        }


    }
}