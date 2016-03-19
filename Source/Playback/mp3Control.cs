using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace DeadDog.Audio.Playback
{
    /// <summary>
    /// Utilizes winmm.dll (Windows Multi Media) to play audiofiles.
    /// </summary>
    /// <remarks>
    /// Basicly works as a frontend to playing audiofiles. 
    /// Despite the name "mp3Control" the player handles other standard filetypes, such as wav, wma and midi. 
    /// You can find more info about the functionalities of winmm.dll here: http://msdn2.microsoft.com/en-us/library/ms712636.aspx .
    /// The mp3Control includes a System.Windows.Forms.Timer object from the .NET 2.0 framework, though it has no included UI.
    /// </remarks>
    public class mp3Control : IFilePlayback
    {
        private static class Mci
        {
            [DllImport("winmm.dll")]
            private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, int hwndCallback);

            public static long Send(string command)
            {
                return mciSendString(command, null, 0, 0);
            }
            public static string GetResponse(string command, int resultBufferSize)
            {
                StringBuilder sb = new StringBuilder(resultBufferSize);

                mciSendString(command, sb, resultBufferSize, 0);

                return sb.ToString();
            }
        }

        private string playerAlias;
        
        private const double MAXVOLUME = 1000;

        private mp3Control()
        {
            playerAlias = "MediaFile";
        }

        private static mp3Control instance;
        public static mp3Control Instance
        {
            get
            {
                if (instance == null)
                    instance = new mp3Control();
                return instance;
            }
        }

        public bool CanOpen(string filepath)
        {
            return true;
        }
        
        public bool Open(string filepath) => Mci.Send($"open \"{filepath}\" type MPEGVideo alias {playerAlias}") == 0;
        public bool Close() => Mci.Send($"close {playerAlias}") == 0;

        public bool StartPlayback() => Mci.Send($"play {playerAlias}") == 0;
        public bool PausePlayback() => Mci.Send($"pause {playerAlias}") == 0;
        public bool ResumePlayback() => Mci.Send($"play {playerAlias}") == 0;
        public bool StopPlayback() => Mci.Send($"stop {playerAlias}") == 0;

        public uint GetTrackLength()
        {
            string response = Mci.GetResponse($"status {playerAlias} length", 128);

            if (response.Length == 0)
                return 0;
            else
                return uint.Parse(response);
        }
        public uint GetTrackPosition()
        {
            string response = Mci.GetResponse($"status {playerAlias} position", 128);
            if (response.Length == 0)
                return 0;
            else
                return uint.Parse(response);
        }
        public bool GetIsPlaying()
        {
            string response = Mci.GetResponse($"status {playerAlias} mode", 128);
            return response.Equals("playing");
        }
        
        public bool Seek(PlayerSeekOrigin origin, uint offset)
        {
            uint seek = translateSeek(origin, offset);
            return Mci.Send($"seek {playerAlias} to {seek}") == 0;
        }
        private uint translateSeek(PlayerSeekOrigin origin, uint offset)
        {
            uint position = GetTrackPosition();
            uint length = GetTrackLength();
            uint newPosition;

            switch (origin)
            {
                case PlayerSeekOrigin.Begin:
                    newPosition = offset;
                    break;
                case PlayerSeekOrigin.CurrentForwards:
                    newPosition = position + offset;
                    break;
                case PlayerSeekOrigin.CurrentBackwards:
                    newPosition = position - offset;
                    break;
                case PlayerSeekOrigin.End:
                    newPosition = length - offset;
                    break;
                default:
                    throw new ArgumentException("Unknown PlayerSeekOrigin: " + origin, "origin");
            }

            if (newPosition < 0) newPosition = 0;
            if (newPosition >= length) newPosition = length - 1;

            return newPosition;
        }

        public void SetVolume(double left, double right)
        {
            int l = (int)(left * MAXVOLUME);
            int r = (int)(right * MAXVOLUME);

            Mci.Send($"setaudio {playerAlias} left volume to {l}");
            Mci.Send($"setaudio {playerAlias} right volume to {r}");
        }
        public void GetVolume(out double left, out double right)
        {
            var lStr = Mci.GetResponse($"status {playerAlias} left volume", 128);
            var rStr = Mci.GetResponse($"status {playerAlias} left volume", 128);

            int l = 0;
            int r = 0;

            left = l / MAXVOLUME;
            right = r / MAXVOLUME;
        }

        #region IDisposable Members
        
        public void Dispose()
        {
        }

        #endregion
    }
}
