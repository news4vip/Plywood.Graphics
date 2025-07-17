namespace Plywood.Graphics.Sample;

public partial class MainPage : ContentPage
{
	private PwRenderer renderer;
	
	public MainPage()
	{
		InitializeComponent();

		renderer = new MobileRenderer();
		rendererView.Renderer = renderer;
	}

}

