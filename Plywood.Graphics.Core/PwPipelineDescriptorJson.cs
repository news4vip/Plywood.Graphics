// Plywood.Graphics.Core/PwPipelineDescriptorJson.cs
using System.Text.Json.Serialization;

namespace Plywood.Graphics
{
    public class PwPipelineDescriptorJson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("vertexDescriptors")]
        public PwVertexDescriptorJson[] VertexDescriptors { get; set; }
        
        [JsonPropertyName("blendState")]
        public PwBlendStateJson BlendState { get; set; }
        
        [JsonPropertyName("depthStencilState")]
        public PwDepthStencilStateJson DepthStencilState { get; set; }
        
        [JsonPropertyName("rasterizerState")]
        public PwRasterizerStateJson RasterizerState { get; set; }
        
        [JsonPropertyName("primitiveTopology")]
        public string PrimitiveTopology { get; set; }
        
        [JsonPropertyName("renderTargetFormats")]
        public PwRenderTargetFormatJson[] RenderTargetFormats { get; set; }
        
        [JsonPropertyName("uniforms")]
        public PwUniformDescriptorJson[] Uniforms { get; set; }
        
        [JsonPropertyName("textures")]
        public PwTextureDescriptorJson[] Textures { get; set; }
        
        public PwPipelineDescriptor ToPipelineDescriptor()
        {
            return new PwPipelineDescriptor
            {
                VertexDescriptors = VertexDescriptors?.Select(v => v.ToVertexDescriptor()).ToArray() ?? Array.Empty<PwVertexDescriptor>(),
                BlendState = BlendState?.ToBlendState() ?? PwBlendState.Default,
                DepthStencilState = DepthStencilState?.ToDepthStencilState() ?? PwDepthStencilState.Default,
                RasterizerState = RasterizerState?.ToRasterizerState() ?? PwRasterizerState.Default,
                PrimitiveTopology = Enum.Parse<PwPrimitiveTopology>(PrimitiveTopology ?? "TriangleList"),
                RenderTargetFormats = RenderTargetFormats?.Select(r => r.ToRenderTargetFormat()).ToArray() ?? Array.Empty<PwRenderTargetFormat>()
            };
        }
    }
    
    public class PwVertexDescriptorJson
    {
        [JsonPropertyName("bindingIndex")]
        public int BindingIndex { get; set; }
        
        [JsonPropertyName("inputRate")]
        public string InputRate { get; set; }
        
        [JsonPropertyName("attributes")]
        public PwVertexAttributeDescriptorJson[] Attributes { get; set; }
        
        public PwVertexDescriptor ToVertexDescriptor()
        {
            return new PwVertexDescriptor
            {
                BindingIndex = BindingIndex,
                Attributes = Attributes?.Select(a => a.ToAttributeDescriptor()).ToArray() ?? Array.Empty<PwVertexAttributeDescriptor>(),
                Stride = Attributes?.LastOrDefault()?.Offset + GetAttributeSize(Attributes?.LastOrDefault()?.Format) ?? 0
            };
        }
        
        private int GetAttributeSize(string format)
        {
            return format switch
            {
                "Float" => 4,
                "Float2" => 8,
                "Float3" => 12,
                "Float4" => 16,
                "Matrix4x4" => 64,
                _ => 4
            };
        }
    }
    
    public class PwVertexAttributeDescriptorJson
    {
        [JsonPropertyName("semantic")]
        public string Semantic { get; set; }
        
        [JsonPropertyName("index")]
        public int Index { get; set; }
        
        [JsonPropertyName("format")]
        public string Format { get; set; }
        
        [JsonPropertyName("offset")]
        public int Offset { get; set; }
        
        public PwVertexAttributeDescriptor ToAttributeDescriptor()
        {
            return new PwVertexAttributeDescriptor
            {
                Semantic = Enum.Parse<PwVertexSemantic>(Semantic),
                Index = Index,
                Format = Enum.Parse<PwVertexFormat>(Format),
                Offset = Offset,
                Size = PwVertexFormatHelper.GetSizeInBytes(Enum.Parse<PwVertexFormat>(Format))
            };
        }
    }
    
    public class PwBlendStateJson
    {
        [JsonPropertyName("blendEnabled")]
        public bool BlendEnabled { get; set; }
        
        [JsonPropertyName("sourceBlend")]
        public string SourceBlend { get; set; }
        
        [JsonPropertyName("destinationBlend")]
        public string DestinationBlend { get; set; }
        
        [JsonPropertyName("blendOperation")]
        public string BlendOperation { get; set; }
        
        public PwBlendState ToBlendState()
        {
            return new PwBlendState
            {
                BlendEnabled = BlendEnabled,
                SourceBlend = Enum.Parse<PwBlendFactor>(SourceBlend),
                DestinationBlend = Enum.Parse<PwBlendFactor>(DestinationBlend),
                BlendOperation = Enum.Parse<PwBlendOperation>(BlendOperation)
            };
        }
    }
    
    public class PwDepthStencilStateJson
    {
        [JsonPropertyName("depthTestEnabled")]
        public bool DepthTestEnabled { get; set; }
        
        [JsonPropertyName("depthWriteEnabled")]
        public bool DepthWriteEnabled { get; set; }
        
        [JsonPropertyName("depthComparison")]
        public string DepthComparison { get; set; }
        
        public PwDepthStencilState ToDepthStencilState()
        {
            return new PwDepthStencilState
            {
                DepthTestEnabled = DepthTestEnabled,
                DepthWriteEnabled = DepthWriteEnabled,
                DepthComparison = Enum.Parse<PwComparisonFunction>(DepthComparison)
            };
        }
    }
    
    public class PwRasterizerStateJson
    {
        [JsonPropertyName("cullMode")]
        public string CullMode { get; set; }
        
        [JsonPropertyName("fillMode")]
        public string FillMode { get; set; }
        
        [JsonPropertyName("scissorTestEnabled")]
        public bool ScissorTestEnabled { get; set; }
        
        public PwRasterizerState ToRasterizerState()
        {
            return new PwRasterizerState
            {
                CullMode = Enum.Parse<PwCullMode>(CullMode),
                FillMode = Enum.Parse<PwFillMode>(FillMode),
                ScissorTestEnabled = ScissorTestEnabled
            };
        }
    }
    
    public class PwRenderTargetFormatJson
    {
        [JsonPropertyName("format")]
        public string Format { get; set; }
        
        [JsonPropertyName("index")]
        public int Index { get; set; }
        
        public PwRenderTargetFormat ToRenderTargetFormat()
        {
            return new PwRenderTargetFormat
            {
                Format = Enum.Parse<PwTextureFormat>(Format),
                Index = Index
            };
        }
    }
    
    public class PwUniformDescriptorJson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("bindingIndex")]
        public int BindingIndex { get; set; }
    }
    
    public class PwTextureDescriptorJson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("format")]
        public string Format { get; set; }
        
        [JsonPropertyName("bindingIndex")]
        public int BindingIndex { get; set; }
    }
}
