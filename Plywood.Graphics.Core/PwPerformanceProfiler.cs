// Plywood.Graphics.Core/PwPerformanceProfiler.cs
using System.Diagnostics;

namespace Plywood.Graphics
{
    public class PwPerformanceProfiler
    {
        private readonly Dictionary<string, PwProfileSection> sections;
        private readonly Stack<PwProfileSection> sectionStack;
        private readonly Stopwatch frameTimer;
        private readonly Dictionary<string, float> metrics;
        
        public PwPerformanceProfiler()
        {
            sections = new Dictionary<string, PwProfileSection>();
            sectionStack = new Stack<PwProfileSection>();
            frameTimer = new Stopwatch();
            metrics = new Dictionary<string, float>();
        }
        
        public void BeginFrame()
        {
            frameTimer.Restart();
            
            // 前フレームの結果をリセット
            foreach (var section in sections.Values)
            {
                section.Reset();
            }
        }
        
        public void EndFrame()
        {
            frameTimer.Stop();
            
            // フレーム時間を記録
            SetMetric("Frame Time (ms)", (float)frameTimer.Elapsed.TotalMilliseconds);
            SetMetric("FPS", 1000.0f / (float)frameTimer.Elapsed.TotalMilliseconds);
        }
        
        public IDisposable BeginSection(string name)
        {
            if (!sections.TryGetValue(name, out var section))
            {
                section = new PwProfileSection(name);
                sections[name] = section;
            }
            
            section.Begin();
            sectionStack.Push(section);
            
            return new PwProfileSectionScope(this);
        }
        
        internal void EndCurrentSection()
        {
            if (sectionStack.Count > 0)
            {
                var section = sectionStack.Pop();
                section.End();
            }
        }
        
        public void SetMetric(string name, float value)
        {
            metrics[name] = value;
        }
        
        public float GetMetric(string name)
        {
            return metrics.TryGetValue(name, out var value) ? value : 0.0f;
        }
        
        public PwPerformanceReport GenerateReport()
        {
            var report = new PwPerformanceReport
            {
                FrameTime = GetMetric("Frame Time (ms)"),
                FPS = GetMetric("FPS"),
                Sections = sections.Values.ToArray(),
                Metrics = new Dictionary<string, float>(metrics)
            };
            
            return report;
        }
        
        public void LogPerformance()
        {
            Console.WriteLine("=== Performance Report ===");
            Console.WriteLine($"Frame Time: {GetMetric("Frame Time (ms)"):F2} ms");
            Console.WriteLine($"FPS: {GetMetric("FPS"):F1}");
            
            foreach (var section in sections.Values)
            {
                Console.WriteLine($"{section.Name}: {section.LastTime:F2} ms ({section.CallCount} calls)");
            }
            
            foreach (var metric in metrics)
            {
                if (metric.Key != "Frame Time (ms)" && metric.Key != "FPS")
                {
                    Console.WriteLine($"{metric.Key}: {metric.Value}");
                }
            }
            Console.WriteLine("========================");
        }
    }
    
    public class PwProfileSection
    {
        private readonly Stopwatch stopwatch;
        
        public string Name { get; }
        public float LastTime { get; private set; }
        public float TotalTime { get; private set; }
        public int CallCount { get; private set; }
        public float AverageTime => CallCount > 0 ? TotalTime / CallCount : 0.0f;
        
        public PwProfileSection(string name)
        {
            Name = name;
            stopwatch = new Stopwatch();
        }
        
        public void Begin()
        {
            stopwatch.Restart();
        }
        
        public void End()
        {
            stopwatch.Stop();
            LastTime = (float)stopwatch.Elapsed.TotalMilliseconds;
            TotalTime += LastTime;
            CallCount++;
        }
        
        public void Reset()
        {
            LastTime = 0.0f;
            TotalTime = 0.0f;
            CallCount = 0;
        }
    }
    
    public class PwProfileSectionScope : IDisposable
    {
        private readonly PwPerformanceProfiler profiler;
        
        public PwProfileSectionScope(PwPerformanceProfiler profiler)
        {
            this.profiler = profiler;
        }
        
        public void Dispose()
        {
            profiler.EndCurrentSection();
        }
    }
    
    public class PwPerformanceReport
    {
        public float FrameTime { get; set; }
        public float FPS { get; set; }
        public PwProfileSection[] Sections { get; set; }
        public Dictionary<string, float> Metrics { get; set; }
    }
}
