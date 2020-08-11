﻿using ProcessAudiobooks_UI.CustomControls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ProcessAudiobooks_UI.DataObjects
{
    public class Audiobook
    {
        public String Name { set; get; }
        public String outputName { set; get; }
        public String Album { set; get; }
        public String Genre { set; get; }
        public String Year { set; get; }
        public String Writer { set; get; }
        public DataObjects.AudiobookProcessingStatus Status { set; get; }
        public List<String> FileList;


        public Audiobook(String Name, String outputName, String Album, String Genre, String Year, String Writer, List<String> FileList)
        {
            this.Name = Name;
            this.outputName = outputName;
            this.Album = Album;
            this.Genre = Genre;
            this.Year = Year;
            this.Writer = Writer;
            this.Status = DataObjects.AudiobookProcessingStatus.Ready;
            this.FileList = FileList;
        }
    }
}