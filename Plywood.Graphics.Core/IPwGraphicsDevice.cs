// Plywood.Graphics.Core/IPwGraphicsDevice.cs
namespace Plywood.Graphics
{
    public interface IPwGraphicsDevice
    {
        // バッファ管理
        PwBuffer CreateVertexBuffer<T>(T[] data) where T : struct, IPwVertexData;
        PwBuffer CreateIndexBuffer(ushort[] indices);
        PwBuffer CreateUniformBuffer<T>(T data) where T : struct;
        void UpdateBuffer<T>(PwBuffer buffer, T[] data) where T : struct;
        
        // 生のバッファ作成（プール用）
        PwBuffer CreateVertexBufferRaw(int size, PwBufferFlags flags);
        PwBuffer CreateIndexBufferRaw(int size, PwBufferFlags flags);
        PwBuffer CreateUniformBufferRaw(int size, PwBufferFlags flags);
        PwBuffer CreateInstanceBufferRaw(int size, PwBufferFlags flags);
        void DestroyBuffer(PwBuffer buffer);
        
        // テクスチャ管理
        PwTexture CreateTexture(int width, int height, PwTextureFormat format);
        PwTexture CreateRenderTarget(int width, int height, PwTextureFormat format);
        PwTexture LoadTexture(string path);
        
        // パイプライン管理
        PwPipeline CreatePipeline(PwPipelineDescriptor descriptor);
        PwRenderPass CreateRenderPass(PwRenderPassDescriptor descriptor);
        
        // シェーダー管理
        PwShader LoadShader(byte[] shaderData, PwShaderStage stage);
        
        // メモリ管理
        PwMemoryRequirements GetMemoryRequirements<T>() where T : struct;
        
        // 同期
        PwFence CreateFence();
        void WaitForFence(PwFence fence);
        void SignalFence(PwFence fence);
        
        // 描画
        void Present();
        void BeginFrame();
        void EndFrame();
        
        // コマンドエンコーダー用
        void SetViewport(int x, int y, int width, int height);
        void BeginRenderPass(PwRenderPass renderPass);
        void EndRenderPass(PwRenderPass renderPass);
        void SetPipeline(PwPipeline pipeline);
        void SetVertexBuffer(PwBuffer buffer, int bindingIndex);
        void SetIndexBuffer(PwBuffer buffer);
        void SetInstanceBufferOffset(PwBuffer buffer, int offset);
        void SetTexture(PwTexture texture, int bindingIndex);
        void SetUniformBuffer(PwBuffer buffer, int bindingIndex);
        void DrawIndexedInstanced(int indexCount, int instanceCount, int firstIndex);
        void DrawInstanced(int vertexCount, int instanceCount, int firstVertex);
    }
}
