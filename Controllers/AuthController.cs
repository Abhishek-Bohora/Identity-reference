using AspNetCoreHero.ToastNotification.Abstractions;
using AuthMvc.Models;
using AuthMvc.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace AuthMvc.Controllers
{
    public class AuthController : Controller
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;
        public INotyfService _notifyService { get; }
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, INotyfService notifyService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notifyService = notifyService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
       public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var checkUsername = await _userManager.FindByNameAsync(vm.UserName);
            //if this checkUsername is not null than the username already exist cannot make user of same name twice
            if(checkUsername != null) { return View(vm); }
            var user = new ApplicationUser() {
                UserName = vm.UserName,
                Email = vm.Email,
                FirstName= vm.FirstName,
                LastName= vm.LastName,
            };
            var result = await _userManager.CreateAsync(user, vm.Password);
            if(!result.Succeeded) { _notifyService.Error("Registration Failed"); }
            if (result.Succeeded)
            {
                _notifyService.Success("Registration Successfull");
                return RedirectToAction("Login");
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm) {
            if(!ModelState.IsValid) { return View(vm); }
            var checkuser = await _userManager.FindByNameAsync(vm.Username);
            if(checkuser == null) { 
                _notifyService.Error("Username doesnot exists");
                return View(vm);
            }
            var verifyPassword = await _userManager.CheckPasswordAsync(checkuser, vm.Password);
            if(!verifyPassword) {
                _notifyService.Error("password doesnot match");
                return View(vm);
            }
            await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, false);
            _notifyService.Success("Login Successfull");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _notifyService.Success("Logged Out");
            return RedirectToAction("Login");
        }
    }
}