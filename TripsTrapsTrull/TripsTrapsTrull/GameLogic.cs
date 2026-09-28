using System;
using System.Collections.Generic;
using System.Text;

namespace TripsTrapsTrull
{
    public class GameLogic
    {
        // 1. UUS: Hoiab praeguse mängulaua suurust (vaikimisi 3)
        public int Size { get; private set; } = 3;

        public string[,] Board { get; private set; } = new string[3, 3];
        public string CurrentPlayer { get; private set; } = "X";
        public int WinsX { get; set; } = 0;
        public int WinsO { get; set; } = 0;
        public int Draws { get; set; } = 0;

        // 2. UUS: Meetod laua suuruse muutmiseks MainPage poolt
        public void UpdateGridSize(int newSize)
        {
            Size = newSize;
            ResetGame();
        }

        public void ResetGame()
        {
            // 3. MUUDETUD: Loob uue massiivi dünaamilise suurusega
            Board = new string[Size, Size];
        }

        public void SetStartingPlayer(string player)
        {
            CurrentPlayer = player;
        }

        public void TogglePlayer()
        {
            CurrentPlayer = (CurrentPlayer == "X") ? "O" : "X";
        }

        public bool MakeMove(int row, int col)
        {
            if (string.IsNullOrEmpty(Board[row, col]))
            {
                Board[row, col] = CurrentPlayer;
                return true;
            }
            return false;
        }

        public string CheckWinner()
        {
            // 4. MUUDETUD: Dünaamiline ridade kontroll
            for (int r = 0; r < Size; r++)
            {
                if (!string.IsNullOrEmpty(Board[r, 0]))
                {
                    bool match = true;
                    for (int c = 1; c < Size; c++)
                    {
                        if (Board[r, c] != Board[r, 0])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match) return Board[r, 0];
                }
            }

            // 5. MUUDETUD: Dünaamiline veergude kontroll
            for (int c = 0; c < Size; c++)
            {
                if (!string.IsNullOrEmpty(Board[0, c]))
                {
                    bool match = true;
                    for (int r = 1; r < Size; r++)
                    {
                        if (Board[r, c] != Board[0, c])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match) return Board[0, c];
                }
            }

            // 6. MUUDETUD: Dünaamiline peadiagonaali kontroll (ülalt vasakult alla paremale)
            if (!string.IsNullOrEmpty(Board[0, 0]))
            {
                bool match = true;
                for (int i = 1; i < Size; i++)
                {
                    if (Board[i, i] != Board[0, 0])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return Board[0, 0];
            }

            // 7. MUUDETUD: Dünaamiline kõrvaldiagonaali kontroll (ülalt paremalt alla vasakule)
            if (!string.IsNullOrEmpty(Board[0, Size - 1]))
            {
                bool match = true;
                for (int i = 1; i < Size; i++)
                {
                    if (Board[i, Size - 1 - i] != Board[0, Size - 1])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return Board[0, Size - 1];
            }

            // Kontrolli viiki
            bool isFull = true;
            foreach (var cell in Board)
            {
                if (string.IsNullOrEmpty(cell)) isFull = false;
            }

            if (isFull) return "Viik";

            return null; // Mäng jätkub
        }
    }
}
