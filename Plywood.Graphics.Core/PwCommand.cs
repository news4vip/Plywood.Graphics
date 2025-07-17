// Plywood.Graphics.Core/PwCommand.cs
using System.Numerics;

namespace Plywood.Graphics
{
    public abstract class PwCommand
    {
        public abstract void Execute(IPwGraphicsDevice device);
    }
    
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
    }
    
    public class PwBeginRenderPassCommand : PwCommand
    {
        public PwRenderPass RenderPass { get; }
        
        public PwBeginRenderPassCommand(PwRenderPass renderPass)
        {
            RenderPass = renderPass;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.BeginRenderPass(RenderPass);
        }
    }
    
    public class PwEndRenderPassCommand : PwCommand
    {
        public PwRenderPass RenderPass { get; }
        
        public PwEndRenderPassCommand(PwRenderPass renderPass)
        {
            RenderPass = renderPass;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.EndRenderPass(RenderPass);
        }
    }
    
    public class PwSetPipelineCommand : PwCommand
    {
        public PwPipeline Pipeline { get; }
        
        public PwSetPipelineCommand(PwPipeline pipeline)
        {
            Pipeline = pipeline;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetPipeline(Pipeline);
        }
    }
    
    public class PwSetVertexBufferCommand : PwCommand
    {
        public PwBuffer Buffer { get; }
        public int BindingIndex { get; }
        
        public PwSetVertexBufferCommand(PwBuffer buffer, int bindingIndex)
        {
            Buffer = buffer;
            BindingIndex = bindingIndex;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetVertexBuffer(Buffer, BindingIndex);
        }
    }
    
    public class PwSetIndexBufferCommand : PwCommand
    {
        public PwBuffer Buffer { get; }
        
        public PwSetIndexBufferCommand(PwBuffer buffer)
        {
            Buffer = buffer;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetIndexBuffer(Buffer);
        }
    }
    
    public class PwSetInstanceBufferOffsetCommand : PwCommand
    {
        public PwBuffer Buffer { get; }
        public int Offset { get; }
        
        public PwSetInstanceBufferOffsetCommand(PwBuffer buffer, int offset)
        {
            Buffer = buffer;
            Offset = offset;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetInstanceBufferOffset(Buffer, Offset);
        }
    }
    
    public class PwSetTextureCommand : PwCommand
    {
        public PwTexture Texture { get; }
        public int BindingIndex { get; }
        
        public PwSetTextureCommand(PwTexture texture, int bindingIndex)
        {
            Texture = texture;
            BindingIndex = bindingIndex;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetTexture(Texture, BindingIndex);
        }
    }
    
    public class PwSetUniformBufferCommand : PwCommand
    {
        public PwBuffer Buffer { get; }
        public int BindingIndex { get; }
        
        public PwSetUniformBufferCommand(PwBuffer buffer, int bindingIndex)
        {
            Buffer = buffer;
            BindingIndex = bindingIndex;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SetUniformBuffer(Buffer, BindingIndex);
        }
    }
    
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
    }
    
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
    }
    
    public class PwWaitFenceCommand : PwCommand
    {
        public PwFence Fence { get; }
        
        public PwWaitFenceCommand(PwFence fence)
        {
            Fence = fence;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.WaitForFence(Fence);
        }
    }
    
    public class PwSignalFenceCommand : PwCommand
    {
        public PwFence Fence { get; }
        
        public PwSignalFenceCommand(PwFence fence)
        {
            Fence = fence;
        }
        
        public override void Execute(IPwGraphicsDevice device)
        {
            device.SignalFence(Fence);
        }
    }
}
