using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Web.Models.DB
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accounts> Accounts { get; set; }
        public virtual DbSet<Dialogs> Dialogs { get; set; }
        public virtual DbSet<HightLights> HightLights { get; set; }
        public virtual DbSet<LoginChangeLog> LoginChangeLog { get; set; }
        public virtual DbSet<Logins> Logins { get; set; }
        public virtual DbSet<Markers> Markers { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<Stories> Stories { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=insta.upax.club;Database=Di;user id=sa;password=Ketrine8586;Trusted_Connection=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.Property(e => e.Login).IsUnicode(false);
            });

           
            modelBuilder.Entity<HightLights>(entity =>
            {
                entity.Property(e => e.HightLight).IsUnicode(false);
            });

            modelBuilder.Entity<LoginChangeLog>(entity =>
            {
                entity.Property(e => e.Followers).IsUnicode(false);

                entity.Property(e => e.FollowersAdd).IsUnicode(false);

                entity.Property(e => e.FollowersDel).IsUnicode(false);

                entity.Property(e => e.Followings).IsUnicode(false);

                entity.Property(e => e.FollowingsAdd).IsUnicode(false);

                entity.Property(e => e.FollowingsDel).IsUnicode(false);
            });

            modelBuilder.Entity<Logins>(entity =>
            {
                entity.Property(e => e.Login).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.ProxyIp).IsUnicode(false);
            });

            modelBuilder.Entity<Markers>(entity =>
            {
                entity.Property(e => e.Marker).IsUnicode(false);
            });

            modelBuilder.Entity<Posts>(entity =>
            {
                entity.Property(e => e.Commentators).IsUnicode(false);

                entity.Property(e => e.GeoCord).IsUnicode(false);

                entity.Property(e => e.GeoName).IsUnicode(false);

                entity.Property(e => e.Likers).IsUnicode(false);

                entity.Property(e => e.Markers).IsUnicode(false);

                entity.Property(e => e.PostId).IsUnicode(false);

                entity.Property(e => e.PostSrc).IsUnicode(false);

                entity.Property(e => e.TagUsers).IsUnicode(false);
            });

            modelBuilder.Entity<Stories>(entity =>
            {
                entity.Property(e => e.GeoName).IsUnicode(false);

                entity.Property(e => e.Src).IsUnicode(false);

                entity.Property(e => e.Tags).IsUnicode(false);

                entity.Property(e => e.Users).IsUnicode(false);
            });

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.Property(e => e.Title).IsUnicode(false);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.FindIn).IsUnicode(false);

                entity.Property(e => e.Followers).IsUnicode(false);

                entity.Property(e => e.Followings).IsUnicode(false);

                entity.Property(e => e.Friends).IsUnicode(false);

                entity.Property(e => e.FullName).IsUnicode(false);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.ParsingLogin).IsUnicode(false);

                entity.Property(e => e.Pk).IsUnicode(false);

                entity.Property(e => e.PublicEmail).IsUnicode(false);

                entity.Property(e => e.PublicPhone).IsUnicode(false);
            });
        }
    }
}
