using DevOne.Security.Cryptography.BCrypt;
using hackaton.Models;
using hackaton.Models.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace hackaton.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Context _context;

        private readonly UserCacheService _userCacheService;
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

        [HttpPost]
        public IActionResult Login(User user)
        {


            var userRetrieve = _userCacheService.GetUserByCPFAsync(user.CPF);//_context.Users.FirstOrDefault(u => u.CPF.Equals(user.CPF));

            //Usuario não existe ou credenciais estão inválidas
            if (userRetrieve == null || !BCryptHelper.CheckPassword(user.Password, userRetrieve.Password))
            {
                ModelState.AddModelError("CPF", "CPF ou Senha inválidos");
                ModelState.AddModelError("Password", "CPF ou Senha inválidos");
                return View();
            }
            Console.WriteLine("TESTE");
            return View("Privacy");
           
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            return View();
        }

      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}