// Plywood.Graphics.Core/PwTexture.cs
namespace Plywood.Graphics
{
    /// <summary>
    /// GPU テクスチャリソースの抽象化クラス
    /// 2D テクスチャ、レンダーターゲット、深度バッファなどを統一的に管理
    /// </summary>
    public class PwTexture
    {
        /// <summary>
        /// プラットフォーム依存の実際のテクスチャオブジェクト
        /// iOS: MTLTexture, Android: VkImage など
        /// </summary>
        internal object PlatformTexture { get; set; }
        
        /// <summary>テクスチャの幅（ピクセル）</summary>
        public int Width { get; internal set; }
        
        /// <summary>テクスチャの高さ（ピクセル）</summary>
        public int Height { get; internal set; }
        
        /// <summary>テクスチャのピクセルフォーマット</summary>
        public PwTextureFormat Format { get; internal set; }
        
        /// <summary>ミップマップレベル数</summary>
        public int MipLevels { get; internal set; }
        
        /// <summary>テクスチャの用途</summary>
        public PwTextureUsage Usage { get; internal set; }
        
        /// <summary>
        /// テクスチャのコンストラクタ
        /// </summary>
        /// <param name="platformTexture">プラットフォーム依存のテクスチャオブジェクト</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="format">ピクセルフォーマット</param>
        internal PwTexture(object platformTexture, int width, int height, PwTextureFormat format)
        {
            PlatformTexture = platformTexture;
            Width = width;
            Height = height;
            Format = format;
            MipLevels = 1;
            Usage = PwTextureUsage.ShaderRead;
        }
        
        /// <summary>
        /// テクスチャのメモリ使用量を計算
        /// </summary>
        /// <returns>バイト単位のメモリ使用量</returns>
        public int CalculateMemoryUsage()
        {
            var bytesPerPixel = GetBytesPerPixel(Format);
            var totalSize = 0;
            
            // ミップマップを含むサイズを計算
            for (int mip = 0; mip < MipLevels; mip++)
            {
                var mipWidth = Math.Max(1, Width >> mip);
                var mipHeight = Math.Max(1, Height >> mip);
                totalSize += mipWidth * mipHeight * bytesPerPixel;
            }
            
            return totalSize;
        }
        
        /// <summary>
        /// フォーマットあたりのバイト数を取得
        /// </summary>
        private int GetBytesPerPixel(PwTextureFormat format)
        {
            return format switch
            {
                PwTextureFormat.RGBA8 => 4,
                PwTextureFormat.RGB8 => 3,
                PwTextureFormat.Depth32Float => 4,
                PwTextureFormat.Depth24Stencil8 => 4,
                _ => 4
            };
        }
    }
    
    /// <summary>
    /// テクスチャのピクセルフォーマット
    /// GPU での実際のメモリ配置を決定
    /// </summary>
    public enum PwTextureFormat
    {
        /// <summary>RGBA 8bit 各成分</summary>
        RGBA8,
        /// <summary>RGB 8bit 各成分</summary>
        RGB8,
        /// <summary>32bit 浮動小数点深度</summary>
        Depth32Float,
        /// <summary>24bit 深度 + 8bit ステンシル</summary>
        Depth24Stencil8
    }
    
    /// <summary>
    /// テクスチャの用途
    /// GPU での最適化に使用
    /// </summary>
    [Flags]
    public enum PwTextureUsage
    {
        /// <summary>シェーダーでの読み込み</summary>
        ShaderRead = 1,
        /// <summary>レンダーターゲット</summary>
        RenderTarget = 2,
        /// <summary>深度ステンシル</summary>
        DepthStencil = 4
    }
}
