using IImage = Microsoft.Maui.Graphics.IImage;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using TowerDefense.Models;
using TowerDefense.Themes;

namespace TowerDefense;

public class Shot
{
    public float X1, Y1, X2, Y2;

    public float Progress { get; set; }

    public float Duration { get; set; } = 0.25f;

    public string ProjectileImage { get; set; } = "arrow_projectile.png";

    public float CurrentX =>
        X1 + (X2 - X1) * Math.Clamp(Progress, 0f, 1f);

    public float CurrentY =>
        Y1 + (Y2 - Y1) * Math.Clamp(Progress, 0f, 1f);
}

public class GameDrawable : IDrawable
{
    public Game Game { get; set; } = null!;
    public Theme Theme { get; set; } = null!;

    public List<Shot> Shots { get; } = new();

    private readonly Dictionary<string, IImage?> _images = new();

    public float CellSize { get; private set; }
    public float OffsetX { get; private set; }
    public float OffsetY { get; private set; }

    public async Task LoadEnemyImagesAsync()
    {
        string[] names =
        {
            "basic_enemy_front.png",
            "basic_enemy_side.png",
            "basic_enemy_back.png",

            "fast_enemy_front.png",
            "fast_enemy_side.png",
            "fast_enemy_back.png",

            "tank_enemy_front.png",
            "tank_enemy_side.png",
            "tank_enemy_back.png",

            "archer_tower_front.png",
            "archer_tower_side.png",
            "archer_tower_back.png",

            "cannon_tower_front.png",
            "cannon_tower_side.png",
            "cannon_tower_back.png",

            "mage_tower_front.png",
            "mage_tower_side.png",
            "mage_tower_back.png",

            "arrow_projectile.png",
            "cannonball_projectile.png",
            "fireball_projectile.png",
            "magic_bolt_projectile.png",
            "magic_orb_projectile.png",
            "magic_beam_projectile.png"
        };

        foreach (string name in names)
        {
            _images[name] = await LoadImageAsync(name);
        }

        int loaded = _images.Count(pair => pair.Value != null);

        System.Diagnostics.Debug.WriteLine(
            $"Sprite-pildid laaditud: {loaded}/{names.Length}");
    }

    private static async Task<IImage?> LoadImageAsync(string fileName)
    {
        try
        {
            using Stream stream =
                await FileSystem.OpenAppPackageFileAsync(fileName);

            return PlatformImage.FromStream(stream);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Pildi laadimine ebaõnnestus ({fileName}): {ex.Message}");

            return null;
        }
    }

    private IImage? GetImage(string name)
    {
        return _images.TryGetValue(name, out var image)
            ? image
            : null;
    }

    private IImage? GetDirectionalImage(
        string prefix,
        double dx,
        double dy)
    {
        string direction;

        if (Math.Abs(dy) > Math.Abs(dx))
            direction = dy >= 0 ? "front" : "back";
        else
            direction = "side";

        return GetImage($"{prefix}_{direction}.png")
            ?? GetImage($"{prefix}_front.png");
    }

    private IImage? GetEnemyImage(Enemy enemy)
    {
        string type = enemy.GetType().Name.ToLowerInvariant();

        string prefix =
            type.Contains("tank") ? "tank_enemy" :
            type.Contains("fast") ? "fast_enemy" :
            "basic_enemy";

        if (Game.Path == null || Game.Path.Count < 2)
            return GetImage($"{prefix}_front.png");

        int nextIndex = Math.Clamp(
            enemy.WaypointIndex + 1,
            0,
            Game.Path.Count - 1);

        Point target = Game.Path[nextIndex];

        double dx = target.X - enemy.X;
        double dy = target.Y - enemy.Y;

        return GetDirectionalImage(prefix, dx, dy);
    }

    private IImage? GetTowerImage(Tower tower)
    {
        string type = tower.GetType().Name.ToLowerInvariant();
        string symbol = tower.Symbol.ToLowerInvariant();

        string prefix;

        if (type.Contains("archer") || symbol.Contains("🏹"))
            prefix = "archer_tower";
        else if (type.Contains("cannon") || symbol.Contains("💣"))
            prefix = "cannon_tower";
        else if (type.Contains("mage") || symbol.Contains("🔮"))
            prefix = "mage_tower";
        else
            return null;

        return GetImage($"{prefix}_front.png");
    }

    public void AddShot(Tower tower, Enemy enemy)
    {
        string projectile;
        float duration;

        if (tower is ArcherTower)
        {
            projectile = "arrow_projectile.png";
            duration = 0.20f;
        }
        else if (tower is CannonTower)
        {
            projectile = "cannonball_projectile.png";
            duration = 0.38f;
        }
        else if (tower is MageTower)
        {
            projectile = "magic_orb_projectile.png";
            duration = 0.28f;
        }
        else
        {
            projectile = "arrow_projectile.png";
            duration = 0.25f;
        }

        Shots.Add(new Shot
        {
            X1 = tower.Column + 0.5f,
            Y1 = tower.Row + 0.5f,
            X2 = (float)enemy.X + 0.5f,
            Y2 = (float)enemy.Y + 0.5f,
            Progress = 0f,
            Duration = duration,
            ProjectileImage = projectile
        });
    }

    public void UpdateShots(double deltaSeconds)
    {
        for (int i = Shots.Count - 1; i >= 0; i--)
        {
            Shot shot = Shots[i];

            shot.Progress += (float)(
                deltaSeconds / Math.Max(0.01f, shot.Duration));

            if (shot.Progress >= 1f)
                Shots.RemoveAt(i);
        }
    }

    private void DrawProjectile(ICanvas canvas, Shot shot)
    {
        float x = OffsetX + shot.CurrentX * CellSize;
        float y = OffsetY + shot.CurrentY * CellSize;

        float startX = OffsetX + shot.X1 * CellSize;
        float startY = OffsetY + shot.Y1 * CellSize;

        float dx = x - startX;
        float dy = y - startY;
        float length = MathF.Sqrt(dx * dx + dy * dy);

        // Mürsu liikumise jälg
        if (length > 0.1f)
        {
            float nx = dx / length;
            float ny = dy / length;

            float trailLength = Math.Min(CellSize * 0.75f, length);

            canvas.StrokeColor = Colors.LightSkyBlue.WithAlpha(0.65f);
            canvas.StrokeSize = Math.Max(1.5f, CellSize * 0.07f);

            canvas.DrawLine(
                x - nx * trailLength,
                y - ny * trailLength,
                x,
                y);
        }

        IImage? image = GetImage(shot.ProjectileImage);

        if (image == null)
            return;

        float angle = (float)(
            Math.Atan2(dy, dx) * 180.0 / Math.PI);

        float size = CellSize * 0.42f;

        canvas.SaveState();
        canvas.Translate(x, y);
        canvas.Rotate(angle);

        canvas.DrawImage(
            image,
            -size / 2f,
            -size / 2f,
            size,
            size);

        canvas.RestoreState();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Game == null || Theme == null)
            return;

        if (Game.Columns <= 0 || Game.Rows <= 0 ||
            dirtyRect.Width <= 0 || dirtyRect.Height <= 0)
            return;

        CellSize = Math.Min(
            dirtyRect.Width / Game.Columns,
            dirtyRect.Height / Game.Rows);

        OffsetX = (dirtyRect.Width - CellSize * Game.Columns) / 2f;
        OffsetY = (dirtyRect.Height - CellSize * Game.Rows) / 2f;

        // Ruudustik
        for (int c = 0; c < Game.Columns; c++)
        {
            for (int r = 0; r < Game.Rows; r++)
            {
                float x = OffsetX + c * CellSize;
                float y = OffsetY + r * CellSize;

                canvas.FillColor = Theme.CardColor;
                canvas.FillRectangle(x, y, CellSize, CellSize);

                canvas.StrokeColor = Theme.TextColor.WithAlpha(0.15f);
                canvas.StrokeSize = 1;
                canvas.DrawRectangle(x, y, CellSize, CellSize);
            }
        }

        // Mängutee
        if (Game.Path != null && Game.Path.Count > 0)
            DrawRoundedPath(canvas);

        // Tornid
        foreach (Tower tower in Game.Towers)
        {
            float cx = OffsetX + (tower.Column + 0.5f) * CellSize;
            float cy = OffsetY + (tower.Row + 0.5f) * CellSize;

            IImage? image = GetTowerImage(tower);

            if (image != null)
            {
                float size = CellSize * 0.95f;

                canvas.DrawImage(
                    image,
                    cx - size / 2f,
                    cy - size / 2f,
                    size,
                    size);
            }
            else
            {
                DrawSymbol(
                    canvas,
                    tower.Symbol,
                    cx,
                    cy,
                    CellSize * 0.55f);
            }
        }

        // Liikuvad mürsud
        foreach (Shot shot in Shots)
            DrawProjectile(canvas, shot);

        // Vaenlased ja eluribad
        foreach (Enemy enemy in Game.Enemies)
        {
            float cx = OffsetX + ((float)enemy.X + 0.5f) * CellSize;
            float cy = OffsetY + ((float)enemy.Y + 0.5f) * CellSize;

            IImage? image = GetEnemyImage(enemy);

            if (image != null)
            {
                float size = CellSize * 0.8f;

                canvas.DrawImage(
                    image,
                    cx - size / 2f,
                    cy - size / 2f,
                    size,
                    size);
            }
            else
            {
                DrawSymbol(
                    canvas,
                    enemy.Symbol,
                    cx,
                    cy,
                    CellSize * 0.55f);
            }

            float barWidth = CellSize * 0.6f;
            float barX = cx - barWidth / 2f;
            float barY = cy - CellSize * 0.42f;

            float ratio = enemy.MaxHealth > 0
                ? (float)enemy.Health / enemy.MaxHealth
                : 0f;

            ratio = Math.Clamp(ratio, 0f, 1f);

            canvas.FillColor = Colors.DarkRed;
            canvas.FillRectangle(barX, barY, barWidth, 4);

            canvas.FillColor = Colors.LimeGreen;
            canvas.FillRectangle(barX, barY, barWidth * ratio, 4);
        }
    }

    private void DrawSymbol(
        ICanvas canvas,
        string symbol,
        float cx,
        float cy,
        float fontSize)
    {
        if (string.IsNullOrEmpty(symbol) ||
            !float.IsFinite(cx) ||
            !float.IsFinite(cy) ||
            !float.IsFinite(fontSize) ||
            !float.IsFinite(CellSize) ||
            CellSize <= 0 ||
            fontSize <= 0)
            return;

        canvas.FontSize = fontSize;
        canvas.FontColor = Theme.TextColor;

        canvas.DrawString(
            symbol,
            cx - CellSize / 2f,
            cy - CellSize / 2f,
            CellSize,
            CellSize,
            HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }

    private void DrawRoundedPath(ICanvas canvas)
    {
        var path = new PathF();

        Point first = Game.Path[0];

        path.MoveTo(
            OffsetX + ((float)first.X + 0.5f) * CellSize,
            OffsetY + ((float)first.Y + 0.5f) * CellSize);

        for (int i = 1; i < Game.Path.Count; i++)
        {
            Point p = Game.Path[i];

            path.LineTo(
                OffsetX + ((float)p.X + 0.5f) * CellSize,
                OffsetY + ((float)p.Y + 0.5f) * CellSize);
        }

        canvas.StrokeColor = Theme.AccentColor.WithAlpha(0.7f);
        canvas.StrokeSize = CellSize * 0.65f;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;

        canvas.DrawPath(path);
    }
}