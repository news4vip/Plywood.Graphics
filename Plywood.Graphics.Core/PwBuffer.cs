// Plywood.Graphics.Core/PwBuffer.cs
namespace Plywood.Graphics
{
    public class PwBuffer
    {
        internal object PlatformBuffer { get; set; }
        public int Size { get; internal set; }
        public PwBufferUsage Usage { get; internal set; }
        
        internal PwBuffer(object platformBuffer, int size, PwBufferUsage usage)
        {
            PlatformBuffer = platformBuffer;
            Size = size;
            Usage = usage;
        }
    }
    
    public enum PwBufferUsage
    {
        Vertex,
        Index,
        Uniform,
        Instance
    }
}