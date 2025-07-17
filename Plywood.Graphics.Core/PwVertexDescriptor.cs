// Plywood.Graphics.Core/PwVertexDescriptor.cs
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Plywood.Graphics
{
    public class PwVertexAttributeDescriptor
    {
        public PwVertexSemantic Semantic { get; set; }
        public int Index { get; set; }
        public PwVertexFormat Format { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
    }

    public class PwVertexDescriptor
    {
        public PwVertexAttributeDescriptor[] Attributes { get; set; } = Array.Empty<PwVertexAttributeDescriptor>();
        public int Stride { get; set; }
        public int BindingIndex { get; set; }
        
        public static PwVertexDescriptor CreateFromType<T>() where T : struct, IPwVertexData
        {
            return PwVertexDescriptorCache.GetOrCreate<T>();
        }
        
        internal static PwVertexDescriptor CreateFromTypeInternal(Type vertexType)
        {
            if (!typeof(IPwVertexData).IsAssignableFrom(vertexType))
                throw new ArgumentException($"Type {vertexType.Name} must implement IPwVertexData");
                
            var attributes = new List<PwVertexAttributeDescriptor>();
            int currentOffset = 0;
            
            // フィールドを取得してアトリビュートを処理
            var fields = vertexType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<PwVertexAttribute>();
                if (attribute == null) continue;
                
                var format = PwVertexFormatHelper.GetFormatFromType(field.FieldType);
                var size = PwVertexFormatHelper.GetSizeInBytes(format);
                
                attributes.Add(new PwVertexAttributeDescriptor
                {
                    Semantic = attribute.Semantic,
                    Index = attribute.Index,
                    Format = format,
                    Offset = currentOffset,
                    Size = size
                });
                
                currentOffset += size;
            }
            
            return new PwVertexDescriptor
            {
                Attributes = attributes.ToArray(),
                Stride = currentOffset,
                BindingIndex = 0
            };
        }
    }
    
    /// <summary>
    /// 頂点記述子キャッシュ
    /// </summary>
    public static class PwVertexDescriptorCache
    {
        private static readonly ConcurrentDictionary<Type, PwVertexDescriptor> Cache = new();
        
        public static PwVertexDescriptor GetOrCreate<T>() where T : struct, IPwVertexData
        {
            return Cache.GetOrAdd(typeof(T), PwVertexDescriptor.CreateFromTypeInternal);
        }
    }
}
