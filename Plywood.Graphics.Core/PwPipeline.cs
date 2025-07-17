// Plywood.Graphics.Core/PwPipeline.cs
using System.Numerics;

namespace Plywood.Graphics
{
    public class PwPipeline
    {
        internal object PlatformPipeline { get; set; }
        public PwPipelineDescriptor Descriptor { get; internal set; }
        
        internal PwPipeline(object platformPipeline, PwPipelineDescriptor descriptor)
        {
            PlatformPipeline = platformPipeline;
            Descriptor = descriptor;
        }
    }
    
    public class PwPipelineDescriptor
    {
        public PwShader VertexShader { get; set; }
        public PwShader FragmentShader { get; set; }
        public PwVertexDescriptor[] VertexDescriptors { get; set; } = Array.Empty<PwVertexDescriptor>();
        public PwBlendState BlendState { get; set; } = PwBlendState.Default;
        public PwDepthStencilState DepthStencilState { get; set; } = PwDepthStencilState.Default;
        public PwRasterizerState RasterizerState { get; set; } = PwRasterizerState.Default;
        public PwPrimitiveTopology PrimitiveTopology { get; set; } = PwPrimitiveTopology.TriangleList;
        public PwRenderTargetFormat[] RenderTargetFormats { get; set; } = Array.Empty<PwRenderTargetFormat>();
    }
    
    public class PwShader
    {
        internal object PlatformShader { get; set; }
        public PwShaderStage Stage { get; internal set; }
        
        internal PwShader(object platformShader, PwShaderStage stage)
        {
            PlatformShader = platformShader;
            Stage = stage;
        }
    }
    
    public enum PwShaderStage
    {
        Vertex,
        Fragment
    }
    
    public enum PwPrimitiveTopology
    {
        TriangleList,
        TriangleStrip,
        LineList,
        LineStrip,
        PointList
    }
    
    public struct PwBlendState
    {
        public bool BlendEnabled { get; set; }
        public PwBlendFactor SourceBlend { get; set; }
        public PwBlendFactor DestinationBlend { get; set; }
        public PwBlendOperation BlendOperation { get; set; }
        
        public static readonly PwBlendState Default = new()
        {
            BlendEnabled = false,
            SourceBlend = PwBlendFactor.One,
            DestinationBlend = PwBlendFactor.Zero,
            BlendOperation = PwBlendOperation.Add
        };
    }
    
    public enum PwBlendFactor
    {
        Zero,
        One,
        SourceAlpha,
        OneMinusSourceAlpha,
        DestinationAlpha,
        OneMinusDestinationAlpha
    }
    
    public enum PwBlendOperation
    {
        Add,
        Subtract,
        ReverseSubtract,
        Min,
        Max
    }
    
    public struct PwDepthStencilState
    {
        public bool DepthTestEnabled { get; set; }
        public bool DepthWriteEnabled { get; set; }
        public PwComparisonFunction DepthComparison { get; set; }
        
        public static readonly PwDepthStencilState Default = new()
        {
            DepthTestEnabled = true,
            DepthWriteEnabled = true,
            DepthComparison = PwComparisonFunction.Less
        };
    }
    
    public enum PwComparisonFunction
    {
        Never,
        Less,
        Equal,
        LessEqual,
        Greater,
        NotEqual,
        GreaterEqual,
        Always
    }
    
    public struct PwRasterizerState
    {
        public PwCullMode CullMode { get; set; }
        public PwFillMode FillMode { get; set; }
        public bool ScissorTestEnabled { get; set; }
        
        public static readonly PwRasterizerState Default = new()
        {
            CullMode = PwCullMode.Back,
            FillMode = PwFillMode.Solid,
            ScissorTestEnabled = false
        };
    }
    
    public enum PwCullMode
    {
        None,
        Front,
        Back
    }
    
    public enum PwFillMode
    {
        Solid,
        Wireframe
    }
    
    public struct PwRenderTargetFormat
    {
        public PwTextureFormat Format { get; set; }
        public int Index { get; set; }
    }
}
