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
    public class mp3Control : IDisposable
    {
        private string playerAlias;
        private bool isPlaying = false;
        private bool fileOpen = false;

        private bool swapped = false;
        private int vol = 1000, right = 1000, left = 1000, bass = 1000, treble = 1000;
        private int pubLeft = 1000, pubRight = 1000;

        //todo Recreate without the use of Forms.
        private double percent = 0;
        private int TIMER_INTERVAL = 100;
        private int TIMER_INFINITE = System.Threading.Timeout.Infinite;
        private System.Threading.Timer timer;

        private mp3Control()
        {
            timer = new System.Threading.Timer(obj => update(), null, TIMER_INFINITE, TIMER_INFINITE);
            timer.Change(TIMER_INFINITE, TIMER_INFINITE);

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

        /// <summary>
        /// Occurs when the mp3Control starts playback.
        /// </summary>
        public event EventHandler Playing;
        /// <summary>
        /// Occurs when the mp3Control is paused.
        /// </summary>
        public event EventHandler Pausing;
        /// <summary>
        /// Occurs when the mp3Control resumes playback.
        /// </summary>
        public event EventHandler Resuming;
        /// <summary>
        /// Occurs when the mp3Control stops playback.
        /// </summary>
        public event EventHandler Stopping;
        /// <summary>
        /// Occurs when the mp3Control changes position in the file being played.
        /// </summary>
        public event EventHandler Seeking;
        /// <summary>
        /// Occurs when a file is loaded into the mp3Control.
        /// </summary>
        public event EventHandler Opening;
        /// <summary>
        /// Occurs when a file is unloaded from the mp3Control.
        /// </summary>
        public event EventHandler Closing;
        /// <summary>
        /// Occurs when the playing has reached it's end.
        /// </summary>
        public event EventHandler ReachedEnd;
        /// <summary>
        /// Occurs when a small portion of the currently playing file has been played. Is therefore not called when the file is paused.
        /// </summary>
        public event EventHandler Tick;

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, int hwndCallback);

        /// <summary>
        /// Loads a new file into this mp3Control
        /// </summary>
        /// <param name="fullpath">The full path of the file to load.</param>
        public void LoadFile(string fullpath)
        {
            if (fileOpen)
                CloseFile();
            string command = "open " + "\"" + fullpath + "\"" + " type MPEGVideo alias " + playerAlias;
            long err = mciSendString(command, null, 0, 0);
            long b = 1 + err;
            fileOpen = true;
            if (Opening != null)
                Opening(this, EventArgs.Empty);

            string s = status;

            UpdateVolumes();
        }
        /// <summary>
        /// Unloads the currently loaded file.
        /// </summary>
        public void CloseFile()
        {
            if (!fileOpen)
                return;
            mciSendString("close " + playerAlias, null, 0, 0);
            fileOpen = false;

            if (Closing != null)
                Closing(this, EventArgs.Empty);

            if (throwTick && timer.Enabled)
            {
                timer_Tick(null, null);
                timer.Stop();
            }
        }

        /// <summary>
        /// Starts playback.
        /// </summary>
        public void Play()
        {
            if (!fileOpen)
                return;

            mciSendString("play " + playerAlias, null, 0, 0);
            isPlaying = true;
            if (Playing != null)
                Playing(this, EventArgs.Empty);

            if (throwTick && !timer.Enabled)
            {
                timer_Tick(null, null);
                timer.Start();
            }
        }
        /// <summary>
        /// Pauses playback.
        /// </summary>
        public void Pause()
        {
            if (!fileOpen)
                return;
            mciSendString("pause " + playerAlias, null, 0, 0);
            if (Position != Length)
                isPlaying = false;
            if (Pausing != null)
                Pausing(this, EventArgs.Empty);

            if (throwTick && timer.Enabled)
            {
                timer_Tick(null, null);
                timer.Stop();
            }
        }
        /// <summary>
        /// Resumes playback.
        /// </summary>
        public void Resume()
        {
            if (!fileOpen)
                return;

            mciSendString("play " + playerAlias, null, 0, 0);
            if (Position != Length)
                isPlaying = true;
            else
                isPlaying = false;
            if (Resuming != null)
                Resuming(this, EventArgs.Empty);

            if (throwTick && !timer.Enabled)
            {
                timer_Tick(null, null);
                timer.Start();
            }

        }
        /// <summary>
        /// Stops playback.
        /// </summary>
        public void Stop()
        {
            if (!fileOpen)
                return;
            mciSendString("stop " + playerAlias, null, 0, 0);
            Seek(PlayerSeekOrigin.Begin, false);
            if (Position != Length)
                isPlaying = false;
            if (Stopping != null)
                Stopping(this, EventArgs.Empty);

            if (throwTick && timer.Enabled)
            {
                timer_Tick(null, null);
                timer.Stop();
            }
        }

        /// <summary>
        /// Getter of the mp3Control status.
        /// </summary>
        public PlayerStatus Status
        {
            get
            {
                switch (status)
                {
                    case "playing": return PlayerStatus.Playing;
                    case "paused": return PlayerStatus.Paused;
                    case "stopped": return PlayerStatus.Stopped;
                    default: return PlayerStatus.NoFileOpen;
                }
            }
        }
        private string status
        {
            get
            {
                System.Text.StringBuilder buffer = new System.Text.StringBuilder(128);
                mciSendString("status " + playerAlias + " mode", buffer, 128, 0);
                return buffer.ToString();
            }
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
        public int Position
        {
            get
            {
                if (!fileOpen)
                    return 0;
                
                StringBuilder buffer = new StringBuilder(128);
                mciSendString("status " + playerAlias + " position", buffer, 128, 0);
                if (buffer.Length == 0)
                    return 0;

                return int.Parse(buffer.ToString());
            }
        }
        /// <summary>
        /// Gets the length, in milliseconds, of the currently loaded file.
        /// </summary>
        public int Length
        {
            get
            {
                if (!fileOpen)
                    return 0;

                StringBuilder buffer = new StringBuilder(128);
                mciSendString("status " + playerAlias + " length", buffer, 128, 0);
                if (buffer.Length == 0)
                    return 0;

                return int.Parse(buffer.ToString());
            }
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

        /// <summary>
        /// Getter of the played percentage of the file.
        /// </summary>
        public double PercentPlayed
        {
            get
            {
                return percent;
            }
        }

        public bool ThrowsTickEvent
        {
            get { return throwTick; }
            set
            {
                if (value == throwTick)
                    return;
                throwTick = value;
                if (throwTick)
                    timer.Start();
                else
                    timer.Stop();
            }
        }
        private void update()
        {
            double pos = (double)Position;
            double total = (double)Length;
            if (total > 0)
                percent = ((double)pos) / ((double)total);
            else
                percent = 0;
            if (total > 0 && pos == total && isPlaying && ReachedEnd != null)
                ReachedEnd(this, EventArgs.Empty);
            if (Tick != null)
                Tick(this, EventArgs.Empty);
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
