// Plywood.Graphics.Maui/PwView.cs
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Plywood.Graphics;

namespace Plywood.Graphics.Platform.Maui
{
    /// <summary>
    /// PwRenderer の描画サーフェスをホストする MAUI ビュー。
    /// </summary>
    public class PwView : View
    {
        /// <summary>
        /// PwRenderer を設定／取得します。
        /// </summary>
        public static readonly BindableProperty RendererProperty =
            BindableProperty.Create(
                nameof(Renderer),
                typeof(PwRenderer),
                typeof(PwView),
                propertyChanged: OnRendererChanged);

        public PwRenderer? Renderer
        {
            get => (PwRenderer?)GetValue(RendererProperty);
            set => SetValue(RendererProperty, value);
        }

        private static void OnRendererChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (PwView)bindable;
            view.Handler?.UpdateValue(nameof(Renderer));
        }
    }
}