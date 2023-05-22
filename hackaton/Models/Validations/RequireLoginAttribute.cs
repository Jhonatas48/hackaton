using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace hackaton.Models.Validations
{
    public class RequireLoginAttribute : Attribute, IAsyncAuthorizationFilter
    {

        //Chamado sempre que possuir o cabeçalho Authorization
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {

        string authorizationHeader = context.HttpContext.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ") || authorizationHeader.StartsWith("Bearer undefined"))
        {

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
        if (string.IsNullOrEmpty(token) || token.Equals("undefined"))
        {
            return false;
        }
        // Lógica de validação do token
        // Retorna true se o token for válido, caso contrário, retorna false
        // Implemente a lógica de validação de acordo com os requisitos do seu sistema
        // Você pode usar serviços, bancos de dados, listas de tokens válidos, etc.
        return true;
    }
}
}
