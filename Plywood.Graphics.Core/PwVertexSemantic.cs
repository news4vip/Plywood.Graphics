// Plywood.Graphics.Core/PwVertexSemantic.cs
namespace Plywood.Graphics
{
    /// <summary>
    /// 頂点属性のセマンティック種別
    /// GPU シェーダーでの頂点属性の用途を定義
    /// Metal や Vulkan など、異なるグラフィックス API 間で統一された意味を持つ
    /// </summary>
    public enum PwVertexSemantic
    {
        /// <summary>位置座標 (通常は Vector3)</summary>
        Position = 0,
        /// <summary>法線ベクトル (通常は Vector3)</summary>
        Normal = 1,
        /// <summary>接線ベクトル (通常は Vector3)</summary>
        Tangent = 2,
        /// <summary>従法線ベクトル (通常は Vector3)</summary>
        Binormal = 3,
        /// <summary>テクスチャ座標 (通常は Vector2)</summary>
        TexCoord = 4,
        /// <summary>頂点カラー (通常は Vector4)</summary>
        Color = 5,
        /// <summary>ブレンドウェイト (スキニング用)</summary>
        BlendWeight = 6,
        /// <summary>ブレンドインデックス (スキニング用)</summary>
        BlendIndices = 7,
        /// <summary>インスタンス変換行列 (通常は Matrix4x4)</summary>
        InstanceTransform = 8,
        /// <summary>インスタンスカラー (通常は Vector4)</summary>
        InstanceColor = 9,
        /// <summary>インスタンススケール (通常は float)</summary>
        InstanceScale = 10
    }

    /// <summary>
    /// 頂点属性のデータフォーマット
    /// GPU での実際のデータ型を定義し、メモリレイアウトを決定する
    /// </summary>
    public enum PwVertexFormat
    {
        Float,      // 32bit 浮動小数点
        Float2,     // 32bit 浮動小数点 x2
        Float3,     // 32bit 浮動小数点 x3
        Float4,     // 32bit 浮動小数点 x4
        Int,        // 32bit 符号付き整数
        Int2,       // 32bit 符号付き整数 x2
        Int3,       // 32bit 符号付き整数 x3
        Int4,       // 32bit 符号付き整数 x4
        UInt,       // 32bit 符号なし整数
        UInt2,      // 32bit 符号なし整数 x2
        UInt3,      // 32bit 符号なし整数 x3
        UInt4,      // 32bit 符号なし整数 x4
        Short2,     // 16bit 符号付き整数 x2
        Short4,     // 16bit 符号付き整数 x4
        UShort2,    // 16bit 符号なし整数 x2
        UShort4,    // 16bit 符号なし整数 x4
        Byte4,      // 8bit 符号付き整数 x4
        UByte4,     // 8bit 符号なし整数 x4
        Matrix4x4   // 4x4 行列 (64 bytes)
    }

    /// <summary>
    /// 頂点属性を示すカスタムアトリビュート
    /// C# の構造体フィールドに付与することで、GPU 頂点シェーダーでの
    /// 入力レイアウトを自動的に生成する
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PwVertexAttribute : Attribute
    {
        /// <summary>頂点属性のセマンティック（用途）</summary>
        public PwVertexSemantic Semantic { get; }
        
        /// <summary>同じセマンティックでの複数属性の区別用インデックス</summary>
        public int Index { get; }
        
        /// <summary>
        /// 頂点属性アトリビュートのコンストラクタ
        /// </summary>
        /// <param name="semantic">属性のセマンティック</param>
        /// <param name="index">同一セマンティック内でのインデックス（デフォルト0）</param>
        public PwVertexAttribute(PwVertexSemantic semantic, int index = 0)
        {
            Semantic = semantic;
            Index = index;
        }
    }
}
