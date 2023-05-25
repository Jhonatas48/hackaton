using hackaton.Controllers;
using hackaton.Models.Caches;
using hackaton.Models.DAO;
using hackaton.Models.Injectors;
using hackaton.Models.WebSocket;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddControllersWithViews();

            //Configura o sistema de Cache
            services.AddMemoryCache();

            //Configura o aplicativo para poder receber conexões websockets
            services.AddSignalR();
            //Configura a Classe RedirectClient para ser usado no injetor do aspnet
            services.AddScoped<RedirectClient>();
            services.AddScoped<RequireLoginAttributeFactory>();
            services.AddScoped<RequireLoginAdminAttributeFactory>();
            //Adiciona a Classe UserCacheService no escopo para ser usado como cache
            services.AddScoped<UserCacheService>();
            //Adiciona a Classe QRCodeService no escopo para ser usado como cache
            services.AddScoped<QRCodeCacheService>();
            services.AddScoped<HomeController>();
            //configuração para acesso ao banco de dados
            services.AddDbContext<Context>(options => options.UseSqlServer(
               Configuration["Data:ConnectionString"]));
            services.AddMvc();
            services.AddAuthentication( options =>
            {
                options.DefaultScheme = "MyAuthenticationScheme";
                options.DefaultForbidScheme = "MyAuthenticationScheme";
            }
            ).AddCookie("MyAuthenticationScheme", options =>
            {
                // Nome do cookie usado para armazenar as informações de autenticação
                options.Cookie.Name = "MyAuthCookie";

                // Define se o cookie é acessível apenas por HTTP (HttpOnly)
                options.Cookie.HttpOnly = true;

                // Define se o cookie deve ser enviado apenas em conexões seguras (HTTPS)
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;

                // Define o tempo de expiração do cookie
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                // Define a rota para redirecionar o usuário quando ocorrer uma autenticação falhada
                options.AccessDeniedPath = "/Home/PermissionDenied";

                // Define a rota para redirecionar o usuário quando ocorrer um desafio de autenticação
               // options.LoginPath = "/Home/Login";

                // Define a rota para redirecionar o usuário após fazer logout
                options.LogoutPath = "/Home";
            });
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserCacheService userCache, QRCodeCacheService qrCodeCache,Context context)
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
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //Adiciona o RedirectClient no sistema de mapeamento de endpoint
                endpoints.MapHub<RedirectClient>("/RedirectClient");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //DadosIniciais.popularBancoDeDados(app);
            }

            );
            
            // Obter a lista de usuários do banco de dados
            var users = context.Users;//.ToList();
           
            if(users != null)
            {
                var usersList = users.ToList();
                foreach (var user in usersList)
                {
                    userCache.AddUserToCache(user);
                }
            }
            // Adicionar cada usuário ao cache

            var qrCodes = context.QrCodes;
            if(qrCodes != null)
            {

                var qrList = qrCodes.ToList();
                foreach (var qr in qrList)
                {
                     qrCodeCache.AddQRCodeToCache(qr);
                }
            }

        }

    }
}
