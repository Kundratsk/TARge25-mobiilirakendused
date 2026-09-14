

namespace TARge25MAUI;

public partial class TimerPage : ContentPage
{
    bool on_off = false;

    public TimerPage()
    {
        InitializeComponent();
        KäivitaTaimer();
    }

    private void KäivitaTaimer()
    {
        on_off = true;
        ShowTime();
    }

    private async void ShowTime()
    {
        while (on_off)
        {
            if (timer_btn != null)
            {
                timer_btn.Text = DateTime.Now.ToString("T");
            }
            await Task.Delay(1000);
        }
    }

    private void timer_btn_Clicked(object sender, EventArgs e)
    {
        if (on_off)
        {
            on_off = false;
            timer_btn.Text = "Näita kella";
        }
        else
        {
            KäivitaTaimer();
        }
    }

    private async void tagasi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void TapGestureRecognizer_tappede(object sender, EventArgs e)
    {
        // Tekstisildi (Label) vajutuse loogika
    }
}
