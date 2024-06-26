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
            /*
             modelBuilder.Entity<MediaSubTitle>()
                 .HasOne(ms => ms.media_id)
                 .WithMany(m => m.mediaSubTitles)
                 .HasForeignKey(ms => ms.media_id)
                 .HasPrincipalKey(m => m.id);
             */

            modelBuilder.Entity<TTUser>().HasKey(k => new { k.open_id, k.app_id });
            modelBuilder.Entity<ClubJoinApp>().HasKey(k => new { k.club_id, k.user_id });
        }

        public DbSet<MiniUser> miniUser { get; set; }

        public DbSet<UnicUser> unicUser { get; set; }

        public DbSet<Token> token { get; set; }

        public DbSet<Question> Question { get; set; }

        
        public DbSet<LuqinMiniAppBase.Models.SyncSns> SyncSns { get; set; }

        
        public DbSet<LuqinMiniAppBase.Models.QrCodeScanLog> QrCodeScanLog { get; set; }

        public DbSet<Media> media { get; set; }

        public DbSet<MediaSubTitle> mediaSubtitle { get; set; }

        public DbSet<UserMediaAsset> userMediaAsset { get; set; }

        public DbSet<LuqinMiniAppBase.Models.UserStudyProgress> UserStudyProgress { get; set; }

        public DbSet<LuqinMiniAppBase.Models.OAReceive> oAReceive { get; set; }

        public DbSet<LuqinMiniAppBase.Models.OASent> oASent { get; set; }

        public DbSet<LuqinMiniAppBase.Models.WepayKey> wepayKey { get; set; }

        public DbSet<LuqinMiniAppBase.Models.WepayOrder> wepayOrder { get; set; }

        public DbSet<WepayOrderRefund> wepayOrderRefund { get; set; }

        public DbSet<Promote> promote { get; set; }

        public DbSet<LuqinMiniAppBase.Models.Reserve> Reserve { get; set; }

        public DbSet<LuqinMiniAppBase.Models.Health> Health { get; set; }

        public DbSet<LuqinMiniAppBase.Models.MiniSession> miniSession { get; set; }

        public DbSet<LuqinMiniAppBase.Models.CampRegistration> CampRegistration { get; set; }

        public DbSet<LuqinMiniAppBase.Models.TTUser> tiktokUser { get; set; }

        public DbSet<LuqinMiniAppBase.Models.UserCollected> userCollected { get; set; }

        public DbSet<LuqinMiniAppBase.Models.ClubJoinApp> clubJoinApp { get; set; }

        public DbSet<VisaCity> visaCity { get; set; }


    }
}
