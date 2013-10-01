using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using DeadDog.Audio.Parsing;

namespace DeadDog.Audio.Scan
{
    public partial class AudioScanner
    {
        public static AudioScanner Load(IDataParser parser, string filename)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            if (filename == null)
                throw new ArgumentNullException("filename");

            FileInfo file = new FileInfo(filename);
            if (!file.Exists)
                throw new ArgumentException(file.FullName + " doesn't exist", "filename");

            AudioScanner scanner;
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
            {
                try
                {
                    scanner = Load(parser, fs);
                }
                catch
                {
                    scanner = null;
                }
            }
            return scanner;
        }
        public static AudioScanner Load(IDataParser parser, Stream input)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (input == null)
                throw new ArgumentNullException("input");
            if (!input.CanRead)
                throw new ArgumentException("Input stream must support reading", "input");

            IOAssistant io = new IOAssistant(input);

            string path = io.ReadString();
            DirectoryInfo directory = new DirectoryInfo(path);

            int extCount = io.ReadInt32();
            string[] extensions = new string[extCount];
            for (int i = 0; i < extCount; i++)
                extensions[i] = io.ReadString();

            FlagByte1 flagbyte1 = (FlagByte1)input.ReadByte();
            SearchOption searchoption = SearchOption.TopDirectoryOnly;
            if ((flagbyte1 & FlagByte1.AllDirectories) == FlagByte1.AllDirectories)
                searchoption = SearchOption.AllDirectories;

            AudioScanner ac = new AudioScanner(parser, directory, searchoption, extensions);

            ac.ParseAdd = (flagbyte1 & FlagByte1.ParseAdd) == FlagByte1.ParseAdd;
            ac.ParseUpdate = (flagbyte1 & FlagByte1.ParseUpdate) == FlagByte1.ParseUpdate;
            ac.RemoveDeadFiles = (flagbyte1 & FlagByte1.RemoveDeadFiles) == FlagByte1.RemoveDeadFiles;

            int existCount = io.ReadInt32();
            for (int i = 0; i < existCount; i++)
                ac.existingFiles.Add(RawTrack.FromStream(input));

            return ac;
        }

        public static void Save(AudioScanner ac, Stream output)
        {
            if (output == null)
                throw new ArgumentNullException("output");
            if (!output.CanWrite)
                throw new ArgumentException("Output stream must support writing", "output");

            IOAssistant io = new IOAssistant(output);
            io.Write(ac.directory.FullName);
            io.Write(ac.extensionList.Count);
            for (int i = 0; i < ac.extensionList.Count; i++)
                io.Write(ac.extensionList[i]);

            FlagByte1 fb1 = FlagByte1.NoFlags;
            if (ac.searchoption == SearchOption.AllDirectories)
                fb1 = fb1 | FlagByte1.AllDirectories;
            if (ac.ParseAdd)
                fb1 = fb1 | FlagByte1.ParseAdd;
            if (ac.ParseUpdate)
                fb1 = fb1 | FlagByte1.ParseUpdate;
            if (ac.RemoveDeadFiles)
                fb1 = fb1 | FlagByte1.RemoveDeadFiles;
            output.WriteByte((byte)fb1);

            io.Write(ac.existingFiles.Count);
            foreach (RawTrack rt in ac.existingFiles)
            {
                rt.Save(output);
            }
        }

        [Flags]
        private enum FlagByte1
        {
            NoFlags = 0,
            /// <summary>
            /// If set, SearchOption is set to AllDirectories. If not set, SearchOption is set to TopDirectoryOnly
            /// </summary>
            AllDirectories = 1,
            ParseAdd = 2,
            ParseUpdate = 4,
            RemoveDeadFiles = 8
        }
    }
}
