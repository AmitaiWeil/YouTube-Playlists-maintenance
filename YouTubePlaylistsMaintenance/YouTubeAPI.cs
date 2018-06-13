using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System.IO;
using System.Threading;

using System.Windows.Forms;


namespace YouTubePlaylistsMaintenance
{
    class YouTubeAPI
    {
        private static YouTubeService ytService = Auth();

        private static YouTubeService Auth()
        {
            UserCredential creds;
            using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))        // storing json information
            {
                creds = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                                                                    new[] { YouTubeService.Scope.YoutubeReadonly },
                                                                    "user",
                                                                    CancellationToken.None,
                                                                    new FileDataStore("YouTubeAPI")
                                                                   ).Result;
            }

            var service = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = creds,
                ApplicationName = "YouTubeAPI"
            });

            return service;
        }

        internal static YouTubeVideo[] GetPlaylistVideos(string playlistId)
        {
            var request = ytService.PlaylistItems.List("contentDetails");
            request.PlaylistId = playlistId;
            LinkedList<YouTubeVideo> videos = new LinkedList<YouTubeVideo>();

            string nextPage = "";
            while (nextPage != null)
            {
                request.PageToken = nextPage;

                var respose = request.Execute();

                foreach (var item in respose.Items)
                {
                    videos.AddLast(new YouTubeVideo(item.ContentDetails.VideoId));

                    //if (item.ContentDetails.)
                    //{

                    //}
                }

                nextPage = respose.NextPageToken;
            }

            return videos.ToArray<YouTubeVideo>();
        }


        public static void GetVideoInfo(YouTubeVideo video)                     // Get title of video by ID
        {
            var videoRequest = ytService.Videos.List("snippet");
            videoRequest.Id = video.id;

            var respose = videoRequest.Execute();
            if (respose.Items.Count > 0)
            {
                video.title = respose.Items[0].Snippet.Title;
 //               video.description = respose.Items[0].Snippet.Description;
            }
            else
            {
                // Video ID not found
            }
        }

        internal static YouTubePlaylist[] GetPlaylists(string channelId)
        {
            try
            {
                var request = ytService.Playlists.List("snippet");                                     // or "contentDetails"?
                request.ChannelId = channelId;                                                         // or .ChannelId?
                LinkedList<YouTubePlaylist> playlists = new LinkedList<YouTubePlaylist>();

                string nextPage = "";
                while (nextPage != null)
                {
                    request.PageToken = nextPage;

                    var respose = request.Execute();

                    foreach (var item in respose.Items)
                    {
                        playlists.AddLast(new YouTubePlaylist(item.Id));
                    }

                    nextPage = respose.NextPageToken;
                }

                return playlists.ToArray<YouTubePlaylist>();
            }
            catch (Exception)
            {
                MessageBox.Show("Error in retrieving channel's data.\nCheck if the channel ID is entered correctly." +
                                "\n\nFinding your channel ID can be done after signing-in \nto your YouTube account " +
                                "and following the path: \n" +
                                "clicking the account icon -> settings -> Advanced -> Account information.");

                return null;
            }
        }

        public static void GetplaylistInfo(YouTubePlaylist playlist)                     // Get title of playlist by ID
        {
            var playlistRequest = ytService.Playlists.List("snippet");                   // "contentDetails"?
            playlistRequest.Id = playlist.id;                                            // or .userID?

            var respose = playlistRequest.Execute();
            if (respose.Items.Count > 0)
            {
                playlist.title = respose.Items[0].Snippet.Title;
                //video.description = respose.Items[0].Snippet.Description;
            }
            else
            {
                // playlist ID not found
                MessageBox.Show("Error in retrieving current playlist's data");
            }
        }

    }

    public class YouTubeVideo
    {
        public string id, title;                    //, description;
        //public DateTime publishedDate;

        public YouTubeVideo(string id)
        {
            this.id = id;
            YouTubeAPI.GetVideoInfo(this);
        }
    }

    public class YouTubePlaylist
    {
        public string id, title;

        public YouTubePlaylist(string id)
        {
            this.id = id;
            YouTubeAPI.GetplaylistInfo(this);
        }
    }

}
