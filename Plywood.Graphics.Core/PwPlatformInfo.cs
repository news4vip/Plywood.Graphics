// Plywood.Graphics.Core/PwPlatformInfo.cs
using System.Runtime.InteropServices;

namespace Plywood.Graphics
{
    /// <summary>
    /// グラフィックス API の種類
    /// </summary>
    public enum PwGraphicsApi
    {
        /// <summary>Apple Metal</summary>
        Metal,
        /// <summary>Khronos Vulkan</summary>
        Vulkan,
        /// <summary>Microsoft DirectX 12</summary>
        DirectX12,
        /// <summary>Khronos OpenGL</summary>
        OpenGL
    }
    
    /// <summary>
    /// プラットフォームの種類
    /// </summary>
    public enum PwPlatform
    {
        /// <summary>Apple iOS</summary>
        iOS,
        /// <summary>Google Android</summary>
        Android,
        /// <summary>Microsoft Windows</summary>
        Windows,
        /// <summary>Apple macOS</summary>
        macOS,
        /// <summary>Linux</summary>
        Linux
    }
    
    /// <summary>
    /// プラットフォーム情報
    /// 実行環境に応じた機能制限や最適化の判断に使用
    /// </summary>
    public class PwPlatformInfo
    {
        /// <summary>プラットフォーム</summary>
        public PwPlatform Platform { get; set; }
        
        /// <summary>グラフィックス API</summary>
        public PwGraphicsApi GraphicsApi { get; set; }
        
        /// <summary>API バージョン</summary>
        public string Version { get; set; } = "1.0";
        
        /// <summary>ハードウェアインスタンシング対応</summary>
        public bool SupportsHardwareInstancing { get; set; }
        
        /// <summary>マルチプルレンダーターゲット対応</summary>
        public bool SupportsMultipleRenderTargets { get; set; }
        
        /// <summary>コンピュートシェーダー対応</summary>
        public bool SupportsComputeShaders { get; set; }
        
        /// <summary>テッセレーション対応</summary>
        public bool SupportsTessellation { get; set; }
        
        /// <summary>最大テクスチャサイズ</summary>
        public int MaxTextureSize { get; set; } = 4096;
        
        /// <summary>最大同時レンダーターゲット数</summary>
        public int MaxRenderTargets { get; set; } = 4;
        
        /// <summary>現在のプラットフォーム情報</summary>
        public static PwPlatformInfo Current => DetectCurrentPlatform();
        
        /// <summary>
        /// 現在のプラットフォームを自動検出
        /// </summary>
        /// <returns>プラットフォーム情報</returns>
        private static PwPlatformInfo DetectCurrentPlatform()
        {
            #if __IOS__
            return new PwPlatformInfo
            {
                Platform = PwPlatform.iOS,
                GraphicsApi = PwGraphicsApi.Metal,
                Version = "3.0",
                SupportsHardwareInstancing = true,
                SupportsMultipleRenderTargets = true,
                SupportsComputeShaders = true,
                SupportsTessellation = true,
                MaxTextureSize = 16384,
                MaxRenderTargets = 8
            };
            #elif __ANDROID__
            return new PwPlatformInfo
            {
                Platform = PwPlatform.Android,
                GraphicsApi = PwGraphicsApi.Vulkan,
                Version = "1.3",
                SupportsHardwareInstancing = true,
                SupportsMultipleRenderTargets = true,
                SupportsComputeShaders = true,
                SupportsTessellation = true,
                MaxTextureSize = 8192,
                MaxRenderTargets = 4
            };
            #elif WINDOWS
            return new PwPlatformInfo
            {
                Platform = PwPlatform.Windows,
                GraphicsApi = PwGraphicsApi.DirectX12,
                Version = "12.0",
                SupportsHardwareInstancing = true,
                SupportsMultipleRenderTargets = true,
                SupportsComputeShaders = true,
                SupportsTessellation = true,
                MaxTextureSize = 16384,
                MaxRenderTargets = 8
            };
            #else
            return new PwPlatformInfo
            {
                Platform = PwPlatform.Linux,
                GraphicsApi = PwGraphicsApi.Vulkan,
                Version = "1.3",
                SupportsHardwareInstancing = true,
                SupportsMultipleRenderTargets = true,
                SupportsComputeShaders = true,
                SupportsTessellation = false,
                MaxTextureSize = 8192,
                MaxRenderTargets = 4
            };
            #endif
        }
        
        /// <summary>
        /// 指定された機能がサポートされているかチェック
        /// </summary>
        /// <param name="feature">機能名</param>
        /// <returns>サポートされている場合true</returns>
        public bool IsFeatureSupported(string feature)
        {
            return feature switch
            {
                "HardwareInstancing" => SupportsHardwareInstancing,
                "MultipleRenderTargets" => SupportsMultipleRenderTargets,
                "ComputeShaders" => SupportsComputeShaders,
                "Tessellation" => SupportsTessellation,
                _ => false
            };
        }
    }
}
