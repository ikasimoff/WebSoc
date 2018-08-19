using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
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

        public Insta(string login, string password) {
            Login = login;
            Password = password;
        }

        public async Task StartInstaAsync(string login, string password)
        {
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

            InstaApi = InstaApiBuilder.CreateBuilder()
                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                .SetUser(userSession)
                .Build();

            var loginResult = await InstaApi.LoginAsync();
            if (loginResult.Succeeded == true)
            {
                IsOnline = true;
            }
        }


        public async Task StartInstaAsync()
        {
            var userSession = new UserSessionData
            {
                UserName = Login,
                Password = Password
            };
            
            InstaApi = InstaApiBuilder.CreateBuilder()
                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                .SetUser(userSession)
                .Build();

            var loginResult = await InstaApi.LoginAsync();
            if (loginResult.Succeeded == true)
            {
                IsOnline = true;
            }
        }

        public async Task<InstaUserInfo> GetUserInfoAsync(string login) {
            var info = await InstaApi.GetUserInfoByUsernameAsync(login);
            return info.Value;
        }

        public void GetPosts(string login)
        {
            var pp = PaginationParameters.MaxPagesToLoad(5);



            //var posts = InstaApi.StoryProcessor.GetUserStoryFeedAsync(36063485).GetAwaiter().GetResult();
            var posts0 = InstaApi.StoryProcessor.GetUserHightLightsAsync(36063485).GetAwaiter().GetResult();
            //var posts0 = InstaApi.MediaProcessor.GetMediaByIdAsync("1844387017805196314_36063485").GetAwaiter().GetResult();
            //var posts01 = InstaApi.CommentProcessor.CommentMediaAsync("1844387017805196314_36063485", "ЧТО и сейчас нет?").GetAwaiter().GetResult();
            //var posts02 = InstaApi.MessagingProcessor.SendDirectMessage("36063485", "", "ЧТО и сейчас нет?").GetAwaiter().GetResult();
            //var posts00 = InstaApi.StoryProcessor.GetStoryFeedAsync().GetAwaiter().GetResult();
            var posts1 = InstaApi.UserProcessor.GetUserInfoByIdAsync(4429043156).GetAwaiter().GetResult();
            //posts1 = InstaApi.UserProcessor.GetUserInfoByIdAsync(1710646720).GetAwaiter().GetResult();
            //var posts2 = InstaApi.UserProcessor.(pp).GetAwaiter().GetResult();
            //var posts3 = InstaApi.UserProcessor.GetUserAsync("karinchess").GetAwaiter().GetResult();
            //var posts4 = InstaApi.StoryProcessor.GetUserStoryAsync(36063485).GetAwaiter().GetResult();//.GetUserAsync("karinchess").GetAwaiter().GetResult();

        }
    }
}
