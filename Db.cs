using System;
using Microsoft.EntityFrameworkCore;

using LuqinMiniAppBase.Models;
namespace LuqinMiniAppBase
{
    public class Db : DbContext
    {
        public Db(DbContextOptions<Db> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<MiniUser> miniUser { get; set; }

        public DbSet<UnicUser> unicUser { get; set; }

        public DbSet<Token> token { get; set; }

        public DbSet<Question> Question { get; set; }

        
        public DbSet<LuqinMiniAppBase.Models.SyncSns> SyncSns { get; set; }

    }
}
