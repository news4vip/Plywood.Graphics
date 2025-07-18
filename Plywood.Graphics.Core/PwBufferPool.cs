// Plywood.Graphics.Core/PwBufferPool.cs
using System.Collections.Concurrent;

namespace Plywood.Graphics
{
    public class PwBufferPool
    {
        private readonly IPwGraphicsDevice device;
        private readonly ConcurrentDictionary<PwBufferKey, ConcurrentQueue<PwBuffer>> pools;
        private readonly ConcurrentDictionary<PwBuffer, PwBufferInfo> bufferInfo;
        private readonly object lockObject = new object();
        
        public PwBufferPool(IPwGraphicsDevice device)
        {
            this.device = device;
            this.pools = new ConcurrentDictionary<PwBufferKey, ConcurrentQueue<PwBuffer>>();
            this.bufferInfo = new ConcurrentDictionary<PwBuffer, PwBufferInfo>();
        }
        
        public PwBuffer RentBuffer(int size, PwBufferUsage usage, PwBufferFlags flags = PwBufferFlags.None)
        {
            var key = new PwBufferKey(size, usage, flags);
            var pool = pools.GetOrAdd(key, _ => new ConcurrentQueue<PwBuffer>());
            
            if (pool.TryDequeue(out var buffer))
            {
                // プールから既存バッファを取得
                var info = bufferInfo[buffer];
                info.IsInUse = true;
                info.LastUsedFrame = PwFrameCounter.Current;
                
                return buffer;
            }
            
            // 新しいバッファを作成
            buffer = CreateNewBuffer(size, usage, flags);
            bufferInfo[buffer] = new PwBufferInfo
            {
                Key = key,
                IsInUse = true,
                CreatedFrame = PwFrameCounter.Current,
                LastUsedFrame = PwFrameCounter.Current
            };
            
            return buffer;
        }
        
        public void ReturnBuffer(PwBuffer buffer)
        {
            if (!bufferInfo.TryGetValue(buffer, out var info))
                return;
            
            info.IsInUse = false;
            
            var pool = pools.GetOrAdd(info.Key, _ => new ConcurrentQueue<PwBuffer>());
            pool.Enqueue(buffer);
        }
        
        public void CleanupUnusedBuffers(int maxFramesUnused = 60)
        {
            var currentFrame = PwFrameCounter.Current;
            var buffersToRemove = new List<PwBuffer>();
            
            foreach (var kvp in bufferInfo)
            {
                var buffer = kvp.Key;
                var info = kvp.Value;
                
                if (!info.IsInUse && (currentFrame - info.LastUsedFrame) > maxFramesUnused)
                {
                    buffersToRemove.Add(buffer);
                }
            }
            
            foreach (var buffer in buffersToRemove)
            {
                if (bufferInfo.TryRemove(buffer, out var info))
                {
                    // プラットフォーム依存層でバッファ解放
                    device.DestroyBuffer(buffer);
                }
            }
        }
        
        private PwBuffer CreateNewBuffer(int size, PwBufferUsage usage, PwBufferFlags flags)
        {
            return usage switch
            {
                PwBufferUsage.Vertex => device.CreateVertexBufferRaw(size, flags),
                PwBufferUsage.Index => device.CreateIndexBufferRaw(size, flags),
                PwBufferUsage.Uniform => device.CreateUniformBufferRaw(size, flags),
                PwBufferUsage.Instance => device.CreateInstanceBufferRaw(size, flags),
                _ => throw new ArgumentOutOfRangeException(nameof(usage))
            };
        }
    }
    
    public struct PwBufferKey : IEquatable<PwBufferKey>
    {
        public int Size { get; }
        public PwBufferUsage Usage { get; }
        public PwBufferFlags Flags { get; }
        
        public PwBufferKey(int size, PwBufferUsage usage, PwBufferFlags flags)
        {
            Size = size;
            Usage = usage;
            Flags = flags;
        }
        
        public bool Equals(PwBufferKey other)
        {
            return Size == other.Size && Usage == other.Usage && Flags == other.Flags;
        }
        
        public override bool Equals(object obj)
        {
            return obj is PwBufferKey other && Equals(other);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Size, Usage, Flags);
        }
    }
    
    public class PwBufferInfo
    {
        public PwBufferKey Key { get; set; }
        public bool IsInUse { get; set; }
        public long CreatedFrame { get; set; }
        public long LastUsedFrame { get; set; }
    }
}
