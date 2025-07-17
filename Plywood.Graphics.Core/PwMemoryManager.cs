// Plywood.Graphics.Core/PwMemoryManager.cs
using System.Runtime.InteropServices;

namespace Plywood.Graphics
{
    public class PwMemoryRequirements
    {
        public int Size { get; set; }
        public int Alignment { get; set; }
        public int MinAlignment { get; set; }
        public PwMemoryType MemoryType { get; set; }
    }
    
    public enum PwMemoryType
    {
        VertexBuffer,
        IndexBuffer,
        UniformBuffer,
        Texture
    }
    
    public class PwMemoryManager
    {
        private readonly IPwGraphicsDevice device;
        private readonly Dictionary<Type, PwMemoryRequirements> requirementsCache;
        
        public PwMemoryManager(IPwGraphicsDevice device)
        {
            this.device = device;
            this.requirementsCache = new Dictionary<Type, PwMemoryRequirements>();
        }
        
        // 頂点データ専用の最適化バッファ作成
        public PwBuffer CreateOptimizedVertexBuffer<T>(T[] data) where T : struct, IPwVertexData
        {
            var requirements = GetOrCreateRequirements<T>(PwBufferUsage.Vertex);
            var descriptor = PwVertexDescriptor.CreateFromType<T>();
            var optimizedData = RestructureVertexData(data, descriptor, requirements);
            
            return device.CreateVertexBuffer(optimizedData);
        }
        
        // 汎用最適化バッファ作成（型制約に応じて分岐）
        public PwBuffer CreateOptimizedBuffer<T>(T[] data, PwBufferUsage usage) where T : struct
        {
            var requirements = GetOrCreateRequirements<T>(usage);
            
            return usage switch
            {
                PwBufferUsage.Vertex => CreateOptimizedVertexBufferInternal(data, requirements),
                PwBufferUsage.Index => CreateOptimizedIndexBuffer(data, requirements),
                PwBufferUsage.Uniform => CreateOptimizedUniformBuffer(data, requirements),
                PwBufferUsage.Instance => CreateOptimizedInstanceBuffer(data, requirements),
                _ => throw new ArgumentOutOfRangeException(nameof(usage))
            };
        }
        
        // 内部的な頂点バッファ作成（型制約チェック付き）
        private PwBuffer CreateOptimizedVertexBufferInternal<T>(T[] data, PwMemoryRequirements requirements) where T : struct
        {
            // 頂点データの場合はIPwVertexDataインターフェースが必要
            if (typeof(IPwVertexData).IsAssignableFrom(typeof(T)))
            {
                // 安全なキャスト
                var vertexData = data.Cast<IPwVertexData>().ToArray();
                return CreateVertexBufferFromVertexData(vertexData, requirements);
            }
            else
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} must implement IPwVertexData for vertex buffer creation");
            }
        }
        
        // 頂点データから頂点バッファを作成
        private PwBuffer CreateVertexBufferFromVertexData<T>(T[] data, PwMemoryRequirements requirements) where T : IPwVertexData
        {
            // 型制約を満たすように元の型に戻す
            if (data is IPwVertexData[] vertexArray)
            {
                // リフレクションを使用して適切な型での呼び出し
                var originalType = data.GetType().GetElementType();
                var method = typeof(IPwGraphicsDevice).GetMethod(nameof(IPwGraphicsDevice.CreateVertexBuffer));
                var genericMethod = method.MakeGenericMethod(originalType);
                
                return (PwBuffer)genericMethod.Invoke(device, new object[] { data });
            }
            
            throw new InvalidOperationException("Invalid vertex data type");
        }
        
        // インデックスバッファの最適化作成
        private PwBuffer CreateOptimizedIndexBuffer<T>(T[] data, PwMemoryRequirements requirements) where T : struct
        {
            if (typeof(T) == typeof(ushort))
            {
                var indexData = data.Cast<ushort>().ToArray();
                var optimizedData = RestructureIndexData(indexData, requirements);
                return device.CreateIndexBuffer(optimizedData);
            }
            else if (typeof(T) == typeof(uint))
            {
                // uint を ushort に変換
                var indexData = data.Cast<uint>().Select(x => (ushort)x).ToArray();
                var optimizedData = RestructureIndexData(indexData, requirements);
                return device.CreateIndexBuffer(optimizedData);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported index type: {typeof(T).Name}");
            }
        }
        
        // ユニフォームバッファの最適化作成
        private PwBuffer CreateOptimizedUniformBuffer<T>(T[] data, PwMemoryRequirements requirements) where T : struct
        {
            if (data.Length != 1)
            {
                throw new InvalidOperationException("Uniform buffer must contain exactly one element");
            }
            
            var optimizedData = RestructureUniformData(data[0], requirements);
            return device.CreateUniformBuffer(optimizedData);
        }
        
        // インスタンスバッファの最適化作成
        private PwBuffer CreateOptimizedInstanceBuffer<T>(T[] data, PwMemoryRequirements requirements) where T : struct
        {
            if (typeof(IPwVertexData).IsAssignableFrom(typeof(T)))
            {
                // インスタンスデータも頂点データとして扱う
                return CreateOptimizedVertexBufferInternal(data, requirements);
            }
            else
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} must implement IPwVertexData for instance buffer creation");
            }
        }
        
        // 型安全な頂点データ再構築
        private T[] RestructureVertexData<T>(T[] data, PwVertexDescriptor descriptor, PwMemoryRequirements requirements) where T : struct, IPwVertexData
        {
            // プラットフォーム最適化のための頂点データ再構築
            var optimizedStride = CalculateOptimizedStride(descriptor, requirements);
            
            if (optimizedStride == descriptor.Stride)
            {
                // 最適化が不要な場合はそのまま返す
                return data;
            }
            
            // メモリレイアウト最適化
            var optimizedData = new T[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                optimizedData[i] = OptimizeVertexStructure(data[i], descriptor, requirements);
            }
            
            return optimizedData;
        }
        
        // インデックスデータの再構築
        private ushort[] RestructureIndexData(ushort[] indices, PwMemoryRequirements requirements)
        {
            // インデックスデータの最適化（必要に応じて）
            return indices;
        }
        
        // ユニフォームデータの再構築
        private T RestructureUniformData<T>(T data, PwMemoryRequirements requirements) where T : struct
        {
            // ユニフォームデータの最適化
            return OptimizeUniformStructure(data, requirements);
        }
        
        // 頂点構造体の最適化
        private T OptimizeVertexStructure<T>(T vertex, PwVertexDescriptor descriptor, PwMemoryRequirements requirements) where T : struct, IPwVertexData
        {
            // プラットフォーム固有の最適化
            // 実際の実装では、フィールドの再配置やパディング調整を行う
            return vertex;
        }
        
        // ユニフォーム構造体の最適化
        private T OptimizeUniformStructure<T>(T uniform, PwMemoryRequirements requirements) where T : struct
        {
            // ユニフォーム構造体の最適化
            return uniform;
        }
        
        // 最適化されたストライドの計算
        private int CalculateOptimizedStride(PwVertexDescriptor descriptor, PwMemoryRequirements requirements)
        {
            var baseStride = descriptor.Stride;
            var alignedStride = AlignToRequirement(baseStride, requirements.Alignment);
            
            return Math.Max(alignedStride, requirements.MinAlignment);
        }
        
        // アライメント調整
        private int AlignToRequirement(int size, int alignment)
        {
            return (size + alignment - 1) & ~(alignment - 1);
        }
        
        // メモリ要件の取得またはキャッシュからの取得
        private PwMemoryRequirements GetOrCreateRequirements<T>(PwBufferUsage usage) where T : struct
        {
            var type = typeof(T);
            var cacheKey = $"{type.FullName}_{usage}";
            
            if (!requirementsCache.TryGetValue(type, out var requirements))
            {
                requirements = device.GetMemoryRequirements<T>();
                requirements.MemoryType = usage switch
                {
                    PwBufferUsage.Vertex => PwMemoryType.VertexBuffer,
                    PwBufferUsage.Index => PwMemoryType.IndexBuffer,
                    PwBufferUsage.Uniform => PwMemoryType.UniformBuffer,
                    PwBufferUsage.Instance => PwMemoryType.VertexBuffer,
                    _ => PwMemoryType.VertexBuffer
                };
                
                requirementsCache[type] = requirements;
            }
            
            return requirements;
        }
        
        // メモリ使用量の取得
        public PwMemoryUsageInfo GetMemoryUsage()
        {
            return new PwMemoryUsageInfo
            {
                CacheEntries = requirementsCache.Count,
                TotalCachedTypes = requirementsCache.Keys.Count
            };
        }
        
        // キャッシュのクリア
        public void ClearCache()
        {
            requirementsCache.Clear();
        }
    }
    
    // メモリ使用量情報
    public class PwMemoryUsageInfo
    {
        public int CacheEntries { get; set; }
        public int TotalCachedTypes { get; set; }
    }
}
