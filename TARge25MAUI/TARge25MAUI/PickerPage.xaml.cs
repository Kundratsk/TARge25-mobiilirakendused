using Microsoft.Maui.Layouts;

namespace TARge25MAUI;

public partial class PickerPage : ContentPage
{
	Picker picker;
	public PickerPage()
	{
		picker = new Picker
		{
			Title= "Vali oma lemmikvärv",
			ItemsSource = new List <string> {"Punane", "Roheline", "Sinine" },
			HorizontalOptions = LayoutOptions.Center
		};
		picker.SelectedIndexChanged += (s, e) =>
		{
			if (picker.SelectedIndex != -1)
			{
				switch (picker.SelectedIndex)
				{
					case 0:
						BackgroundColor = Colors.Red;
						break;
					case 1:
						BackgroundColor = Colors.Green;
						break;
					case 2:
						BackgroundColor = Colors.Blue;
						break;
					default:
						break;
				}

			}	
			else
			{
				DisplayAlertAsync("Viga", "Palun vali värv", "Ok");
			}
		};
		AbsoluteLayout absLayout = new AbsoluteLayout();
		List<View> views = new List<View> { picker };
		for (int i = 0; i < views.Count; i++)
		{
			absLayout.Children.Add(views[i]);
			double yKoht = 0.2 + i * 0.2;
			AbsoluteLayout.SetLayoutBounds(views[i], new Rect(0.5, yKoht, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
			AbsoluteLayout.SetLayoutFlags(views[i], AbsoluteLayoutFlags.PositionProportional);
		}
		Content = absLayout;
	}
}