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
    public class mp3Control : IPlayback<string>
    {
        private string playerAlias;
        private PlayerStatus plStatus;
        private uint position;
        private uint length;

        private bool swapped = false;
        private int vol = 1000, right = 1000, left = 1000, bass = 1000, treble = 1000;
        private int pubLeft = 1000, pubRight = 1000;

        private int TIMER_INTERVAL = 100;
        private int TIMER_INFINITE = System.Threading.Timeout.Infinite;
        private System.Threading.Timer timer;

        private mp3Control()
        {
            timer = new System.Threading.Timer(obj => updatePosition(), null, TIMER_INFINITE, TIMER_INFINITE);
            timer.Change(TIMER_INFINITE, TIMER_INFINITE);

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

        public event EventHandler StatusChanged;
        public event PositionChangedEventHandler PositionChanged;

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, int hwndCallback);

        /// <summary>
        /// Loads a new file into this mp3Control
        /// </summary>
        /// <param name="element">The path of the file to load.</param>
        public bool Open(string element)
        {
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                case PlayerStatus.Stopped:
                    Close();
                    return Open(element);
                case PlayerStatus.NoFileOpen:
                    if (element == null)
                        return false;
                    System.IO.FileInfo file = new System.IO.FileInfo(element);

                    if (!file.Exists)
                        return false;
                    string command = "open " + "\"" + file.FullName + "\"" + " type MPEGVideo alias " + playerAlias;
                    long err = mciSendString(command, null, 0, 0);
                    long b = 1 + err;

                    StringBuilder buffer = new StringBuilder(128);
                    mciSendString("status " + playerAlias + " length", buffer, 128, 0);
                    if (buffer.Length == 0)
                        length = 0;
                    else
                        length = uint.Parse(buffer.ToString());

                    updateStatus();
                    UpdateVolumes();
                    timer.Change(0, TIMER_INFINITE);
                    return plStatus != PlayerStatus.NoFileOpen;
                default:
                    throw new Exception("Unknown PlayerStatus.");
            }
        }
        /// <summary>
        /// Unloads the currently loaded file.
        /// </summary>
        public bool Close()
        {
            if (plStatus == PlayerStatus.NoFileOpen)
                return true;
            else
            {
                Stop();
                mciSendString("close " + playerAlias, null, 0, 0);
                updateStatus();
                return plStatus == PlayerStatus.NoFileOpen;
            }
        }

        /// <summary>
        /// Starts (or resumes) playback.
        /// </summary>
        public bool Play()
        {
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                    return true;
                case PlayerStatus.Paused:
                case PlayerStatus.Stopped:
                    mciSendString("play " + playerAlias, null, 0, 0);
                    updateStatus();
                    timer.Change(0, TIMER_INTERVAL);
                    return true;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    throw new Exception("Unknown PlayerStatus.");
            }
        }
        /// <summary>
        /// Pauses playback.
        /// </summary>
        public bool Pause()
        {
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                    mciSendString("pause " + playerAlias, null, 0, 0);
                    updateStatus();
                    timer.Change(0, TIMER_INFINITE);
                    return true;
                case PlayerStatus.Paused:
                    return true;
                case PlayerStatus.Stopped:
                    return false;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    throw new Exception("Unknown PlayerStatus.");
            }
        }
        /// <summary>
        /// Stops playback.
        /// </summary>
        public bool Stop()
        {
            switch (plStatus)
            {
                case PlayerStatus.Playing:
                case PlayerStatus.Paused:
                    mciSendString("stop " + playerAlias, null, 0, 0);
                    Seek(PlayerSeekOrigin.Begin, false);
                    updateStatus();
                    timer.Change(0, TIMER_INFINITE);
                    return true;
                case PlayerStatus.Stopped:
                    return true;
                case PlayerStatus.NoFileOpen:
                    return false;
                default:
                    throw new Exception("Unknown PlayerStatus.");
            }
        }

        /// <summary>
        /// Getter of the mp3Control status.
        /// </summary>
        public PlayerStatus Status
        {
            get { return plStatus; }
        }
        private void updateStatus()
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder(128);
            mciSendString("status " + playerAlias + " mode", buffer, 128, 0);
            string status = buffer.ToString();

            PlayerStatus old = plStatus;

            switch (status)
            {
                case "playing": plStatus = PlayerStatus.Playing; break;
                case "paused": plStatus = PlayerStatus.Paused; break;
                case "stopped": plStatus = PlayerStatus.Stopped; break;
                default: plStatus = PlayerStatus.NoFileOpen; break;
            }

            if (plStatus != old && StatusChanged != null)
                StatusChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Getter of whether or not a file is loaded.
        /// </summary>
        public bool FileOpen
        {
            get
            {
                return fileOpen;
            }
        }

        /// <summary>
        /// Gets the position, in milliseconds, in the currently loaded file.
        /// </summary>
        public uint Position
        {
            get { return position; }
        }
        private void updatePosition()
        {
            StringBuilder buffer = new StringBuilder(128);
            mciSendString("status " + playerAlias + " position", buffer, 128, 0);
            if (buffer.Length == 0)
                position = 0;

            position = uint.Parse(buffer.ToString());
            bool endreached = length > 0 && position == length && plStatus == PlayerStatus.Playing;
            if (PositionChanged != null)
                PositionChanged(this, new PositionChangedEventArgs(endreached));
        }
        /// <summary>
        /// Gets the length, in milliseconds, of the currently loaded file.
        /// </summary>
        public uint Length
        {
            get { return length; }
        }

        /// <summary>
        /// Sets the current position in the audiofile.
        /// </summary>
        /// <param name="offset">An offset, in milliseconds, relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <param name="resumeplayback">A boolean indicating whether or not playback should be resumed.</param>
        /// <returns>The new position in the audiofile.</returns>
        public int Seek(int offset, PlayerSeekOrigin origin, bool resumeplayback)
        {
            int position = this.Position;
            int length = this.Length;
            int newP;

            switch (origin)
            {
                case PlayerSeekOrigin.Begin:
                    newP = offset;
                    break;
                case PlayerSeekOrigin.CurrentForwards:
                    newP = position + offset;
                    break;
                case PlayerSeekOrigin.End:
                    newP = length + offset;
                    break;
                default:
                    newP = offset;
                    break;
            }

            if (newP < 0)
                newP = 0;
            if (newP >= length)
                newP = length - 1;

            mciSendString("seek " + playerAlias + " to " + newP, null, 0, 0);

            if (Seeking != null)
                Seeking(this, EventArgs.Empty);

            if (newP != length && resumeplayback)
                Resume();

            return newP;
        }
        /// <summary>
        /// Sets the current position in the audiofile.
        /// </summary>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position in the audiofile, in milliseconds.</returns>
        /// <param name="resumeplayback">A boolean indicating whether or not playback should be resumed.</param>
        public int Seek(PlayerSeekOrigin origin, bool resumeplayback)
        {
            int newP = 0;

            switch (origin)
            {
                case PlayerSeekOrigin.Begin:
                    newP = 0;
                    break;
                case PlayerSeekOrigin.CurrentForwards:
                    return this.Position;
                case PlayerSeekOrigin.End:
                    newP = this.Length;
                    break;
            }

            mciSendString("seek " + playerAlias + " to " + newP, null, 0, 0);

            if (Seeking != null)
                Seeking(this, EventArgs.Empty);

            if (origin != PlayerSeekOrigin.End && resumeplayback)
                Resume();

            return newP;
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
            timer.Stop();
            timer.Dispose();
            Stop();
            CloseFile();
        }

        #endregion
    }
}
