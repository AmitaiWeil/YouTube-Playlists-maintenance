using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Office.Core;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace YouTubePlaylistsMaintenance
{
    class ExcelHandling
    {
        public static string myPath = @"\MyPlaylists.xlsx";

        private static Excel.Application MyExcelApp = null;
        private static Excel.Workbook MyWorkBook = null;
        private static Excel.Worksheet MyWorkSheet = null;
        private static bool firstEntry = false;

        public static bool ObtainExcelFile()
        {
            // choosing folder's path
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Please choose directory";
                if (dialog.ShowDialog() == DialogResult.OK)  //check for OK...they might press cancel, so don't do anything if they did.
                {
                    var path = dialog.SelectedPath;
                    myPath = path + myPath;
                }
            }

            FileInfo myFile = new FileInfo(myPath);
            if (!File.Exists(myPath))
            {
                try
                {
                    firstEntry = true;
                    object misValue = System.Reflection.Missing.Value;       // no value is supplied for respective parameter

                    MessageBox.Show("Did not manage to find an existing playlists-file in chosen directory,\ntherefor the titles of deleted videos can't be found." +
                                    "\nA new file will be created in chosen directory.\n");

                    MyExcelApp = new Excel.Application();
                    MyExcelApp.Visible = true;
                    MyWorkBook = MyExcelApp.Workbooks.Add(1);
                    MyWorkSheet = (Excel.Worksheet)MyWorkBook.Sheets[1];
                    MyWorkBook.SaveAs(myPath, misValue, misValue, misValue, misValue,
                                      misValue, Excel.XlSaveAsAccessMode.xlNoChange, misValue, misValue, misValue,
                                      misValue, misValue);
                    MyWorkBook.Close(true, misValue, misValue);
                    MyExcelApp.Quit();

                    //Marshal.ReleaseComObject(MyWorkSheet);
                    //Marshal.ReleaseComObject(MyWorkBook);
                    //Marshal.ReleaseComObject(MyExcelApp);

                }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
                catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    throw;
                }
            }
            else
            {
                if (IsFileLocked(myFile))
                {
                    MessageBox.Show("Please close open playlists file\n and try again");
                    return false;
                }
            }
            // also if file created save in location and re-open
            MyExcelApp = new Excel.Application();
            MyWorkBook = MyExcelApp.Workbooks.Open(myPath);
            MyWorkSheet = (Excel.Worksheet)MyWorkBook.Sheets[1];
            MyExcelApp.Visible = true;
            return true;
        }

        public static void SaveCurrentPlaylistToSheet(string PlaylistTitle, YouTubeVideo[] CurrentPlaylist)
        {
            object misValue = System.Reflection.Missing.Value;       // in case no value is supplied for respective parameter

            //MyExcelApp = new Excel.Application();
            //MyExcelApp.Visible = true;

            foreach (Excel.Worksheet wSheet in MyWorkBook.Worksheets)
            {
                if (wSheet.Name.ToString() == PlaylistTitle)
                {
                    MyWorkSheet = (Excel.Worksheet)MyWorkBook.Sheets[wSheet.Index];
                }
            }

            if (MyWorkSheet.Name.ToString() != PlaylistTitle)                // no such playlist found in excel file - first time saving current playlist
            {
                if (firstEntry == true)
                {
                    MyWorkSheet = MyWorkBook.Sheets[1];
                    firstEntry = false;
                }
                else
                {
                    MyWorkBook.Sheets.Add(misValue, MyWorkBook.Sheets[MyWorkBook.Sheets.Count]);
                    MyWorkSheet = MyWorkBook.Sheets[MyWorkBook.Sheets.Count];
                }
                MyWorkSheet.Name = PlaylistTitle;
            }
            
            for (int i = 0; i < CurrentPlaylist.Length; i++)
            {
                MyWorkSheet.Cells[(i + 1), 1] = CurrentPlaylist[i].title;
            }
            MyWorkSheet.Columns[1].ColumnWidth = 110;

        }

        public static void SaveMissingVideos(List<MissingVideo> myMissingVideos)
        {
            object misValue = System.Reflection.Missing.Value;       // in case no value is supplied for respective parameter

            foreach (Excel.Worksheet wSheet in MyWorkBook.Worksheets)
            {
                if (wSheet.Name.ToString().Contains("Missing Videos-"))
                {
                    MyWorkSheet = (Excel.Worksheet)MyWorkBook.Sheets[wSheet.Index];
                }
            }
            if (!MyWorkSheet.Name.ToString().Contains("Missing Videos-"))
            {
                MyWorkBook.Sheets.Add(misValue, MyWorkBook.Sheets[MyWorkBook.Sheets.Count]);
                MyWorkSheet = MyWorkBook.Sheets[MyWorkBook.Sheets.Count];
            }

            MyWorkSheet.Cells.Clear();
            MyWorkSheet.Columns[1].ColumnWidth = 70;
            MyWorkSheet.Columns[2].ColumnWidth = 50;
            MyWorkSheet.Columns[2].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            MyWorkSheet.Columns[1].Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            MyWorkSheet.Cells[1, 1] = "Playlist";
            MyWorkSheet.Cells[1, 1].Font.Bold = true;
            MyWorkSheet.Cells[1, 2].Font.Bold = true;
            MyWorkSheet.Cells[1, 2] = "at location";

            for (int i = 0; i < myMissingVideos.Count(); i++)
            {
                MyWorkSheet.Cells[i + 2, 1] = myMissingVideos[i].Playlist;
                MyWorkSheet.Cells[i + 2, 2] = myMissingVideos[i].VideoTitle;

            }
            MyWorkSheet.Name = "Missing Videos- " + DateTime.Now.ToString("yyyyMMddTHHmmss");

            CloseFile();
        }

        public static void CloseFile()
        {
            object misValue = System.Reflection.Missing.Value;       

            MyWorkBook.SaveAs(myPath);                               //, misValue, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            MyWorkBook.Close(true, misValue, misValue);
            MyExcelApp.Quit();

            Marshal.ReleaseComObject(MyWorkSheet);                   // http://csharp.net-informations.com/excel/csharp-create-excel.htm
            Marshal.ReleaseComObject(MyWorkBook);
            Marshal.ReleaseComObject(MyExcelApp);
        }

        public static string SearchMissingVideo(int location, string playlistTitle, int playlistLength)
        {
            int lastRow;

            foreach (Excel.Worksheet wSheet in MyWorkBook.Worksheets)
            {
                if (wSheet.Name.ToString() == playlistTitle)
                {
                    MyWorkSheet = (Excel.Worksheet)MyWorkBook.Sheets[wSheet.Index];
                }
            }
            if (MyWorkSheet.Name.ToString() != playlistTitle)                // no such playlist found in excel file - first time saving current playlist
            {
                return " *** missing video title ***";
            }

            lastRow = MyWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;

            if ( (lastRow <= playlistLength) && (MyWorkSheet.Cells[location, 1].Value != null) )               // order of playlist hasn't changed
            {
                return (MyWorkSheet.Cells[location, 1].Value.ToString());
            }

            return " *** missing video title ***";
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
