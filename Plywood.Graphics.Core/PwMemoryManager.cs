// Plywood.Graphics.Core/PwMemoryManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Plywood.Graphics
{
    /// <summary>
    /// メモリ要件とバッファ作成を管理するクラス
    /// 型制約を適切に処理し、リフレクションを使用して型安全性を保つ
    /// </summary>
    public class PwMemoryManager
    {
        private readonly IPwGraphicsDevice device;
        private readonly Dictionary<(Type, PwBufferUsage), PwMemoryRequirements> requirementsCache;

        public PwMemoryManager(IPwGraphicsDevice device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            this.requirementsCache = new Dictionary<(Type, PwBufferUsage), PwMemoryRequirements>();
        }

        /// <summary>
        /// 頂点データ専用の最適化バッファを作成
        /// IPwVertexDataを実装した構造体のみを受け入れる
        /// </summary>
        /// <typeparam name="T">IPwVertexDataを実装した構造体型</typeparam>
        /// <param name="data">頂点データ配列</param>
        /// <returns>最適化された頂点バッファ</returns>
        public PwBuffer CreateOptimizedVertexBuffer<T>(T[] data) where T : struct, IPwVertexData
        {
            var requirements = GetOrCreateRequirements<T>(PwBufferUsage.Vertex);
            var descriptor = PwVertexDescriptor.CreateFromType<T>();
            var optimizedData = RestructureVertexData(data, descriptor, requirements);
            
            return device.CreateVertexBuffer(optimizedData);
        }

        /// <summary>
        /// 汎用バッファ作成メソッド
        /// 型制約を適切に処理し、リフレクションを使用して型安全性を保つ
        /// </summary>
        /// <typeparam name="T">バッファデータの型</typeparam>
        /// <param name="data">データ配列</param>
        /// <param name="usage">バッファの用途</param>
        /// <returns>作成されたバッファ</returns>
        public PwBuffer CreateOptimizedBuffer<T>(T[] data, PwBufferUsage usage) where T : struct
        {
            var requirements = GetOrCreateRequirements<T>(usage);

            return usage switch
            {
                PwBufferUsage.Vertex => CreateVertexBufferInternal(data, requirements),
                PwBufferUsage.Index => CreateIndexBufferInternal(data, requirements),
                PwBufferUsage.Uniform => CreateUniformBufferInternal(data, requirements),
                PwBufferUsage.Instance => CreateInstanceBufferInternal(data, requirements),
                _ => throw new ArgumentOutOfRangeException(nameof(usage))
            };
        }

        /// <summary>
        /// 頂点バッファの内部作成処理
        /// IPwVertexDataの実装チェックとリフレクションによる型安全な呼び出し
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="data">データ配列</param>
        /// <param name="requirements">メモリ要件</param>
        /// <returns>作成された頂点バッファ</returns>
        private PwBuffer CreateVertexBufferInternal<T>(T[] data, PwMemoryRequirements requirements) where T : struct
        {
            // IPwVertexDataの実装チェック
            if (!typeof(IPwVertexData).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} must implement IPwVertexData for vertex buffer creation");
            }

            // 型制約を満たすCreateOptimizedVertexBufferをリフレクションで呼び出し
            var method = typeof(PwMemoryManager).GetMethod(nameof(CreateOptimizedVertexBuffer));
            if (method == null)
            {
                throw new InvalidOperationException("CreateOptimizedVertexBuffer method not found");
            }

            var genericMethod = method.MakeGenericMethod(typeof(T));
            var result = genericMethod.Invoke(this, new object[] { data });
            
            return (PwBuffer)result!;
        }

        /// <summary>
        /// インデックスバッファの内部作成処理
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="data">データ配列</param>
        /// <param name="requirements">メモリ要件</param>
        /// <returns>作成されたインデックスバッファ</returns>
        private PwBuffer CreateIndexBufferInternal<T>(T[] data, PwMemoryRequirements requirements) where T : struct
        {
            // ushort または uint のみサポート
            if (typeof(T) == typeof(ushort))
            {
                var indexData = data.Cast<ushort>().ToArray();
                return device.CreateIndexBuffer(indexData);
            }
            else if (typeof(T) == typeof(uint))
            {
                // uint を ushort に変換（範囲チェック付き）
                var indexData = data.Cast<uint>().Select(x => 
                {
                    if (x > ushort.MaxValue)
                        throw new ArgumentOutOfRangeException($"Index value {x} exceeds ushort.MaxValue");
                    return (ushort)x;
                }).ToArray();
                return device.CreateIndexBuffer(indexData);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported index type: {typeof(T).Name}. Only ushort and uint are supported.");
            }
        }

        /// <summary>
        /// ユニフォームバッファの内部作成処理
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="data">データ配列</param>
        /// <param name="requirements">メモリ要件</param>
        /// <returns>作成されたユニフォームバッファ</returns>
        private PwBuffer CreateUniformBufferInternal<T>(T[] data, PwMemoryRequirements requirements) where T : struct
        {
            if (data.Length != 1)
            {
                throw new InvalidOperationException("Uniform buffer must contain exactly one element");
            }

            var optimizedData = RestructureUniformData(data[0], requirements);
            return device.CreateUniformBuffer(optimizedData);
        }

        /// <summary>
        /// インスタンスバッファの内部作成処理
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="data">データ配列</param>
        /// <param name="requirements">メモリ要件</param>
        /// <returns>作成されたインスタンスバッファ</returns>
        private PwBuffer CreateInstanceBufferInternal<T>(T[] data, PwMemoryRequirements requirements) where T : struct
        {
            // インスタンスデータもIPwVertexDataの実装が必要
            if (!typeof(IPwVertexData).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} must implement IPwVertexData for instance buffer creation");
            }

            // 頂点バッファと同じ処理を行う
            return CreateVertexBufferInternal(data, requirements);
        }

        /// <summary>
        /// 型安全な頂点データ再構築
        /// プラットフォーム固有のアライメントとパディングを適用
        /// </summary>
        /// <typeparam name="T">頂点データ型</typeparam>
        /// <param name="data">元のデータ</param>
        /// <param name="descriptor">頂点記述子</param>
        /// <param name="requirements">メモリ要件</param>
        /// <returns>最適化されたデータ</returns>
        private T[] RestructureVertexData<T>(T[] data, PwVertexDescriptor descriptor, PwMemoryRequirements requirements) 
            where T : struct, IPwVertexData
        {
            // 最適化されたストライドを計算
            var optimizedStride = CalculateOptimizedStride(descriptor, requirements);
            
            if (optimizedStride == descriptor.Stride)
            {
                // 最適化が不要な場合はそのまま返す
                return data;
            }

            // メモリレイアウト最適化
            var optimizedData = new T[data.Length];
            
            for (int i = 0; i < data.Length; i++)
            {
                optimizedData[i] = OptimizeVertexStructure(data[i], descriptor, requirements);
            }

            return optimizedData;
        }

        /// <summary>
        /// ユニフォームデータの再構築
        /// </summary>
        /// <typeparam name="T">ユニフォームデータ型</typeparam>
        /// <param name="data">元のデータ</param>
        /// <param name="requirements">メモリ要件</param>
        /// <returns>最適化されたデータ</returns>
        private T RestructureUniformData<T>(T data, PwMemoryRequirements requirements) where T : struct
        {
            // プラットフォーム固有のユニフォームバッファ最適化
            // 実際の実装では、パディングやアライメントの調整を行う
            return data;
        }

        /// <summary>
        /// 頂点構造体の最適化
        /// </summary>
        /// <typeparam name="T">頂点データ型</typeparam>
        /// <param name="vertex">頂点データ</param>
        /// <param name="descriptor">頂点記述子</param>
        /// <param name="requirements">メモリ要件</param>
        /// <returns>最適化された頂点データ</returns>
        private T OptimizeVertexStructure<T>(T vertex, PwVertexDescriptor descriptor, PwMemoryRequirements requirements) 
            where T : struct, IPwVertexData
        {
            // プラットフォーム固有の最適化
            // 実際の実装では、フィールドの再配置やパディング調整を行う
            // 現在はそのまま返す
            return vertex;
        }

        /// <summary>
        /// 最適化されたストライドの計算
        /// </summary>
        /// <param name="descriptor">頂点記述子</param>
        /// <param name="requirements">メモリ要件</param>
        /// <returns>最適化されたストライド</returns>
        private int CalculateOptimizedStride(PwVertexDescriptor descriptor, PwMemoryRequirements requirements)
        {
            var baseStride = descriptor.Stride;
            var alignedStride = AlignToRequirement(baseStride, requirements.Alignment);
            
            return Math.Max(alignedStride, requirements.MinAlignment);
        }

        /// <summary>
        /// アライメント調整
        /// </summary>
        /// <param name="size">元のサイズ</param>
        /// <param name="alignment">アライメント要件</param>
        /// <returns>アライメントされたサイズ</returns>
        private int AlignToRequirement(int size, int alignment)
        {
            return (size + alignment - 1) & ~(alignment - 1);
        }

        /// <summary>
        /// メモリ要件の取得またはキャッシュからの取得
        /// </summary>
        /// <typeparam name="T">データ型</typeparam>
        /// <param name="usage">バッファ用途</param>
        /// <returns>メモリ要件</returns>
        private PwMemoryRequirements GetOrCreateRequirements<T>(PwBufferUsage usage) where T : struct
        {
            var key = (typeof(T), usage);
            
            if (!requirementsCache.TryGetValue(key, out var requirements))
            {
                requirements = device.GetMemoryRequirements<T>();
                requirements.MemoryType = usage switch
                {
                    PwBufferUsage.Vertex => PwMemoryType.VertexBuffer,
                    PwBufferUsage.Index => PwMemoryType.IndexBuffer,
                    PwBufferUsage.Uniform => PwMemoryType.UniformBuffer,
                    PwBufferUsage.Instance => PwMemoryType.VertexBuffer,
                    _ => PwMemoryType.VertexBuffer
                };
                
                requirementsCache[key] = requirements;
            }
            
            return requirements;
        }

        /// <summary>
        /// メモリ使用量の取得
        /// </summary>
        /// <returns>メモリ使用量情報</returns>
        public PwMemoryUsageInfo GetMemoryUsage()
        {
            return new PwMemoryUsageInfo
            {
                CacheEntries = requirementsCache.Count,
                TotalCachedTypes = requirementsCache.Keys.Select(k => k.Item1).Distinct().Count()
            };
        }

        /// <summary>
        /// キャッシュのクリア
        /// </summary>
        public void ClearCache()
        {
            requirementsCache.Clear();
        }
    }

    /// <summary>
    /// メモリ使用量情報
    /// </summary>
    public class PwMemoryUsageInfo
    {
        /// <summary>キャッシュエントリ数</summary>
        public int CacheEntries { get; set; }
        
        /// <summary>キャッシュされた型の総数</summary>
        public int TotalCachedTypes { get; set; }
    }
}
