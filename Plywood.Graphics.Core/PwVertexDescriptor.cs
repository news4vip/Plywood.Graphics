// Plywood.Graphics.Core/PwVertexDescriptor.cs
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Plywood.Graphics
{
    /// <summary>
    /// 頂点属性記述子
    /// GPU シェーダーでの頂点入力レイアウトを定義
    /// </summary>
    public class PwVertexAttributeDescriptor
    {
        /// <summary>属性のセマンティック</summary>
        public PwVertexSemantic Semantic { get; set; }
        
        /// <summary>セマンティック内でのインデックス</summary>
        public int Index { get; set; }
        
        /// <summary>GPU での実際のデータフォーマット</summary>
        public PwVertexFormat Format { get; set; }
        
        /// <summary>頂点構造体内でのバイトオフセット</summary>
        public int Offset { get; set; }
        
        /// <summary>この属性のバイトサイズ</summary>
        public int Size { get; set; }
    }

    /// <summary>
    /// 頂点記述子
    /// 頂点バッファの全体的なレイアウトを定義
    /// </summary>
    public class PwVertexDescriptor
    {
        /// <summary>頂点属性の配列</summary>
        public PwVertexAttributeDescriptor[] Attributes { get; set; } = Array.Empty<PwVertexAttributeDescriptor>();
        
        /// <summary>1頂点あたりのバイト数（ストライド）</summary>
        public int Stride { get; set; }
        
        /// <summary>頂点バッファのバインディングインデックス</summary>
        public int BindingIndex { get; set; }
        
        /// <summary>
        /// ジェネリック型から頂点記述子を自動生成
        /// </summary>
        /// <typeparam name="T">頂点データ型</typeparam>
        /// <returns>生成された頂点記述子</returns>
        public static PwVertexDescriptor CreateFromType<T>() where T : struct, IPwVertexData
        {
            return PwVertexDescriptorCache.GetOrCreate<T>();
        }
        
        /// <summary>
        /// 型情報から頂点記述子を生成する内部メソッド
        /// リフレクションを使用して構造体フィールドを解析
        /// </summary>
        /// <param name="vertexType">頂点データ型</param>
        /// <returns>生成された頂点記述子</returns>
        internal static PwVertexDescriptor CreateFromTypeInternal(Type vertexType)
        {
            // IPwVertexData インターフェースの実装チェック
            if (!typeof(IPwVertexData).IsAssignableFrom(vertexType))
                throw new ArgumentException($"Type {vertexType.Name} must implement IPwVertexData");
                
            var attributes = new List<PwVertexAttributeDescriptor>();
            int currentOffset = 0;
            
            // 構造体のフィールドを取得（publicインスタンスフィールドのみ）
            var fields = vertexType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            // 各フィールドを順番に処理
            foreach (var field in fields)
            {
                // PwVertexAttribute の取得
                var attribute = field.GetCustomAttribute<PwVertexAttribute>();
                if (attribute == null) continue;  // アトリビュートがない場合はスキップ
                
                // C# 型から GPU フォーマットへの変換
                var format = PwVertexFormatHelper.GetFormatFromType(field.FieldType);
                var size = PwVertexFormatHelper.GetSizeInBytes(format);
                
                // 属性記述子を生成
                attributes.Add(new PwVertexAttributeDescriptor
                {
                    Semantic = attribute.Semantic,
                    Index = attribute.Index,
                    Format = format,
                    Offset = currentOffset,
                    Size = size
                });
                
                // 次のフィールドのオフセットを計算
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
    /// 同じ型の頂点記述子を再利用することで、リフレクションのオーバーヘッドを削減
    /// </summary>
    public static class PwVertexDescriptorCache
    {
        /// <summary>
        /// 型をキーとした頂点記述子のキャッシュ
        /// ConcurrentDictionary を使用してスレッドセーフに実装
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PwVertexDescriptor> Cache = new();
        
        /// <summary>
        /// 型から頂点記述子を取得（キャッシュから取得または新規生成）
        /// </summary>
        /// <typeparam name="T">頂点データ型</typeparam>
        /// <returns>頂点記述子</returns>
        public static PwVertexDescriptor GetOrCreate<T>() where T : struct, IPwVertexData
        {
            return Cache.GetOrAdd(typeof(T), PwVertexDescriptor.CreateFromTypeInternal);
        }
        
        /// <summary>
        /// キャッシュをクリア（メモリリーク防止）
        /// </summary>
        public static void ClearCache()
        {
            Cache.Clear();
        }
    }
}
