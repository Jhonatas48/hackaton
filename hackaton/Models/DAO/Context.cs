using Microsoft.EntityFrameworkCore;
using hackaton.Models;

namespace hackaton.Models.DAO
{
    public class Context:DbContext
    {

        /* O Método construtor usa os objetos da superclasse para buscar as configurações */
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<User> QRCodes { get; set; }
        public DbSet<hackaton.Models.QrCode>? QrCode { get; set; }
    }
}
