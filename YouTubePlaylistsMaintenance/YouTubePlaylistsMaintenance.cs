using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;

namespace YouTubePlaylistsMaintenance
{
    public partial class FormYouTubePlaylistSave : Form
    {

        List<MissingVideo> myMissingVideos = new List<MissingVideo>();

        public FormYouTubePlaylistSave()
        {
            InitializeComponent();
        }

        public void ExitForm()
        {
            Close();
            Application.ExitThread();
            Environment.Exit(0);
        }

        public void GetPlaylistData(YouTubePlaylist myPlaylist)
        {
            YouTubeVideo[] videos = YouTubeAPI.GetPlaylist(myPlaylist.id);                                 // lists all titles of videos of a playlist (via each video ID)
            string missingVideoTitle;

            for (int i = 0; i < videos.Length; i++)
            {
                if (videos[i].title == null)
                {
                    missingVideoTitle = ExcelHandling.SearchMissingVideo(i + 1, myPlaylist.title, videos.Length);
                    MissingVideo missingVideo = new MissingVideo((i + 1).ToString() + ": " + missingVideoTitle, myPlaylist.title);
                    myMissingVideos.Add(missingVideo);
                }
            }

            labelCurrentPlaylist.Visible = true;
            labelCurrentPlaylist.Text = "Currently Processing: " + myPlaylist.title;
            ExcelHandling.SaveCurrentPlaylistToSheet(myPlaylist.title, videos);
            labelCurrentPlaylist.Visible = false;
        }

        private void ButtonGetAllplaylists_Click(object sender, EventArgs e)
        {
            string temp;

            labelCurrentPlaylist.Visible = true;
            labelCurrentPlaylist.Text = "Waiting for user's data...";

            if (!ExcelHandling.ObtainExcelFile())
            {
                ExitForm();
            }

            listBoxPlaylists.Items.Clear();

            string channelID = textBoxChannelID.Text;

            YouTubePlaylist[] playLists = YouTubeAPI.GetPlaylists(channelID);

            for (int i = 0 ; i < playLists.Length; i++)
            {
                listBoxPlaylists.Items.Add((i + 1).ToString() + ": " + playLists[i].title);
            }

            progressBarDownloadRate.Maximum = playLists.Length - 1;
            progressBarDownloadRate.Minimum = 0;
            progressBarDownloadRate.Step = 1;
            for (int i = 0; i < playLists.Length; i++)
            {
                temp = listBoxPlaylists.Items[i].ToString() + " - SAVED"; 
                GetPlaylistData(playLists[i]);
                progressBarDownloadRate.Value = i;
                listBoxPlaylists.Items[i] = temp;
            }

            if (myMissingVideos.Count() == 0)
            {
                MessageBox.Show("No missing videos at all playlists");
                ExcelHandling.CloseFile();
            }
            else
            {
                ExcelHandling.SaveMissingVideos(myMissingVideos);
                myMissingVideos.Clear();
                MessageBox.Show("Process finished successfully.\n" +
                                "A list of the Missing videos from all playlists appear at the last worksheet of the file");
            }
        }
    }

    class MissingVideo
    {
        public string VideoTitle { get; set; }
        public string Playlist   { get; set; }

        public MissingVideo(string videoTitle, string playlist)
        {
            Playlist = playlist;
            VideoTitle = videoTitle;
        }
    }

}