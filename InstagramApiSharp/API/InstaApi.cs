﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InstagramApiSharp.API.Processors;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Classes.ResponseWrappers.BaseResponse;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstagramApiSharp.API
{
    internal class InstaApi : IInstaApi
    {
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly IInstaLogger _logger;
        private ICollectionProcessor _collectionProcessor;
        private ICommentProcessor _commentProcessor;
        private AndroidDevice _deviceInfo;
        private IFeedProcessor _feedProcessor;

        private IHashtagProcessor _hashtagProcessor;
        private ILocationProcessor _locationProcessor;
        private IMediaProcessor _mediaProcessor;
        private IMessagingProcessor _messagingProcessor;
        private IStoryProcessor _storyProcessor;
        private TwoFactorLoginInfo _twoFactorInfo;
        private InstaChallengeLoginInfo _challengeinfo;
        private UserSessionData _userSession;
        private UserSessionData _user
        {
            get { return _userSession; }
            set { _userSession = value; _userAuthValidate.User = value; }
        }
        private UserAuthValidate _userAuthValidate;
        private IUserProcessor _userProcessor;

        private ILiveProcessor _liveProcessor;
        /// <summary>
        ///     Live api functions.
        /// </summary>
        public ILiveProcessor LiveProcessor => _liveProcessor;

        private IDiscoverProcessor _discoverProcessor;
        /// <summary>
        ///     Discover api functions.
        /// </summary>
        public IDiscoverProcessor DiscoverProcessor => _discoverProcessor;

        private IAccountProcessor _accountProcessor;
        /// <summary>
        ///     Account api functions.
        /// </summary>
        public IAccountProcessor AccountProcessor => _accountProcessor;
        /// <summary>
        ///     Comments api functions.
        /// </summary>
        public ICommentProcessor CommentProcessor => _commentProcessor;
        /// <summary>
        ///     Story api functions.
        /// </summary>
        public IStoryProcessor StoryProcessor => _storyProcessor;
        /// <summary>
        ///     Media api functions.
        /// </summary>
        public IMediaProcessor MediaProcessor => _mediaProcessor;
        /// <summary>
        ///     Messaging (direct) api functions.
        /// </summary>
        public IMessagingProcessor MessagingProcessor => _messagingProcessor;
        /// <summary>
        ///     Feed api functions.
        /// </summary>
        public IFeedProcessor FeedProcessor => _feedProcessor;
        /// <summary>
        ///     Collection api functions.
        /// </summary>
        public ICollectionProcessor CollectionProcessor => _collectionProcessor;
        /// <summary>
        /// Location api functions.
        /// </summary>
        public ILocationProcessor LocationProcessor => _locationProcessor;
        /// <summary>
        ///     Hashtag api functions.
        /// </summary>
        public IHashtagProcessor HashtagProcessor => _hashtagProcessor;
        public IUserProcessor UserProcessor => _userProcessor;
        public InstaApi(UserSessionData user, IInstaLogger logger, AndroidDevice deviceInfo,
            IHttpRequestProcessor httpRequestProcessor)
        {
            _userAuthValidate = new UserAuthValidate();
            _user = user;
            _logger = logger;
            _deviceInfo = deviceInfo;
            _httpRequestProcessor = httpRequestProcessor;
        }

        public UserSessionData GetLoggedUser()
        {
            return _user;
        }

        /// <summary>
        ///     Get user timeline feed (feed of recent posts from users you follow) asynchronously.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaFeed" />
        /// </returns>
        public async Task<IResult<InstaFeed>> GetUserTimelineFeedAsync(PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _feedProcessor.GetUserTimelineFeedAsync(paginationParameters);
        }

        public async Task<IResult<InstaPendingRequest>> GetPendingFriendRequests()
        {
            ValidateUser();
            ValidateLoggedIn();
            try
            {
                var cookies =
                    _httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(_httpRequestProcessor.Client
                        .BaseAddress);
                var csrftoken = cookies[InstaApiConstants.CSRFTOKEN]?.Value ?? String.Empty;
                _user.CsrfToken = csrftoken;
                var instaUri = new Uri($"https://i.instagram.com/api/v1/friendships/pending/?rank_mutual=0&rank_token={_user.RankToken}", UriKind.RelativeOrAbsolute);               
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var JRes = JsonConvert.DeserializeObject<InstaPendingRequest>(json);
                    return Result.Success(JRes);
                }
                else
                {
                    return Result.Fail<InstaPendingRequest>(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                return Result.Fail<InstaPendingRequest>(ex.Message);
            }
        }
        /// <summary>
        /// Accept user friendship requst
        /// </summary>
        /// <param name="UserID">User.PK</param>
        /// <returns></returns>
        public async Task<IResult<InstaFriendshipStatus>> AcceptFriendshipRequest(long UserID)
        {
            ValidateUser();
            ValidateLoggedIn();
            try
            {
                var instaUri = new Uri($"https://i.instagram.com/api/v1/friendships/approve/{UserID}/", UriKind.RelativeOrAbsolute);
                var fields = new Dictionary<string, string>
                {
                    {"user_id", UserID.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                };
                var request =
                    HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, fields);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaFriendshipStatus>(response, json);
                var JRes = JsonConvert.DeserializeObject<InstaFriendshipStatus>(json);
                return Result.Success(JRes);
            }
            catch (Exception ex)
            {
                return Result.Fail<InstaFriendshipStatus>(ex.Message);
            }
        }
        /// <summary>
        /// Ignore user friendship requst
        /// </summary>
        /// <param name="UserID">User.PK</param>
        /// <returns></returns>
        public async Task<IResult<InstaFriendshipStatus>> IgnoreFriendshipRequest(long UserID)
        {
            ValidateUser();
            ValidateLoggedIn();
            try
            {
                var instaUri = new Uri($"https://i.instagram.com/api/v1/friendships/ignore/{UserID}/", UriKind.RelativeOrAbsolute);
                var fields = new Dictionary<string, string>
                {
                    {"user_id", UserID.ToString()},
                    {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                    {"_uid", _user.LoggedInUser.Pk.ToString()},
                    {"_csrftoken", _user.CsrfToken},
                };
                var request =
                    HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, fields);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<InstaFriendshipStatus>(response, json);
                var JRes = JsonConvert.DeserializeObject<InstaFriendshipStatus>(json);
                return Result.Success(JRes);
            }
            catch (Exception ex)
            {
                return Result.Fail<InstaFriendshipStatus>(ex.Message);
            }
        }
        /// <summary>
        ///     Get user story reel feed. Contains user info last story including all story items.
        /// </summary>
        /// <param name="userId">User identifier (PK)</param>
        /// <returns></returns>
        public async Task<IResult<InstaReelFeed>> GetUserStoryFeedAsync(long userId)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _storyProcessor.GetUserStoryFeedAsync(userId);
        }


        /// <summary>
        ///     Get user explore feed (Explore tab info) asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaExploreFeed" />&gt;
        /// </returns>
        public async Task<IResult<InstaExploreFeed>> GetExploreFeedAsync(PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _feedProcessor.GetExploreFeedAsync(paginationParameters);
        }

        /// <summary>
        ///     Get all user media by username asynchronously
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetUserMediaAsync(string username,
            PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetUserMediaAsync(username, paginationParameters);
        }

        /// <summary>
        ///     Get media by its id asynchronously
        /// </summary>
        /// <param name="mediaId">Maximum count of pages to retrieve</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaMedia" />
        /// </returns>
        public async Task<IResult<InstaMedia>> GetMediaByIdAsync(string mediaId)
        {
            ValidateUser();
            return await _mediaProcessor.GetMediaByIdAsync(mediaId);
        }

        /// <summary>
        ///     Get user info by its user name asynchronously
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaUser" />
        /// </returns>
        public async Task<IResult<InstaUser>> GetUserAsync(string username)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetUserAsync(username);
        }


        /// <summary>
        ///     Get currently logged in user info asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaCurrentUser" />
        /// </returns>
        public async Task<IResult<InstaCurrentUser>> GetCurrentUserAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetCurrentUserAsync();
        }

        /// <summary>
        ///     Get tag feed by tag value asynchronously
        /// </summary>
        /// <param name="tag">Tag value</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaTagFeed" />
        /// </returns>
        public async Task<IResult<InstaTagFeed>> GetTagFeedAsync(string tag, PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _feedProcessor.GetTagFeedAsync(tag, paginationParameters);
        }

        /// <summary>
        ///     Get followers list by username asynchronously
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="searchQuery">Search string to locate specific followers</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaUserShortList" />
        /// </returns>
        public async Task<IResult<InstaUserShortList>> GetUserFollowersAsync(string username,
            PaginationParameters paginationParameters, string searchQuery = "")
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetUserFollowersAsync(username, paginationParameters, searchQuery);
        }

        /// <summary>
        ///     Get following list by username asynchronously
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="searchQuery">Search string to locate specific followings</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaUserShortList" />
        /// </returns>
        public async Task<IResult<InstaUserShortList>> GetUserFollowingAsync(string username,
            PaginationParameters paginationParameters, string searchQuery = "")
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetUserFollowingAsync(username, paginationParameters, searchQuery);
        }

        /// <summary>
        ///     Gets the user extended information (followers count, following count, bio, etc) by user identifier.
        /// </summary>
        /// <param name="pk">User Id, like "123123123"</param>
        /// <returns></returns>
        public async Task<IResult<InstaUserInfo>> GetUserInfoByIdAsync(long pk)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetUserInfoByIdAsync(pk);
        }

        /// <summary>
        ///     Gets the user extended information (followers count, following count, bio, etc) by username.
        /// </summary>
        /// <param name="username">Username, like "instagram"</param>
        /// <returns></returns>
        public async Task<IResult<InstaUserInfo>> GetUserInfoByUsernameAsync(string username)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetUserInfoByUsernameAsync(username);
        }

        /// <summary>
        ///     Get followers list for currently logged in user asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaUserShortList" />
        /// </returns>
        public async Task<IResult<InstaUserShortList>> GetCurrentUserFollowersAsync(
            PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetCurrentUserFollowersAsync(paginationParameters);
        }

        /// <summary>
        ///     Get user tags by username asynchronously
        ///     <remarks>Returns media list containing tags</remarks>
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetUserTagsAsync(string username,
            PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();
            var user = await GetUserAsync(username);
            if (!user.Succeeded)
                return Result.Fail($"Unable to get user {username} to get tags", (InstaMediaList)null);
            return await _userProcessor.GetUserTagsAsync(user.Value.Pk, paginationParameters);
        }


        /// <summary>
        ///     Get direct inbox threads for current user asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaDirectInboxContainer" />
        /// </returns>
        public async Task<IResult<InstaDirectInboxContainer>> GetDirectInboxAsync(PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _messagingProcessor.GetDirectInboxAsync(paginationParameters);
        }

        /// <summary>
        ///     Get direct inbox thread by its id asynchronously
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaDirectInboxThread" />
        /// </returns>
        public async Task<IResult<InstaDirectInboxThread>> GetDirectInboxThreadAsync(string threadId, PaginationParameters pagination)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _messagingProcessor.GetDirectInboxThreadAsync(threadId, pagination);
        }

        /// <summary>
        ///     Send direct message to provided users and threads
        /// </summary>
        /// <param name="recipients">Comma-separated users PK</param>
        /// <param name="threadIds">Message thread ids</param>
        /// <param name="text">Message text</param>
        /// <returns>
        ///     List of threads
        /// </returns>
        public async Task<IResult<InstaDirectInboxThreadList>> SendDirectMessage(string recipients, string threadIds,
            string text)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _messagingProcessor.SendDirectMessage(recipients, threadIds, text);
        }

        /// <summary>
        ///     Get recent recipients (threads and users) asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaRecipientThreads" />
        /// </returns>
        public async Task<IResult<InstaRecipients>> GetRecentRecipientsAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _messagingProcessor.GetRecentRecipientsAsync();
        }

        /// <summary>
        ///     Get ranked recipients (threads and users) asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaRecipientThreads" />
        /// </returns>
        public async Task<IResult<InstaRecipients>> GetRankedRecipientsAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _messagingProcessor.GetRankedRecipientsAsync();
        }

        /// <summary>
        ///     Get recent activity info asynchronously
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaActivityFeed" />
        /// </returns>
        public async Task<IResult<InstaActivityFeed>> GetRecentActivityAsync(PaginationParameters paginationParameters)
        {
            return await _feedProcessor.GetRecentActivityFeedAsync(paginationParameters);
        }

        /// <summary>
        ///     Get activity of following asynchronously
        /// </summary>
        /// <param name="paginationParameters"></param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaActivityFeed" />
        /// </returns>
        public async Task<IResult<InstaActivityFeed>> GetFollowingRecentActivityAsync(
            PaginationParameters paginationParameters)
        {
            return await _feedProcessor.GetFollowingRecentActivityFeedAsync(paginationParameters);
        }


        /// <summary>
        ///     Like media (photo or video)
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <returns></returns>
        public async Task<IResult<bool>> LikeMediaAsync(string mediaId)
        {
            return await _mediaProcessor.LikeMediaAsync(mediaId);
        }

        /// <summary>
        ///     Remove like from media (photo or video)
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <returns></returns>
        public async Task<IResult<bool>> UnLikeMediaAsync(string mediaId)
        {
            return await _mediaProcessor.UnLikeMediaAsync(mediaId);
        }


        /// <summary>
        ///     Get media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="paginationParameters">Maximum amount of pages to load and start id</param>
        /// <returns></returns>
        public async Task<IResult<InstaCommentList>> GetMediaCommentsAsync(string mediaId,
            PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();

            return await _commentProcessor.GetMediaCommentsAsync(mediaId, paginationParameters);
        }

        /// <summary>
        ///     Allow media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<bool>> EnableMediaCommentAsync(string mediaId)
        {
            ValidateUser();
            ValidateLoggedIn();

            return await _commentProcessor.EnableMediaCommentAsync(mediaId);
        }
        /// <summary>
        ///     Disable media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<bool>> DisableMediaCommentAsync(string mediaId)
        {
            ValidateUser();
            ValidateLoggedIn();

            return await _commentProcessor.DisableMediaCommentAsync(mediaId);
        }
        /// <summary>
        ///     Get media comments likers
        /// </summary>
        /// <param name="mediaId">Media id</param>
        public async Task<IResult<bool>> GetMediaCommentLikersAsync(string mediaId)
        {
            ValidateUser();
            ValidateLoggedIn();

            return await _commentProcessor.GetMediaCommentLikersAsync(mediaId);
        }
        /// <summary>
        ///     Report media comment
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="commentId">Comment id</param>
        public async Task<IResult<bool>> ReportCommentAsync(string mediaId, string commentId)
        {
            ValidateUser();
            ValidateLoggedIn();

            return await _commentProcessor.ReportCommentAsync(mediaId, commentId);
        }
        /// <summary>
        ///     Get media inline comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="paginationParameters">Maximum amount of pages to load and start id</param>
        /// <returns></returns>
        public async Task<IResult<InstaInlineCommentListResponse>> GetMediaInlineCommentsAsync(string mediaId, string targetCommentId,
            PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();

            return await _commentProcessor.GetMediaInlineCommentsAsync(mediaId, targetCommentId, paginationParameters);
        }

        /// <summary>
        ///     Get users (short) who liked certain media. Normaly it return around 1000 last users.
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <returns></returns>
        public async Task<IResult<InstaLikersList>> GetMediaLikersAsync(string mediaId)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _mediaProcessor.GetMediaLikersAsync(mediaId);
        }

        /// <summary>
        ///     Follow user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public async Task<IResult<InstaFriendshipStatus>> FollowUserAsync(long userId)
        {
            return await _userProcessor.FollowUserAsync(userId);
        }

        /// <summary>
        ///     Stop follow user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public async Task<IResult<InstaFriendshipStatus>> UnFollowUserAsync(long userId)
        {
            return await _userProcessor.UnFollowUserAsync(userId);
        }


        /// <summary>
        ///     Block user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public async Task<IResult<InstaFriendshipStatus>> BlockUserAsync(long userId)
        {
            return await _userProcessor.BlockUserAsync(userId);
        }

        /// <summary>
        ///     Stop Block user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public async Task<IResult<InstaFriendshipStatus>> UnBlockUserAsync(long userId)
        {
            return await _userProcessor.UnBlockUserAsync(userId);
        }
        [Obsolete("SetAccountPrivateAsync is deprecated, please use AccountProcessor.SetAccountPrivateAsync instead.\r\nThis will be deleted in the next update.")]
        /// <summary>
        ///     Set current account private
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<InstaUserShort>> SetAccountPrivateAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _accountProcessor.SetAccountPrivateAsync();
        }
        [Obsolete("SetAccountPublicAsync is deprecated, please use AccountProcessor.SetAccountPublicAsync instead.\r\nThis will be deleted in the next update.")]
        /// <summary>
        ///     Set current account public
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<InstaUserShort>> SetAccountPublicAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _accountProcessor.SetAccountPublicAsync();
        }


        /// <summary>
        ///     Comment media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="text">Comment text</param>
        /// <returns></returns>
        public async Task<IResult<InstaComment>> CommentMediaAsync(string mediaId, string text)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _commentProcessor.CommentMediaAsync(mediaId, text);
        }

        /// <summary>
        ///     Inline comment media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="text">Comment text</param>
        /// <returns></returns>
        public async Task<IResult<InstaComment>> InlineCommentMediaAsync(string mediaId, string targetCommentId, string text)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _commentProcessor.InlineCommentMediaAsync(mediaId, targetCommentId, text);
        }

        /// <summary>
        ///     Delete comment from media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="commentId">Comment id</param>
        /// <returns></returns>
        public async Task<IResult<bool>> DeleteCommentAsync(string mediaId, string commentId)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _commentProcessor.DeleteCommentAsync(mediaId, commentId);
        }
        /// <summary>
        ///     Upload video
        /// </summary>
        /// <param name="video">Video to upload</param>
        /// <param name="imageThumbnail">Image thumbnail</param>
        /// <param name="caption">Caption</param>
        /// <returns></returns>
        public async Task<IResult<InstaMedia>> UploadVideoAsync(InstaVideo video, InstaImage imageThumbnail, string caption)
        {
            ValidateUser();
            ValidateLoggedIn();

            return await _mediaProcessor.UploadVideoAsync(video, imageThumbnail, caption);
        }
        /// <summary>
        ///     Upload photo
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="caption">Caption</param>
        /// <returns></returns>
        public async Task<IResult<InstaMedia>> UploadPhotoAsync(InstaImage image, string caption)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _mediaProcessor.UploadPhotoAsync(image, caption);
        }

        /// <summary>
        ///     Upload photo
        /// </summary>
        /// <param name="images">Array of photos to upload</param>
        /// <param name="caption">Caption</param>
        /// <returns></returns>
        public async Task<IResult<InstaMedia>> UploadPhotosAlbumAsync(InstaImage[] images, string caption)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _mediaProcessor.UploadPhotosAlbumAsync(images, caption);
        }

        /// <summary>
        ///     Configure photo
        /// </summary>
        /// <param name="image">Photo to configure</param>
        /// <param name="uploadId">Upload id</param>
        /// <param name="caption">Caption</param>
        /// <returns></returns>
        //public async Task<IResult<InstaMedia>> ConfigurePhotoAsync(InstaImage image, string uploadId, string caption)
        //{
        //    ValidateUser();
        //    ValidateLoggedIn();
        //    return await _mediaProcessor.ConfigurePhotoAsync(image, uploadId, caption);
        //}

        /// <summary>
        ///     Configure photos for Album
        /// </summary>
        /// <param name="uploadIds">Array of upload IDs to configure</param>
        /// ///
        /// <param name="caption">Caption</param>
        /// <returns></returns>
        //public async Task<IResult<InstaMedia>> ConfigureAlbumAsync(string[] uploadIds, string caption)
        //{
        //    ValidateUser();
        //    ValidateLoggedIn();
        //    return await _mediaProcessor.ConfigureAlbumAsync(uploadIds, caption);
        //}


        /// <summary>
        ///     Get user story feed (stories from users followed by current user).
        /// </summary>
        /// <returns></returns>
        public async Task<IResult<InstaStoryFeed>> GetStoryFeedAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _storyProcessor.GetStoryFeedAsync();
        }

        /// <summary>
        ///     Get the story by userId
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public async Task<IResult<InstaStory>> GetUserStoryAsync(long userId)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _storyProcessor.GetUserStoryAsync(userId);
        }

        /// <summary>
        ///     Upload story photo
        /// </summary>
        /// <param name="image">Photo to upload</param>
        /// <param name="caption">Caption</param>
        /// <returns></returns>
        public async Task<IResult<InstaStoryMedia>> UploadStoryPhotoAsync(InstaImage image, string caption)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _storyProcessor.UploadStoryPhotoAsync(image, caption);
        }

        /// <summary>
        ///     Configure story photo
        /// </summary>
        /// <param name="image">Photo to configure</param>
        /// <param name="uploadId">Upload id</param>
        /// <param name="caption">Caption</param>
        /// <returns></returns>
        //public async Task<IResult<InstaStoryMedia>> ConfigureStoryPhotoAsync(InstaImage image, string uploadId,
        //    string caption)
        //{
        //    ValidateUser();
        //    ValidateLoggedIn();
        //    return await _storyProcessor.ConfigureStoryPhotoAsync(image, uploadId, caption);
        //}
        [Obsolete("ChangePasswordAsync is deprecated, please use AccountProcessor.ChangePasswordAsync instead.\r\nThis will be deleted in the next update.")]
        /// <summary>
        ///     Change password
        /// </summary>
        /// <param name="oldPassword">The old password</param>
        /// <param name="newPassword">
        ///     The new password (shouldn't be the same old password, and should be a password you never used
        ///     here)
        /// </param>
        /// <returns>
        ///     Return true if the password is changed
        /// </returns>
        public async Task<IResult<bool>> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _accountProcessor.ChangePasswordAsync(oldPassword, newPassword);
        }

        /// <summary>
        ///     Delete a media (photo or video)
        /// </summary>
        /// <param name="mediaId">The media ID</param>
        /// <param name="mediaType">The type of the media</param>
        /// <returns>
        ///     Return true if the media is deleted
        /// </returns>
        public async Task<IResult<bool>> DeleteMediaAsync(string mediaId, InstaMediaType mediaType)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _mediaProcessor.DeleteMediaAsync(mediaId, mediaType);
        }

        /// <summary>
        ///     Edit the caption of the media (photo/video)
        /// </summary>
        /// <param name="mediaId">The media ID</param>
        /// <param name="caption">The new caption</param>
        /// <returns>
        ///     Return true if everything is ok
        /// </returns>
        public async Task<IResult<bool>> EditMediaAsync(string mediaId, string caption)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _mediaProcessor.EditMediaAsync(mediaId, caption);
        }

        /// <summary>
        ///     Get feed of media your liked.
        /// </summary>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaMediaList" />
        /// </returns>
        public async Task<IResult<InstaMediaList>> GetLikeFeedAsync(PaginationParameters paginationParameters)
        {
            ValidateUser();
            return await _feedProcessor.GetLikeFeedAsync(paginationParameters);
        }

        /// <summary>
        ///     Get friendship status for given user id.
        /// </summary>
        /// <param name="userId">User identifier (PK)</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaFriendshipStatus" />
        /// </returns>
        public async Task<IResult<InstaFriendshipStatus>> GetFriendshipStatusAsync(long userId)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _userProcessor.GetFriendshipStatusAsync(userId);
        }

        /// <summary>
        ///     Get your collection for given collection id
        /// </summary>
        /// <param name="collectionId">Collection ID</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaCollectionItem" />
        /// </returns>
        public async Task<IResult<InstaCollectionItem>> GetCollectionAsync(long collectionId)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _collectionProcessor.GetCollectionAsync(collectionId);
        }


        /// <summary>
        ///     Get your collections
        /// </summary>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaCollections" />
        /// </returns>
        public async Task<IResult<InstaCollections>> GetCollectionsAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _collectionProcessor.GetCollectionsAsync();
        }

        /// <summary>
        ///     Create a new collection
        /// </summary>
        /// <param name="collectionName">The name of the new collection</param>
        /// <returns>
        ///     <see cref="T:InstagramApiSharp.Classes.Models.InstaCollectionItem" />
        /// </returns>
        public async Task<IResult<InstaCollectionItem>> CreateCollectionAsync(string collectionName)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _collectionProcessor.CreateCollectionAsync(collectionName);
        }

        public async Task<IResult<InstaCollectionItem>> AddItemsToCollectionAsync(long collectionId,
            params string[] mediaIds)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _collectionProcessor.AddItemsToCollectionAsync(collectionId, mediaIds);
        }

        /// <summary>
        ///     Delete your collection for given collection id
        /// </summary>
        /// <param name="collectionId">Collection ID to delete</param>
        /// <returns>true if succeed</returns>
        public async Task<IResult<bool>> DeleteCollectionAsync(long collectionId)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _collectionProcessor.DeleteCollectionAsync(collectionId);
        }

        /// <summary>
        ///     Get media ID from an url (got from "share link")
        /// </summary>
        /// <param name="uri">Uri to get media ID</param>
        /// <returns>Media ID</returns>
        public async Task<IResult<string>> GetMediaIdFromUrlAsync(Uri uri)
        {
            ValidateLoggedIn();
            ValidateRequestMessage();
            return await _mediaProcessor.GetMediaIdFromUrlAsync(uri);
        }

        /// <summary>
        ///     Get share link from media Id
        /// </summary>
        /// <param name="mediaId">media ID</param>
        /// <returns>Share link as Uri</returns>
        public async Task<IResult<Uri>> GetShareLinkFromMediaIdAsync(string mediaId)
        {
            return await _mediaProcessor.GetShareLinkFromMediaIdAsync(mediaId);
        }

        /// <summary>
        ///     Searches for specific location by provided geo-data or search query.
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="query">Search query</param>
        /// <returns>
        ///     List of locations (short format)
        /// </returns>
        public async Task<IResult<InstaLocationShortList>> SearchLocation(double latitude, double longitude,
            string query)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _locationProcessor.SearchLocationAsync(latitude, longitude, query);
        }

        /// <summary>
        ///     Gets the feed of particular location.
        /// </summary>
        /// <param name="locationId">Location identifier</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     Location feed
        /// </returns>
        public async Task<IResult<InstaLocationFeed>> GetLocationFeed(long locationId,
            PaginationParameters paginationParameters)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _locationProcessor.GetLocationFeedAsync(locationId, paginationParameters);
        }

        /// <summary>
        ///     Searches for specific hashtag by search query.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <param name="excludeList">Array of numerical hashtag IDs (ie "17841562498105353") to exclude from the response, allowing you to skip tags from a previous call to get more results</param>
        /// <param name="rankToken">The rank token from the previous page's response</param>
        /// <returns>
        ///     List of hashtags
        /// </returns>
        public async Task<IResult<InstaHashtagSearch>> SearchHashtag(string query, IEnumerable<long> excludeList, string rankToken)
        {
            ValidateUser();
            ValidateLoggedIn();
            return await _hashtagProcessor.SearchHashtagAsync(query, excludeList, rankToken);
        }

        /// <summary>
        ///     Gets the hashtag information by user tagname.
        /// </summary>
        /// <param name="tagname">Tagname</param>
        /// <returns>Hashtag information</returns>
        public async Task<IResult<InstaHashtag>> GetHashtagInfo(string tagname)
        {
            //ValidateUser();
            //ValidateLoggedIn();
            UserAuthValidator.Validate(_user, IsUserAuthenticated);
            return await _hashtagProcessor.GetHashtagInfoAsync(tagname);
        }


        #region Authentication/State data
        private bool _isUserAuthenticated;
        /// <summary>
        ///     Indicates whether user authenticated or not
        /// </summary>
        public bool IsUserAuthenticated
        {
            get { return _isUserAuthenticated; }
            internal set { _isUserAuthenticated = value; _userAuthValidate.IsUserAuthenticated = value; }
        }
        /// <summary>
        ///     Create a new instagram account
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="email">Email</param>
        /// <param name="firstName">First name (optional)</param>
        /// <returns></returns>
        public async Task<IResult<CreationResponse>> CreateNewAccount(string username, string password, string email, string firstName)
        {
            CreationResponse createResponse = new CreationResponse();
            try
            {
                var postData = new Dictionary<string, string>
                {
                    {"email",     email },
                    {"username",    username},
                    {"password",    password},
                    {"device_id",   ApiRequestMessage.GenerateDeviceId()},
                    {"guid",        _deviceInfo.DeviceGuid.ToString()},
                    {"first_name",  firstName}
                };

                var instaUri = UriCreator.GetCreateAccountUri();
                var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, postData);
                var response = await _httpRequestProcessor.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                return Result.Success(JsonConvert.DeserializeObject<CreationResponse>(result));
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<CreationResponse>(exception);
            }
        }

        /// <summary>
        ///     Share an user
        /// </summary>
        /// <param name="userIdToSend">User id(PK)</param>
        /// <param name="threadId">Thread id</param>
        /// <returns></returns>
        public async Task<IResult<InstaSharing>> ShareUserAsync(string userIdToSend, string threadId)
        {
            try
            {
                var instaUri = new Uri(InstaApiConstants.BASE_INSTAGRAM_API_URL + "direct_v2/threads/broadcast/profile/");
                var uploadId = ApiRequestMessage.GenerateUploadId();
                var requestContent = new MultipartFormDataContent(uploadId)
                {
                    {new StringContent(userIdToSend), "\"profile_user_id\""},
                    {new StringContent("1"), "\"unified_broadcast_format\""},
                    {new StringContent("send_item"), "\"action\""},
                    {new StringContent($"[{threadId}]"), "\"thread_ids\""},
                    {new StringContent(_deviceInfo.DeviceGuid.ToString()), "\"_uuid\""},
                    {new StringContent(_user.LoggedInUser.Pk.ToString()), "\"_uid\""},
                    {new StringContent(_user.CsrfToken), "\"_csrftoken\""}

                };
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo);
                request.Content = requestContent;
                request.Headers.Add("Host", "i.instagram.com");
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.Fail("Status code: " + response.StatusCode, (InstaSharing)null);
                var obj = JsonConvert.DeserializeObject<InstaSharing>(json);

                return Result.Success(obj);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                _logger?.LogException(exception);
                return Result.Fail<InstaSharing>(exception);
            }
        }

        /// <summary>
        ///     Login using given credentials asynchronously
        /// </summary>
        /// <param name="isNewLogin"></param>
        /// <returns>
        ///     Success --> is succeed
        ///     TwoFactorRequired --> requires 2FA login.
        ///     BadPassword --> Password is wrong
        ///     InvalidUser --> User/phone number is wrong
        ///     Exception --> Something wrong happened
        /// </returns>
        public async Task<IResult<InstaLoginResult>> LoginAsync(bool isNewLogin = true)
        {
            ValidateUser();
            ValidateRequestMessage();
            try
            {
                if (isNewLogin)
                {
                    var firstResponse = await _httpRequestProcessor.GetAsync(_httpRequestProcessor.Client.BaseAddress);
                    var html = await firstResponse.Content.ReadAsStringAsync();
                    Debug.WriteLine(html);
                    _logger?.LogResponse(firstResponse);
                }
                var cookies =
                    _httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(_httpRequestProcessor.Client
                        .BaseAddress);
              
                var csrftoken = cookies[InstaApiConstants.CSRFTOKEN]?.Value ?? String.Empty;
                _user.CsrfToken = csrftoken;
                Debug.WriteLine("login token: " + csrftoken);
                var instaUri = UriCreator.GetLoginUri();
                var signature =
                    $"{_httpRequestProcessor.RequestMessage.GenerateSignature(InstaApiConstants.IG_SIGNATURE_KEY, out string devid)}.{_httpRequestProcessor.RequestMessage.GetMessageString()}";
                _deviceInfo.DeviceId = devid;
                var fields = new Dictionary<string, string>
                {
                    {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
                    {InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION}
                };
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo);
                request.Content = new FormUrlEncodedContent(fields);
                request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);
                request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION, InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK) //If the password is correct BUT 2-Factor Authentication is enabled, it will still get a 400 error (bad request)
                {
                    //Then check it
                    var loginFailReason = JsonConvert.DeserializeObject<InstaLoginBaseResponse>(json);

                    if (loginFailReason.InvalidCredentials)
                        return Result.Fail("Invalid Credentials",
                            loginFailReason.ErrorType == "bad_password"
                                ? InstaLoginResult.BadPassword
                                : InstaLoginResult.InvalidUser);
                    if (loginFailReason.TwoFactorRequired)
                    {
                        _twoFactorInfo = loginFailReason.TwoFactorLoginInfo;
                        //2FA is required!
                        return Result.Fail("Two Factor Authentication is required", InstaLoginResult.TwoFactorRequired);
                    }
                    if (loginFailReason.ErrorType == "checkpoint_challenge_required")
                    {
                        _challengeinfo = loginFailReason.Challenge;

                        return Result.Fail("Challenge is required", InstaLoginResult.ChallengeRequired);
                    }
                    if (loginFailReason.ErrorType == "rate_limit_error")
                    {
                        return Result.Fail("Please wait a few minutes before you try again.", InstaLoginResult.LimitError);
                    }
                    return Result.UnExpectedResponse<InstaLoginResult>(response, json);
                }
                var loginInfo = JsonConvert.DeserializeObject<InstaLoginResponse>(json);
                IsUserAuthenticated = loginInfo.User?.UserName.ToLower() == _user.UserName.ToLower();
                var converter = ConvertersFabric.Instance.GetUserShortConverter(loginInfo.User);
                _user.LoggedInUser = converter.Convert();
                _user.RankToken = $"{_user.LoggedInUser.Pk}_{_httpRequestProcessor.RequestMessage.phone_id}";
                if(string.IsNullOrEmpty(_user.CsrfToken))
                {
                    cookies =
                      _httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(_httpRequestProcessor.Client
                          .BaseAddress);
                    _user.CsrfToken = cookies[InstaApiConstants.CSRFTOKEN]?.Value ?? String.Empty;
                }
                return Result.Success(InstaLoginResult.Success);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Result.Fail(exception, InstaLoginResult.Exception);
            }
            finally
            {
                InvalidateProcessors();
            }
        }

        /// <summary>
        ///     2-Factor Authentication Login using a verification code
        ///     Before call this method, please run LoginAsync first.
        /// </summary>
        /// <param name="verificationCode">Verification Code sent to your phone number</param>
        /// <returns>
        ///     Success --> is succeed
        ///     InvalidCode --> The code is invalid
        ///     CodeExpired --> The code is expired, please request a new one.
        ///     Exception --> Something wrong happened
        /// </returns>
        public async Task<IResult<InstaLoginTwoFactorResult>> TwoFactorLoginAsync(string verificationCode)
        {
            if (_twoFactorInfo == null)
                return Result.Fail<InstaLoginTwoFactorResult>("Run LoginAsync first");

            try
            {
                var twoFactorRequestMessage = new ApiTwoFactorRequestMessage(verificationCode,
                    _httpRequestProcessor.RequestMessage.username,
                    _httpRequestProcessor.RequestMessage.device_id,
                    _twoFactorInfo.TwoFactorIdentifier);

                var instaUri = UriCreator.GetTwoFactorLoginUri();
                var signature =
                    $"{twoFactorRequestMessage.GenerateSignature(InstaApiConstants.IG_SIGNATURE_KEY)}.{twoFactorRequestMessage.GetMessageString()}";
                var fields = new Dictionary<string, string>
                {
                    {InstaApiConstants.HEADER_IG_SIGNATURE, signature},
                    {
                        InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                        InstaApiConstants.IG_SIGNATURE_KEY_VERSION
                    }
                };
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo);
                request.Content = new FormUrlEncodedContent(fields);
                request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE, signature);
                request.Properties.Add(InstaApiConstants.HEADER_IG_SIGNATURE_KEY_VERSION,
                    InstaApiConstants.IG_SIGNATURE_KEY_VERSION);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var loginInfo =
                        JsonConvert.DeserializeObject<InstaLoginResponse>(json);
                    IsUserAuthenticated = IsUserAuthenticated =
                        loginInfo.User != null && loginInfo.User.UserName.ToLower() == _user.UserName.ToLower();
                    var converter = ConvertersFabric.Instance.GetUserShortConverter(loginInfo.User);
                    _user.LoggedInUser = converter.Convert();
                    _user.RankToken = $"{_user.LoggedInUser.Pk}_{_httpRequestProcessor.RequestMessage.phone_id}";

                    return Result.Success(InstaLoginTwoFactorResult.Success);
                }

                var loginFailReason = JsonConvert.DeserializeObject<InstaLoginTwoFactorBaseResponse>(json);

                if (loginFailReason.ErrorType == "sms_code_validation_code_invalid")
                    return Result.Fail("Please check the security code.", InstaLoginTwoFactorResult.InvalidCode);
                return Result.Fail("This code is no longer valid, please, call LoginAsync again to request a new one",
                    InstaLoginTwoFactorResult.CodeExpired);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Result.Fail(exception, InstaLoginTwoFactorResult.Exception);
            }
        }

        /// <summary>
        ///     Get Two Factor Authentication details
        /// </summary>
        /// <returns>
        ///     An instance of TwoFactorInfo if success.
        ///     A null reference if not success; in this case, do LoginAsync first and check if Two Factor Authentication is
        ///     required, if not, don't run this method
        /// </returns>
        public async Task<IResult<TwoFactorLoginInfo>> GetTwoFactorInfoAsync()
        {
            return await Task.Run(() =>
                _twoFactorInfo != null
                    ? Result.Success(_twoFactorInfo)
                    : Result.Fail<TwoFactorLoginInfo>("No Two Factor info available."));
        }

        /// <summary>
        ///     Logout from instagram asynchronously
        /// </summary>
        /// <returns>
        ///     True if logged out without errors
        /// </returns>
        public async Task<IResult<bool>> LogoutAsync()
        {
            ValidateUser();
            ValidateLoggedIn();
            try
            {
                var instaUri = UriCreator.GetLogoutUri();
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK) return Result.UnExpectedResponse<bool>(response, json);
                var logoutInfo = JsonConvert.DeserializeObject<BaseStatusResponse>(json);
                if (logoutInfo.Status == "ok")
                    IsUserAuthenticated = false;
                return Result.Success(!IsUserAuthenticated);
            }
            catch (Exception exception)
            {
                LogException(exception);
                return Result.Fail(exception, false);
            }
        }
        ///// <summary>
        /////     Get Challenge information
        ///// </summary>
        ///// <returns></returns>
        //public InstaChallengeLoginInfo GetChallenge()
        //{
        //    return _challengeinfo;
        //}
        string _challengeGuid, _challengeDeviceId;
        public async Task<IResult<ChallengeRequireVerifyMethod>> GetChallengeRequireVerifyMethodAsync()
        {
            if (_challengeinfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", (ChallengeRequireVerifyMethod)null);

            try
            {
                _challengeGuid = Guid.NewGuid().ToString();
                _challengeDeviceId = ApiRequestMessage.GenerateDeviceId();
                var instaUri = UriCreator.GetChallengeRequireFirstUri(_challengeinfo.ApiPath, _challengeGuid, _challengeDeviceId);
                var request = HttpHelper.GetDefaultRequest(HttpMethod.Get, instaUri, _deviceInfo);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "";
                    try
                    {
                        var j = JsonConvert.DeserializeObject<ChallengeRequireVerifyMethod>(json);
                        msg = j.Message;
                    }
                    catch { }
                    return Result.Fail(msg +"\t\tStatus code: " + response.StatusCode+"\r\n"+ json, (ChallengeRequireVerifyMethod)null);
                }
                else
                {
                    var obj = JsonConvert.DeserializeObject<ChallengeRequireVerifyMethod>(json);
                    return Result.Success(obj);
                }
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (ChallengeRequireVerifyMethod)null);
            }
        }

        public async Task<IResult<ChallengeRequireVerifyMethod>> ResetChallengeRequireVerifyMethodAsync()
        {
            if (_challengeinfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", (ChallengeRequireVerifyMethod)null);

            try
            {
                _challengeGuid = Guid.NewGuid().ToString();
                _challengeDeviceId = ApiRequestMessage.GenerateDeviceId();
                var instaUri = UriCreator.GetResetChallengeRequireUri(_challengeinfo.ApiPath);
                var data = new JObject
                {
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", _challengeGuid},
                    {"device_id", _challengeDeviceId},
                };
                var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "";
                    try
                    {
                        var j = JsonConvert.DeserializeObject<ChallengeRequireVerifyMethod>(json);
                        msg = j.Message;
                    }
                    catch { }
                    return Result.Fail(msg + "\t\tStatus code: " + response.StatusCode + "\r\n" + json, (ChallengeRequireVerifyMethod)null);
                }
                else
                {
                    var obj = JsonConvert.DeserializeObject<ChallengeRequireVerifyMethod>(json);
                    return Result.Success(obj);
                }
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (ChallengeRequireVerifyMethod)null);
            }
        }

        public async Task<IResult<ChallengeRequireSMSVerify>> RequestVerifyCodeToSMSForChallengeRequireAsync()
        {
            if (_challengeinfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", (ChallengeRequireSMSVerify)null);

            try
            {
                var instaUri = UriCreator.GetChallengeRequireUri(_challengeinfo.ApiPath);
                if (string.IsNullOrEmpty(_challengeGuid))
                    _challengeGuid = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(_challengeDeviceId))
                    _challengeDeviceId = ApiRequestMessage.GenerateDeviceId();
                var data = new JObject
                {
                    {"choice", "0"},
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", _challengeGuid},
                    {"device_id", _challengeDeviceId},
                };
                var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Add("Host", "i.instagram.com");
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "";
                    try
                    {
                        var j = JsonConvert.DeserializeObject<ChallengeRequireSMSVerify>(json);
                        msg = j.Message;
                    }
                    catch { }
                    return Result.Fail(msg, (ChallengeRequireSMSVerify)null);
                }
                else
                {
                    var obj = JsonConvert.DeserializeObject<ChallengeRequireSMSVerify>(json);
                    return Result.Success(obj);
                }
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (ChallengeRequireSMSVerify)null);
            }
        }

        public async Task<IResult<ChallengeRequireEmailVerify>> RequestVerifyCodeToEmailForChallengeRequireAsync()
        {
            if (_challengeinfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", (ChallengeRequireEmailVerify)null);

            try
            {
                var instaUri = UriCreator.GetChallengeRequireUri(_challengeinfo.ApiPath);
                if (string.IsNullOrEmpty(_challengeGuid))
                    _challengeGuid = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(_challengeDeviceId))
                    _challengeDeviceId = ApiRequestMessage.GenerateDeviceId();
                var data = new JObject
                {
                    {"choice", "1"},
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", _challengeGuid},
                    {"device_id", _challengeDeviceId},
                };
                var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Add("Host", "i.instagram.com");
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "";
                    try
                    {
                        var j = JsonConvert.DeserializeObject<ChallengeRequireEmailVerify>(json);
                        msg = j.Message;
                    }
                    catch { }
                    return Result.Fail(msg, (ChallengeRequireEmailVerify)null);
                }
                else
                {
                    var obj = JsonConvert.DeserializeObject<ChallengeRequireEmailVerify>(json);
                    return Result.Success(obj);
                }
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (ChallengeRequireEmailVerify)null);
            }
        }

        public async Task<IResult<ChallengeRequireVerifyCode>> VerifyCodeForChallengeRequireAsync(string verifyCode)
        {
            if(verifyCode.Length != 6)
                return Result.Fail("Verify code must be an 6 digit number.", (ChallengeRequireVerifyCode)null);

            if (_challengeinfo == null)
                return Result.Fail("challenge require info is empty.\r\ntry to call LoginAsync function first.", (ChallengeRequireVerifyCode)null);

            try
            {
                var cookies =
            _httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(_httpRequestProcessor.Client
                .BaseAddress);
                var csrftoken = cookies[InstaApiConstants.CSRFTOKEN]?.Value ?? String.Empty;
                _user.CsrfToken = csrftoken;
                Debug.WriteLine("verify token: " + csrftoken);

                var instaUri = UriCreator.GetChallengeRequireUri(_challengeinfo.ApiPath);
                if (string.IsNullOrEmpty(_challengeGuid))
                    _challengeGuid = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(_challengeDeviceId))
                    _challengeDeviceId = ApiRequestMessage.GenerateDeviceId();
                var data = new JObject
                {
                    {"security_code", verifyCode},
                    {"_csrftoken", _user.CsrfToken},
                    {"guid", _challengeGuid},
                    {"device_id", _challengeDeviceId},
                };
                var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                request.Headers.Add("Host", "i.instagram.com");
                var response = await _httpRequestProcessor.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var msg = "";
                    try
                    {
                        var j = JsonConvert.DeserializeObject<ChallengeRequireVerifyCode>(json);
                        msg = j.Message;
                    }
                    catch { }
                    return Result.Fail(msg, (ChallengeRequireVerifyCode)null);
                }
                else
                {
                    var obj = JsonConvert.DeserializeObject<ChallengeRequireVerifyCode>(json);
                    if (obj != null)
                    {
                        if (obj.LoggedInUser != null)
                        {
                            ValidateChallengeAsync(obj.LoggedInUser, csrftoken);
                            await Task.Delay(1500);
                            await GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
                            await _feedProcessor.GetRecentActivityFeedAsync(PaginationParameters.MaxPagesToLoad(1));
                        }
                        else
                        {

                        }
                    }
                    

                    return Result.Success(obj);
                }
            }
            catch (Exception ex)
            {
                return Result.Fail(ex, (ChallengeRequireVerifyCode)null);
            }
        }


        private void ValidateChallengeAsync(InstaUserShortResponse user, string csrfToken)
        {
            try
            {
                var converter = ConvertersFabric.Instance.GetUserShortConverter(user);
                _user.LoggedInUser = converter.Convert();
                _user.RankToken = $"{_user.LoggedInUser.Pk}_{_httpRequestProcessor.RequestMessage.phone_id}";
                _user.CsrfToken = csrfToken;
                IsUserAuthenticated = true;
                InvalidateProcessors();
            }
            catch { }
        }
        /// <summary>
        ///     Set cookie and html document to verify login information.
        /// </summary>
        /// <param name="htmlDocument">Html document source</param>
        /// <param name="cookies">Cookies from webview or webbrowser control</param>
        /// <returns>True if logged in, False if not</returns>
        public async Task<IResult<bool>> SetCookiesAndHtmlForFbLoginAndChallenge(string htmlDocument, string cookie, bool facebookLogin = false)
        {
            if (!string.IsNullOrEmpty(cookie) && !string.IsNullOrEmpty(htmlDocument))
            {
                try
                {
                    var start = "<script type=\"text/javascript\">window._sharedData";
                    var end = ";</script>";

                    var str = htmlDocument.Substring(htmlDocument.IndexOf(start) + start.Length);
                    str = str.Substring(0, str.IndexOf(end));
                    str = str.Substring(str.IndexOf("=") + 2);
                    var o = JsonConvert.DeserializeObject<WebBrowserResponse>(str);
                    return await SetCookiesAndHtmlForFbLoginAndChallenge(o, cookie, facebookLogin);
                }
                catch (Exception ex)
                {
                    return Result.Fail(ex.Message, false);
                }
            }
            return Result.Fail("", false);
        }
        /// <summary>
        ///     Set cookie and web browser response object to verify login information.
        /// </summary>
        /// <param name="webBrowserResponse">Web browser response object</param>
        /// <param name="cookies">Cookies from webview or webbrowser control</param>
        /// <returns>True if logged in, False if not</returns>
        public async Task<IResult<bool>> SetCookiesAndHtmlForFbLoginAndChallenge(WebBrowserResponse webBrowserResponse, string cookie, bool facebookLogin = false)
        {
            if(webBrowserResponse == null)
                return Result.Fail("", false);
            if(webBrowserResponse.Config == null)
                return Result.Fail("", false);
            if(webBrowserResponse.Config.Viewer == null)
                return Result.Fail("", false);

            if (!string.IsNullOrEmpty(cookie))
            {
                try
                {
                    var uri = new Uri(InstaApiConstants.INSTAGRAM_URL);
                    //if (cookie.Contains("urlgen"))
                    //{
                    //    var removeStart = "urlgen=";
                    //    var removeEnd = ";";
                    //    var t = cookie.Substring(cookie.IndexOf(removeStart) + 0);
                    //    t = t.Substring(0, t.IndexOf(removeEnd) + 2);
                    //    cookie = cookie.Replace(t, "");
                    //}
                    cookie = cookie.Replace(';', ',');
                    _httpRequestProcessor.HttpHandler.CookieContainer.SetCookies(uri, cookie);

                    InstaUserShort user = new InstaUserShort
                    {
                        Pk = long.Parse(webBrowserResponse.Config.Viewer.Id),
                        UserName = _user.UserName,
                        ProfilePictureId = "unknown",
                        FullName = webBrowserResponse.Config.Viewer.FullName,
                        ProfilePicture = webBrowserResponse.Config.Viewer.ProfilePicUrl
                    };
                    _user.LoggedInUser = user;
                    _user.CsrfToken = webBrowserResponse.Config.CsrfToken;
                    _user.RankToken = $"{webBrowserResponse.Config.Viewer.Id}_{_httpRequestProcessor.RequestMessage.phone_id}";
                    IsUserAuthenticated = true;
                    if (facebookLogin)
                    {
                        try
                        {
                            var instaUri = UriCreator.GetFacebookSignUpUri();
                            var data = new JObject
                            {
                                {"dryrun", "true"},
                                {"phone_id", _deviceInfo.DeviceGuid.ToString()},
                                {"_csrftoken", _user.CsrfToken},
                                {"adid", Guid.NewGuid().ToString()},
                                {"guid", Guid.NewGuid().ToString()},
                                {"device_id", ApiRequestMessage.GenerateDeviceId()},
                                {"waterfall_id", Guid.NewGuid().ToString()},
                                {"fb_access_token", InstaApiConstants.FB_ACCESS_TOKEN},
                            };
                            var request = HttpHelper.GetSignedRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                            request.Headers.Add("Host", "i.instagram.com");
                            var response = await _httpRequestProcessor.SendAsync(request);
                            var json = await response.Content.ReadAsStringAsync();
                            var obj = JsonConvert.DeserializeObject<FacebookLoginResponse>(json);
                            _user.FacebookUserId = obj.FbUserId;
                        }
                        catch(Exception)
                        {
                        }
                        InvalidateProcessors();
                    }
                    return Result.Success(true);
                }
                catch (Exception ex)
                {
                    return Result.Fail(ex.Message, false);
                }
            }
            return Result.Fail("", false);
        }
        /// <summary>
        ///     Get current state info as Memory stream
        /// </summary>
        /// <returns>
        ///     State data
        /// </returns>
        public Stream GetStateDataAsStream()
        {

            var Cookies = _httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(new Uri(InstaApiConstants.INSTAGRAM_URL));
            var RawCookiesList = new List<Cookie>();
            foreach (Cookie cookie in Cookies)
            {
                RawCookiesList.Add(cookie);
            }


            var state = new StateData
            {
                DeviceInfo = _deviceInfo,
                IsAuthenticated = IsUserAuthenticated,
                UserSession = _user,
                Cookies = _httpRequestProcessor.HttpHandler.CookieContainer,
                RawCookies = RawCookiesList
            };
            return SerializationHelper.SerializeToStream(state);
        }
        /// <summary>
        ///     Get current state info as Memory stream
        /// </summary>
        /// <returns>
        ///     State data
        /// </returns>
        public string GetStateDataAsString()
        {

            var Cookies = _httpRequestProcessor.HttpHandler.CookieContainer.GetCookies(new Uri(InstaApiConstants.INSTAGRAM_URL));
            var RawCookiesList = new List<Cookie>();
            foreach (Cookie cookie in Cookies)
            {
                RawCookiesList.Add(cookie);
            }

            var state = new StateData
            {
                DeviceInfo = _deviceInfo,
                IsAuthenticated = IsUserAuthenticated,
                UserSession = _user,
                Cookies = _httpRequestProcessor.HttpHandler.CookieContainer,
                RawCookies = RawCookiesList
            };
            return SerializationHelper.SerializeToString(state);
        }

        /// <summary>
        ///     Loads the state data from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void LoadStateDataFromStream(Stream stream)
        {
            var data = SerializationHelper.DeserializeFromStream<StateData>(stream);
            _deviceInfo = data.DeviceInfo;
            _user = data.UserSession;
            // _httpRequestProcessor.HttpHandler.CookieContainer = data.Cookies;

            //Load Stream Edit 
            _httpRequestProcessor.RequestMessage.username = data.UserSession.UserName;
            _httpRequestProcessor.RequestMessage.password = data.UserSession.Password;
            // _httpRequestProcessor.HttpHandler.CookieContainer = data.Cookies;
            _httpRequestProcessor.RequestMessage.device_id = data.DeviceInfo.DeviceId;
            _httpRequestProcessor.RequestMessage.phone_id = data.DeviceInfo.PhoneGuid.ToString();
            _httpRequestProcessor.RequestMessage.guid = data.DeviceInfo.DeviceGuid;

            foreach (Cookie cookie in data.RawCookies)
            {
                _httpRequestProcessor.HttpHandler.CookieContainer.Add(new Uri(InstaApiConstants.INSTAGRAM_URL), cookie);
            }

            //_httpRequestProcessor.HttpHandler.CookieContainer.SetCookies(new Uri(InstaApiConstants.INSTAGRAM_URL),data.RawCookies);



            IsUserAuthenticated = data.IsAuthenticated;
            InvalidateProcessors();
        }
        public void LoadStateDataFromString(string str)
        {
            var data = SerializationHelper.DeserializeFromString<StateData>(str);
            _deviceInfo = data.DeviceInfo;
            _user = data.UserSession;
            // _httpRequestProcessor.HttpHandler.CookieContainer = data.Cookies;

            //Load Stream Edit 
            _httpRequestProcessor.RequestMessage.username = data.UserSession.UserName;
            _httpRequestProcessor.RequestMessage.password = data.UserSession.Password;
            // _httpRequestProcessor.HttpHandler.CookieContainer = data.Cookies;
            _httpRequestProcessor.RequestMessage.device_id = data.DeviceInfo.DeviceId;
            _httpRequestProcessor.RequestMessage.phone_id = data.DeviceInfo.PhoneGuid.ToString();
            _httpRequestProcessor.RequestMessage.guid = data.DeviceInfo.DeviceGuid;

            foreach (Cookie cookie in data.RawCookies)
            {
                _httpRequestProcessor.HttpHandler.CookieContainer.Add(new Uri(InstaApiConstants.INSTAGRAM_URL), cookie);
            }

            //_httpRequestProcessor.HttpHandler.CookieContainer.SetCookies(new Uri(InstaApiConstants.INSTAGRAM_URL),data.RawCookies);



            IsUserAuthenticated = data.IsAuthenticated;
            InvalidateProcessors();
        }
        #endregion


        #region private part

        private void InvalidateProcessors()
        {
            _hashtagProcessor = new HashtagProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _locationProcessor = new LocationProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _collectionProcessor = new CollectionProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _mediaProcessor = new MediaProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _userProcessor = new UserProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _storyProcessor = new StoryProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _commentProcessor = new CommentProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _messagingProcessor = new MessagingProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _feedProcessor = new FeedProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);

            _liveProcessor = new LiveProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _discoverProcessor = new DiscoverProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);
            _accountProcessor = new AccountProcessor(_deviceInfo, _user, _httpRequestProcessor, _logger, _userAuthValidate);

        }
        internal void ValidateUserAndLogin()
        {
            ValidateUser();
            ValidateLoggedIn();
        }
        private void ValidateUser()
        {
            if (string.IsNullOrEmpty(_user.UserName) || string.IsNullOrEmpty(_user.Password))
                throw new ArgumentException("user name and password must be specified");
        }

        private void ValidateLoggedIn()
        {
            if (!IsUserAuthenticated)
                throw new ArgumentException("user must be authenticated");
        }

        private void ValidateRequestMessage()
        {
            if (_httpRequestProcessor.RequestMessage == null || _httpRequestProcessor.RequestMessage.IsEmpty())
                throw new ArgumentException("API request message null or empty");
        }

        private void LogException(Exception exception)
        {
            _logger?.LogException(exception);
        }

        #endregion
    }
}