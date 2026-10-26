using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using TowerDefense.Models;
using TowerDefense.Themes;

namespace TowerDefense;

// Lühiajaline laskujoon torni ja vaenlase vahel
public class Shot
{
    public float X1, Y1, X2, Y2;
    public double Ttl = 0.12;
}

public class GameDrawable : IDrawable
{
    public Game Game { get; set; } = null!;
    public Theme Theme { get; set; } = null!;
    public List<Shot> Shots { get; } = new();

    private Microsoft.Maui.Graphics.IImage? _frontImage;
    private Microsoft.Maui.Graphics.IImage? _sideImage;
    private Microsoft.Maui.Graphics.IImage? _backImage;

    // Ruudustiku mõõtmed puuteasukoha arvutamiseks
    public float CellSize { get; private set; }
    public float OffsetX { get; private set; }
    public float OffsetY { get; private set; }

    public async Task LoadEnemyImagesAsync()
    {
        _frontImage = await LoadImageAsync("basic_enemy_front.png");
        _sideImage = await LoadImageAsync("basic_enemy_side.png");
        _backImage = await LoadImageAsync("basic_enemy_back.png");
        System.Diagnostics.Debug.WriteLine(
            $"Vaenlase pildid: front={_frontImage != null}, " +
            $"side={_sideImage != null}, back={_backImage != null}");
    }




    private static async Task<Microsoft.Maui.Graphics.IImage?> LoadImageAsync(
    string fileName)
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




    private Microsoft.Maui.Graphics.IImage? GetEnemyImage(Enemy enemy)
    {
        if (Game.Path == null || Game.Path.Count < 2)
            return _frontImage;

        int nextIndex = Math.Min(
            enemy.WaypointIndex + 1,
            Game.Path.Count - 1);

        Point target = Game.Path[nextIndex];

        double dx = target.X - enemy.X;
        double dy = target.Y - enemy.Y;

        if (Math.Abs(dy) > Math.Abs(dx))
        {
            return dy >= 0 ? _frontImage : _backImage;
        }

        return _sideImage;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Game == null || Theme == null)
            return;

        if (Game.Columns <= 0 || Game.Rows <= 0 ||
            dirtyRect.Width <= 0 || dirtyRect.Height <= 0)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Vigased mõõtmed: Width={dirtyRect.Width}, " +
                $"Height={dirtyRect.Height}, " +
                $"Columns={Game.Columns}, Rows={Game.Rows}");

            return;
        }

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

        // Tee
        if (Game.Path != null && Game.Path.Count > 0)
            DrawRoundedPath(canvas);

        // Tornid
        foreach (Tower tower in Game.Towers)
        {
            DrawSymbol(
                canvas,
                tower.Symbol,
                OffsetX + (tower.Column + 0.5f) * CellSize,
                OffsetY + (tower.Row + 0.5f) * CellSize,
                CellSize * 0.55f);
        }

        // Laskujooned
        canvas.StrokeColor = Colors.Yellow;
        canvas.StrokeSize = 3;

        foreach (Shot shot in Shots)
        {
            canvas.DrawLine(
                OffsetX + shot.X1 * CellSize,
                OffsetY + shot.Y1 * CellSize,
                OffsetX + shot.X2 * CellSize,
                OffsetY + shot.Y2 * CellSize);
        }

        // Vaenlased ja nende eluribad
        foreach (Enemy enemy in Game.Enemies)
        {
            float cx = OffsetX + ((float)enemy.X + 0.5f) * CellSize;
            float cy = OffsetY + ((float)enemy.Y + 0.5f) * CellSize;

            Microsoft.Maui.Graphics.IImage? image =
                GetEnemyImage(enemy);

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

            // Eluriba
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
        {
            System.Diagnostics.Debug.WriteLine(
                $"DrawSymbol jäeti vahele: symbol={symbol}, " +
                $"cx={cx}, cy={cy}, fontSize={fontSize}, " +
                $"CellSize={CellSize}");

            return;
        }

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
