// Plywood.Graphics.Sample/Vertex.cs
using System.Numerics;

namespace Plywood.Graphics.Sample
{
    public struct Vertex : IPwVertexData
    {
        [PwVertex(PwVertexSemantic.Position)]
        public Vector3 Position;
        
        [PwVertex(PwVertexSemantic.Normal)]
        public Vector3 Normal;
        
        [PwVertex(PwVertexSemantic.TexCoord, 0)]
        public Vector2 TexCoord;
        
        [PwVertex(PwVertexSemantic.Color)]
        public Vector4 Color;
        
        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord, Vector4 color)
        {
            Position = position;
            Normal = normal;
            TexCoord = texCoord;
            Color = color;
        }
    }
    
    public struct InstanceData : IPwVertexData
    {
        [PwVertex(PwVertexSemantic.InstanceTransform)]
        public Matrix4x4 ModelMatrix;
        
        [PwVertex(PwVertexSemantic.InstanceColor)]
        public Vector4 Color;
        
        [PwVertex(PwVertexSemantic.InstanceScale)]
        public float Scale;
        
        public InstanceData(Matrix4x4 modelMatrix, Vector4 color, float scale)
        {
            ModelMatrix = modelMatrix;
            Color = color;
            Scale = scale;
        }
    }
}