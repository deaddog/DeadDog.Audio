using System;
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
        private Dictionary<YouTubeID, Download> files;

        public Downloader(string directory)
        {
            this.directory = Path.GetFullPath(directory);
            this.files = new Dictionary<YouTubeID, Download>();
        }

        public void LoadExisting()
        {
            YouTubeID[] ids = new YouTubeID[files.Count];
            files.Keys.CopyTo(ids, 0);

            for (int i = 0; i < ids.Length; i++)
                Load(ids[i], false);
        }

        public Download Load(YouTubeID id, bool loadAsync)
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
                    if (loadAsync)
                        new Thread(() => download.Start(onComplete)).Start();
                    else
                        download.Start(onComplete);
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

            RawTrack trackinfo = YouTubeParser.ParseTrack(download.ID, download.Path);
            download.TrackInfo = trackinfo;
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
