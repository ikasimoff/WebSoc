using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Digillect.AspNetCore.Authentication.VKontakte;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using Web.Models;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            //services.AddTransient<IEmailSender, EmailSender>();

            var auth = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

            auth.AddCookie(o => o.LoginPath = new Microsoft.AspNetCore.Http.PathString("/login"));
            auth.AddOAuth("VKontakte", "VKontakte", o =>
            {

                o.ClientId = "5234011";
                o.ClientSecret = "FiAu6kq6rN8JLxhfA7sS";

                o.ClaimsIssuer = VKontakteDefaults.Issuer;
                o.CallbackPath = new Microsoft.AspNetCore.Http.PathString("/vklogin");
                o.AuthorizationEndpoint = VKontakteDefaults.AuthorizationEndpoint;
                o.TokenEndpoint = VKontakteDefaults.TokenEndpoint;

                o.Scope.Add("email");
                o.SaveTokens = true;


                // In this case email will return in OAuthTokenResponse, 
                // but all scope values will be merged with user response
                //// so we can claim it as field
                //o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "uid");
                //o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                //o.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "first_name");
                //o.ClaimActions.MapJsonKey(ClaimTypes.Surname, "last_name");


                o.Events = new OAuthEvents
                {
                    OnCreatingTicket = context =>
                    {
                        var user = context.TokenResponse.Response.Value<string>("user_id");
                        context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                        var email = context.TokenResponse.Response.Value<string>("email");
                        if (!string.IsNullOrEmpty(email))
                        {
                            context.Identity.AddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.Email, context.Options.ClaimsIssuer));
                        }
                        else
                        {
                            context.Identity.AddClaim(new Claim(ClaimTypes.Email, user, ClaimValueTypes.Email, context.Options.ClaimsIssuer));
                            //context.RunClaimActions();
                        }


                        //o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "uid");
                        //o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                        //o.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "first_name");
                        //o.ClaimActions.MapJsonKey(ClaimTypes.Surname, "last_name");


                        //context.RunClaimActions();
                        return Task.FromResult(0);
                    }
                    //OnRemoteFailure = HandleOnRemoteFailure
                };
            });



            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); ;

            services.AddSignalR();
            services.AddKendo();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
