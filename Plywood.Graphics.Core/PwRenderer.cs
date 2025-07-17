// Plywood.Graphics.Core/PwRenderer.cs
namespace Plywood.Graphics
{
    public abstract class PwRenderer
    {
        protected IPwGraphicsDevice Device { get; private set; }
        protected PwCommandEncoder CommandEncoder { get; private set; }
        protected PwShaderArchive ShaderArchive { get; private set; }
        protected PwMemoryManager MemoryManager { get; private set; }
        
        public void Initialize(IPwGraphicsDevice device)
        {
            Device = device;
            CommandEncoder = new PwCommandEncoder(device);
            ShaderArchive = new PwShaderArchive(device);
            MemoryManager = new PwMemoryManager(device);
            Initialize();
        }
        
        public void RunFrame(float deltaTime)
        {
            Update(deltaTime);
            
            CommandEncoder.Begin();
            Render(CommandEncoder);
            CommandEncoder.End();
            
            Present();
        }
        
        protected abstract void Initialize();
        protected abstract void Update(float deltaTime);
        protected abstract void Render(PwCommandEncoder encoder);
        
        // 型安全な頂点バッファ作成
        protected PwBuffer CreateVertexBuffer<T>(T[] data) where T : struct, IPwVertexData
        {
            return MemoryManager.CreateOptimizedVertexBuffer(data);
        }
        
        // 型安全でない汎用バッファ作成
        protected PwBuffer CreateBuffer<T>(T[] data, PwBufferUsage usage) where T : struct
        {
            return MemoryManager.CreateOptimizedBuffer(data, usage);
        }
        
        protected PwBuffer CreateIndexBuffer(ushort[] indices)
        {
            return Device.CreateIndexBuffer(indices);
        }
        
        protected PwBuffer CreateUniformBuffer<T>(T data) where T : struct
        {
            return Device.CreateUniformBuffer(data);
        }
        
        protected PwTexture CreateTexture(int width, int height, PwTextureFormat format)
        {
            return Device.CreateTexture(width, height, format);
        }
        
        protected PwPipeline LoadPipelineFromZip(string zipPath, string pipelineName)
        {
            return ShaderArchive.LoadPipeline(zipPath, pipelineName);
        }
        
        protected PwPipeline CreatePipelineFromVertexTypes<TVertex, TInstance>(string zipPath, string pipelineName)
            where TVertex : struct, IPwVertexData
            where TInstance : struct, IPwVertexData
        {
            // 頂点記述子を自動生成
            var vertexDescriptor = PwVertexDescriptor.CreateFromType<TVertex>();
            vertexDescriptor.BindingIndex = 0;
            
            var instanceDescriptor = PwVertexDescriptor.CreateFromType<TInstance>();
            instanceDescriptor.BindingIndex = 1;
            
            // シェーダーアーカイブから基本設定を読み込み
            var basePipeline = ShaderArchive.LoadPipeline(zipPath, pipelineName);
            
            // 頂点記述子を置き換え
            basePipeline.Descriptor.VertexDescriptors = new[] { vertexDescriptor, instanceDescriptor };
            
            // 新しいパイプラインを作成
            return Device.CreatePipeline(basePipeline.Descriptor);
        }
        
        protected virtual void Present()
        {
            Device.Present();
        }
        
        // メモリ使用量の監視
        protected PwMemoryUsageInfo GetMemoryUsage()
        {
            return MemoryManager.GetMemoryUsage();
        }
        
        // リソースクリーンアップ
        protected virtual void Cleanup()
        {
            MemoryManager.ClearCache();
        }
    }
}
