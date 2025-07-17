// Plywood.Graphics.Core/PwVertexFormatHelper.cs
using System.Numerics;

namespace Plywood.Graphics
{
    public static class PwVertexFormatHelper
    {
        private static readonly Dictionary<Type, PwVertexFormat> TypeToFormatMap = new()
        {
            [typeof(float)] = PwVertexFormat.Float,
            [typeof(Vector2)] = PwVertexFormat.Float2,
            [typeof(Vector3)] = PwVertexFormat.Float3,
            [typeof(Vector4)] = PwVertexFormat.Float4,
            [typeof(int)] = PwVertexFormat.Int,
            [typeof(uint)] = PwVertexFormat.UInt,
            [typeof(Matrix4x4)] = PwVertexFormat.Matrix4x4,
            [typeof(short)] = PwVertexFormat.Short2,
            [typeof(ushort)] = PwVertexFormat.UShort2,
            [typeof(byte)] = PwVertexFormat.Byte4,
        };
        
        public static PwVertexFormat GetFormatFromType(Type type)
        {
            if (TypeToFormatMap.TryGetValue(type, out var format))
                return format;
                
            throw new NotSupportedException($"Unsupported vertex attribute type: {type.Name}");
        }
        
        public static int GetSizeInBytes(PwVertexFormat format)
        {
            return format switch
            {
                PwVertexFormat.Float => 4,
                PwVertexFormat.Float2 => 8,
                PwVertexFormat.Float3 => 12,
                PwVertexFormat.Float4 => 16,
                PwVertexFormat.Int => 4,
                PwVertexFormat.UInt => 4,
                PwVertexFormat.Matrix4x4 => 64,
                PwVertexFormat.Short2 => 4,
                PwVertexFormat.UShort2 => 4,
                PwVertexFormat.Byte4 => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(format))
            };
        }
    }
}