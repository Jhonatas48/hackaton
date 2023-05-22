using hackaton.Controllers;
using hackaton.Models.Caches;
using hackaton.Models.DAO;
using Microsoft.EntityFrameworkCore;

namespace hackaton
{
    public class Startup
    {
        private readonly Context _context;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           
           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //Configura o sistema de Cache
            services.AddMemoryCache();
            //Adiciona a Classe UserCacheService no escopo para ser usado como cache
            services.AddScoped<UserCacheService>();
            //Adiciona a Classe QRCodeService no escopo para ser usado como cache
            services.AddScoped<QRCodeCacheService>();


            services.AddScoped<HomeController>();
            //configuração para acesso ao banco de dados
            services.AddDbContext<Context>(options => options.UseSqlServer(
               Configuration["Data:ConnectionString"]));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserCacheService userCache, Context context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //DadosIniciais.popularBancoDeDados(app);
            }

            );
            /*
            // Obter a lista de usuários do banco de dados
            var users = context.Users.;//.ToList();
           
            if(users != null)
            {
                var usersList = users.ToList();
                foreach (var user in usersList)
                {
                    userCache.AddUserToCache(user);
                }
            }
            // Adicionar cada usuário ao cache
           */

        }

    }
}
