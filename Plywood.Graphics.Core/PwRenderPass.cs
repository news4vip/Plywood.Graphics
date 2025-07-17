// Plywood.Graphics.Core/PwRenderPass.cs
using System.Numerics;

namespace Plywood.Graphics
{
    public class PwRenderPass
    {
        internal object PlatformRenderPass { get; set; }
        public PwRenderPassDescriptor Descriptor { get; internal set; }
        
        internal PwRenderPass(object platformRenderPass, PwRenderPassDescriptor descriptor)
        {
            PlatformRenderPass = platformRenderPass;
            Descriptor = descriptor;
        }
    }
    
    public class PwRenderPassDescriptor
    {
        public PwTexture ColorTarget { get; set; }
        public PwTexture DepthTarget { get; set; }
        public Vector4 ClearColor { get; set; } = new Vector4(0, 0, 0, 1);
        public float ClearDepth { get; set; } = 1.0f;
        public PwLoadAction LoadAction { get; set; } = PwLoadAction.Clear;
        public PwStoreAction StoreAction { get; set; } = PwStoreAction.Store;
        public bool EnableMultipleRenderTargets { get; set; } = true;
    }
    
    public enum PwLoadAction
    {
        DontCare,
        Clear,
        Load
    }
    
    public enum PwStoreAction
    {
        DontCare,
        Store
    }
}