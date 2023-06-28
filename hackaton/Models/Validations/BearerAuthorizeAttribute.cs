using hackaton.Models.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NuGet.Protocol;
using System.Net;

namespace hackaton.Models.Security
{
    public class BearerAuthorizeAttribute: Attribute, IAsyncAuthorizationFilter
    {
        private readonly Context _context;
        public BearerAuthorizeAttribute(Context context)
        {
            _context = context;

        }

        //Chamado sempre que possuir o cabeçalho Authorization
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
        
            string authorizationHeader = context.HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ") || authorizationHeader.StartsWith("Bearer undefined")){

                //Retorna uma Status HTTP Bad Request com a message Bearer Token is required
                context.Result = new BadRequestObjectResult(new { message = "Bearer <Your_Token> is required" });

                return;
            }

            // Obter o token do cabeçalho Authorization com a Bearer TokenS
            string token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            // Realizar a validação do token
            if (!IsValidToken(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        private bool IsValidToken(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Equals("undefined")) {
                return false;
            }

           var apiRetrieve=  _context.Apis.Where(api=> api.Token.Equals(token)).FirstOrDefault();   
            
            return apiRetrieve != null;
        }
    }
}
