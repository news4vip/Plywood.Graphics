// Plywood.Graphics.Core/PwVertexFormatHelper.cs
using System.Numerics;

namespace Plywood.Graphics
{
    /// <summary>
    /// 頂点フォーマット変換と型情報管理のヘルパークラス
    /// C# の型システムと GPU の頂点フォーマットの橋渡しを行う
    /// </summary>
    public static class PwVertexFormatHelper
    {
        /// <summary>
        /// C# の型から GPU 頂点フォーマットへのマッピングテーブル
        /// コンパイル時に確定する静的マッピングにより、実行時のオーバーヘッドを最小化
        /// </summary>
        private static readonly Dictionary<Type, PwVertexFormat> TypeToFormatMap = new()
        {
            // 基本型
            [typeof(float)] = PwVertexFormat.Float,
            // System.Numerics ベクトル型
            [typeof(Vector2)] = PwVertexFormat.Float2,
            [typeof(Vector3)] = PwVertexFormat.Float3,
            [typeof(Vector4)] = PwVertexFormat.Float4,
            // 整数型
            [typeof(int)] = PwVertexFormat.Int,
            [typeof(uint)] = PwVertexFormat.UInt,
            // 行列型
            [typeof(Matrix4x4)] = PwVertexFormat.Matrix4x4,
            // 短精度型
            [typeof(short)] = PwVertexFormat.Short2,
            [typeof(ushort)] = PwVertexFormat.UShort2,
            [typeof(byte)] = PwVertexFormat.Byte4,
        };
        
        /// <summary>
        /// C# の型から対応する GPU 頂点フォーマットを取得
        /// </summary>
        /// <param name="type">C# の型</param>
        /// <returns>対応する頂点フォーマット</returns>
        /// <exception cref="NotSupportedException">サポートされていない型の場合</exception>
        public static PwVertexFormat GetFormatFromType(Type type)
        {
            if (TypeToFormatMap.TryGetValue(type, out var format))
                return format;
                
            throw new NotSupportedException($"Unsupported vertex attribute type: {type.Name}");
        }
        
        /// <summary>
        /// 頂点フォーマットのバイトサイズを取得
        /// GPU メモリレイアウトの計算に使用
        /// </summary>
        /// <param name="format">頂点フォーマット</param>
        /// <returns>バイト単位のサイズ</returns>
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
                PwVertexFormat.Matrix4x4 => 64,  // 4x4 float 行列
                PwVertexFormat.Short2 => 4,
                PwVertexFormat.UShort2 => 4,
                PwVertexFormat.Byte4 => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(format))
            };
        }
        
        /// <summary>
        /// 頂点フォーマットのアライメント要件を取得
        /// GPU アーキテクチャによってはアライメントが性能に大きく影響する
        /// </summary>
        /// <param name="format">頂点フォーマット</param>
        /// <returns>必要なアライメント（バイト）</returns>
        public static int GetAlignment(PwVertexFormat format)
        {
            return format switch
            {
                PwVertexFormat.Float => 4,
                PwVertexFormat.Float2 => 8,
                PwVertexFormat.Float3 => 4,    // 通常 4 バイトアライメント
                PwVertexFormat.Float4 => 16,
                PwVertexFormat.Matrix4x4 => 16,
                _ => 4  // デフォルト 4 バイトアライメント
            };
        }
    }
}
