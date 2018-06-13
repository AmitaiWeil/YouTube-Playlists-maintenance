using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using OfficeOpenXml;
using System.IO;

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


        public List<string> GetPlaylistVideos(YouTubePlaylist myPlaylist)
        {
            List<string> titlesList = new List<string>();

            labelCurrentPlaylist.Visible = true;
            labelCurrentPlaylist.Text = "Currently Processing: " + myPlaylist.title;

            YouTubeVideo[] videos = YouTubeAPI.GetPlaylistVideos(myPlaylist.id);               // lists all titles of videos of a playlist (via each video ID)

            for (int i = 0; i < videos.Length; i++)
            {
                titlesList.Add(videos[i].title);
            }

            return titlesList;
        }


        public void ButtonGetAllplaylists_Click(object sender, EventArgs e)
        {
            List<string> videosList = new List<string>();
            bool worksheetExists = false;

            listBoxPlaylists.Items.Clear();
            progressBarDownloadRate.Minimum = 0;
            progressBarDownloadRate.Step = 1;

            string channelID = textBoxChannelID.Text;
            
            if (!ExcelHandling.FileStatus())
            {
                ExitForm();                                                         // File locked (already in use), or excel not installed
            }

            labelCurrentPlaylist.Text = "Waiting for user's data...";
            labelCurrentPlaylist.Visible = true;

            // Get playlists from YouTube, order in ascending order and show in SW list-box
            YouTubePlaylist[] playLists = YouTubeAPI.GetPlaylists(channelID);
            if (playLists == null)
            {
                ExitForm();
            }
            playLists = playLists.OrderBy(c => c.title).ToArray();
            for (int i = 0 ; i < playLists.Length; i++)
            {
                listBoxPlaylists.Items.Add((i + 1).ToString() + ": " + playLists[i].title);
            }
            progressBarDownloadRate.Maximum = playLists.Length - 1;

            // Save all playlists to a file
            if (ExcelHandling.FileCreation)
            {
                using (ExcelPackage excel = new ExcelPackage())             // https://www.codebyamir.com/blog/create-excel-files-in-c-sharp
                {
                    // loop all playlists
                    for (int i = 0; i < playLists.Length; i++)
                    {
                        videosList = GetPlaylistVideos(playLists[i]);
                        ExcelWorksheet newWorksheet = excel.Workbook.Worksheets.Add(playLists[i].title);

                        // maintain videos in current playlist
                        for (int j = 0; j < videosList.Count; j++)
                        {
                            // check if title exist at last session
                            if (videosList[j] == null)
                            {
                                videosList[j] = "MISSING VIDEO ~~~~~~~~~~";

                                MissingVideo missingVideo = new MissingVideo((j + 1).ToString() + ": " +
                                                                             "Missing video title ; first time saving playlist", playLists[i].title);
                                myMissingVideos.Add(missingVideo);
                            }
                        }

                        newWorksheet.Cells[1, 1].LoadFromCollection(videosList);
                        newWorksheet.Cells["A:A"].AutoFitColumns();

                        labelCurrentPlaylist.Visible = false;
                        progressBarDownloadRate.Value = i;
                        listBoxPlaylists.Items[i] = listBoxPlaylists.Items[i].ToString() + " - SAVED";
                    }

                    if (myMissingVideos.Count() == 0)
                    {
                        MessageBox.Show("No missing videos at all playlists");
                    }
                    else
                    {
                        string missingVideosSheetName = "[Missing Videos- " + DateTime.Now.ToString("yyyyMMddTHHmmss");
                        ExcelWorksheet missingVideosWorksheet = excel.Workbook.Worksheets.Add(missingVideosSheetName);
                        missingVideosWorksheet.Cells[1, 1].LoadFromCollection(myMissingVideos);
                        missingVideosWorksheet.Cells["A:B"].AutoFitColumns();
                        myMissingVideos.Clear();

                        MessageBox.Show("Process finished successfully.\n" +
                                        "A list of the Missing videos from all playlists appear at the last worksheet of the file");

                        listBoxPlaylists.Items.Clear();
                        progressBarDownloadRate.Value = 0;

                    }

                    FileInfo excelFile = new FileInfo(ExcelHandling.MyPath);
                    excel.SaveAs(excelFile);
                }
            }
            else                    // File already exists
            {
                FileInfo file = new FileInfo(ExcelHandling.MyPath);
                using (ExcelPackage excel = new ExcelPackage(file))             
                {
                    ExcelWorkbook excelWorkBook = excel.Workbook;

                    // Check if file exists
                    for (int i = 0; i < playLists.Length; i++)
                    {
                        worksheetExists = false;
                        videosList = GetPlaylistVideos(playLists[i]);

                        // check if playlist exists in file
                        foreach (var sheet in excelWorkBook.Worksheets)
                        {
                            if (sheet.Name == playLists[i].title)
                            {
                                worksheetExists = true;
                                ExcelWorksheet existWorksheet = excelWorkBook.Worksheets[playLists[i].title];

                                int totRow = existWorksheet.Dimension.End.Row;              // https://www.codeproject.com/Articles/680421/Create-Read-Edit-Advance-Excel-Report-in

                                // maintain videos in current playlist
                                for (int j = 0; j < videosList.Count; j++)
                                {
                                    // check if title exist at last session
                                    if ( (videosList[j] == null) && (( j + 1) <= totRow) )
                                    {
                                        videosList[j] = "MISSING VIDEO ~~~~~~~~~~";

                                        MissingVideo missingVideo = new MissingVideo((j + 1).ToString() + ": " + 
                                                                                     existWorksheet.Cells[j + 1, 1].Value, playLists[i].title);
                                        myMissingVideos.Add(missingVideo);
                                    }
                                    else if (videosList[j] == null)
                                    {
                                        videosList[j] = "MISSING VIDEO ~~~~~~~~~~";

                                        MissingVideo missingVideo = new MissingVideo((j + 1).ToString() + ": " +
                                                                                     " missing video title ; new entry since last saved session", playLists[i].title);
                                        myMissingVideos.Add(missingVideo);
                                    }
                                }

                                existWorksheet.Cells[1, 1].LoadFromCollection(videosList);
                                existWorksheet.Cells["A:A"].AutoFitColumns();
                                break;
                            }
                        }
                        if (worksheetExists == false)
                        {
                            ExcelWorksheet newWorksheet = excel.Workbook.Worksheets.Add(playLists[i].title);

                            // maintain videos in current playlist
                            foreach (string videoTitle in videosList)
                            {
                                if (videoTitle == null)
                                {
                                    videosList[videosList.IndexOf(videoTitle)] = "MISSING VIDEO ~~~~~~~~~~";

                                    MissingVideo missingVideo = new MissingVideo(videosList.IndexOf(videoTitle).ToString() + ": " +
                                                                                 " missing video title ; new playlist", playLists[i].title);
                                    myMissingVideos.Add(missingVideo);
                                }
                            }

                            newWorksheet.Cells[1, 1].LoadFromCollection(videosList);
                            newWorksheet.Cells["A:A"].AutoFitColumns();
                        }

                        labelCurrentPlaylist.Visible = false;
                        progressBarDownloadRate.Value = i;
                        listBoxPlaylists.Items[i] = listBoxPlaylists.Items[i].ToString() + " - SAVED";
                    }

                    if (myMissingVideos.Count() == 0)
                    {
                        MessageBox.Show("No missing videos at all playlists");
                    }
                    else
                    {
                        string missingVideosSheetName = "[Missing Videos- " + DateTime.Now.ToString("yyyyMMddTHHmmss");
                        ExcelWorksheet missingVideosWorksheet = excel.Workbook.Worksheets.Add(missingVideosSheetName);
                        missingVideosWorksheet.Cells[1, 1].LoadFromCollection(myMissingVideos);
                        missingVideosWorksheet.Cells["A:B"].AutoFitColumns();
                        myMissingVideos.Clear();

                        MessageBox.Show("Process finished successfully.\n" +
                                        "A list of the Missing videos from all playlists appear at the last worksheet of the file");

                        listBoxPlaylists.Items.Clear();
                        progressBarDownloadRate.Value = 0;

                    }

                    FileInfo excelFile = new FileInfo(ExcelHandling.MyPath);
                    excel.Save();
                }
            }
        }
    }


    public class MissingVideo
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