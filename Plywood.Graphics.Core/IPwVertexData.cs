// Plywood.Graphics.Core/IPwVertexData.cs
namespace Plywood.Graphics
{
    /// <summary>
    /// 頂点データ構造体のマーカーインターフェース
    /// このインターフェースを実装することで、構造体が頂点データとして
    /// 自動的に認識され、リフレクションによる頂点記述子の自動生成対象となる
    /// </summary>
    public interface IPwVertexData
    {
        // マーカーインターフェースのため、メソッドの実装は不要
        // 単にコンパイル時の型チェックとリフレクション処理の対象識別に使用
    }
}