using DevOne.Security.Cryptography.BCrypt;
using Microsoft.EntityFrameworkCore;
namespace hackaton.Models.DAO
{
    public class PopulateDataBase
    {

        public static void initialize(IApplicationBuilder app)
        {
            //associa os dados ao contexto
            Context context = app.ApplicationServices.GetRequiredService<Context>();

            //inserir os dados nas entidades do contexto
            context.Database.Migrate();

            //Se o contexto estiver vazio
            if (!context.Users.Any())
            {
                context.Users.Add(new User { Name = "Jhonatas", CPF = "1", Password = BCryptHelper.HashPassword("123",BCryptHelper.GenerateSalt()), IsAdmin = true });

            }
            context.SaveChanges();
        }
    }
}
