// Plywood.Graphics.Core/PwFence.cs
namespace Plywood.Graphics
{
    public class PwFence
    {
        internal object PlatformFence { get; set; }
        public bool IsSignaled { get; internal set; }
        
        internal PwFence(object platformFence)
        {
            PlatformFence = platformFence;
            IsSignaled = false;
        }
        
        public void Wait()
        {
            if (!IsSignaled)
            {
                WaitInternal();
                IsSignaled = true;
            }
        }
        
        public void Reset()
        {
            IsSignaled = false;
            ResetInternal();
        }
        
        internal virtual void WaitInternal()
        {
            // プラットフォーム依存層で実装
        }
        
        internal virtual void ResetInternal()
        {
            // プラットフォーム依存層で実装
        }
    }
}