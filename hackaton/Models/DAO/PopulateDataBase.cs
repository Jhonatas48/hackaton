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
               
                context.Apis.Add(new Api { Name = "Grupo TechAgro", Token = Guid.NewGuid().ToString(), Active = true});
                context.Apis.Add(new Api { Name = "Grupo Isabela", Token = Guid.NewGuid().ToString(), Active = true });
               
                context.Users.Add(new User { Name = "Jhonatas", CPF = "136.621.076-00", Password = BCryptHelper.HashPassword("136.621.076-00", BCryptHelper.GenerateSalt()), IsAdmin = true});
                context.SaveChanges();
            }
          
        }
    }
}
