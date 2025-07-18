// Plywood.Graphics.Core/IPwGraphicsDevice.cs
namespace Plywood.Graphics
{
    /// <summary>
    /// グラフィックスデバイスの抽象化インターフェース
    /// プラットフォーム固有の実装を隠蔽し、統一的な API を提供
    /// </summary>
    public interface IPwGraphicsDevice
    {
        #region バッファ管理
        
        /// <summary>
        /// 型安全な頂点バッファを作成
        /// </summary>
        /// <typeparam name="T">頂点データ型（IPwVertexData を実装）</typeparam>
        /// <param name="data">頂点データ配列</param>
        /// <returns>作成された頂点バッファ</returns>
        PwBuffer CreateVertexBuffer<T>(T[] data) where T : struct, IPwVertexData;
        
        /// <summary>
        /// インデックスバッファを作成
        /// </summary>
        /// <param name="indices">インデックスデータ</param>
        /// <returns>作成されたインデックスバッファ</returns>
        PwBuffer CreateIndexBuffer(ushort[] indices);
        
        /// <summary>
        /// ユニフォームバッファを作成
        /// </summary>
        /// <typeparam name="T">ユニフォームデータ型</typeparam>
        /// <param name="data">ユニフォームデータ</param>
        /// <returns>作成されたユニフォームバッファ</returns>
        PwBuffer CreateUniformBuffer<T>(T data) where T : struct;
        
        /// <summary>
        /// バッファの内容を更新
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="buffer">更新対象のバッファ</param>
        /// <param name="data">新しいデータ</param>
        void UpdateBuffer<T>(PwBuffer buffer, T[] data) where T : struct;
        
        #endregion
        
        #region 生バッファ作成（プール用）
        
        /// <summary>
        /// 生の頂点バッファを作成（バッファプール用）
        /// </summary>
        /// <param name="size">バッファサイズ（バイト）</param>
        /// <param name="flags">バッファフラグ</param>
        /// <returns>作成されたバッファ</returns>
        PwBuffer CreateVertexBufferRaw(int size, PwBufferFlags flags);
        
        /// <summary>
        /// 生のインデックスバッファを作成（バッファプール用）
        /// </summary>
        /// <param name="size">バッファサイズ（バイト）</param>
        /// <param name="flags">バッファフラグ</param>
        /// <returns>作成されたバッファ</returns>
        PwBuffer CreateIndexBufferRaw(int size, PwBufferFlags flags);
        
        /// <summary>
        /// 生のユニフォームバッファを作成（バッファプール用）
        /// </summary>
        /// <param name="size">バッファサイズ（バイト）</param>
        /// <param name="flags">バッファフラグ</param>
        /// <returns>作成されたバッファ</returns>
        PwBuffer CreateUniformBufferRaw(int size, PwBufferFlags flags);
        
        /// <summary>
        /// 生のインスタンスバッファを作成（バッファプール用）
        /// </summary>
        /// <param name="size">バッファサイズ（バイト）</param>
        /// <param name="flags">バッファフラグ</param>
        /// <returns>作成されたバッファ</returns>
        PwBuffer CreateInstanceBufferRaw(int size, PwBufferFlags flags);
        
        /// <summary>
        /// バッファを破棄
        /// </summary>
        /// <param name="buffer">破棄対象のバッファ</param>
        void DestroyBuffer(PwBuffer buffer);
        
        #endregion
        
        #region テクスチャ管理
        
        /// <summary>
        /// テクスチャを作成
        /// </summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="format">ピクセルフォーマット</param>
        /// <returns>作成されたテクスチャ</returns>
        PwTexture CreateTexture(int width, int height, PwTextureFormat format);
        
        /// <summary>
        /// レンダーターゲットを作成
        /// </summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="format">ピクセルフォーマット</param>
        /// <returns>作成されたレンダーターゲット</returns>
        PwTexture CreateRenderTarget(int width, int height, PwTextureFormat format);
        
        /// <summary>
        /// ファイルからテクスチャを読み込み
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>読み込まれたテクスチャ</returns>
        PwTexture LoadTexture(string path);
        
        #endregion
        
        #region パイプライン管理
        
        /// <summary>
        /// レンダリングパイプラインを作成
        /// </summary>
        /// <param name="descriptor">パイプライン記述子</param>
        /// <returns>作成されたパイプライン</returns>
        PwPipeline CreatePipeline(PwPipelineDescriptor descriptor);
        
        /// <summary>
        /// レンダーパスを作成
        /// </summary>
        /// <param name="descriptor">レンダーパス記述子</param>
        /// <returns>作成されたレンダーパス</returns>
        PwRenderPass CreateRenderPass(PwRenderPassDescriptor descriptor);
        
        #endregion
        
        #region シェーダー管理
        
        /// <summary>
        /// シェーダーを読み込み
        /// </summary>
        /// <param name="shaderData">コンパイル済みシェーダーデータ</param>
        /// <param name="stage">シェーダーステージ</param>
        /// <returns>読み込まれたシェーダー</returns>
        PwShader LoadShader(byte[] shaderData, PwShaderStage stage);
        
        #endregion
        
        #region メモリ管理
        
        /// <summary>
        /// 型のメモリ要件を取得
        /// </summary>
        /// <typeparam name="T">対象の型</typeparam>
        /// <returns>メモリ要件</returns>
        PwMemoryRequirements GetMemoryRequirements<T>() where T : struct;
        
        #endregion
        
        #region 同期
        
        /// <summary>
        /// フェンスを作成
        /// </summary>
        /// <returns>作成されたフェンス</returns>
        PwFence CreateFence();
        
        /// <summary>
        /// フェンスを待機
        /// </summary>
        /// <param name="fence">待機対象のフェンス</param>
        void WaitForFence(PwFence fence);
        
        /// <summary>
        /// フェンスをシグナル
        /// </summary>
        /// <param name="fence">シグナル対象のフェンス</param>
        void SignalFence(PwFence fence);
        
        #endregion
        
        #region 描画制御
        
        /// <summary>
        /// 描画結果を画面に表示
        /// </summary>
        void Present();
        
        /// <summary>
        /// フレーム処理を開始
        /// </summary>
        void BeginFrame();
        
        /// <summary>
        /// フレーム処理を終了
        /// </summary>
        void EndFrame();
        
        #endregion
        
        #region コマンドエンコーダー用
        
        /// <summary>
        /// ビューポートを設定
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        void SetViewport(int x, int y, int width, int height);
        
        /// <summary>
        /// レンダーパスを開始
        /// </summary>
        /// <param name="renderPass">レンダーパス</param>
        void BeginRenderPass(PwRenderPass renderPass);
        
        /// <summary>
        /// レンダーパスを終了
        /// </summary>
        /// <param name="renderPass">レンダーパス</param>
        void EndRenderPass(PwRenderPass renderPass);
        
        /// <summary>
        /// パイプラインを設定
        /// </summary>
        /// <param name="pipeline">パイプライン</param>
        void SetPipeline(PwPipeline pipeline);
        
        /// <summary>
        /// 頂点バッファを設定
        /// </summary>
        /// <param name="buffer">頂点バッファ</param>
        /// <param name="bindingIndex">バインディングインデックス</param>
        void SetVertexBuffer(PwBuffer buffer, int bindingIndex);
        
        /// <summary>
        /// インデックスバッファを設定
        /// </summary>
        /// <param name="buffer">インデックスバッファ</param>
        void SetIndexBuffer(PwBuffer buffer);
        
        /// <summary>
        /// インスタンスバッファのオフセットを設定
        /// </summary>
        /// <param name="buffer">インスタンスバッファ</param>
        /// <param name="offset">オフセット</param>
        void SetInstanceBufferOffset(PwBuffer buffer, int offset);
        
        /// <summary>
        /// テクスチャを設定
        /// </summary>
        /// <param name="texture">テクスチャ</param>
        /// <param name="bindingIndex">バインディングインデックス</param>
        void SetTexture(PwTexture texture, int bindingIndex);
        
        /// <summary>
        /// ユニフォームバッファを設定
        /// </summary>
        /// <param name="buffer">ユニフォームバッファ</param>
        /// <param name="bindingIndex">バインディングインデックス</param>
        void SetUniformBuffer(PwBuffer buffer, int bindingIndex);
        
        /// <summary>
        /// インデックス付きインスタンス描画
        /// </summary>
        /// <param name="indexCount">インデックス数</param>
        /// <param name="instanceCount">インスタンス数</param>
        /// <param name="firstIndex">最初のインデックス</param>
        void DrawIndexedInstanced(int indexCount, int instanceCount, int firstIndex);
        
        /// <summary>
        /// インスタンス描画
        /// </summary>
        /// <param name="vertexCount">頂点数</param>
        /// <param name="instanceCount">インスタンス数</param>
        /// <param name="firstVertex">最初の頂点</param>
        void DrawInstanced(int vertexCount, int instanceCount, int firstVertex);
        
        #endregion
    }
}
