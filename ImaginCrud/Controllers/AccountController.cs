using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ImaginCrud.Models;

namespace ImaginCrud.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Index

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    var isAuth = Request.IsAuthenticated;
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "El nombre o el password es incorrecto.";
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterModel());
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (model.Password.Equals(model.ConfirmPassword) == false)
                ViewBag.Error = "Confirmación de contraseña incorrecta.";
            // Attempt to register the user
            MembershipCreateStatus createStatus;
            Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                //FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                return RedirectToAction("Edit", "Users", new { id = model.UserName });
            }
            else
            {
                ViewBag.Error = ErrorCodeToString(createStatus);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El usuario ya existe.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "El email ya se encuentra registrado.";

                case MembershipCreateStatus.InvalidPassword:
                    return "La contraseña no es válida, debe tener al menos una letra mayúscula, una minúscula, un número y un caracter especial";

                case MembershipCreateStatus.InvalidEmail:
                    return "Correo electrónico no es válido.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Nombre de usuario incorrecto.";

                case MembershipCreateStatus.ProviderError:
                    return "Error de autenticación, contacte al administrador.";

                case MembershipCreateStatus.UserRejected:
                    return "La creación de usuario ha sido cancelada.";

                default:
                    return "Error desconocido.";
            }
        }
        #endregion
    }
}
