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

            FlagByte1 fb1 = ConstructFlag1(ac);
            output.WriteByte((byte)fb1);

            io.Write(ac.existingFiles.Count);
            foreach (RawTrack rt in ac.existingFiles)
                rt.Save(output);
        }

        private static FlagByte1 ConstructFlag1(AudioScanner scanner)
        {
            FlagByte1 b = FlagByte1.NoFlags;

            if (scanner.searchoption == SearchOption.AllDirectories)
                b |= FlagByte1.AllDirectories;
            if (scanner.ParseAdd)
                b |= FlagByte1.ParseAdd;
            if (scanner.ParseUpdate)
                b |= FlagByte1.ParseUpdate;
            if (scanner.RemoveDeadFiles)
                b |= FlagByte1.RemoveDeadFiles;

            return b;
        }

        /// <summary>
        /// Defines constants for the first AudioScanner flag byte
        /// </summary>
        [Flags]
        private enum FlagByte1
        {
            /// <summary>
            /// No flags, see other flags for details.
            /// </summary>
            NoFlags = 0x00,
            /// <summary>
            /// If set, SearchOption is set to AllDirectories. If not set, SearchOption is set to TopDirectoryOnly
            /// </summary>
            AllDirectories = 0x01,
            /// <summary>
            /// If set, ParseAdd in the AudioScanner is set to true.
            /// </summary>
            ParseAdd = 0x02,
            /// <summary>
            /// If set, ParseUpdate in the AudioScanner is set to true.
            /// </summary>
            ParseUpdate = 0x04,
            /// <summary>
            /// If set, RemoveDeadFiles in the AudioScanner is set to true.
            /// </summary>
            RemoveDeadFiles = 8,
            /// <summary>
            /// If set, there is an additional flag byte.
            /// </summary>
            AdditionalByte = 0x80
        }
    }
}
