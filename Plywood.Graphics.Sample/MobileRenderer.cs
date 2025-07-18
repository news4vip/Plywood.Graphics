// Plywood.Graphics.Sample/MobileRenderer.cs
using System.Numerics;

namespace Plywood.Graphics.Sample
{
    public class MobileRenderer : PwRenderer
    {
        // バッファリソース
        private PwBuffer cubeVertexBuffer;
        private PwBuffer cubeIndexBuffer;
        private PwBuffer sphereVertexBuffer;
        private PwBuffer sphereIndexBuffer;
        private PwBuffer cylinderVertexBuffer;
        private PwBuffer cylinderIndexBuffer;
        private PwBuffer instanceBuffer;
        private PwBuffer viewUniformBuffer;
        private PwBuffer lightUniformBuffer;
        
        // テクスチャリソース
        private PwTexture shadowMap;
        
        // パイプライン
        private PwPipeline mainPipeline;
        private PwPipeline shadowPipeline;
        
        // プリミティブデータ
        private PrimitiveGenerator primitiveGenerator;
        private PrimitiveData cubeData;
        private PrimitiveData sphereData;
        private PrimitiveData cylinderData;
        
        // カメラとライト
        private PwCamera camera;
        private float currentTime;
        private Vector3 cameraPosition;
        private Vector3 lightDirection;
        
        // カリングシステム
        private PwCullingSystem cullingSystem;
        private List<PwRenderObject> renderObjects;
        private List<PwCullableObject> cullableObjects;
        
        // 定数
        private const int InstanceCount = 100;
        private const int ShadowMapSize = 1024;
        
        protected override void OnInitialize()
        {
            // カメラ初期化
            camera = new PwCamera
            {
                Position = new Vector3(0, 5, 10),
                Target = Vector3.Zero,
                Up = Vector3.UnitY,
                FieldOfView = MathF.PI / 4.0f,
                AspectRatio = 1080.0f / 1920.0f,
                NearPlane = 0.1f,
                FarPlane = 100.0f
            };
            
            // カリングシステム初期化
            cullingSystem = new PwCullingSystem();
            renderObjects = new List<PwRenderObject>();
            cullableObjects = new List<PwCullableObject>();
            
            // 初期値設定
            cameraPosition = new Vector3(0, 5, 10);
            lightDirection = Vector3.Normalize(new Vector3(-1, -1, -1));
            
            // プリミティブ生成
            InitializePrimitives();
            
            // シェーダーとパイプライン
            InitializeShaders();
            
            // レンダーオブジェクト作成
            InitializeRenderObjects();
            
            Console.WriteLine("MobileRenderer initialized successfully");
            Console.WriteLine($"Cube: {cubeData.Vertices.Length} vertices, {cubeData.Indices.Length} indices");
            Console.WriteLine($"Sphere: {sphereData.Vertices.Length} vertices, {sphereData.Indices.Length} indices");
            Console.WriteLine($"Cylinder: {cylinderData.Vertices.Length} vertices, {cylinderData.Indices.Length} indices");
        }
        
        private void InitializePrimitives()
        {
            primitiveGenerator = new PrimitiveGenerator();
            
            cubeData = primitiveGenerator.GenerateCube();
            sphereData = primitiveGenerator.GenerateSphere(16, 16);
            cylinderData = primitiveGenerator.GenerateCylinder(16);
            
            // 頂点バッファ作成
            cubeVertexBuffer = CreateVertexBuffer(cubeData.Vertices);
            cubeIndexBuffer = CreateIndexBuffer(cubeData.Indices);
            
            sphereVertexBuffer = CreateVertexBuffer(sphereData.Vertices);
            sphereIndexBuffer = CreateIndexBuffer(sphereData.Indices);
            
            cylinderVertexBuffer = CreateVertexBuffer(cylinderData.Vertices);
            cylinderIndexBuffer = CreateIndexBuffer(cylinderData.Indices);
            
            // インスタンスデータ作成
            var instanceData = GenerateInstanceData(InstanceCount);
            instanceBuffer = CreateVertexBuffer(instanceData);
            
            // シャドウマップ作成
            shadowMap = CreateTexture(ShadowMapSize, ShadowMapSize, PwTextureFormat.Depth32Float);
        }
        
        private void InitializeShaders()
        {
            // シェーダーアーカイブからパイプライン読み込み
            try
            {
                mainPipeline = CreatePipelineFromVertexTypes<Vertex, InstanceData>("shaders.zip", "main_pipeline");
                shadowPipeline = CreatePipelineFromVertexTypes<Vertex, InstanceData>("shaders.zip", "shadow_pipeline");
                
                Console.WriteLine("Pipelines loaded successfully from shader archive");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load pipelines: {ex.Message}");
                // フォールバック処理
            }
        }
        
        private void InitializeRenderObjects()
        {
            renderObjects.Clear();
            cullableObjects.Clear();
            
            // キューブレンダーオブジェクト
            for (int i = 0; i < InstanceCount / 3; i++)
            {
                var renderObj = new PwRenderObject
                {
                    Pipeline = mainPipeline,
                    VertexBuffer = cubeVertexBuffer,
                    IndexBuffer = cubeIndexBuffer,
                    InstanceBuffer = instanceBuffer,
                    Textures = new[] { shadowMap },
                    IndexCount = cubeData.Indices.Length,
                    InstanceCount = 1,
                    FirstIndex = 0,
                    InstanceOffset = i
                };
                
                var cullableObj = new PwCullableObject
                {
                    Position = GetInstancePosition(i),
                    BoundingBox = new PwBoundingBox(
                        new Vector3(-1, -1, -1),
                        new Vector3(1, 1, 1)
                    ),
                    MaxDrawDistance = 50.0f,
                    EnableOcclusionCulling = true,
                    RenderObject = renderObj
                };
                
                renderObjects.Add(renderObj);
                cullableObjects.Add(cullableObj);
                cullingSystem.RegisterObject(cullableObj);
            }
            
            // スフィアとシリンダーも同様に追加
            AddSphereRenderObjects();
            AddCylinderRenderObjects();
            
            // ユニフォームバッファ作成
            var viewUniforms = CreateViewUniforms();
            var lightUniforms = CreateLightUniforms();
            
            viewUniformBuffer = Device.CreateUniformBuffer(viewUniforms);
            lightUniformBuffer = Device.CreateUniformBuffer(lightUniforms);
        }
        
        private void AddSphereRenderObjects()
        {
            for (int i = 0; i < InstanceCount / 3; i++)
            {
                var renderObj = new PwRenderObject
                {
                    Pipeline = mainPipeline,
                    VertexBuffer = sphereVertexBuffer,
                    IndexBuffer = sphereIndexBuffer,
                    InstanceBuffer = instanceBuffer,
                    Textures = new[] { shadowMap },
                    IndexCount = sphereData.Indices.Length,
                    InstanceCount = 1,
                    FirstIndex = 0,
                    InstanceOffset = InstanceCount / 3 + i
                };
                
                var cullableObj = new PwCullableObject
                {
                    Position = GetInstancePosition(InstanceCount / 3 + i),
                    BoundingBox = new PwBoundingBox(
                        new Vector3(-1, -1, -1),
                        new Vector3(1, 1, 1)
                    ),
                    MaxDrawDistance = 50.0f,
                    EnableOcclusionCulling = true,
                    RenderObject = renderObj
                };
                
                renderObjects.Add(renderObj);
                cullableObjects.Add(cullableObj);
                cullingSystem.RegisterObject(cullableObj);
            }
        }
        
        private void AddCylinderRenderObjects()
        {
            for (int i = 0; i < InstanceCount / 3; i++)
            {
                var renderObj = new PwRenderObject
                {
                    Pipeline = mainPipeline,
                    VertexBuffer = cylinderVertexBuffer,
                    IndexBuffer = cylinderIndexBuffer,
                    InstanceBuffer = instanceBuffer,
                    Textures = new[] { shadowMap },
                    IndexCount = cylinderData.Indices.Length,
                    InstanceCount = 1,
                    FirstIndex = 0,
                    InstanceOffset = (InstanceCount / 3) * 2 + i
                };
                
                var cullableObj = new PwCullableObject
                {
                    Position = GetInstancePosition((InstanceCount / 3) * 2 + i),
                    BoundingBox = new PwBoundingBox(
                        new Vector3(-1, -1, -1),
                        new Vector3(1, 1, 1)
                    ),
                    MaxDrawDistance = 50.0f,
                    EnableOcclusionCulling = true,
                    RenderObject = renderObj
                };
                
                renderObjects.Add(renderObj);
                cullableObjects.Add(cullableObj);
                cullingSystem.RegisterObject(cullableObj);
            }
        }
        
        protected override void OnUpdate(float deltaTime)
        {
            currentTime += deltaTime;
            
            // カメラ更新
            UpdateCamera(deltaTime);
            
            // ユニフォーム更新
            UpdateUniforms();
        }
        
        private void UpdateCamera(float deltaTime)
        {
            // カメラ回転
            var radius = 15.0f;
            cameraPosition = new Vector3(
                MathF.Cos(currentTime * 0.5f) * radius,
                5.0f + MathF.Sin(currentTime * 0.3f) * 2.0f,
                MathF.Sin(currentTime * 0.5f) * radius
            );
            
            camera.Position = cameraPosition;
            camera.UpdateMatrices();
        }
        
        private void UpdateUniforms()
        {
            // ライト回転
            lightDirection = Vector3.Normalize(new Vector3(
                MathF.Cos(currentTime * 0.3f),
                -0.7f,
                MathF.Sin(currentTime * 0.3f)
            ));
            
            var viewUniforms = CreateViewUniforms();
            var lightUniforms = CreateLightUniforms();
            
            Device.UpdateBuffer(viewUniformBuffer, new[] { viewUniforms });
            Device.UpdateBuffer(lightUniformBuffer, new[] { lightUniforms });
        }
        
        protected override void OnRender(PwCommandEncoder encoder)
        {
            // 縦持ち用ビューポート設定
            encoder.SetViewport(0, 0, 1080, 1920);
            
            // カリング実行
            var visibleObjects = cullingSystem.CullObjects(camera);
            
            // シャドウマップ生成
            RenderShadowMap(encoder, visibleObjects);
            
            // メイン描画
            RenderMainPass(encoder, visibleObjects);
        }
        
        private void RenderShadowMap(PwCommandEncoder encoder, IReadOnlyList<PwCullableObject> visibleObjects)
        {
            encoder.BeginRenderPass(shadowMap);
            encoder.SetPipeline(shadowPipeline);
            encoder.SetUniformBuffer(lightUniformBuffer, 0);
            
            // 可視オブジェクトのみ描画
            foreach (var cullableObj in visibleObjects)
            {
                var renderObj = cullableObj.RenderObject;
                encoder.SetVertexBuffer(renderObj.VertexBuffer);
                encoder.SetIndexBuffer(renderObj.IndexBuffer);
                encoder.SetInstanceBufferOffset(renderObj.InstanceBuffer, renderObj.InstanceOffset);
                encoder.DrawIndexedInstanced(renderObj.IndexCount, renderObj.InstanceCount, renderObj.FirstIndex);
            }
            
            encoder.EndRenderPass();
        }
        
        private void RenderMainPass(PwCommandEncoder encoder, IReadOnlyList<PwCullableObject> visibleObjects)
        {
            encoder.BeginMainRenderPass();
            encoder.SetPipeline(mainPipeline);
            encoder.SetUniformBuffer(viewUniformBuffer, 0);
            encoder.SetUniformBuffer(lightUniformBuffer, 1);
            encoder.SetTexture(shadowMap, 0);
            
            // 可視オブジェクトのみ描画
            foreach (var cullableObj in visibleObjects)
            {
                var renderObj = cullableObj.RenderObject;
                encoder.SetVertexBuffer(renderObj.VertexBuffer);
                encoder.SetIndexBuffer(renderObj.IndexBuffer);
                encoder.SetInstanceBufferOffset(renderObj.InstanceBuffer, renderObj.InstanceOffset);
                encoder.DrawIndexedInstanced(renderObj.IndexCount, renderObj.InstanceCount, renderObj.FirstIndex);
            }
            
            encoder.EndRenderPass();
        }
        
        private InstanceData[] GenerateInstanceData(int count)
        {
            var instances = new InstanceData[count];
            var random = new Random(42); // 固定シードで再現性を保つ
            
            for (int i = 0; i < count; i++)
            {
                // 縦持ち画面に適した配置
                var x = (random.NextSingle() - 0.5f) * 10.0f;
                var y = (random.NextSingle() - 0.5f) * 15.0f; // 縦長なので縦方向を広く
                var z = (random.NextSingle() - 0.5f) * 10.0f;
                
                var translation = Matrix4x4.CreateTranslation(x, y, z);
                var rotation = Matrix4x4.CreateRotationY(random.NextSingle() * MathF.PI * 2);
                var scale = random.NextSingle() * 1.5f + 0.5f;
                var scaleMatrix = Matrix4x4.CreateScale(scale);
                
                instances[i] = new InstanceData(
                    scaleMatrix * rotation * translation,
                    new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), 1.0f),
                    scale
                );
            }
            
            return instances;
        }
        
        private Vector3 GetInstancePosition(int index)
        {
            var random = new Random(42 + index);
            return new Vector3(
                (random.NextSingle() - 0.5f) * 10.0f,
                (random.NextSingle() - 0.5f) * 15.0f,
                (random.NextSingle() - 0.5f) * 10.0f
            );
        }
        
        private ViewUniforms CreateViewUniforms()
        {
            return new ViewUniforms(
                camera.ViewMatrix,
                camera.ProjectionMatrix,
                cameraPosition,
                currentTime
            );
        }
        
        private LightUniforms CreateLightUniforms()
        {
            var lightPosition = lightDirection * -20.0f;
            var lightViewMatrix = Matrix4x4.CreateLookAt(lightPosition, Vector3.Zero, Vector3.UnitY);
            var lightProjectionMatrix = Matrix4x4.CreateOrthographic(30.0f, 30.0f, 0.1f, 50.0f);
            
            return new LightUniforms(
                lightDirection,
                1.0f,
                new Vector3(1.0f, 0.9f, 0.8f),
                0.005f,
                lightViewMatrix,
                lightProjectionMatrix
            );
        }
    }
}
