﻿/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    internal class DiscoverProcessor : IDiscoverProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public DiscoverProcessor(AndroidDevice deviceInfo, UserSessionData user,
            IHttpRequestProcessor httpRequestProcessor, IInstaLogger logger, UserAuthValidate userAuthValidate)
        {
            _deviceInfo = deviceInfo;
            _user = user;
            _httpRequestProcessor = httpRequestProcessor;
            _logger = logger;
            _userAuthValidate = userAuthValidate;
        }


        /// <summary>
        /// Get recent searches
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<DiscoverRecentSearchsResponse>> GetRecentSearchsAsync()
        {
            try
            {
                var instaUri = UriCreator.GetRecentSearchUri();
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.Fail("Status code: " + response.StatusCode, (DiscoverRecentSearchsResponse)null);
                var obj = JsonConvert.DeserializeObject<DiscoverRecentSearchsResponse>(json);
                return Result.Success(obj);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<DiscoverRecentSearchsResponse>(exception);
            }
        }
        /// <summary>
        /// Clear Recent searches
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<DicoverDefaultResponse>> ClearRecentSearchsAsync()
        {
            try
            {
                var instaUri = UriCreator.GetClearSearchHistoryUri();
                var data = new JObject
                {
                    { "_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                };
                var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.Fail("Status code: " + response.StatusCode, (DicoverDefaultResponse)null);
                var obj = JsonConvert.DeserializeObject<DicoverDefaultResponse>(json);
                return Result.Success(obj);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<DicoverDefaultResponse>(exception);
            }
        }
        /// <summary>
        /// Get suggested searches
        /// </summary>
        /// <param name="searchType">Search type(only blended and users works)</param>
        /// <returns></returns>
        public async Task<IResult<DiscoverSuggestionResponse>> GetSuggestedSearchesAsync(DiscoverSearchType searchType)
        {
            try
            {
                var instaUri = UriCreator.GetSuggestedSearchUri(searchType);
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.Fail("Status code: " + response.StatusCode, (DiscoverSuggestionResponse)null);
                var obj = JsonConvert.DeserializeObject<DiscoverSuggestionResponse>(json);
                return Result.Success(obj);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<DiscoverSuggestionResponse>(exception);
            }
        }
        /// <summary>
        /// Search user people
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <param name="count">Count</param>
        /// <returns></returns>
        public async Task<IResult<DiscoverSearchResponse>> SearchPeopleAsync(string text, int count = 30)
        {
            try
            {
                var instaUri = UriCreator.GetSearchUserUri(text, count);
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.Fail("Status code: " + response.StatusCode, (DiscoverSearchResponse)null);
                var obj = JsonConvert.DeserializeObject<DiscoverSearchResponse>(json);
                return Result.Success(obj);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<DiscoverSearchResponse>(exception);
            }
        }
        
        
        
        
        
        
        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<object>> AcceptFriendshipAsync(long userId)
        {
            try
            {
                var instaUri = UriCreator.GetAcceptFriendshipUri(userId);
                Debug.WriteLine(instaUri.ToString());
                var data = new JObject
                {
                    { "_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"user_id", userId},
                    {"radio_type", "wifi"}
                };
                var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(json);
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.Fail("Status code: " + response.StatusCode, (DiscoverRecentSearchsResponse)null);
                var obj = JsonConvert.DeserializeObject<DiscoverRecentSearchsResponse>(json);
                return Result.Success(obj);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<DiscoverRecentSearchsResponse>(exception);
            }
        }
        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<object>> RejectFriendshipAsync(long userId)
        {
            try
            {

                var instaUri = UriCreator.GetDenyFriendshipUri(userId);
                Debug.WriteLine(instaUri.ToString());
                var data = new JObject
                {
                    { "_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"user_id", userId},
                    {"radio_type", "wifi"}
                };
                var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.Fail("Status code: " + response.StatusCode, (DiscoverRecentSearchsResponse)null);
                Debug.WriteLine(json);
                var obj = JsonConvert.DeserializeObject<DiscoverRecentSearchsResponse>(json);
                return Result.Success(obj);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<DiscoverRecentSearchsResponse>(exception);
            }
        }
        /// <summary>
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<object>> DiscoverPeopleAsync()
        {
            try
            {
                var instaUri = UriCreator.GetDiscoverPeopleUri();
                Debug.WriteLine(instaUri.ToString());

                var data = new JObject
                {
                    { "phone_id", _deviceInfo.DeviceGuid.ToString()},
                    { "module","discover_people"},
                    { "_csrftoken", _user.CsrfToken},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    { "paginate","true"}
                    //{"_uid", _user.LoggedInUder.Pk.ToString()},
                };

                var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Host = "i.instagram.com";
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(json);
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.Fail("Status code: " + response.StatusCode, (DicoverDefaultResponse)null);
                var obj = JsonConvert.DeserializeObject<DicoverDefaultResponse>(json);
                return Result.Success(obj);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<DicoverDefaultResponse>(exception);
            }
        }
    }
}
