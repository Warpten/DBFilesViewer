using System;
using System.Collections.Generic;
using DBFilesViewer.Data.IO.CASC;

namespace DBFilesViewer.Graphics.Scene
{
    public class TextureWorkItem
    {
        public Texture Texture { get; }
        public string FileName { get; }
        public uint FileDataID { get; }
        public bool RootLookup { get; }

        public TextureWorkItem(string file, Texture tex)
        {
            Texture = tex;
            FileName = file;
        }

        public TextureWorkItem(uint fileDataID, Texture tex)
        {
            Texture = tex;
            FileDataID = fileDataID;
            RootLookup = true;
        }
    }


    /// <remarks>
    /// https://bitbucket.org/mugadr_m/neo
    /// </remarks>
    public class TextureManager : IDisposable
    {
        private GxContext mContext;
        private Dictionary<int, Texture> mCache = new Dictionary<int, Texture>();
        private List<TextureWorkItem> mWorkItems = new List<TextureWorkItem>();

        public TextureManager(GxContext context)
        {
            mContext = context;
        }

        ~TextureManager()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Shutdown()
        {
            lock (mWorkItems)
                mWorkItems.Clear();
        }

        public void Dispose(bool disposing)
        {
            Shutdown();
            lock (mCache)
            {
                foreach (var @ref in mCache)
                    @ref.Value.Dispose();
                mCache.Clear();
            }
        }

        public Texture GetTexture(uint fileDataID)
        {
            Texture retTexture;
            var hash = fileDataID.GetHashCode();
            lock (mCache)
            {
                if (mCache.TryGetValue(hash, out retTexture))
                    return retTexture;

                retTexture = new Texture(mContext);
                mCache.Add(hash, retTexture);
                var workItem = new TextureWorkItem(fileDataID, retTexture);
                if (!workItem.RootLookup)
                {
                    var loadInfo = TextureReader.Load(workItem.FileName);
                    if (loadInfo != null)
                        workItem.Texture.LoadFromLoadInfo(loadInfo);
                }
                else
                {
                    var loadInfo = TextureReader.Load(workItem.FileDataID);
                    if (loadInfo != null)
                        workItem.Texture.LoadFromLoadInfo(loadInfo);
                }
            }

            return retTexture;
        }

        public Texture GetTexture(string path)
        {
            if (string.IsNullOrEmpty(path))
                return new Texture(mContext);

            var hash = JenkinsHashing.Instance.ComputeHash(path).GetHashCode();
            Texture retTexture;
            lock (mCache)
            {
                if (mCache.TryGetValue(hash, out retTexture))
                    return retTexture;
                
                retTexture = new Texture(mContext);
                mCache.Add(hash, retTexture);
                var workItem = new TextureWorkItem(path, retTexture);
                if (!workItem.RootLookup)
                {
                    var loadInfo = TextureReader.Load(workItem.FileName);
                    if (loadInfo != null)
                        workItem.Texture.LoadFromLoadInfo(loadInfo);
                }
                else
                {
                    var loadInfo = TextureReader.Load(workItem.FileDataID);
                    if (loadInfo != null)
                        workItem.Texture.LoadFromLoadInfo(loadInfo);
                }
            }

            return retTexture;
        }
    }
}
