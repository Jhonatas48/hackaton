using Microsoft.AspNetCore.Mvc;
using hackaton.Models.Security;
using hackaton.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using hackaton.Models.DAO;
using hackaton.Models.WebSocket;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using hackaton.Models.Injectors;

namespace hackaton.Controllers
{
    public class ApiController : Controller
    {

        private readonly HomeController _homeController;
        private readonly Context _context;
        private readonly IHubContext<RedirectClient> _redirectClient;
        public ApiController(HomeController homeController,Context context, IHubContext<RedirectClient> redirectClient)
       {
         _homeController = homeController;
         _context = context;
         _redirectClient = redirectClient;

        }
        // GET: ApiController/validateqrcode
        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        public async Task<ActionResult> validateqrcode([FromBody] QrCode qrCode)
        {
            if (qrCode == null || string.IsNullOrEmpty(qrCode.Content)) {
                return new BadRequestObjectResult(new { message = "QrCode is required" });
            }

            QrCode qrCodeRetrieve = _context.QrCodes.Where(qr => qr.Content.Equals(qrCode.Content)).FirstOrDefault();
            
            if(qrCodeRetrieve == null)
            {
                return NotFound();
            }
            qrCode = new QrCode
            {
                     QRCodeId = qrCodeRetrieve.QRCodeId,
                    Content = qrCodeRetrieve.Content,
                    User = qrCodeRetrieve.User,
                    UserId = qrCodeRetrieve.UserId,
                    Expired = qrCodeRetrieve.Expired,
                    TimeExpiration = qrCodeRetrieve.TimeExpiration

             };

            qrCodeRetrieve.Expired = true;
            //_context.Update(qrCodeRetrieve);
            _context.QrCodes.Remove(qrCodeRetrieve);
            _context.SaveChanges();

            if (qrCodeRetrieve.TimeExpiration <= DateTime.Now || qrCode.Expired)
            {
                var message = new
                {
                    Message = "QrCode is expired"

                };
                return new UnauthorizedObjectResult(message);
            }
            //Direciona o usuario para o /Client/Index
            await _redirectClient.Clients.Group("pc_user"+qrCode.UserId).SendAsync("redirect","/Client/Index");
            return Json(qrCodeRetrieve);
        }


        // GET: ApiController/validateUser
        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        public ActionResult validateUser([FromBody] User user)
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
          
            if (user == null)
            {
                return new BadRequestObjectResult(new { message = "User is required" });
            }
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
                return BadRequest(response);
            }
            user.ApiId = api.ApiId;
            if (_homeController.validateLogin(user))
            {
                User userRetrieve = _context.Users.Where(u => u.CPF.Equals(user.CPF) && u.Active==true).FirstOrDefault();
                userRetrieve.Password = "";
                return Ok(userRetrieve);
            }

            return new UnauthorizedObjectResult(new { message = "Invalid Credentials" });
        }

    }
}
