// Plywood.Graphics.Core/PwCommand.cs
using System.Numerics;

namespace Plywood.Graphics
{
    /// <summary>
    /// GPU コマンドの抽象基底クラス
    /// Command Pattern を使用して描画操作を遅延実行
    /// </summary>
    public abstract class PwCommand
    {
        /// <summary>コマンドの作成時刻</summary>
        public DateTime CreatedTime { get; } = DateTime.Now;
        
        /// <summary>コマンドの実行優先度</summary>
        public int Priority { get; set; } = 0;
        
        /// <summary>
        /// コマンドを実行
        /// </summary>
        /// <param name="device">実行対象のグラフィックスデバイス</param>
        public abstract void Execute(IPwGraphicsDevice device);
        
        /// <summary>
        /// コマンドの妥当性を検証
        /// </summary>
        /// <returns>有効な場合true</returns>
        public virtual bool IsValid() => true;
    }
    
    /// <summary>
    /// ビューポート設定コマンド
    /// </summary>
    public class PwViewportCommand : PwCommand
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        
        public PwViewportCommand(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetViewport(X, Y, Width, Height);
        }
        
        public override bool IsValid()
        {
            return Width > 0 && Height > 0;
        }
    }
    
    /// <summary>
    /// レンダーパス開始コマンド
    /// </summary>
    public class PwBeginRenderPassCommand : PwCommand
    {
        public PwRenderPass RenderPass { get; }
        
        public PwBeginRenderPassCommand(PwRenderPass renderPass)
        {
            RenderPass = renderPass ?? throw new ArgumentNullException(nameof(renderPass));
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.BeginRenderPass(RenderPass);
        }
        
        public override bool IsValid()
        {
            return RenderPass?.Descriptor?.IsValid() == true;
        }
    }
    
    /// <summary>
    /// レンダーパス終了コマンド
    /// </summary>
    public class PwEndRenderPassCommand : PwCommand
    {
        public PwRenderPass RenderPass { get; }
        
        public PwEndRenderPassCommand(PwRenderPass renderPass)
        {
            RenderPass = renderPass ?? throw new ArgumentNullException(nameof(renderPass));
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.EndRenderPass(RenderPass);
        }
    }
    
    /// <summary>
    /// パイプライン設定コマンド
    /// </summary>
    public class PwSetPipelineCommand : PwCommand
    {
        public PwPipeline Pipeline { get; }
        
        public PwSetPipelineCommand(PwPipeline pipeline)
        {
            Pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetPipeline(Pipeline);
        }
    }
    
    /// <summary>
    /// 頂点バッファ設定コマンド
    /// </summary>
    public class PwSetVertexBufferCommand : PwCommand
    {
        public PwBuffer Buffer { get; }
        public int BindingIndex { get; }
        
        public PwSetVertexBufferCommand(PwBuffer buffer, int bindingIndex)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            BindingIndex = bindingIndex;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetVertexBuffer(Buffer, BindingIndex);
        }
        
        public override bool IsValid()
        {
            return Buffer.Usage == PwBufferUsage.Vertex || Buffer.Usage == PwBufferUsage.Instance;
        }
    }
    
    /// <summary>
    /// インデックスバッファ設定コマンド
    /// </summary>
    public class PwSetIndexBufferCommand : PwCommand
    {
        public PwBuffer Buffer { get; }
        
        public PwSetIndexBufferCommand(PwBuffer buffer)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetIndexBuffer(Buffer);
        }
        
        public override bool IsValid()
        {
            return Buffer.Usage == PwBufferUsage.Index;
        }
    }
    
    /// <summary>
    /// インスタンスバッファオフセット設定コマンド
    /// </summary>
    public class PwSetInstanceBufferOffsetCommand : PwCommand
    {
        public PwBuffer Buffer { get; }
        public int Offset { get; }
        
        public PwSetInstanceBufferOffsetCommand(PwBuffer buffer, int offset)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Offset = offset;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetInstanceBufferOffset(Buffer, Offset);
        }
        
        public override bool IsValid()
        {
            return Offset >= 0 && Offset < Buffer.Size;
        }
    }
    
    /// <summary>
    /// テクスチャ設定コマンド
    /// </summary>
    public class PwSetTextureCommand : PwCommand
    {
        public PwTexture Texture { get; }
        public int BindingIndex { get; }
        
        public PwSetTextureCommand(PwTexture texture, int bindingIndex)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture));
            BindingIndex = bindingIndex;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetTexture(Texture, BindingIndex);
        }
    }
    
    /// <summary>
    /// ユニフォームバッファ設定コマンド
    /// </summary>
    public class PwSetUniformBufferCommand : PwCommand
    {
        public PwBuffer Buffer { get; }
        public int BindingIndex { get; }
        
        public PwSetUniformBufferCommand(PwBuffer buffer, int bindingIndex)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            BindingIndex = bindingIndex;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetUniformBuffer(Buffer, BindingIndex);
        }
        
        public override bool IsValid()
        {
            return Buffer.Usage == PwBufferUsage.Uniform;
        }
    }
    
    /// <summary>
    /// インデックス付きインスタンス描画コマンド
    /// </summary>
    public class PwDrawIndexedInstancedCommand : PwCommand
    {
        public int IndexCount { get; }
        public int InstanceCount { get; }
        public int FirstIndex { get; }
        
        public PwDrawIndexedInstancedCommand(int indexCount, int instanceCount, int firstIndex)
        {
            IndexCount = indexCount;
            InstanceCount = instanceCount;
            FirstIndex = firstIndex;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.DrawIndexedInstanced(IndexCount, InstanceCount, FirstIndex);
        }
        
        public override bool IsValid()
        {
            return IndexCount > 0 && InstanceCount > 0;
        }
    }
    
    /// <summary>
    /// インスタンス描画コマンド
    /// </summary>
    public class PwDrawInstancedCommand : PwCommand
    {
        public int VertexCount { get; }
        public int InstanceCount { get; }
        public int FirstVertex { get; }
        
        public PwDrawInstancedCommand(int vertexCount, int instanceCount, int firstVertex)
        {
            VertexCount = vertexCount;
            InstanceCount = instanceCount;
            FirstVertex = firstVertex;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.DrawInstanced(VertexCount, InstanceCount, FirstVertex);
        }
        
        public override bool IsValid()
        {
            return VertexCount > 0 && InstanceCount > 0;
        }
    }
    
    /// <summary>
    /// フェンス待機コマンド
    /// </summary>
    public class PwWaitFenceCommand : PwCommand
    {
        public PwFence Fence { get; }
        
        public PwWaitFenceCommand(PwFence fence)
        {
            Fence = fence ?? throw new ArgumentNullException(nameof(fence));
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.WaitForFence(Fence);
        }
    }
    
    /// <summary>
    /// フェンスシグナルコマンド
    /// </summary>
    public class PwSignalFenceCommand : PwCommand
    {
        public PwFence Fence { get; }
        
        public PwSignalFenceCommand(PwFence fence)
        {
            Fence = fence ?? throw new ArgumentNullException(nameof(fence));
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SignalFence(Fence);
        }
    }
}
