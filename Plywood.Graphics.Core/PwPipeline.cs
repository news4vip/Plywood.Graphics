// Plywood.Graphics.Core/PwPipeline.cs
using System.Numerics;

namespace Plywood.Graphics
{
    /// <summary>
    /// GPU レンダリングパイプラインの抽象化クラス
    /// シェーダー、描画状態、頂点レイアウトなどを組み合わせた描画設定
    /// </summary>
    public class PwPipeline
    {
        /// <summary>
        /// プラットフォーム依存の実際のパイプラインオブジェクト
        /// iOS: MTLRenderPipelineState, Android: VkPipeline など
        /// </summary>
        internal object PlatformPipeline { get; set; }
        
        /// <summary>パイプラインの作成に使用された記述子</summary>
        public PwPipelineDescriptor Descriptor { get; internal set; }
        
        /// <summary>
        /// パイプラインのコンストラクタ
        /// </summary>
        /// <param name="platformPipeline">プラットフォーム依存のパイプラインオブジェクト</param>
        /// <param name="descriptor">パイプライン記述子</param>
        internal PwPipeline(object platformPipeline, PwPipelineDescriptor descriptor)
        {
            PlatformPipeline = platformPipeline;
            Descriptor = descriptor;
        }
    }
    
    /// <summary>
    /// パイプライン記述子
    /// GPU パイプラインの全設定を定義
    /// </summary>
    public class PwPipelineDescriptor
    {
        /// <summary>頂点シェーダー</summary>
        public PwShader VertexShader { get; set; }
        
        /// <summary>フラグメントシェーダー</summary>
        public PwShader FragmentShader { get; set; }
        
        /// <summary>頂点入力レイアウト</summary>
        public PwVertexDescriptor[] VertexDescriptors { get; set; } = Array.Empty<PwVertexDescriptor>();
        
        /// <summary>ブレンディング設定</summary>
        public PwBlendState BlendState { get; set; } = PwBlendState.Default;
        
        /// <summary>深度ステンシル設定</summary>
        public PwDepthStencilState DepthStencilState { get; set; } = PwDepthStencilState.Default;
        
        /// <summary>ラスタライザー設定</summary>
        public PwRasterizerState RasterizerState { get; set; } = PwRasterizerState.Default;
        
        /// <summary>プリミティブトポロジー</summary>
        public PwPrimitiveTopology PrimitiveTopology { get; set; } = PwPrimitiveTopology.TriangleList;
        
        /// <summary>レンダーターゲットフォーマット</summary>
        public PwRenderTargetFormat[] RenderTargetFormats { get; set; } = Array.Empty<PwRenderTargetFormat>();
    }
    
    /// <summary>
    /// シェーダーオブジェクト
    /// コンパイル済みシェーダーコードを表現
    /// </summary>
    public class PwShader
    {
        /// <summary>プラットフォーム依存のシェーダーオブジェクト</summary>
        internal object PlatformShader { get; set; }
        
        /// <summary>シェーダーステージ</summary>
        public PwShaderStage Stage { get; internal set; }
        
        /// <summary>シェーダーのエントリポイント名</summary>
        public string EntryPoint { get; internal set; }
        
        /// <summary>
        /// シェーダーのコンストラクタ
        /// </summary>
        /// <param name="platformShader">プラットフォーム依存のシェーダーオブジェクト</param>
        /// <param name="stage">シェーダーステージ</param>
        internal PwShader(object platformShader, PwShaderStage stage)
        {
            PlatformShader = platformShader;
            Stage = stage;
            EntryPoint = "main";  // デフォルトエントリポイント
        }
    }
    
    /// <summary>
    /// シェーダーステージ
    /// 現在は頂点シェーダーとフラグメントシェーダーのみサポート
    /// </summary>
    public enum PwShaderStage
    {
        /// <summary>頂点シェーダー</summary>
        Vertex,
        /// <summary>フラグメントシェーダー</summary>
        Fragment
    }
    
    /// <summary>
    /// プリミティブトポロジー
    /// 頂点データの解釈方法を定義
    /// </summary>
    public enum PwPrimitiveTopology
    {
        /// <summary>三角形リスト</summary>
        TriangleList,
        /// <summary>三角形ストリップ</summary>
        TriangleStrip,
        /// <summary>線分リスト</summary>
        LineList,
        /// <summary>線分ストリップ</summary>
        LineStrip,
        /// <summary>点リスト</summary>
        PointList
    }
    
    /// <summary>
    /// ブレンディング状態
    /// 描画結果の合成方法を定義
    /// </summary>
    public struct PwBlendState
    {
        /// <summary>ブレンディング有効フラグ</summary>
        public bool BlendEnabled { get; set; }
        
        /// <summary>ソースブレンドファクター</summary>
        public PwBlendFactor SourceBlend { get; set; }
        
        /// <summary>デスティネーションブレンドファクター</summary>
        public PwBlendFactor DestinationBlend { get; set; }
        
        /// <summary>ブレンディング演算</summary>
        public PwBlendOperation BlendOperation { get; set; }
        
        /// <summary>
        /// デフォルト設定（ブレンディング無効）
        /// </summary>
        public static readonly PwBlendState Default = new()
        {
            BlendEnabled = false,
            SourceBlend = PwBlendFactor.One,
            DestinationBlend = PwBlendFactor.Zero,
            BlendOperation = PwBlendOperation.Add
        };
        
        /// <summary>
        /// アルファブレンド設定
        /// </summary>
        public static readonly PwBlendState AlphaBlend = new()
        {
            BlendEnabled = true,
            SourceBlend = PwBlendFactor.SourceAlpha,
            DestinationBlend = PwBlendFactor.OneMinusSourceAlpha,
            BlendOperation = PwBlendOperation.Add
        };
    }
    
    /// <summary>
    /// ブレンドファクター
    /// </summary>
    public enum PwBlendFactor
    {
        Zero,
        One,
        SourceAlpha,
        OneMinusSourceAlpha,
        DestinationAlpha,
        OneMinusDestinationAlpha
    }
    
    /// <summary>
    /// ブレンド演算
    /// </summary>
    public enum PwBlendOperation
    {
        Add,
        Subtract,
        ReverseSubtract,
        Min,
        Max
    }
    
    /// <summary>
    /// 深度ステンシル状態
    /// 深度テストとステンシルテストの設定
    /// </summary>
    public struct PwDepthStencilState
    {
        /// <summary>深度テスト有効フラグ</summary>
        public bool DepthTestEnabled { get; set; }
        
        /// <summary>深度書き込み有効フラグ</summary>
        public bool DepthWriteEnabled { get; set; }
        
        /// <summary>深度比較関数</summary>
        public PwComparisonFunction DepthComparison { get; set; }
        
        /// <summary>
        /// デフォルト設定（深度テスト有効、Less比較）
        /// </summary>
        public static readonly PwDepthStencilState Default = new()
        {
            DepthTestEnabled = true,
            DepthWriteEnabled = true,
            DepthComparison = PwComparisonFunction.Less
        };
    }
    
    /// <summary>
    /// 比較関数
    /// </summary>
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
    
    /// <summary>
    /// ラスタライザー状態
    /// プリミティブの描画方法を定義
    /// </summary>
    public struct PwRasterizerState
    {
        /// <summary>カリングモード</summary>
        public PwCullMode CullMode { get; set; }
        
        /// <summary>フィルモード</summary>
        public PwFillMode FillMode { get; set; }
        
        /// <summary>シザーテスト有効フラグ</summary>
        public bool ScissorTestEnabled { get; set; }
        
        /// <summary>
        /// デフォルト設定（背面カリング、ソリッドフィル）
        /// </summary>
        public static readonly PwRasterizerState Default = new()
        {
            CullMode = PwCullMode.Back,
            FillMode = PwFillMode.Solid,
            ScissorTestEnabled = false
        };
    }
    
    /// <summary>
    /// カリングモード
    /// </summary>
    public enum PwCullMode
    {
        /// <summary>カリング無効</summary>
        None,
        /// <summary>前面カリング</summary>
        Front,
        /// <summary>背面カリング</summary>
        Back
    }
    
    /// <summary>
    /// フィルモード
    /// </summary>
    public enum PwFillMode
    {
        /// <summary>塗りつぶし</summary>
        Solid,
        /// <summary>ワイヤーフレーム</summary>
        Wireframe
    }
    
    /// <summary>
    /// レンダーターゲットフォーマット
    /// </summary>
    public struct PwRenderTargetFormat
    {
        /// <summary>テクスチャフォーマット</summary>
        public PwTextureFormat Format { get; set; }
        
        /// <summary>レンダーターゲットインデックス</summary>
        public int Index { get; set; }
    }
}
