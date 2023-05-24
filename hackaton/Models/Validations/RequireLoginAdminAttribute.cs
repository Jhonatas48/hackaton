using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using hackaton.Models.DAO;

namespace hackaton.Models.Validations
{
    public class RequireLoginAdminAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly Context _context;
        public RequireLoginAdminAttribute(Context context)
        {
            _context = context;

        }

        //Chamado sempre que possuir o cabeçalho Authorization
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {

           //Obtem o cpf do usuario
           string cpf =  context.HttpContext.Session.GetString("CPF");
            //Obtem o id do usuario
           int? userId = context.HttpContext.Session.GetInt32("UserId");
           int? sessionId = context.HttpContext.Session.GetInt32("SessionId");

            if (string.IsNullOrEmpty(cpf) || userId == null || sessionId == null)// || !authorizationHeader.StartsWith("Bearer ") || authorizationHeader.StartsWith("Bearer undefined"))
           {
                
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
                //;

            }
            User user = _context.Users.Where(u => u.Id == userId && u.CPF.Equals(cpf) && u.IsAdmin).FirstOrDefault();
            if(user == null)
            {
                //context.Result = new RedirectToActionResult("Login", "Home", null);

                context.Result = new ForbidResult();
                return;

            }
       
    }

  
   }
}
