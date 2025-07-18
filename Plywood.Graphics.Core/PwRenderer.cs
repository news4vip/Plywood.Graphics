// Plywood.Graphics.Core/PwRenderer.cs
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Plywood.Graphics
{
    /// <summary>
    /// 抽象化されたレンダラー基底クラス。プラットフォーム非依存層のエントリポイントを提供。
    /// </summary>
    public abstract class PwRenderer
    {
        /// <summary>グラフィックスデバイス</summary>
        protected IPwGraphicsDevice Device { get; private set; }

        /// <summary>コマンドエンコーダー</summary>
        protected PwCommandEncoder CommandEncoder { get; private set; }

        /// <summary>シェーダーアーカイブマネージャー</summary>
        protected PwShaderArchive ShaderArchive { get; private set; }

        /// <summary>メモリ管理マネージャー</summary>
        protected PwMemoryManager MemoryManager { get; private set; }

        /// <summary>
        /// 初期化処理。アプリケーション側で呼び出し、デバイスをセットアップする。
        /// </summary>
        /// <param name="device">プラットフォーム固有グラフィックスデバイス</param>
        public void Initialize(IPwGraphicsDevice device)
        {
            Device = device ?? throw new ArgumentNullException(nameof(device));
            CommandEncoder = new PwCommandEncoder(device);
            ShaderArchive = new PwShaderArchive(device);
            MemoryManager = new PwMemoryManager(device);
            OnInitialize();
        }

        /// <summary>各フレームの更新と描画処理を実行</summary>
        /// <param name="deltaTime">前フレームからの経過秒数</param>
        public void RunFrame(float deltaTime)
        {
            // デバイス固有のフレームカウンター更新
            PwFrameCounter.Increment(deltaTime);

            // アプリケーション固有の更新
            OnUpdate(deltaTime);

            // コマンド記録開始
            CommandEncoder.Begin();
            OnRender(CommandEncoder);
            CommandEncoder.End();

            // フレーム終了
            Device.Present();
        }

        /// <summary>初期化時のアプリケーション固有処理を実装</summary>
        protected abstract void OnInitialize();

        /// <summary>フレームごとの更新処理を実装</summary>
        protected abstract void OnUpdate(float deltaTime);

        /// <summary>フレームごとの描画コードを実装</summary>
        protected abstract void OnRender(PwCommandEncoder encoder);

        #region ヘルパーメソッド

        /// <summary>型安全な頂点バッファ作成</summary>
        protected PwBuffer CreateVertexBuffer<T>(T[] data) where T : struct, IPwVertexData
            => MemoryManager.CreateOptimizedVertexBuffer(data);

        /// <summary>汎用バッファ作成</summary>
        protected PwBuffer CreateBuffer<T>(T[] data, PwBufferUsage usage) where T : struct
            => MemoryManager.CreateOptimizedBuffer(data, usage);

        /// <summary>インデックスバッファ作成</summary>
        protected PwBuffer CreateIndexBuffer(ushort[] indices)
            => Device.CreateIndexBuffer(indices);

        /// <summary>ユニフォームバッファ作成</summary>
        protected PwBuffer CreateUniformBuffer<T>(T data) where T : struct
            => Device.CreateUniformBuffer(data);

        /// <summary>テクスチャ作成</summary>
        protected PwTexture CreateTexture(int width, int height, PwTextureFormat format)
            => Device.CreateTexture(width, height, format);

        /// <summary>Zip からパイプライン読み込み</summary>
        protected PwPipeline LoadPipelineFromZip(string zipPath, string pipelineName)
            => ShaderArchive.LoadPipeline(zipPath, pipelineName);

        /// <summary>頂点／インスタンスタイプからパイプライン作成</summary>
        protected PwPipeline CreatePipelineFromVertexTypes<TV, TI>(string zipPath, string pipelineName)
            where TV : struct, IPwVertexData
            where TI : struct, IPwVertexData
        {
            // 自動生成した頂点記述子を注入
            var vd = PwVertexDescriptor.CreateFromType<TV>(); vd.BindingIndex = 0;
            var id = PwVertexDescriptor.CreateFromType<TI>(); id.BindingIndex = 1;
            var basePipe = ShaderArchive.LoadPipeline(zipPath, pipelineName);
            basePipe.Descriptor.VertexDescriptors = new[] { vd, id };
            return Device.CreatePipeline(basePipe.Descriptor);
        }

        #endregion
    }
}
