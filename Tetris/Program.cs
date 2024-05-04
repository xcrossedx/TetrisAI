using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Program
    {
        public static bool isRunning = true;
        public static bool isGameOver = false;
        public static bool isPaused = false;

        public static int level = 0;
        public static int linesMade = 0;
        public static int score = 0;

        public static Random rng = new Random();

        public static Piece activePiece;
        public static Piece[] nextPieces = new Piece[3];
        public static Piece heldPiece;

        public static bool held = false;

        //This will be a game of Tetris
        static void Main()
        {
            Task.Run(GameLoop);

            while (isRunning)
            {
                if (Console.KeyAvailable == false)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        case ConsoleKey.Escape:
                            isPaused = !isPaused;
                            break;
                        case ConsoleKey.Spacebar:
                            if (isPaused)
                            {
                                isGameOver = true;
                            }
                            else
                            {
                                if (!held)
                                {
                                    activePiece.ClearPieceFromState();
                                    Piece tempHeldPiece = activePiece;
                                    held = true;

                                    if (heldPiece != null)
                                    {
                                        activePiece = heldPiece;
                                    }
                                    else
                                    {
                                        NewActivePiece(true);
                                    }

                                    heldPiece = tempHeldPiece;
                                    heldPiece.anchorX = 3;
                                    heldPiece.anchorY = 20;
                                }
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            activePiece.Rotate();
                            break;
                        case ConsoleKey.LeftArrow:
                            activePiece.Move(-1, 0);
                            break;
                        case ConsoleKey.RightArrow:
                            activePiece.Move(1, 0);
                            break;
                        case ConsoleKey.DownArrow:
                            activePiece.Drop();
                            break;
                    }
                }
            }
        }

        static void GameLoop()
        {
            while (isRunning)
            {
                Init();

                while (!isGameOver)
                {
                    while (!isPaused)
                    {
                        level = linesMade / 10;
                        Board.Update();
                        Board.Render();
                        Thread.Sleep((int)Math.Round(500 / (1 + (level * 0.5))));

                        if (isGameOver | !isRunning) { break; }
                    }

                    if (!isGameOver) { Board.Render("paused"); }
                    if (!isRunning) { break; }
                }

                Console.ReadKey(true);
            }
        }

        static void Init()
        {
            isRunning = true;
            isGameOver = false;
            isPaused = false;

            level = 0;
            linesMade = 0;
            score = 0;

            activePiece = new Piece();
            nextPieces = new Piece[3] { new Piece(), new Piece(), new Piece() };
            heldPiece = null;

            Board.Init();
        }

        public static void NewActivePiece(bool holding)
        {
            if (!holding) { held = false; }

            activePiece = nextPieces[0];
            nextPieces[0] = nextPieces[1];
            nextPieces[1] = nextPieces[2];
            nextPieces[2] = new Piece();
        }
    }
}
