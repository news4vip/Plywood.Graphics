// Plywood.Graphics.Core/PwCommandEncoder.cs
using System.Numerics;

namespace Plywood.Graphics
{
    public class PwCommandEncoder
    {
        private readonly IPwGraphicsDevice device;
        private readonly List<PwCommand> commands;
        private PwRenderPass currentRenderPass;
        private PwPipeline currentPipeline;
        private bool isRecording;
        
        internal PwCommandEncoder(IPwGraphicsDevice device)
        {
            this.device = device;
            this.commands = new List<PwCommand>();
        }
        
        public virtual void Begin()
        {
            if (isRecording)
                throw new InvalidOperationException("Command encoder is already recording");
                
            commands.Clear();
            isRecording = true;
            device.BeginFrame();
        }
        
        public virtual void End()
        {
            if (!isRecording)
                throw new InvalidOperationException("Command encoder is not recording");
                
            if (currentRenderPass != null)
                EndRenderPass();
                
            ExecuteCommands();
            device.EndFrame();
            isRecording = false;
        }
        
        // ビューポート設定
        public void SetViewport(int x, int y, int width, int height)
        {
            commands.Add(new PwViewportCommand(x, y, width, height));
        }
        
        // レンダーパス管理
        public void BeginRenderPass(PwTexture renderTarget)
        {
            if (currentRenderPass != null)
                throw new InvalidOperationException("Render pass is already active");
                
            var descriptor = new PwRenderPassDescriptor
            {
                ColorTarget = renderTarget,
                ClearColor = new Vector4(0.2f, 0.3f, 0.4f, 1.0f),
                LoadAction = PwLoadAction.Clear
            };
            
            currentRenderPass = device.CreateRenderPass(descriptor);
            commands.Add(new PwBeginRenderPassCommand(currentRenderPass));
        }
        
        public void BeginMainRenderPass()
        {
            if (currentRenderPass != null)
                throw new InvalidOperationException("Render pass is already active");
                
            var descriptor = new PwRenderPassDescriptor
            {
                ColorTarget = null, // メインフレームバッファ
                ClearColor = new Vector4(0.1f, 0.2f, 0.3f, 1.0f),
                LoadAction = PwLoadAction.Clear
            };
            
            currentRenderPass = device.CreateRenderPass(descriptor);
            commands.Add(new PwBeginRenderPassCommand(currentRenderPass));
        }
        
        public void EndRenderPass()
        {
            if (currentRenderPass == null)
                throw new InvalidOperationException("No active render pass");
                
            commands.Add(new PwEndRenderPassCommand(currentRenderPass));
            currentRenderPass = null;
        }
        
        // パイプライン設定
        public void SetPipeline(PwPipeline pipeline)
        {
            currentPipeline = pipeline;
            commands.Add(new PwSetPipelineCommand(pipeline));
        }
        
        // バッファ設定
        public void SetVertexBuffer(PwBuffer buffer, int bindingIndex = 0)
        {
            commands.Add(new PwSetVertexBufferCommand(buffer, bindingIndex));
        }
        
        public void SetIndexBuffer(PwBuffer buffer)
        {
            commands.Add(new PwSetIndexBufferCommand(buffer));
        }
        
        public void SetInstanceBuffer(PwBuffer buffer, int bindingIndex = 1)
        {
            commands.Add(new PwSetVertexBufferCommand(buffer, bindingIndex));
        }
        
        public void SetInstanceBufferOffset(PwBuffer buffer, int offset)
        {
            commands.Add(new PwSetInstanceBufferOffsetCommand(buffer, offset));
        }
        
        public void SetTexture(PwTexture texture, int bindingIndex)
        {
            commands.Add(new PwSetTextureCommand(texture, bindingIndex));
        }
        
        public void SetUniformBuffer(PwBuffer buffer, int bindingIndex)
        {
            commands.Add(new PwSetUniformBufferCommand(buffer, bindingIndex));
        }
        
        // 描画コマンド
        public void DrawIndexedInstanced(int indexCount, int instanceCount, int firstIndex = 0)
        {
            commands.Add(new PwDrawIndexedInstancedCommand(indexCount, instanceCount, firstIndex));
        }
        
        public void DrawInstanced(int vertexCount, int instanceCount, int firstVertex = 0)
        {
            commands.Add(new PwDrawInstancedCommand(vertexCount, instanceCount, firstVertex));
        }
        
        // 同期
        public void WaitForFence(PwFence fence)
        {
            commands.Add(new PwWaitFenceCommand(fence));
        }
        
        public void SignalFence(PwFence fence)
        {
            commands.Add(new PwSignalFenceCommand(fence));
        }
        
        protected virtual void ExecuteCommands()
        {
            foreach (var command in commands)
            {
                command.Execute(device);
            }
        }
        
        // 派生クラスでオーバーライド可能なコマンド実行メソッド
        protected virtual void ExecuteSetViewportCommand(PwViewportCommand command) { }
        protected virtual void ExecuteBeginRenderPassCommand(PwBeginRenderPassCommand command) { }
        protected virtual void ExecuteEndRenderPassCommand(PwEndRenderPassCommand command) { }
        protected virtual void ExecuteSetPipelineCommand(PwSetPipelineCommand command) { }
        protected virtual void ExecuteSetVertexBufferCommand(PwSetVertexBufferCommand command) { }
        protected virtual void ExecuteSetIndexBufferCommand(PwSetIndexBufferCommand command) { }
        protected virtual void ExecuteSetTextureCommand(PwSetTextureCommand command) { }
        protected virtual void ExecuteDrawIndexedInstancedCommand(PwDrawIndexedInstancedCommand command) { }
    }
}
