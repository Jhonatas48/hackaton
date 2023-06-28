using DevOne.Security.Cryptography.BCrypt;
using hackaton.Models;
using hackaton.Models.Caches;
using hackaton.Models.DAO;
using hackaton.Models.Injectors;
using hackaton.Models.Security;
using hackaton.Models.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Common;
using System.Diagnostics;

namespace hackaton.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private Context _context;
        private readonly IHubContext<RedirectClient> _redirectClient;

        private UserCacheService _userCacheService;
        public HomeController(ILogger<HomeController> logger,Context context,UserCacheService cache)
        {
            _logger = logger;
            _context = context;
            _userCacheService = cache;
           
        }
        public  bool validateLogin(User user) {
           
            if(user.ApiId==0) return false;

            var userRetrieve = _context.Users.FirstOrDefault(u => u.CPF == user.CPF);// _userCacheService.GetUserByCPFAsync(user.CPF);//_context.Users.FirstOrDefault(u => u.CPF.Equals(user.CPF));

            //Usuario não existe ou credenciais estão inválidas
            if (userRetrieve == null || !BCryptHelper.CheckPassword(user.Password, userRetrieve.Password))
            { 
                return false;
            }

           
            return true;
        }
        [HttpPost]
        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        // [ValidateAntiForgeryToken]
        public IActionResult Login([FromBody][Bind("Password,CPF")] User user)
        {
            string apiToken  = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Api api = _context.Apis.Where(a=> a.Token.Equals(apiToken)).FirstOrDefault();

            if (string.IsNullOrEmpty(apiToken))
            {
                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = "Api token invalido",

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }
            if (user == null) {
                return new BadRequestObjectResult(new { message = "User is required" }); ;
            }
         
            user.ApiId = api.ApiId;
            ModelState.Remove("Name");
            ModelState.Remove("Properties");
            ModelState.Remove("Api");

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Keys
                 .Where(key => ModelState[key].Errors.Any())
                 .ToDictionary(key => key, key => ModelState[key].Errors.Select(error => error.ErrorMessage).ToList());

                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = erros,

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }
            user.ApiId = api.ApiId;

            if (!validateLogin(user))
            {
                ModelState.AddModelError("CPF", "CPF ou Senha inválidos");
                ModelState.AddModelError("Password", "CPF ou Senha inválidos");
                return new UnauthorizedObjectResult(new { message = "Invalid Credentials" });
            }
          
          
            return Ok();
           
        }

        [HttpPost]
        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        // [ValidateAntiForgeryToken]
        public IActionResult Register( [FromBody]User user)
        {
            string apiToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Api api = _context.Apis.Where(a => a.Token.Equals(apiToken)).FirstOrDefault();

            if (string.IsNullOrEmpty(apiToken))
            {
                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = "Api token invalido",

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }

            var cpfExists =  _context.Users.Any(u => u.CPF == user.CPF);
                if (cpfExists)
                {
                    ModelState.AddModelError("CPF", "O CPF já está cadastrado.");
                   // return View(user);
                }
            user.ApiId = api.ApiId;
            ModelState.Remove("Api");
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Keys
             .Where(key => ModelState[key].Errors.Any())
             .ToDictionary(key => key, key => ModelState[key].Errors.Select(error => error.ErrorMessage).ToList());

                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = erros,

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }

            user.Password = BCryptHelper.HashPassword(user.Password,BCryptHelper.GenerateSalt());
            user.ApiId = api.ApiId;
            user.Api = api;
            _context.Users.Add(user);
            _context.SaveChanges();
          
            return Json(user);
        }
    }
}