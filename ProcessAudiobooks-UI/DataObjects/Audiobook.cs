using ProcessAudiobooks_UI.CustomControls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ProcessAudiobooks_UI.DataObjects
{
    public class Audiobook
    {
        public string Name { set; get; }
        public string outputName { set; get; }
        public string Artist { set; get; }
        public string Album { set; get; }
        public string Genre { set; get; }
        public string Year { set; get; }
        public string Writer { set; get; }
        public DataObjects.AudiobookProcessingStatus Status { set; get; }
        public List<string> FileList;
        public string outputPath;


        public Audiobook(string Name, string outputName,string Artist, string Album, string Genre, string Year, string Writer, List<string> FileList, string outputPath)
        {
            this.Name = Name;
            this.outputName = outputName;
            this.Artist = Artist;
            this.Album = Album;
            this.Genre = Genre;
            this.Year = Year;
            this.Writer = Writer;
            this.Status = DataObjects.AudiobookProcessingStatus.Ready;
            this.FileList = FileList;
            this.outputPath = outputPath;
        }
    }
}
