// Plywood.Graphics.Sample/PrimitiveGenerator.cs
using System.Numerics;

namespace Plywood.Graphics.Sample
{
    public class PrimitiveData
    {
        public Vertex[] Vertices { get; set; } = Array.Empty<Vertex>();
        public ushort[] Indices { get; set; } = Array.Empty<ushort>();
    }
    
    public class PrimitiveGenerator
    {
        public PrimitiveData GenerateCube()
        {
            var vertices = new Vertex[24]; // 各面4頂点
            var indices = new ushort[36];  // 各面2三角形
            
            // 前面 (Z+)
            vertices[0] = new Vertex(new Vector3(-1, -1, 1), new Vector3(0, 0, 1), new Vector2(0, 1), Vector4.One);
            vertices[1] = new Vertex(new Vector3(1, -1, 1), new Vector3(0, 0, 1), new Vector2(1, 1), Vector4.One);
            vertices[2] = new Vertex(new Vector3(1, 1, 1), new Vector3(0, 0, 1), new Vector2(1, 0), Vector4.One);
            vertices[3] = new Vertex(new Vector3(-1, 1, 1), new Vector3(0, 0, 1), new Vector2(0, 0), Vector4.One);
            
            // 後面 (Z-)
            vertices[4] = new Vertex(new Vector3(1, -1, -1), new Vector3(0, 0, -1), new Vector2(0, 1), Vector4.One);
            vertices[5] = new Vertex(new Vector3(-1, -1, -1), new Vector3(0, 0, -1), new Vector2(1, 1), Vector4.One);
            vertices[6] = new Vertex(new Vector3(-1, 1, -1), new Vector3(0, 0, -1), new Vector2(1, 0), Vector4.One);
            vertices[7] = new Vertex(new Vector3(1, 1, -1), new Vector3(0, 0, -1), new Vector2(0, 0), Vector4.One);
            
            // 右面 (X+)
            vertices[8] = new Vertex(new Vector3(1, -1, 1), new Vector3(1, 0, 0), new Vector2(0, 1), Vector4.One);
            vertices[9] = new Vertex(new Vector3(1, -1, -1), new Vector3(1, 0, 0), new Vector2(1, 1), Vector4.One);
            vertices[10] = new Vertex(new Vector3(1, 1, -1), new Vector3(1, 0, 0), new Vector2(1, 0), Vector4.One);
            vertices[11] = new Vertex(new Vector3(1, 1, 1), new Vector3(1, 0, 0), new Vector2(0, 0), Vector4.One);
            
            // 左面 (X-)
            vertices[12] = new Vertex(new Vector3(-1, -1, -1), new Vector3(-1, 0, 0), new Vector2(0, 1), Vector4.One);
            vertices[13] = new Vertex(new Vector3(-1, -1, 1), new Vector3(-1, 0, 0), new Vector2(1, 1), Vector4.One);
            vertices[14] = new Vertex(new Vector3(-1, 1, 1), new Vector3(-1, 0, 0), new Vector2(1, 0), Vector4.One);
            vertices[15] = new Vertex(new Vector3(-1, 1, -1), new Vector3(-1, 0, 0), new Vector2(0, 0), Vector4.One);
            
            // 上面 (Y+)
            vertices[16] = new Vertex(new Vector3(-1, 1, 1), new Vector3(0, 1, 0), new Vector2(0, 1), Vector4.One);
            vertices[17] = new Vertex(new Vector3(1, 1, 1), new Vector3(0, 1, 0), new Vector2(1, 1), Vector4.One);
            vertices[18] = new Vertex(new Vector3(1, 1, -1), new Vector3(0, 1, 0), new Vector2(1, 0), Vector4.One);
            vertices[19] = new Vertex(new Vector3(-1, 1, -1), new Vector3(0, 1, 0), new Vector2(0, 0), Vector4.One);
            
            // 下面 (Y-)
            vertices[20] = new Vertex(new Vector3(-1, -1, -1), new Vector3(0, -1, 0), new Vector2(0, 1), Vector4.One);
            vertices[21] = new Vertex(new Vector3(1, -1, -1), new Vector3(0, -1, 0), new Vector2(1, 1), Vector4.One);
            vertices[22] = new Vertex(new Vector3(1, -1, 1), new Vector3(0, -1, 0), new Vector2(1, 0), Vector4.One);
            vertices[23] = new Vertex(new Vector3(-1, -1, 1), new Vector3(0, -1, 0), new Vector2(0, 0), Vector4.One);
            
            // インデックス生成
            for (int i = 0; i < 6; i++)
            {
                int baseIndex = i * 4;
                int indexBase = i * 6;
                
                // 三角形1
                indices[indexBase + 0] = (ushort)(baseIndex + 0);
                indices[indexBase + 1] = (ushort)(baseIndex + 1);
                indices[indexBase + 2] = (ushort)(baseIndex + 2);
                
                // 三角形2
                indices[indexBase + 3] = (ushort)(baseIndex + 0);
                indices[indexBase + 4] = (ushort)(baseIndex + 2);
                indices[indexBase + 5] = (ushort)(baseIndex + 3);
            }
            
            return new PrimitiveData { Vertices = vertices, Indices = indices };
        }
        
        public PrimitiveData GenerateSphere(int rings, int sectors)
        {
            var vertexCount = (rings + 1) * (sectors + 1);
            var vertices = new Vertex[vertexCount];
            var indices = new List<ushort>();
            
            // 球面座標から頂点生成
            for (int r = 0; r <= rings; r++)
            {
                for (int s = 0; s <= sectors; s++)
                {
                    var phi = Math.PI * r / rings;
                    var theta = 2.0 * Math.PI * s / sectors;
                    
                    var x = Math.Sin(phi) * Math.Cos(theta);
                    var y = Math.Cos(phi);
                    var z = Math.Sin(phi) * Math.Sin(theta);
                    
                    var position = new Vector3((float)x, (float)y, (float)z);
                    var normal = Vector3.Normalize(position);
                    var texCoord = new Vector2((float)s / sectors, (float)r / rings);
                    
                    vertices[r * (sectors + 1) + s] = new Vertex(position, normal, texCoord, Vector4.One);
                }
            }
            
            // インデックス生成
            for (int r = 0; r < rings; r++)
            {
                for (int s = 0; s < sectors; s++)
                {
                    var current = r * (sectors + 1) + s;
                    var next = current + sectors + 1;
                    
                    // 三角形1
                    indices.Add((ushort)current);
                    indices.Add((ushort)next);
                    indices.Add((ushort)(current + 1));
                    
                    // 三角形2
                    indices.Add((ushort)(current + 1));
                    indices.Add((ushort)next);
                    indices.Add((ushort)(next + 1));
                }
            }
            
            return new PrimitiveData { Vertices = vertices, Indices = indices.ToArray() };
        }
        
        public PrimitiveData GenerateCylinder(int sectors)
        {
            var vertexCount = (sectors + 1) * 2 + 2; // 上下円周 + 中心点
            var vertices = new Vertex[vertexCount];
            var indices = new List<ushort>();
            
            // 上面中心
            vertices[0] = new Vertex(new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector2(0.5f, 0.5f), Vector4.One);
            
            // 下面中心
            vertices[1] = new Vertex(new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector2(0.5f, 0.5f), Vector4.One);
            
            // 上下円周頂点
            for (int i = 0; i <= sectors; i++)
            {
                var angle = 2.0f * Math.PI * i / sectors;
                var x = Math.Cos(angle);
                var z = Math.Sin(angle);
                
                var position = new Vector3((float)x, 1, (float)z);
                var normal = new Vector3((float)x, 0, (float)z);
                var texCoord = new Vector2((float)i / sectors, 0);
                
                vertices[2 + i] = new Vertex(position, normal, texCoord, Vector4.One);
                
                position = new Vector3((float)x, -1, (float)z);
                texCoord = new Vector2((float)i / sectors, 1);
                
                vertices[2 + sectors + 1 + i] = new Vertex(position, normal, texCoord, Vector4.One);
            }
            
            // 側面インデックス
            for (int i = 0; i < sectors; i++)
            {
                var topCurrent = 2 + i;
                var topNext = 2 + i + 1;
                var bottomCurrent = 2 + sectors + 1 + i;
                var bottomNext = 2 + sectors + 1 + i + 1;
                
                // 三角形1
                indices.Add((ushort)topCurrent);
                indices.Add((ushort)bottomCurrent);
                indices.Add((ushort)topNext);
                
                // 三角形2
                indices.Add((ushort)topNext);
                indices.Add((ushort)bottomCurrent);
                indices.Add((ushort)bottomNext);
            }
            
            return new PrimitiveData { Vertices = vertices, Indices = indices.ToArray() };
        }
    }
}
