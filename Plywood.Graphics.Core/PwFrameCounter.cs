// Plywood.Graphics.Core/PwFrameCounter.cs
namespace Plywood.Graphics
{
    /// <summary>
    /// フレームカウンター
    /// フレーム番号の管理とリソースの生存期間追跡に使用
    /// </summary>
    public static class PwFrameCounter
    {
        /// <summary>現在のフレーム番号</summary>
        private static long frameCount = 0;
        
        /// <summary>フレーム開始時刻</summary>
        private static DateTime frameStartTime = DateTime.Now;
        
        /// <summary>前フレームからの経過時間</summary>
        private static float deltaTime = 0.0f;
        
        /// <summary>現在のフレーム番号を取得</summary>
        public static long Current => frameCount;
        
        /// <summary>現在のデルタタイムを取得</summary>
        public static float DeltaTime => deltaTime;
        
        /// <summary>フレームレートを取得</summary>
        public static float FrameRate => deltaTime > 0 ? 1.0f / deltaTime : 0.0f;
        
        /// <summary>
        /// フレームカウンターをインクリメント
        /// 各フレームの開始時に呼び出される
        /// </summary>
        /// <param name="currentDeltaTime">フレーム時間</param>
        internal static void Increment(float currentDeltaTime = 0.0f)
        {
            Interlocked.Increment(ref frameCount);
            deltaTime = currentDeltaTime;
            frameStartTime = DateTime.Now;
        }
        
        /// <summary>
        /// フレームカウンターをリセット
        /// </summary>
        internal static void Reset()
        {
            frameCount = 0;
            deltaTime = 0.0f;
            frameStartTime = DateTime.Now;
        }
        
        /// <summary>
        /// 指定されたフレームからの経過フレーム数を取得
        /// </summary>
        /// <param name="fromFrame">基準フレーム番号</param>
        /// <returns>経過フレーム数</returns>
        public static long GetElapsedFrames(long fromFrame)
        {
            return Math.Max(0, Current - fromFrame);
        }
    }
}