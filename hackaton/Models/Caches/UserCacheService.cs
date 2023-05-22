using hackaton.Models.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace hackaton.Models.Caches
{

    /*
        Classe Responsável para armazenar os usuarios em cache,para melhor performance na aplicação
     */
    public class UserCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly Context _context;

        public UserCacheService(IMemoryCache cache, Context context)
        {
            _cache = cache;
            _context = context;

        }

        public void AddUserToCache(User user)
        {
            _cache.Set(user.CPF, user);
        }


        public User GetUserByCPFAsync(string cpf)
        {
            // Verifica se o usuário está no cache
            if (_cache.TryGetValue(cpf, out User user))
            {
                return user;
            }
          
            // Se o usuário não está no cache, busca no banco de dados
            user = _context.Users.FirstOrDefault(u => u.CPF == cpf);

            // Adiciona o usuário ao cache por 5 minutos
            if (user != null)
            {
                _cache.Set(cpf, user, TimeSpan.FromMinutes(5));
            }

            return user;
        }
    }
}
