// Plywood.Graphics.Core/PwFence.cs
namespace Plywood.Graphics
{
    /// <summary>
    /// GPU 同期フェンス
    /// CPU と GPU の同期、または複数の GPU 操作間の同期に使用
    /// </summary>
    public class PwFence
    {
        /// <summary>プラットフォーム依存のフェンスオブジェクト</summary>
        internal object PlatformFence { get; set; }
        
        /// <summary>フェンスがシグナル状態かどうか</summary>
        public bool IsSignaled { get; internal set; }
        
        /// <summary>フェンスの作成時刻</summary>
        public DateTime CreatedTime { get; internal set; }
        
        /// <summary>
        /// フェンスのコンストラクタ
        /// </summary>
        /// <param name="platformFence">プラットフォーム依存のフェンスオブジェクト</param>
        internal PwFence(object platformFence)
        {
            PlatformFence = platformFence;
            IsSignaled = false;
            CreatedTime = DateTime.Now;
        }
        
        /// <summary>
        /// フェンスがシグナル状態になるまで待機
        /// </summary>
        public void Wait()
        {
            if (!IsSignaled)
            {
                WaitInternal();
                IsSignaled = true;
            }
        }
        
        /// <summary>
        /// タイムアウト付きでフェンスを待機
        /// </summary>
        /// <param name="timeoutMs">タイムアウト時間（ミリ秒）</param>
        /// <returns>シグナル状態になった場合true、タイムアウトした場合false</returns>
        public bool Wait(int timeoutMs)
        {
            if (IsSignaled)
                return true;
                
            var result = WaitInternal(timeoutMs);
            if (result)
                IsSignaled = true;
                
            return result;
        }
        
        /// <summary>
        /// フェンスをリセット（非シグナル状態に戻す）
        /// </summary>
        public void Reset()
        {
            IsSignaled = false;
            ResetInternal();
        }
        
        /// <summary>
        /// プラットフォーム依存の待機処理
        /// </summary>
        internal virtual void WaitInternal()
        {
            // プラットフォーム依存層で実装
        }
        
        /// <summary>
        /// プラットフォーム依存のタイムアウト付き待機処理
        /// </summary>
        /// <param name="timeoutMs">タイムアウト時間</param>
        /// <returns>成功した場合true</returns>
        internal virtual bool WaitInternal(int timeoutMs)
        {
            // プラットフォーム依存層で実装
            return true;
        }
        
        /// <summary>
        /// プラットフォーム依存のリセット処理
        /// </summary>
        internal virtual void ResetInternal()
        {
            // プラットフォーム依存層で実装
        }
    }
}
