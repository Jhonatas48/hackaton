using DevOne.Security.Cryptography.BCrypt;
using hackaton.Models;
using hackaton.Models.Caches;
using hackaton.Models.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace hackaton.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private Context _context;

        private UserCacheService _userCacheService;
        public HomeController(ILogger<HomeController> logger,Context context,UserCacheService cache)
        {
            _logger = logger;
            _context = context;
            _userCacheService = cache;
        }

        public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult Login() {

            return View();
        }
        public  bool validateLogin(User user) {
            var userRetrieve = _context.Users.FirstOrDefault(u => u.CPF == user.CPF);// _userCacheService.GetUserByCPFAsync(user.CPF);//_context.Users.FirstOrDefault(u => u.CPF.Equals(user.CPF));

            //Usuario não existe ou credenciais estão inválidas
            if (userRetrieve == null || !BCryptHelper.CheckPassword(user.Password, userRetrieve.Password))
            { 
                return false;
            }

                return true;
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (user == null) {
                return new BadRequestObjectResult(new { message = "User is required" }); ;
            }

            if (!validateLogin(user))
            {
                ModelState.AddModelError("CPF", "CPF ou Senha inválidos");
                ModelState.AddModelError("Password", "CPF ou Senha inválidos");
                return View();
            }
           
            return View("Privacy");
           
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            return View("SucessRegister");
        }

        public IActionResult SucessRegister() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}