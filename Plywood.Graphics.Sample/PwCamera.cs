// Plywood.Graphics.Sample/PwCamera.cs
using System.Numerics;

namespace Plywood.Graphics.Sample
{
    public class PwCamera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }
        
        public Matrix4x4 ViewMatrix { get; private set; }
        public Matrix4x4 ProjectionMatrix { get; private set; }
        public Matrix4x4 ViewProjectionMatrix { get; private set; }
        
        private PwFrustum frustum = new PwFrustum();
        
        public void UpdateMatrices()
        {
            ViewMatrix = Matrix4x4.CreateLookAt(Position, Target, Up);
            ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                FieldOfView, AspectRatio, NearPlane, FarPlane);
            ViewProjectionMatrix = ViewMatrix * ProjectionMatrix;
            
            frustum.UpdateFromMatrix(ViewProjectionMatrix);
        }
        
        public PwFrustum GetFrustum() => frustum;
    }
}