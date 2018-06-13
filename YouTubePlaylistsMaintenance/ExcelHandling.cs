using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Data;
using OfficeOpenXml;

namespace YouTubePlaylistsMaintenance
{
    public static class ExcelHandling
    {
        public static string MyPath { get; set; } = @"\MyPlaylists.xlsx";

        public static string ConnectionString { get; set; } = null;
        public static bool FileCreation { get; set; } = false;


        public static bool FileStatus()
        {
            // checking if excel is installed
            bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;
            if (!isExcelInstalled)
            {
                MessageBox.Show("Software can run only with Excel software, please install first.");
                return false;
            }

            // choosing folder's path
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Please choose directory";
                if (dialog.ShowDialog() == DialogResult.OK)  //check for OK. They might press cancel, so don't do anything if they did.
                {
                    string path = dialog.SelectedPath;
                    MyPath = path + MyPath;
                }
            }

            // check if the file already exists
            FileInfo myFile = new FileInfo(MyPath);
            if (!File.Exists(MyPath))
            {
                MessageBox.Show("Did not manage to find an existing playlists-file in chosen directory," +
                                "\ntherefor the titles of deleted videos can't be found." +
                                "\nA new file will be created in the chosen directory.\n");

                FileCreation = true;
            }
            // check if the file is available
            else if (IsFileLocked(myFile))
            {
                MessageBox.Show("Please close open the playlists file\n and retry again");
                return false;
            }

            return true;
        }


        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
