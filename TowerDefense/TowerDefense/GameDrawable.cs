using Microsoft.Maui.Graphics;
using TowerDefense.Models;
using TowerDefense.Themes;

namespace TowerDefense;

// Lühiajaline "laskujoon" torni ja vaenlase vahel
public class Shot
{
    public float X1, Y1, X2, Y2;   // ruudustiku koordinaatides
    public double Ttl = 0.12;      // eluiga sekundites
}

public class GameDrawable : IDrawable
{
    public Game Game { get; set; } = null!;
    public Theme Theme { get; set; } = null!;
    public List<Shot> Shots { get; } = new();

    // Salvestame joonistamisel, et puute asukohta ruuduks teisendada
    public float CellSize { get; private set; }
    public float OffsetX { get; private set; }
    public float OffsetY { get; private set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Game == null || Theme == null) return;

        CellSize = Math.Min(dirtyRect.Width / Game.Columns, dirtyRect.Height / Game.Rows);
        OffsetX = (dirtyRect.Width - CellSize * Game.Columns) / 2;
        OffsetY = (dirtyRect.Height - CellSize * Game.Rows) / 2;

        // Ruudustik: rada ja ehitusalad
        for (int c = 0; c < Game.Columns; c++)
        {
            for (int r = 0; r < Game.Rows; r++)
            {
                float x = OffsetX + c * CellSize;
                float y = OffsetY + r * CellSize;

                canvas.FillColor = Game.IsPathCell(c, r)
                    ? Theme.AccentColor.WithAlpha(0.35f)
                    : Theme.CardColor;
                canvas.FillRectangle(x, y, CellSize, CellSize);

                canvas.StrokeColor = Theme.TextColor.WithAlpha(0.15f);
                canvas.StrokeSize = 1;
                canvas.DrawRectangle(x, y, CellSize, CellSize);
            }
        }

        // Tornid
        foreach (Tower tower in Game.Towers)
        {
            DrawSymbol(canvas, tower.Symbol,
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
                OffsetX + shot.X1 * CellSize, OffsetY + shot.Y1 * CellSize,
                OffsetX + shot.X2 * CellSize, OffsetY + shot.Y2 * CellSize);
        }

        // Vaenlased koos elusriba'ga
        foreach (Enemy enemy in Game.Enemies)
        {
            float cx = OffsetX + ((float)enemy.X + 0.5f) * CellSize;
            float cy = OffsetY + ((float)enemy.Y + 0.5f) * CellSize;

            DrawSymbol(canvas, enemy.Symbol, cx, cy, CellSize * 0.55f);

            float barWidth = CellSize * 0.6f;
            float barX = cx - barWidth / 2;
            float barY = cy - CellSize * 0.42f;
            float ratio = (float)enemy.Health / enemy.MaxHealth;

            canvas.FillColor = Colors.DarkRed;
            canvas.FillRectangle(barX, barY, barWidth, 4);
            canvas.FillColor = Colors.LimeGreen;
            canvas.FillRectangle(barX, barY, barWidth * ratio, 4);
        }
    }

    private void DrawSymbol(ICanvas canvas, string symbol, float cx, float cy, float fontSize)
    {
        canvas.FontSize = fontSize;
        canvas.FontColor = Theme.TextColor;
        canvas.DrawString(symbol, cx - CellSize / 2, cy - CellSize / 2, CellSize, CellSize,
            HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}