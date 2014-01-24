﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using DeadDog.Audio.Parsing;
using DeadDog.Audio.Scan;

namespace DeadDog.Audio.YouTube
{
    public class Downloader
    {
        private string directory;
        private string documentPath;
        private XDocument document;
        private Dictionary<YouTubeID, Download> files;

        public Downloader(string directory)
        {
            this.directory = Path.GetFullPath(directory);
            this.files = new Dictionary<YouTubeID, Download>();

            this.documentPath = XML.DocumentPath(directory);
            System.IO.FileInfo file = new System.IO.FileInfo(documentPath);
            if (!file.Exists)
                document = new XDocument(new XElement("tracks"));
            else
            {
                document = XDocument.Load(documentPath);
                foreach (var e in document.Element("tracks").Elements("track"))
                {
                    YouTubeID id = YouTubeID.Parse(e.Attribute("id").Value);
                    string path = Path.Combine(this.directory, e.Element("path").Value);
                    string title = e.Element("title").Value;

                    RawTrack trackinfo = YouTubeParser.ParseTrack(id, path);
                    files.Add(id, new Download(id, trackinfo, title));
                }
            }
        }

        public void LoadExisting()
        {
            YouTubeID[] ids = new YouTubeID[files.Count];
            files.Keys.CopyTo(ids, 0);

            for (int i = 0; i < ids.Length; i++)
                Load(ids[i]);
        }

        public Download Load(YouTubeID id)
        {
            if (id == YouTubeID.Empty)
                throw new ArgumentException("YouTubeID.Empty is not a valid argument.", "id");

            Download download;
            lock (files)
            {
                if (!files.TryGetValue(id, out download))
                {
                    download = new Download(id, Path.Combine(directory, id.ID + ".mp3"));
                    files.Add(id, download);
                    new Thread(() => download.Start(onComplete)).Start();
                }
                else if (files[id].State == Download.States.Failed)
                {
                    new Thread(() => download.Start(onComplete)).Start();
                }
                else
                {
                    RawTrack trackinfo = YouTubeParser.ParseTrack(download.ID, download.Path);

                    if (!trackinfo.Equals(download.TrackInfo))
                    {
                        download.TrackInfo = trackinfo;
                        OnFileParsed(new ScanFileEventArgs(download.Path, trackinfo, FileState.Updated));
                    }
                    else
                        OnFileParsed(new ScanFileEventArgs(download.Path, download.TrackInfo, FileState.Skipped));
                }
            }
            return download;
        }

        private void onComplete(Download download)
        {
            if (download.State == Download.States.Failed)
            {
                OnFileParsed(new ScanFileEventArgs(download.Path, null, FileState.AddError));
                return;
            }

            string path = download.Path;
            path = path.Substring(this.directory.Length).TrimStart('\\');

            XElement track = new XElement("track",
                new XAttribute("id", download.ID),
                new XElement("path", path),
                new XElement("title", download.Title));

            lock (document)
            {
                document.Element("tracks").Add(track);
                document.Save(documentPath);
            }

            RawTrack trackinfo = YouTubeParser.ParseTrack(download.ID, download.Path);
            OnFileParsed(new ScanFileEventArgs(download.Path, trackinfo, FileState.Added));
        }

        private void OnFileParsed(ScanFileEventArgs e)
        {
            if (FileParsed != null)
                FileParsed(this, e);
        }
        public event ScanFileEventHandler FileParsed;
    }
}
