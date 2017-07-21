using System;
using System.IO;
using System.Threading.Tasks;

namespace DeadDog.Audio.Scan
{
    public static class AudioScanner
    {
        public static async Task<ScannedFile[]> ScanDirectory(ScannerSettings settings, IProgress<ScanProgress> progress = null)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
        }
    }
}
