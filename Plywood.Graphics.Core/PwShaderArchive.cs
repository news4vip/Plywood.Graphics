// Plywood.Graphics.Core/PwShaderArchive.cs
using System.IO.Compression;
using System.Text.Json;

namespace Plywood.Graphics
{
    public class PwShaderArchive
    {
        private readonly IPwGraphicsDevice device;
        private readonly Dictionary<string, PwPipeline> pipelineCache;
        private readonly Dictionary<string, PwShader> shaderCache;
        private readonly PwPlatformInfo platformInfo;
        
        public PwShaderArchive(IPwGraphicsDevice device)
        {
            this.device = device;
            this.pipelineCache = new Dictionary<string, PwPipeline>();
            this.shaderCache = new Dictionary<string, PwShader>();
            this.platformInfo = PwPlatformInfo.Current;
        }
        
        public PwPipeline LoadPipeline(string archivePath, string pipelineName)
        {
            var cacheKey = $"{archivePath}:{pipelineName}";
            
            if (pipelineCache.TryGetValue(cacheKey, out var cachedPipeline))
                return cachedPipeline;
            
            using var archive = ZipFile.OpenRead(archivePath);
            
            // パイプライン記述子読み込み
            var pipelineDescriptor = LoadPipelineDescriptor(archive, pipelineName);
            
            // シェーダー読み込み
            var vertexShader = LoadShader(archive, pipelineName, PwShaderStage.Vertex);
            var fragmentShader = LoadShader(archive, pipelineName, PwShaderStage.Fragment);
            
            // パイプライン記述子を完成
            pipelineDescriptor.VertexShader = vertexShader;
            pipelineDescriptor.FragmentShader = fragmentShader;
            
            // パイプライン作成
            var pipeline = device.CreatePipeline(pipelineDescriptor);
            
            // キャッシュに保存
            pipelineCache[cacheKey] = pipeline;
            
            return pipeline;
        }
        
        private PwShader LoadShader(ZipArchive archive, string pipelineName, PwShaderStage stage)
        {
            var shaderFileName = GetShaderFileName(stage);
            var entryPath = $"{pipelineName}/{shaderFileName}";
            var cacheKey = $"{pipelineName}:{stage}";
            
            if (shaderCache.TryGetValue(cacheKey, out var cachedShader))
                return cachedShader;
            
            var entry = archive.GetEntry(entryPath);
            if (entry == null)
                throw new FileNotFoundException($"Shader not found: {entryPath}");
            
            using var stream = entry.Open();
            var shaderData = new byte[entry.Length];
            stream.Read(shaderData, 0, (int)entry.Length);
            
            var shader = device.LoadShader(shaderData, stage);
            shaderCache[cacheKey] = shader;
            
            return shader;
        }
        
        private string GetShaderFileName(PwShaderStage stage)
        {
            var stagePrefix = stage switch
            {
                PwShaderStage.Vertex => "vertex",
                PwShaderStage.Fragment => "fragment",
                _ => throw new ArgumentOutOfRangeException(nameof(stage))
            };
            
            var extension = platformInfo.GraphicsApi switch
            {
                PwGraphicsApi.Metal => ".metallib",
                PwGraphicsApi.Vulkan => ".spv",
                _ => throw new NotSupportedException($"Graphics API not supported: {platformInfo.GraphicsApi}")
            };
            
            return $"{stagePrefix}{extension}";
        }
        
        private PwPipelineDescriptor LoadPipelineDescriptor(ZipArchive archive, string pipelineName)
        {
            var entryPath = $"{pipelineName}/pipeline.json";
            var entry = archive.GetEntry(entryPath);
            
            if (entry == null)
                throw new FileNotFoundException($"Pipeline descriptor not found: {entryPath}");
            
            using var stream = entry.Open();
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            
            return JsonSerializer.Deserialize<PwPipelineDescriptorJson>(json).ToPipelineDescriptor();
        }
    }
}
