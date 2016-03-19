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
        private uint position;
        private uint length;

        private bool swapped = false;
        private int vol = 1000, right = 1000, left = 1000, bass = 1000, treble = 1000;
        private int pubLeft = 1000, pubRight = 1000;

        private mp3Control()
        {
            playerAlias = "MediaFile";
            plStatus = PlayerStatus.NoFileOpen;
            position = 0;
            left = 0;
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

        /// <summary>
        /// Sets the current position in the audiofile.
        /// </summary>
        /// <param name="offset">An offset, in milliseconds, relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>true, if the seek operation was succesfull; otherwise, false.</returns>
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

        /// <summary>
        /// Setter/Getter of the master volume in the range 0 to 1000.
        /// </summary>
        public int MasterVolume
        {
            get
            {
                return vol;
            }
            set
            {
                if (value <= 1000 && value >= 0)
                {
                    vol = value;
                    mciSendString("setaudio " + playerAlias + " volume to " + value, null, 0, 0);
                }
            }
        }
        /// <summary>
        /// Setter/Getter of the bass volume in the range 0 to 1000.
        /// </summary>
        public int Bass
        {
            get
            {
                return bass;
            }
            set
            {
                if (value <= 1000 && value >= 0)
                {
                    bass = value;
                    mciSendString("setaudio " + playerAlias + " bass to " + value, null, 0, 0);
                }
            }
        }
        /// <summary>
        /// Setter/Getter of the treble volume in the range 0 to 1000.
        /// </summary>
        public int Treble
        {
            get
            {
                return treble;
            }
            set
            {
                if (value <= 1000 && value >= 0)
                {
                    treble = value;
                    mciSendString("setaudio " + playerAlias + " treble to " + value, null, 0, 0);
                }
            }
        }

        private void UpdateVolumes()
        {
            MasterVolume = MasterVolume;
            Bass = Bass;
            Treble = Treble;
            RightVolume = RightVolume;
            LeftVolume = LeftVolume;
        }
        /// <summary>
        /// Getter/Setter of the right channel volume in the range 0 to 1000. Setting the volume to 1000 sets it to 100% of the master volume.
        /// </summary>
        public int RightVolume
        {
            get
            {
                return getChannel(!swapped);
            }
            set
            {
                if (value <= 1000 && value >= 0)
                    setChannel(value, !swapped);
            }
        }
        /// <summary>
        /// Getter/Setter of the left channel volume in the range 0 to 1000. Setting the volume to 1000 sets it to 100% of the master volume.
        /// </summary>
        public int LeftVolume
        {
            get
            {
                return getChannel(swapped);
            }
            set
            {
                if (value <= 1000 && value >= 0)
                    setChannel(value, swapped);
            }
        }
        private int getChannel(bool isRight)
        {
            if (isRight)
                return pubRight;
            else
                return pubLeft;
        }
        private void setChannel(int volume, bool isRight)
        {
            double _volume = (double)volume;
            _volume /= 1000;
            _volume *= vol;
            int volumeVal = (int)_volume;
            if (isRight)
            {
                right = volumeVal;
                pubRight = volume;
                mciSendString("setaudio " + playerAlias + " right volume to " + volumeVal, null, 0, 0);
            }
            else
            {
                left = volumeVal;
                pubLeft = volume;
                mciSendString("setaudio " + playerAlias + " left volume to " + volumeVal, null, 0, 0);
            }
        }
        /// <summary>
        /// Swaps the meaning of the RightVolume and the LeftVolume (doesn't swap the actual sound).
        /// </summary>
        public void SwapLeftRight()
        {
            swapped = !swapped;
        }

        #region IDisposable Members

        /// <summary>
        /// Stops playback and releases unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
            Close();
            timer.Dispose();
        }

        #endregion
    }
}
