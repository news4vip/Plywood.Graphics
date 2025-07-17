// Plywood.Graphics.Core/PwPlatformInfo.cs
namespace Plywood.Graphics
{
    public enum PwGraphicsApi
    {
        Metal,
        Vulkan,
        DirectX12,
        OpenGL
    }
    
    public enum PwPlatform
    {
        iOS,
        Android,
        Windows,
        macOS,
        Linux
    }
    
    public class PwPlatformInfo
    {
        public PwPlatform Platform { get; set; }
        public PwGraphicsApi GraphicsApi { get; set; }
        public string Version { get; set; }
        public bool SupportsHardwareInstancing { get; set; }
        public bool SupportsMultipleRenderTargets { get; set; }
        public bool SupportsComputeShaders { get; set; }
        
        public static PwPlatformInfo Current => DetectCurrentPlatform();
        
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
                SupportsComputeShaders = true
            };
#elif __ANDROID__
            return new PwPlatformInfo
            {
                Platform = PwPlatform.Android,
                GraphicsApi = PwGraphicsApi.Vulkan,
                Version = "1.3",
                SupportsHardwareInstancing = true,
                SupportsMultipleRenderTargets = true,
                SupportsComputeShaders = true
            };
#else
            return new PwPlatformInfo
            {
                Platform = PwPlatform.Windows,
                GraphicsApi = PwGraphicsApi.DirectX12,
                Version = "12.0",
                SupportsHardwareInstancing = true,
                SupportsMultipleRenderTargets = true,
                SupportsComputeShaders = true
            };
#endif
        }
    }
}