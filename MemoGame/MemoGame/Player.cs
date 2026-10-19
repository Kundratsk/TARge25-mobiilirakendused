namespace MemoGame;

public class Player
{
    // Väljad
    private string _name;
    private int _moves;
    private int _matchedPairs;

    // Omadused
    public string Name
    {
        get => _name;
        set => _name = value;
    }

    // Käikude arv (iga kahe kaardi avamine = 1 käik)
    public int Moves
    {
        get => _moves;
        private set => _moves = value;
    }

    // Leitud paaride arv
    public int MatchedPairs
    {
        get => _matchedPairs;
        private set => _matchedPairs = value;
    }

    // Punktid: iga leitud paar annab 10 punkti, iga käik võtab 1 punkti
    public int Score => Math.Max(0, _matchedPairs * 10 - _moves);

    // Konstruktor
    public Player(string name)
    {
        _name = name;
        _moves = 0;
        _matchedPairs = 0;
    }

    // Meetodid
    public void AddMove()
    {
        _moves++;
    }

    public void AddMatch()
    {
        _matchedPairs++;
    }

    public void Reset()
    {
        _moves = 0;
        _matchedPairs = 0;
    }
}