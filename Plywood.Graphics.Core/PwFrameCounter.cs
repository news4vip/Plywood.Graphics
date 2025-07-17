// Plywood.Graphics.Core/PwFrameCounter.cs
namespace Plywood.Graphics
{
    public static class PwFrameCounter
    {
        private static long frameCount = 0;
        
        public static long Current => frameCount;
        
        internal static void Increment() => Interlocked.Increment(ref frameCount);
    }
}