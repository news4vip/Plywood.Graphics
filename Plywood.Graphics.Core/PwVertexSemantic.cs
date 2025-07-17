// Plywood.Graphics.Core/PwVertexSemantic.cs
namespace Plywood.Graphics
{
    /// <summary>
    /// 頂点属性のセマンティック種別
    /// </summary>
    public enum PwVertexSemantic
    {
        Position = 0,
        Normal = 1,
        Tangent = 2,
        Binormal = 3,
        TexCoord = 4,
        Color = 5,
        BlendWeight = 6,
        BlendIndices = 7,
        InstanceTransform = 8,
        InstanceColor = 9,
        InstanceScale = 10
    }

    /// <summary>
    /// 頂点属性フォーマット
    /// </summary>
    public enum PwVertexFormat
    {
        Float,
        Float2,
        Float3,
        Float4,
        Int,
        Int2,
        Int3,
        Int4,
        UInt,
        UInt2,
        UInt3,
        UInt4,
        Short2,
        Short4,
        UShort2,
        UShort4,
        Byte4,
        UByte4,
        Matrix4x4
    }

    /// <summary>
    /// 頂点属性を示すアトリビュート
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PwVertexAttribute : Attribute
    {
        public PwVertexSemantic Semantic { get; }
        public int Index { get; }
        
        public PwVertexAttribute(PwVertexSemantic semantic, int index = 0)
        {
            Semantic = semantic;
            Index = index;
        }
    }
}