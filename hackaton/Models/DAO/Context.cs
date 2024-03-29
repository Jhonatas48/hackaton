﻿using Microsoft.EntityFrameworkCore;
using hackaton.Models;

namespace hackaton.Models.DAO
{
    public class Context:DbContext
    {
        public Context()
        {
        }

        /* O Método construtor usa os objetos da superclasse para buscar as configurações */
        public Context(DbContextOptions<Context> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<QrCode> QrCodes { get; set; }
        public DbSet<Api>Apis { get; set; }
        public DbSet<Schedule> Schedules { get; set;}

       
    }
}
