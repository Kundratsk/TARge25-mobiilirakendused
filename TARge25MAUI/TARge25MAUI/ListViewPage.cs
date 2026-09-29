using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Maui.Controls; // Vajalik MAUI elementide jaoks

namespace TARge25MAUI
{
    public class Telefon
    {
        public string Nimetus { get; set; }
        public string Tootja { get; set; }
        public int Hind { get; set; }
        public string Pilt { get; set; }
    }

    // 2. Põhileht, kus on ListView (Lisatud ': ContentPage' pärilus)
    public class ListViewPage : ContentPage
    {
        ListView list;
        ObservableCollection<Telefon> Telefonid;
        Entry entryNimetus, entryTootja, entryHind;

        // Asendasime entryPilt muutujatega galerii jaoks (Samm 1)
        string valitudPildiTee = "";
        Label lblValitudPilt;

        public ListViewPage()
        {
            Title = "Telefonide Nimekiri";

            Telefonid = new ObservableCollection<Telefon>
            {
                new Telefon {Nimetus = "iPhone 13", Tootja = "Apple", Hind = 999, Pilt = "iphone13.png"},
                new Telefon {Nimetus = "Galaxy s21", Tootja = "Samsung", Hind = 799, Pilt = "galaxys21.png"},
                new Telefon {Nimetus = "Pixel 6", Tootja = "Google", Hind = 599, Pilt = "pixel16.png"}
            };

            // Tekstikastide loomine
            entryNimetus = new Entry { Placeholder = "Nimetus" };
            entryTootja = new Entry { Placeholder = "Tootja" };
            entryHind = new Entry { Placeholder = "Hind", Keyboard = Keyboard.Numeric };

            // Galerii nupu ja tagasiside sildi loomine (Samm 1)
            Button btnValiPilt = new Button { Text = "📷 Vali pilt galeriist", BackgroundColor = Colors.LightBlue, TextColor = Colors.Black };
            btnValiPilt.Clicked += BtnValiPilt_Clicked;
            lblValitudPilt = new Label { Text = "Pilti pole valitud (kasutatakse vaikimisi pilti)", FontSize = 12, TextColor = Colors.Gray };

            // Lisa ja Kustuta nupud
            Button btnLisa = new Button { Text = "Lisa seade", BackgroundColor = Colors.Green, TextColor = Colors.White };
            btnLisa.Clicked += Lisa_Clicked;

            Button btnKustuta = new Button { Text = "Kustuta valitud", BackgroundColor = Colors.Red, TextColor = Colors.White };
            btnKustuta.Clicked += Kustuta_Clicked;

            // ListView loomine ja pildi sidumine (Samm 2)
            list = new ListView
            {
                HasUnevenRows = true,
                ItemsSource = Telefonid,
                ItemTemplate = new DataTemplate(() =>
                {
                    // Luuakse Image element pildi kuvamiseks
                    Image imgPilt = new Image { HeightRequest = 50, WidthRequest = 50, Aspect = Aspect.AspectFit, Margin = new Thickness(5) };
                    imgPilt.SetBinding(Image.SourceProperty, "Pilt");

                    Label nimetus = new Label { FontSize = 20, FontAttributes = FontAttributes.Bold };
                    nimetus.SetBinding(Label.TextProperty, "Nimetus");

                    Label hind = new Label();
                    hind.SetBinding(Label.TextProperty, new Binding("Hind", stringFormat: "{0} €"));

                    var textLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Children = { nimetus, hind }
                    };

                    // Samm 2: Pilt vasakule, tekst paremale (Kõrvuti)
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Padding = new Thickness(10, 5),
                            Children = { imgPilt, textLayout }
                        }
                    };
                })
            };

            // Seome sündmuse ListView-ga
            list.ItemTapped += List_ItemTapped;

            // Kogu UI struktuuri lehele panemine
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = new Thickness(20),
                    Spacing = 10,
                    Children =
                    {
                        new Label { Text = "Uue seadme lisamine:", FontSize = 16, FontAttributes = FontAttributes.Bold },
                        entryNimetus,
                        entryTootja,
                        entryHind,
                        btnValiPilt,
                        lblValitudPilt,
                        new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 10, Children = { btnLisa, btnKustuta } },
                        new Label { Text = "Seadmete nimekiri:", FontSize = 16, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 15, 0, 0) },
                        list
                    }
                }
            };
        } // Konstruktori lõpp on nüüd õiges kohas!

        // Sündmuse töötleja (Parandatud DisplayAlert funktsiooni nimi)
        private async void List_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Telefon selectedPhone = e.Item as Telefon;

            if (selectedPhone != null)
            {
                await DisplayAlert("Valitud mudel", $"{selectedPhone.Tootja} - {selectedPhone.Nimetus}", "OK");
            }
        }

        // Uue telefoni lisamise loogika
        private void Lisa_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(entryNimetus.Text))
            {
                int hind = 0;
                int.TryParse(entryHind.Text, out hind);

                // Kui seadmest pilti ei valitud, kasutame vaikeväärtust
                string pildiAllikas = string.IsNullOrEmpty(valitudPildiTee) ? "dotnet_bot.png" : valitudPildiTee;

                Telefonid.Add(new Telefon
                {
                    Nimetus = entryNimetus.Text,
                    Tootja = entryTootja.Text,
                    Hind = hind,
                    Pilt = pildiAllikas
                });

                // Tühjendame tekstiväljad
                entryNimetus.Text = "";
                entryTootja.Text = "";
                entryHind.Text = "";
                valitudPildiTee = "";
                lblValitudPilt.Text = "Pilti pole valitud (kasutatakse vaikimisi pilti)";
                lblValitudPilt.TextColor = Colors.Gray;
            }
        }

        // Kustutamise loogika koos kinnituse küsimisega (Samm 1)
        private async void Kustuta_Clicked(object sender, EventArgs e)
        {
            Telefon phone = list.SelectedItem as Telefon;

            if (phone != null)
            {
                // Kasutame eelnevalt õpitud DisplayAlert struktuuri ("Jah"/"Ei")
                bool vastus = await DisplayAlert("Kustutamine", $"Kas soovid kustutada seadme {phone.Nimetus}?", "Jah", "Ei");

                if (vastus)
                {
                    Telefonid.Remove(phone);
                    list.SelectedItem = null;
                }
            }
        }

        // Pildi valimine telefonist MediaPicker abil (Samm 3)
        private async void BtnValiPilt_Clicked(object sender, EventArgs e)
        {
            try
            {
                FileResult pilt = await MediaPicker.Default.PickPhotoAsync();

                if (pilt != null)
                {
                    valitudPildiTee = pilt.FullPath;
                    lblValitudPilt.Text = $"Valitud pilt: {pilt.FileName}";
                    lblValitudPilt.TextColor = Colors.Green;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Viga", $"Pildi avamisel tekkis viga: {ex.Message}", "OK");
            }
        }
    }
}
