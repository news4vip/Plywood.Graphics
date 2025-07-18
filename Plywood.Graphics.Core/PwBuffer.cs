// Plywood.Graphics.Core/PwBuffer.cs
namespace Plywood.Graphics
{
    /// <summary>
    /// GPU バッファリソースの抽象化クラス
    /// 頂点バッファ、インデックスバッファ、ユニフォームバッファなど全てのバッファを統一的に管理
    /// </summary>
    public class PwBuffer
    {
        /// <summary>
        /// プラットフォーム依存の実際のバッファオブジェクト
        /// iOS: MTLBuffer, Android: VkBuffer など
        /// </summary>
        internal object PlatformBuffer { get; set; }
        
        /// <summary>バッファのバイトサイズ</summary>
        public int Size { get; internal set; }
        
        /// <summary>バッファの用途</summary>
        public PwBufferUsage Usage { get; internal set; }
        
        /// <summary>バッファの作成時刻（フレーム番号）</summary>
        internal long CreatedFrame { get; set; }
        
        /// <summary>最後に使用された時刻（フレーム番号）</summary>
        internal long LastUsedFrame { get; set; }
        
        /// <summary>
        /// バッファのコンストラクタ
        /// </summary>
        /// <param name="platformBuffer">プラットフォーム依存のバッファオブジェクト</param>
        /// <param name="size">バッファサイズ（バイト）</param>
        /// <param name="usage">バッファ用途</param>
        internal PwBuffer(object platformBuffer, int size, PwBufferUsage usage)
        {
            PlatformBuffer = platformBuffer;
            Size = size;
            Usage = usage;
            CreatedFrame = PwFrameCounter.Current;
            LastUsedFrame = PwFrameCounter.Current;
        }
    }
    
    /// <summary>
    /// バッファの用途を示すenum
    /// GPU での最適化やメモリ配置の決定に使用
    /// </summary>
    public enum PwBufferUsage
    {
        /// <summary>頂点データ</summary>
        Vertex,
        /// <summary>インデックスデータ</summary>
        Index,
        /// <summary>ユニフォーム定数</summary>
        Uniform,
        /// <summary>インスタンスデータ</summary>
        Instance
    }
    
    /// <summary>
    /// バッファの特性フラグ
    /// メモリ配置やアクセスパターンの最適化に使用
    /// </summary>
    [Flags]
    public enum PwBufferFlags
    {
        /// <summary>デフォルト</summary>
        None = 0,
        /// <summary>頻繁に更新される</summary>
        Dynamic = 1,
        /// <summary>長期間保持される</summary>
        Persistent = 2,
        /// <summary>CPU/GPU 間で一貫性が必要</summary>
        Coherent = 4
    }
}
