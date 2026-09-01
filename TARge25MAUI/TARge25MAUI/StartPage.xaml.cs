namespace TARge25MAUI;

public partial class StartPage : ContentPage
{
	VerticalStackLayout vst;
	public List<ContentPage> Lehed = new List<ContentPage>() { new TextPage(), new FigurePage() };
	public List<string> Lehenimed = new List<string>() { "Testid", "Kujundus" };

	public StartPage()
	{
		vst = new VerticalStackLayout {Padding=20, Spacing=20 };
		for (int i = 0; i < Lehed.Count; i++)
		{
			Button nupp = new Button
			{
				Text = Lehenimed[i],
				FontSize = 20,
				BackgroundColor = Colors.LightBlue,
				TextColor = Colors.White,
				CornerRadius = 10,
				ZIndex = 1
			};
			nupp.Clicked += (s, a) => 
				{
					var valik = Lehed[nupp.ZIndex];
				Navigation.PushAsync(valik);
			};
			vst.Add(nupp);
		}
		Content = vst;
	}
}