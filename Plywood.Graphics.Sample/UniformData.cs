// Plywood.Graphics.Sample/UniformData.cs
using System.Numerics;

namespace Plywood.Graphics.Sample
{
    public struct ViewUniforms
    {
        public Matrix4x4 ViewMatrix;
        public Matrix4x4 ProjectionMatrix;
        public Vector3 CameraPosition;
        public float Time;
        
        public ViewUniforms(Matrix4x4 view, Matrix4x4 projection, Vector3 cameraPos, float time)
        {
            ViewMatrix = view;
            ProjectionMatrix = projection;
            CameraPosition = cameraPos;
            Time = time;
        }
    }
    
    public struct LightUniforms
    {
        public Vector3 LightDirection;
        public float LightIntensity;
        public Vector3 LightColor;
        public float ShadowBias;
        public Matrix4x4 LightViewMatrix;
        public Matrix4x4 LightProjectionMatrix;
        
        public LightUniforms(Vector3 direction, float intensity, Vector3 color, float bias, Matrix4x4 lightView, Matrix4x4 lightProjection)
        {
            LightDirection = direction;
            LightIntensity = intensity;
            LightColor = color;
            ShadowBias = bias;
            LightViewMatrix = lightView;
            LightProjectionMatrix = lightProjection;
        }
    }
}