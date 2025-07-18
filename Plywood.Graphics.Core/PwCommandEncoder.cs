// Plywood.Graphics.Core/PwCommandEncoder.cs
using System.Numerics;

namespace Plywood.Graphics
{
    /// <summary>
    /// GPU コマンドエンコーダー
    /// 描画コマンドを記録し、バッチ実行する
    /// </summary>
    public class PwCommandEncoder
    {
        /// <summary>グラフィックスデバイス</summary>
        private readonly IPwGraphicsDevice device;
        
        /// <summary>記録されたコマンドリスト</summary>
        private readonly List<PwCommand> commands;
        
        /// <summary>現在のレンダーパス</summary>
        private PwRenderPass currentRenderPass;
        
        /// <summary>現在のパイプライン</summary>
        private PwPipeline currentPipeline;
        
        /// <summary>記録状態フラグ</summary>
        private bool isRecording;
        
        /// <summary>デバッグ情報</summary>
        private string debugLabel;
        
        /// <summary>
        /// コマンドエンコーダーのコンストラクタ
        /// </summary>
        /// <param name="device">グラフィックスデバイス</param>
        internal PwCommandEncoder(IPwGraphicsDevice device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            this.commands = new List<PwCommand>();
            this.debugLabel = "PlywoodCommandEncoder";
        }
        
        /// <summary>
        /// コマンド記録を開始
        /// </summary>
        public virtual void Begin()
        {
            if (isRecording)
                throw new InvalidOperationException("Command encoder is already recording");
                
            commands.Clear();
            isRecording = true;
            currentRenderPass = null;
            currentPipeline = null;
            
            // フレーム開始処理
            device.BeginFrame();
            
            // フレームカウンターをインクリメント
            PwFrameCounter.Increment();
        }
        
        /// <summary>
        /// コマンド記録を終了し、実行
        /// </summary>
        public virtual void End()
        {
            if (!isRecording)
                throw new InvalidOperationException("Command encoder is not recording");
                
            // 未終了のレンダーパスを自動終了
            if (currentRenderPass != null)
            {
                EndRenderPass();
            }
            
            // コマンドを実行
            ExecuteCommands();
            
            // フレーム終了処理
            device.EndFrame();
            isRecording = false;
        }
        
        /// <summary>
        /// デバッグラベルを設定
        /// </summary>
        /// <param name="label">ラベル</param>
        public void SetDebugLabel(string label)
        {
            debugLabel = label ?? "PlywoodCommandEncoder";
        }
        
        #region ビューポート設定
        
        /// <summary>
        /// ビューポートを設定
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        public void SetViewport(int x, int y, int width, int height)
        {
            ValidateRecording();
            var command = new PwViewportCommand(x, y, width, height);
            if (command.IsValid())
            {
                commands.Add(command);
            }
        }
        
        #endregion
        
        #region レンダーパス管理
        
        /// <summary>
        /// レンダーパスを開始（指定されたレンダーターゲットに対して）
        /// </summary>
        /// <param name="renderTarget">レンダーターゲット</param>
        public void BeginRenderPass(PwTexture renderTarget)
        {
            ValidateRecording();
            
            if (currentRenderPass != null)
                throw new InvalidOperationException("Render pass is already active");
                
            var descriptor = new PwRenderPassDescriptor
            {
                ColorTarget = renderTarget,
                ClearColor = new Vector4(0.2f, 0.3f, 0.4f, 1.0f),
                LoadAction = PwLoadAction.Clear,
                StoreAction = PwStoreAction.Store
            };
            
            currentRenderPass = device.CreateRenderPass(descriptor);
            commands.Add(new PwBeginRenderPassCommand(currentRenderPass));
        }
        
        /// <summary>
        /// メインフレームバッファに対するレンダーパスを開始
        /// </summary>
        public void BeginMainRenderPass()
        {
            ValidateRecording();
            
            if (currentRenderPass != null)
                throw new InvalidOperationException("Render pass is already active");
                
            var descriptor = new PwRenderPassDescriptor
            {
                ColorTarget = null, // メインフレームバッファ
                ClearColor = new Vector4(0.1f, 0.2f, 0.3f, 1.0f),
                LoadAction = PwLoadAction.Clear,
                StoreAction = PwStoreAction.Store
            };
            
            currentRenderPass = device.CreateRenderPass(descriptor);
            commands.Add(new PwBeginRenderPassCommand(currentRenderPass));
        }
        
        /// <summary>
        /// 現在のレンダーパスを終了
        /// </summary>
        public void EndRenderPass()
        {
            ValidateRecording();
            
            if (currentRenderPass == null)
                throw new InvalidOperationException("No active render pass");
                
            commands.Add(new PwEndRenderPassCommand(currentRenderPass));
            currentRenderPass = null;
        }
        
        #endregion
        
        #region パイプライン設定
        
        /// <summary>
        /// レンダリングパイプラインを設定
        /// </summary>
        /// <param name="pipeline">パイプライン</param>
        public void SetPipeline(PwPipeline pipeline)
        {
            ValidateRecording();
            
            if (pipeline == null)
                throw new ArgumentNullException(nameof(pipeline));
                
            currentPipeline = pipeline;
            commands.Add(new PwSetPipelineCommand(pipeline));
        }
        
        #endregion
        
        #region バッファ設定
        
        /// <summary>
        /// 頂点バッファを設定
        /// </summary>
        /// <param name="buffer">頂点バッファ</param>
        /// <param name="bindingIndex">バインディングインデックス</param>
        public void SetVertexBuffer(PwBuffer buffer, int bindingIndex = 0)
        {
            ValidateRecording();
            
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
                
            var command = new PwSetVertexBufferCommand(buffer, bindingIndex);
            if (command.IsValid())
            {
                commands.Add(command);
            }
        }
        
        /// <summary>
        /// インデックスバッファを設定
        /// </summary>
        /// <param name="buffer">インデックスバッファ</param>
        public void SetIndexBuffer(PwBuffer buffer)
        {
            ValidateRecording();
            
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
                
            var command = new PwSetIndexBufferCommand(buffer);
            if (command.IsValid())
            {
                commands.Add(command);
            }
        }
        
        /// <summary>
        /// インスタンスバッファを設定
        /// </summary>
        /// <param name="buffer">インスタンスバッファ</param>
        /// <param name="bindingIndex">バインディングインデックス</param>
        public void SetInstanceBuffer(PwBuffer buffer, int bindingIndex = 1)
        {
            ValidateRecording();
            
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
                
            var command = new PwSetVertexBufferCommand(buffer, bindingIndex);
            if (command.IsValid())
            {
                commands.Add(command);
            }
        }
        
        /// <summary>
        /// インスタンスバッファのオフセットを設定
        /// </summary>
        /// <param name="buffer">インスタンスバッファ</param>
        /// <param name="offset">オフセット</param>
        public void SetInstanceBufferOffset(PwBuffer buffer, int offset)
        {
            ValidateRecording();
            
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
                
            var command = new PwSetInstanceBufferOffsetCommand(buffer, offset);
            if (command.IsValid())
            {
                commands.Add(command);
            }
        }
        
        /// <summary>
        /// テクスチャを設定
        /// </summary>
        /// <param name="texture">テクスチャ</param>
        /// <param name="bindingIndex">バインディングインデックス</param>
        public void SetTexture(PwTexture texture, int bindingIndex)
        {
            ValidateRecording();
            
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
                
            commands.Add(new PwSetTextureCommand(texture, bindingIndex));
        }
        
        /// <summary>
        /// ユニフォームバッファを設定
        /// </summary>
        /// <param name="buffer">ユニフォームバッファ</param>
        /// <param name="bindingIndex">バインディングインデックス</param>
        public void SetUniformBuffer(PwBuffer buffer, int bindingIndex)
        {
            ValidateRecording();
            
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
                
            var command = new PwSetUniformBufferCommand(buffer, bindingIndex);
            if (command.IsValid())
            {
                commands.Add(command);
            }
        }
        
        #endregion
        
        #region 描画コマンド
        
        /// <summary>
        /// インデックス付きインスタンス描画
        /// </summary>
        /// <param name="indexCount">インデックス数</param>
        /// <param name="instanceCount">インスタンス数</param>
        /// <param name="firstIndex">最初のインデックス</param>
        public void DrawIndexedInstanced(int indexCount, int instanceCount, int firstIndex = 0)
        {
            ValidateRecording();
            ValidateRenderPass();
            
            var command = new PwDrawIndexedInstancedCommand(indexCount, instanceCount, firstIndex);
            if (command.IsValid())
            {
                commands.Add(command);
            }
        }
        
        /// <summary>
        /// インスタンス描画
        /// </summary>
        /// <param name="vertexCount">頂点数</param>
        /// <param name="instanceCount">インスタンス数</param>
        /// <param name="firstVertex">最初の頂点</param>
        public void DrawInstanced(int vertexCount, int instanceCount, int firstVertex = 0)
        {
            ValidateRecording();
            ValidateRenderPass();
            
            var command = new PwDrawInstancedCommand(vertexCount, instanceCount, firstVertex);
            if (command.IsValid())
            {
                commands.Add(command);
            }
        }
        
        #endregion
        
        #region 同期
        
        /// <summary>
        /// フェンスを待機
        /// </summary>
        /// <param name="fence">フェンス</param>
        public void WaitForFence(PwFence fence)
        {
            ValidateRecording();
            
            if (fence == null)
                throw new ArgumentNullException(nameof(fence));
                
            commands.Add(new PwWaitFenceCommand(fence));
        }
        
        /// <summary>
        /// フェンスをシグナル
        /// </summary>
        /// <param name="fence">フェンス</param>
        public void SignalFence(PwFence fence)
        {
            ValidateRecording();
            
            if (fence == null)
                throw new ArgumentNullException(nameof(fence));
                
            commands.Add(new PwSignalFenceCommand(fence));
        }
        
        #endregion
        
        #region 内部メソッド
        
        /// <summary>
        /// コマンドを実行
        /// </summary>
        protected virtual void ExecuteCommands()
        {
            foreach (var command in commands)
            {
                try
                {
                    command.Execute(device);
                }
                catch (Exception ex)
                {
                    // コマンド実行エラーをログに記録
                    Console.WriteLine($"Command execution failed: {command.GetType().Name} - {ex.Message}");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// 記録状態を検証
        /// </summary>
        private void ValidateRecording()
        {
            if (!isRecording)
                throw new InvalidOperationException("Command encoder is not recording");
        }
        
        /// <summary>
        /// レンダーパスの状態を検証
        /// </summary>
        private void ValidateRenderPass()
        {
            if (currentRenderPass == null)
                throw new InvalidOperationException("No active render pass for draw command");
        }
        
        /// <summary>
        /// 派生クラスでオーバーライド可能なコマンド実行メソッド
        /// </summary>
        protected virtual void ExecuteSetViewportCommand(PwViewportCommand command) { }
        protected virtual void ExecuteBeginRenderPassCommand(PwBeginRenderPassCommand command) { }
        protected virtual void ExecuteEndRenderPassCommand(PwEndRenderPassCommand command) { }
        protected virtual void ExecuteSetPipelineCommand(PwSetPipelineCommand command) { }
        protected virtual void ExecuteSetVertexBufferCommand(PwSetVertexBufferCommand command) { }
        protected virtual void ExecuteSetIndexBufferCommand(PwSetIndexBufferCommand command) { }
        protected virtual void ExecuteSetTextureCommand(PwSetTextureCommand command) { }
        protected virtual void ExecuteDrawIndexedInstancedCommand(PwDrawIndexedInstancedCommand command) { }
        
        #endregion
        
        #region 統計情報
        
        /// <summary>
        /// 記録されたコマンド数を取得
        /// </summary>
        public int CommandCount => commands.Count;
        
        /// <summary>
        /// 描画コマンド数を取得
        /// </summary>
        public int DrawCommandCount => commands.Count(c => c is PwDrawIndexedInstancedCommand || c is PwDrawInstancedCommand);
        
        /// <summary>
        /// デバッグ情報を取得
        /// </summary>
        public string GetDebugInfo()
        {
            return $"Commands: {CommandCount}, Draw calls: {DrawCommandCount}, Recording: {isRecording}";
        }
        
        #endregion
    
        /// <summary>
        /// 現在のコマンドリストをクリア（デバッグ用）
        /// </summary>
        public void ClearCommands()
        {
            if (isRecording)
                throw new InvalidOperationException("Cannot clear commands while recording");
            commands.Clear();
        }

        /// <summary>
        /// コマンドリストを取得（検査用）
        /// </summary>
        /// <returns>コマンドリストのコピー</returns>
        public IReadOnlyList<PwCommand> GetCommands()
        {
            return commands.ToArray();
        }
    }
}
