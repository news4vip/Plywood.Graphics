// Plywood.Graphics.Core/PwTexture.cs
namespace Plywood.Graphics
{
    public class PwTexture
    {
        internal object PlatformTexture { get; set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public PwTextureFormat Format { get; internal set; }
        
        internal PwTexture(object platformTexture, int width, int height, PwTextureFormat format)
        {
            PlatformTexture = platformTexture;
            Width = width;
            Height = height;
            Format = format;
        }
    }
    
    public enum PwTextureFormat
    {
        RGBA8,
        RGB8,
        Depth32Float,
        Depth24Stencil8
    }
}