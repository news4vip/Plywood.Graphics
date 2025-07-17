// Plywood.Graphics.Sample/PwCullingSystem.cs
using System.Numerics;

namespace Plywood.Graphics.Sample
{
    public class PwCullingSystem
    {
        private readonly List<PwCullableObject> objects;
        private readonly List<PwCullableObject> visibleObjects;
        
        public PwCullingSystem()
        {
            objects = new List<PwCullableObject>();
            visibleObjects = new List<PwCullableObject>();
        }
        
        public void RegisterObject(PwCullableObject obj)
        {
            objects.Add(obj);
        }
        
        public void UnregisterObject(PwCullableObject obj)
        {
            objects.Remove(obj);
        }
        
        public IReadOnlyList<PwCullableObject> CullObjects(PwCamera camera)
        {
            visibleObjects.Clear();
            
            var frustum = camera.GetFrustum();
            
            foreach (var obj in objects)
            {
                if (IsObjectVisible(obj, frustum, camera))
                {
                    visibleObjects.Add(obj);
                }
            }
            
            // 距離でソート（近い順）
            SortObjectsByDistance(visibleObjects, camera.Position);
            
            return visibleObjects;
        }
        
        private bool IsObjectVisible(PwCullableObject obj, PwFrustum frustum, PwCamera camera)
        {
            // バウンディングボックスチェック
            if (!frustum.IntersectsBoundingBox(obj.BoundingBox))
                return false;
            
            // 距離チェック
            var distance = Vector3.Distance(obj.Position, camera.Position);
            if (distance > obj.MaxDrawDistance)
                return false;
            
            return true;
        }
        
        private void SortObjectsByDistance(List<PwCullableObject> objects, Vector3 cameraPosition)
        {
            objects.Sort((a, b) =>
            {
                var distA = Vector3.DistanceSquared(a.Position, cameraPosition);
                var distB = Vector3.DistanceSquared(b.Position, cameraPosition);
                return distA.CompareTo(distB);
            });
        }
    }
    
    public class PwCullableObject
    {
        public Vector3 Position { get; set; }
        public PwBoundingBox BoundingBox { get; set; }
        public float MaxDrawDistance { get; set; } = 100.0f;
        public bool EnableOcclusionCulling { get; set; } = true;
        public int LODLevel { get; set; } = 0;
        public PwRenderObject RenderObject { get; set; }
    }
    
    public struct PwBoundingBox
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
        
        public Vector3 Center => (Min + Max) * 0.5f;
        public Vector3 Size => Max - Min;
        
        public PwBoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
        
        public bool Intersects(PwBoundingBox other)
        {
            return Min.X <= other.Max.X && Max.X >= other.Min.X &&
                   Min.Y <= other.Max.Y && Max.Y >= other.Min.Y &&
                   Min.Z <= other.Max.Z && Max.Z >= other.Min.Z;
        }
    }
    
    public class PwFrustum
    {
        private readonly Plane[] planes = new Plane[6];
        
        public void UpdateFromMatrix(Matrix4x4 viewProjectionMatrix)
        {
            var m = viewProjectionMatrix;
            
            // Left plane
            planes[0] = new Plane(
                m.M14 + m.M11,
                m.M24 + m.M21,
                m.M34 + m.M31,
                m.M44 + m.M41
            );
            
            // Right plane
            planes[1] = new Plane(
                m.M14 - m.M11,
                m.M24 - m.M21,
                m.M34 - m.M31,
                m.M44 - m.M41
            );
            
            // Bottom plane
            planes[2] = new Plane(
                m.M14 + m.M12,
                m.M24 + m.M22,
                m.M34 + m.M32,
                m.M44 + m.M42
            );
            
            // Top plane
            planes[3] = new Plane(
                m.M14 - m.M12,
                m.M24 - m.M22,
                m.M34 - m.M32,
                m.M44 - m.M42
            );
            
            // Near plane
            planes[4] = new Plane(
                m.M13,
                m.M23,
                m.M33,
                m.M43
            );
            
            // Far plane
            planes[5] = new Plane(
                m.M14 - m.M13,
                m.M24 - m.M23,
                m.M34 - m.M33,
                m.M44 - m.M43
            );
            
            // 平面を正規化
            for (int i = 0; i < 6; i++)
            {
                planes[i] = Plane.Normalize(planes[i]);
            }
        }
        
        public bool IntersectsBoundingBox(PwBoundingBox box)
        {
            foreach (var plane in planes)
            {
                var positiveVertex = GetPositiveVertex(box, plane.Normal);
                if (Plane.DotCoordinate(plane, positiveVertex) < 0)
                    return false;
            }
            return true;
        }
        
        private Vector3 GetPositiveVertex(PwBoundingBox box, Vector3 normal)
        {
            return new Vector3(
                normal.X >= 0 ? box.Max.X : box.Min.X,
                normal.Y >= 0 ? box.Max.Y : box.Min.Y,
                normal.Z >= 0 ? box.Max.Z : box.Min.Z
            );
        }
    }
    
    public class PwRenderObject
    {
        public PwPipeline Pipeline { get; set; }
        public PwBuffer VertexBuffer { get; set; }
        public PwBuffer IndexBuffer { get; set; }
        public PwBuffer InstanceBuffer { get; set; }
        public PwTexture[] Textures { get; set; }
        public int IndexCount { get; set; }
        public int InstanceCount { get; set; }
        public int FirstIndex { get; set; }
        public int InstanceOffset { get; set; }
        public Vector3 Position { get; set; }
        public PwBoundingBox BoundingBox { get; set; }
    }
}
