using System;
using System.Collections.Generic;
using System.Text;

namespace TripsTrapsTrull
{
    
    public class GameLogic
    {
        public string[,] Board { get; private set; } = new string[3, 3];
        public string CurrentPlayer { get; private set; } = "X";
        public int WinsX { get; set; } = 0;
        public int WinsO { get; set; } = 0;
        public int Draws { get; set; } = 0;

        public void ResetGame()
        {
            Board = new string[3, 3];
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
            // Read
            for (int i = 0; i < 3; i++)
                if (!string.IsNullOrEmpty(Board[i, 0]) && Board[i, 0] == Board[i, 1] && Board[i, 1] == Board[i, 2])
                    return Board[i, 0];

            // Veerud
            for (int i = 0; i < 3; i++)
                if (!string.IsNullOrEmpty(Board[0, i]) && Board[0, i] == Board[1, i] && Board[1, i] == Board[2, i])
                    return Board[0, i];

            // Diagonaalid
            if (!string.IsNullOrEmpty(Board[0, 0]) && Board[0, 0] == Board[1, 1] && Board[1, 1] == Board[2, 2])
                return Board[0, 0];
            if (!string.IsNullOrEmpty(Board[0, 2]) && Board[0, 2] == Board[1, 1] && Board[1, 1] == Board[2, 0])
                return Board[0, 2];

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