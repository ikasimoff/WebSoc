using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
using InstaService.Models;

namespace InstaService.Instagram
{
    public class Insta
    {
        public TimeSpan delay = new TimeSpan(0, 0, 0, 0, 300);
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsOnline { get; set; }
        public IInstaApi InstaApi { get; set; }
        public List<HelperLogin> Logins { get; set; }

        public async Task StartInstaAsync(string login, string password) {

            Login = login;
            Password = password;

            var userSession = new UserSessionData
            {
                UserName = Login,
                Password = Password
            };
            
            //var httpHndler = new HttpClientHandler();
            //httpHndler.Proxy = new WebProxy("vashdoom.ru", 3128);
            //httpHndler.Proxy = new WebProxy("92.255.195.118", 8080);

            var api = InstaApiBuilder.CreateBuilder()
                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                .SetUser(userSession)
                .Build();

            var loginResult = await api.LoginAsync();
            if(loginResult.Succeeded == true)
            {
                IsOnline = true;
            }
        }

        public async Task GetUserInfo(string login) {
            
        }
    }
}
