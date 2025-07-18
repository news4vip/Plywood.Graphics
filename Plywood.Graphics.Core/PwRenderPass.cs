// Plywood.Graphics.Core/PwRenderPass.cs
using System.Numerics;

namespace Plywood.Graphics
{
    /// <summary>
    /// レンダーパス
    /// 一連の描画操作とその設定を定義
    /// </summary>
    public class PwRenderPass
    {
        /// <summary>プラットフォーム依存のレンダーパスオブジェクト</summary>
        internal object PlatformRenderPass { get; set; }
        
        /// <summary>レンダーパスの記述子</summary>
        public PwRenderPassDescriptor Descriptor { get; internal set; }
        
        /// <summary>
        /// レンダーパスのコンストラクタ
        /// </summary>
        /// <param name="platformRenderPass">プラットフォーム依存のレンダーパスオブジェクト</param>
        /// <param name="descriptor">レンダーパス記述子</param>
        internal PwRenderPass(object platformRenderPass, PwRenderPassDescriptor descriptor)
        {
            PlatformRenderPass = platformRenderPass;
            Descriptor = descriptor;
        }
    }
    
    /// <summary>
    /// レンダーパス記述子
    /// レンダーパスの全設定を定義
    /// </summary>
    public class PwRenderPassDescriptor
    {
        /// <summary>カラーレンダーターゲット</summary>
        public PwTexture ColorTarget { get; set; }
        
        /// <summary>深度レンダーターゲット</summary>
        public PwTexture DepthTarget { get; set; }
        
        /// <summary>クリアカラー</summary>
        public Vector4 ClearColor { get; set; } = new Vector4(0, 0, 0, 1);
        
        /// <summary>クリア深度値</summary>
        public float ClearDepth { get; set; } = 1.0f;
        
        /// <summary>ロードアクション</summary>
        public PwLoadAction LoadAction { get; set; } = PwLoadAction.Clear;
        
        /// <summary>ストアアクション</summary>
        public PwStoreAction StoreAction { get; set; } = PwStoreAction.Store;
        
        /// <summary>マルチプルレンダーターゲット有効フラグ</summary>
        public bool EnableMultipleRenderTargets { get; set; } = true;
        
        /// <summary>追加のカラーターゲット（MRT用）</summary>
        public PwTexture[] AdditionalColorTargets { get; set; } = Array.Empty<PwTexture>();
        
        /// <summary>
        /// レンダーパスの有効性を検証
        /// </summary>
        /// <returns>有効な場合true</returns>
        public bool IsValid()
        {
            // カラーターゲットまたは深度ターゲットのいずれかが必要
            return ColorTarget != null || DepthTarget != null;
        }
    }
    
    /// <summary>
    /// ロードアクション
    /// レンダーパス開始時のアクション
    /// </summary>
    public enum PwLoadAction
    {
        /// <summary>既存の内容を無視</summary>
        DontCare,
        /// <summary>クリアする</summary>
        Clear,
        /// <summary>既存の内容を保持</summary>
        Load
    }
    
    /// <summary>
    /// ストアアクション
    /// レンダーパス終了時のアクション
    /// </summary>
    public enum PwStoreAction
    {
        /// <summary>結果を保存しない</summary>
        DontCare,
        /// <summary>結果を保存</summary>
        Store
    }
}
