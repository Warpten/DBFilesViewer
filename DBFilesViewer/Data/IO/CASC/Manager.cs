using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DBFilesViewer.Utils.Extensions;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DBFilesViewer.Data.IO.CASC
{
    public static class Manager
    {
        public static Encoding Encoding { get; } = new Encoding();
        public static Root Root { get; } = new Root();
        public static IndexStore Index { get; } = new IndexStore();

        public static string InstallPath { get; private set; }
        public static string DataPath { get; private set; }
        private static TokenConfig _buildInfo { get; } = new TokenConfig();
        private static KeyValueConfig _buildConfig { get; } = new KeyValueConfig();

        private static Dictionary<int, Stream> _dataFiles = new Dictionary<int, Stream>();

        private static JenkinsHashing _hasher = new JenkinsHashing();

        public static event Action OnLoadComplete;
        public static event Action<string> OnLoadProgress;

        public static async void Initialize()
        {
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            await Task.Factory.StartNew(() =>
            {
                InstallPath = dialog.FileName;
                DataPath = Path.Combine(InstallPath, @"Data\data");

                var buildInfoPath = Path.Combine(InstallPath, ".build.info");
                try
                {
                    using (var buildInfo = new StreamReader(File.OpenRead(buildInfoPath)))
                        _buildInfo.Load(buildInfo);
                }
                catch (FileNotFoundException /* fnfe */)
                {
                    // TODO: Log
                    return;
                }

                var buildKey = _buildInfo["Build Key"].FirstOrDefault();
                if (string.IsNullOrEmpty(buildKey))
                    throw new InvalidOperationException(".build.info is missing a build key");

                var buildConfigPath = Path.Combine(InstallPath, @"Data\config", buildKey.Substring(0, 2),
                    buildKey.Substring(2, 2), buildKey);
                using (var buildConfig = new StreamReader(File.OpenRead(buildConfigPath)))
                    _buildConfig.Load(buildConfig);

                // Load index
                OnLoadProgress?.Invoke("Loading index table ...");
                for (var i = 0; i < 0x10; ++i)
                {
                    var indexFile = Directory.GetFiles(DataPath, $"{i:X2}*.idx").Last();
                    using (var indexFileStream = File.OpenRead(indexFile))
                    {
                        var stopwatch = Stopwatch.StartNew();
                        Index.FromStream(indexFileStream);
                        stopwatch.Stop();
#if DEBUG
                        Console.WriteLine("[DEBUG] Index loaded in {0} ms", stopwatch.ElapsedMilliseconds);
#endif
                    }
                }

                // Load encoding
                OnLoadProgress?.Invoke("Loading encoding table ...");
                var encodingKey = _buildConfig["encoding"].ElementAtOrDefault(1).ToByteArray(9);

                var encodingBinary = new Binary(encodingKey);
                var encodingIndexEntry = Index[encodingBinary];
                if (encodingIndexEntry == null)
                    throw new InvalidOperationException("Encoding file not found");

                var encodingDataFile = GetDataFile(encodingIndexEntry.ArchiveIndex);

                using (
                    var blteStream = new BLTE(encodingDataFile, encodingIndexEntry.Offset + 30,
                        encodingIndexEntry.Size - 30))
                {
                    var stopwatch = Stopwatch.StartNew();
                    if (!Encoding.FromStream(blteStream))
                        throw new InvalidOperationException("Unable to parse encoding file!");
                    stopwatch.Stop();
#if DEBUG
                    Console.WriteLine("[DEBUG] Encoding loaded in {0} ms", stopwatch.ElapsedMilliseconds);
#endif
                }

                // Load root
                OnLoadProgress?.Invoke("Loading root table ...");
                var rootKey = _buildConfig["root"].FirstOrDefault().ToByteArray();

                Encoding.Entry encodingEntry;
                if (!Encoding.TryGetValue(rootKey, out encodingEntry) || encodingEntry.Keys.Length == 0)
                    throw new InvalidOperationException("Unable to find encoding value for root file");

                IndexStore.Record rootIndexEntry = null;
                for (var i = 0; i < encodingEntry.Keys.Length && rootIndexEntry == null; ++i)
                    rootIndexEntry = Index[encodingEntry.Keys[i]];

                if (rootIndexEntry == null)
                    throw new InvalidOperationException("Root index entry not found!");

                using (
                    var blteStream = new BLTE(GetDataFile(rootIndexEntry.ArchiveIndex),
                        rootIndexEntry.Offset + 30, rootIndexEntry.Size - 30))
                {
                    var stopwatch = Stopwatch.StartNew();
                    if (!Root.FromStream(blteStream, Index, Encoding))
                        throw new InvalidOperationException("Unable to read root!");
                    stopwatch.Stop();
#if DEBUG
                    Console.WriteLine("[DEBUG] Root loaded in {0} ms", stopwatch.ElapsedMilliseconds);
#endif
                }
                OnLoadComplete?.Invoke();
            });
        }

        private static Stream GetDataFile(int fileIndex)
        {
            if (!_dataFiles.ContainsKey(fileIndex))
                _dataFiles[fileIndex] = File.OpenRead(Path.Combine(DataPath, $"data.{fileIndex:D3}"));
            return _dataFiles[fileIndex];
        }

        public static BLTE OpenFile(string fileName) => OpenFile(_hasher.ComputeHash(fileName));

        public static BLTE OpenFile(ulong fileHash)
        {
            if (!Root.ContainsKey(fileHash))
                return null;

            return Root[fileHash].Where(entry => Encoding.ContainsKey(entry.MD5))
                .Select(rootEntry => OpenFile(Encoding[rootEntry.MD5]))
                .FirstOrDefault(fileStream => fileStream != null);
        }

        public static BLTE OpenFile(uint fileDataID)
        {
            var rootEntries = from kv in Root from fvr in kv.Value where fileDataID == fvr.FileDataID select fvr;
            return rootEntries.Where(entry => Encoding.ContainsKey(entry.MD5))
                .Select(rootEntry => OpenFile(Encoding[rootEntry.MD5]))
                .FirstOrDefault(fileStream => fileStream != null);
        }

        private static BLTE OpenFile(Encoding.Entry encodingEntry)
        {
            if (encodingEntry.Keys.Length == 0)
                return null;

            IndexStore.Record indexEntry = null;
            for (var i = 0; i < encodingEntry.Keys.Length && indexEntry == null; ++i)
                if (Index.ContainsKey(encodingEntry.Keys[i]))
                    indexEntry = Index[encodingEntry.Keys[i]];

            if (indexEntry == null)
                return null;

            var dataFile = GetDataFile(indexEntry.ArchiveIndex);
            return new BLTE(dataFile, indexEntry.Offset + 30, indexEntry.Size - 30);
        }
    }
}
