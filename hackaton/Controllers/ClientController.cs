using hackaton.Models;
using hackaton.Models.Caches;
using hackaton.Models.Injectors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hackaton.Controllers
{
    public class ClientController : Controller
    {
        private readonly UserCacheService _userService;
      
        public ClientController(UserCacheService cache) { 
            _userService = cache;
        }
        // GET: ClientController
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public ActionResult Index()
        {
            string cpf = HttpContext.Session.GetString("CPF");
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            User user = _userService.GetUserByCPFAsync(cpf);
            Console.WriteLine(user.IsAdmin);
            if (user != null && user.Id == userId && user.IsAdmin) {
                return RedirectToAction("Index", "Users");
            }
            
            return View(user);
        }

        // GET: ClientController/Logout
        public ActionResult Logout()
        {
            //limpa a session do client
            HttpContext.Session.Clear();
            //Deleta o cookie que tem os dados de sessão
            Response.Cookies.Delete("MyAuthCookie");

            //Direciona o usuario a pagina /Home/Index
            return RedirectToAction("Index","Home");
        }

    }
}
