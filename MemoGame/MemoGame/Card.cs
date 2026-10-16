namespace MemoGame;

public class Card
{
    // Väljad
    private int _id;
    private string _symbol;
    private bool _isFaceUp;
    private bool _isMatched;
    private Button _button;

    // Omadused
    public int Id
    {
        get => _id;
    }

    // Kaardi pilt (emoji), mille järgi paare otsitakse
    public string Symbol
    {
        get => _symbol;
    }

    public bool IsFaceUp
    {
        get => _isFaceUp;
        private set => _isFaceUp = value;
    }

    public bool IsMatched
    {
        get => _isMatched;
        private set => _isMatched = value;
    }

    // Nupp, mis kaarti ekraanil kujutab (määratakse MainPage'is)
    public Button Button
    {
        get => _button;
        set => _button = value;
    }

    // Konstruktor
    public Card(int id, string symbol)
    {
        _id = id;
        _symbol = symbol;
        _isFaceUp = false;
        _isMatched = false;
    }

    // Pöörab kaardi esikülje ülespoole (animatsiooniga)
    public async Task FlipUp()
    {
        _isFaceUp = true;
        await _button.RotateYTo(90, 150);   // pöörab küljeli
        _button.Text = _symbol;             // vahetab pildi
        await _button.RotateYTo(0, 150);    // pöörab tagasi
    }

    // Pöörab kaardi tagasi alla (animatsiooniga)
    public async Task FlipDown()
    {
        _isFaceUp = false;
        await _button.RotateYTo(90, 150);
        _button.Text = "?";
        await _button.RotateYTo(0, 150);
    }

    // Märgib kaardi leituks (kustub poolläbipaistvaks ja ei reageeri enam)
    public async Task SetMatched()
    {
        _isMatched = true;
        _button.IsEnabled = false;
        await _button.FadeTo(0.3, 300);
    }

    // Lähtestab kaardi uue mängu jaoks (ilma animatsioonita)
    public void Reset()
    {
        _isFaceUp = false;
        _isMatched = false;
        _button.Text = "?";
        _button.Opacity = 1;
        _button.RotationY = 0;
        _button.IsEnabled = true;
    }
}