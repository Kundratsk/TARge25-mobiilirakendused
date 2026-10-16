using System.Diagnostics;

namespace MemoGame;

public class Game
{
    // Väljad
    private Player _player;
    private List<Card> _cards;
    private Card? _firstCard;      // esimene avatud kaart (ootab paarilist)
    private bool _isBusy;          // true, kui animatsioon käib (siis klõpse ei võeta vastu)
    private Stopwatch _stopwatch;
    private int _pairCount;

    private readonly string[] _symbols =
    {
        "🐶", "🐱", "🦊", "🐸", "🐼", "🦁", "🐵", "🐰"
    };

    // Omadused
    public Player Player => _player;
    public List<Card> Cards => _cards;
    public TimeSpan Elapsed => _stopwatch.Elapsed;
    public bool IsFinished => _player.MatchedPairs == _pairCount;

    // Sündmused, millele MainPage tellib
    public event Action? StatsChanged;   // käikude/punktide muutus
    public event Action? GameFinished;   // kõik paarid leitud

    // Konstruktor
    public Game(Player player)
    {
        _player = player;
        _cards = new List<Card>();
        _stopwatch = new Stopwatch();
        _pairCount = _symbols.Length;
    }

    // Käivitab uue mängu: loob kaardid, segab need ja nullib statistika
    public void Start()
    {
        _player.Reset();
        _stopwatch.Reset();
        _firstCard = null;
        _isBusy = false;

        _cards.Clear();
        int id = 0;
        foreach (string symbol in _symbols)
        {
            // iga sümbol kaks korda = paar
            _cards.Add(new Card(id++, symbol));
            _cards.Add(new Card(id++, symbol));
        }

        // Segamine
        _cards = _cards.OrderBy(c => Random.Shared.Next()).ToList();

        StatsChanged?.Invoke();
    }

    // Kutsutakse välja, kui kasutaja kaarti puudutab
    public async Task SelectCard(Card card)
    {
        // Ignoreeri klõpsu, kui animatsioon käib või kaart on juba avatud/leitud
        if (_isBusy || card.IsFaceUp || card.IsMatched)
            return;

        _isBusy = true;

        // Aeg hakkab jooksma esimesest klõpsust
        if (!_stopwatch.IsRunning)
            _stopwatch.Start();

        // Esimene kaart paaris
        if (_firstCard == null)
        {
            await card.FlipUp();
            _firstCard = card;
            _isBusy = false;
            return;
        }

        // Teine kaart paaris
        await card.FlipUp();
        _player.AddMove();

        if (_firstCard.Symbol == card.Symbol)
        {
            // Paar leitud
            await Task.WhenAll(_firstCard.SetMatched(), card.SetMatched());
            _player.AddMatch();
        }
        else
        {
            // Ei klapi: näita hetk ja keera mõlemad tagasi
            await Task.Delay(500);
            await Task.WhenAll(_firstCard.FlipDown(), card.FlipDown());
        }

        _firstCard = null;
        _isBusy = false;
        StatsChanged?.Invoke();

        if (IsFinished)
        {
            _stopwatch.Stop();
            GameFinished?.Invoke();
        }
    }
}