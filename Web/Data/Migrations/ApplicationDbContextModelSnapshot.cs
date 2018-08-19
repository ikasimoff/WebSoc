﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Web.Data;

namespace Web.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Web.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("BirthDay");

                    b.Property<string>("City");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Description");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("Fb");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("InstaLink")
                        .HasMaxLength(150);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("Photo");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<string>("Vk");

                    b.Property<string>("Website");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Web.Models.DB.Dialogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AccountId");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<long>("LoginId");

                    b.Property<long>("ToUserId");

                    b.HasKey("Id");

                    b.ToTable("Dialogs");
                });

            modelBuilder.Entity("Web.Models.DB.HightLights", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count");

                    b.Property<string>("HightLight")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.ToTable("HightLights");
                });

            modelBuilder.Entity("Web.Models.DB.InstaUsers", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("datetime");

                    b.Property<string>("Description");

                    b.Property<string>("FindIn")
                        .HasMaxLength(50);

                    b.Property<string>("Followers");

                    b.Property<string>("FollowersCount")
                        .HasMaxLength(10);

                    b.Property<string>("Followings");

                    b.Property<string>("FollowingsCount")
                        .HasMaxLength(10);

                    b.Property<string>("Friends");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<int?>("Gender");

                    b.Property<int?>("HasChildValue");

                    b.Property<int?>("HasPetsValue");

                    b.Property<string>("Image")
                        .HasMaxLength(500);

                    b.Property<string>("ImageId")
                        .HasMaxLength(10);

                    b.Property<bool?>("IsAds");

                    b.Property<bool>("IsBuisnes");

                    b.Property<bool?>("IsGroup");

                    b.Property<bool?>("IsInstagrammer");

                    b.Property<bool>("IsPrivate");

                    b.Property<DateTime?>("LastParsings")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("LastUpdate")
                        .HasColumnType("datetime");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<int?>("Old");

                    b.Property<string>("ParsingLogin")
                        .HasMaxLength(50);

                    b.Property<string>("Pk")
                        .IsRequired()
                        .HasColumnName("PK")
                        .HasMaxLength(50);

                    b.Property<string>("PublicEmail")
                        .HasMaxLength(250);

                    b.Property<string>("PublicPhone")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("InstaUsers");
                });

            modelBuilder.Entity("Web.Models.DB.LoginChangeLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Followers")
                        .IsRequired();

                    b.Property<string>("FollowersAdd");

                    b.Property<string>("FollowersCount")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("FollowersCountAdd")
                        .HasMaxLength(10);

                    b.Property<string>("FollowersCountDel")
                        .HasMaxLength(10);

                    b.Property<string>("FollowersDel");

                    b.Property<string>("Followings")
                        .IsRequired();

                    b.Property<string>("FollowingsAdd");

                    b.Property<string>("FollowingsCount")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("FollowingsCountAdd")
                        .HasMaxLength(10);

                    b.Property<string>("FollowingsCountDel")
                        .HasMaxLength(10);

                    b.Property<string>("FollowingsDel");

                    b.HasKey("Id");

                    b.ToTable("LoginChangeLog");
                });

            modelBuilder.Entity("Web.Models.DB.Logins", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("AccountId");

                    b.Property<string>("Description");

                    b.Property<bool>("IsBlocked");

                    b.Property<DateTime?>("LastUpdate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("LastUse")
                        .HasColumnType("datetime");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ProxyIp")
                        .HasMaxLength(50);

                    b.Property<bool>("UseForParsing");

                    b.Property<bool?>("UseProxy");

                    b.HasKey("Id");

                    b.ToTable("Logins");
                });

            modelBuilder.Entity("Web.Models.DB.Markers", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Marker")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.ToTable("Markers");
                });

            modelBuilder.Entity("Web.Models.DB.Messages", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime");

                    b.Property<long>("DialogId");

                    b.Property<long>("FromUserId");

                    b.Property<bool>("IsSeen");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<DateTime?>("SeenDate")
                        .HasColumnType("datetime");

                    b.Property<long>("ToUserId");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Web.Models.DB.Posts", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Commentators");

                    b.Property<string>("Comments");

                    b.Property<int?>("CommntsCount");

                    b.Property<DateTime>("DateTimeInsta")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("DateTimeParsing")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateTimeShot")
                        .HasColumnType("datetime");

                    b.Property<string>("Description");

                    b.Property<string>("GeoCord")
                        .HasMaxLength(50);

                    b.Property<string>("GeoName")
                        .HasMaxLength(500);

                    b.Property<bool?>("IsAds");

                    b.Property<string>("Likers");

                    b.Property<int?>("LikersCount");

                    b.Property<string>("Markers");

                    b.Property<string>("PostId")
                        .HasMaxLength(50);

                    b.Property<string>("PostSrc")
                        .HasMaxLength(500);

                    b.Property<string>("TagUsers");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Web.Models.DB.Stories", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<string>("GeoName");

                    b.Property<string>("Src");

                    b.Property<string>("Tags");

                    b.Property<long>("UserId");

                    b.Property<string>("Users");

                    b.HasKey("Id");

                    b.ToTable("Stories");
                });

            modelBuilder.Entity("Web.Models.DB.Tasks", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Web.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Web.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Web.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Web.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
