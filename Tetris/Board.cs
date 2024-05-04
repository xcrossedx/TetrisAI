using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Permissions;

namespace Tetris
{
    public class Board
    {
        public static ConsoleColor[,] state;
        public static ConsoleColor[,] previousState;
        static bool paused = false;

        public static void Init()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.SetWindowSize(43, 26);
            Console.SetBufferSize(43, 26);
            Console.CursorVisible = false;

            for (int i = 1; i < 25; i++)
            {
                if (i < 5)
                {
                    Console.SetCursorPosition(2, i);
                    Console.Write("|");
                }
                if (i < 13)
                {
                    Console.SetCursorPosition(41, i);
                    Console.Write("|");
                }
                Console.SetCursorPosition(11, i);
                Console.Write("|");
                Console.SetCursorPosition(32, i);
                Console.Write("|");
            }

            for (int i = 3; i < 41; i++)
            {
                if (i < 11 | i > 32)
                {
                    Console.SetCursorPosition(i, 4);
                    Console.Write("_");
                }
                if (i > 32)
                {
                    Console.SetCursorPosition(i, 8);
                    Console.Write("_");
                    Console.SetCursorPosition(i, 12);
                    Console.Write("_");
                }
                if (i > 11 & i < 32)
                {
                    Console.SetCursorPosition(i, 24);
                    Console.Write("_");
                }
                Console.SetCursorPosition(i, 0);
                Console.Write("_");
            }

            state = new ConsoleColor[10, 24];
            previousState = new ConsoleColor[10, 24];
        }

        public static void Update()
        {
            bool piecePlaced = false;

            if (Program.activePiece.Move(0, -1) & Program.activePiece.secondWarning)
            {
                Program.NewActivePiece(false);
                piecePlaced = true;
            }

            List<int> lines = new List<int>();

            for (int y = 0; y < 20; y++)
            {
                bool gap = true;
                bool line = true;

                for (int x = 0; x < 10; x++)
                {
                    if (state[x, y] == ConsoleColor.Black)
                    {
                        line = false;
                    }
                    else { gap = false; };
                }

                if (line & piecePlaced)
                {
                    lines.Add(y);

                    for (int x = 0; x < 10; x++)
                    {
                        state[x, y] = ConsoleColor.Black;
                    }
                }
            }

            if (lines.Count > 0)
            {
                Program.linesMade += lines.Count();
                Program.score += (int)Math.Pow(2, lines.Count() - 1) * 100;

                for (int i = 0; i < lines.Count; i++)
                {
                    for (int y = lines[i] - i; y < 23; y++)
                    {
                        for (int x = 0; x < 10; x++)
                        {
                            state[x, y] = state[x, y + 1];
                        }
                    }

                    Render();
                    Thread.Sleep(250);
                }
            }

            if (piecePlaced)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (state[x, 20] != ConsoleColor.Black)
                    {
                        Program.isGameOver = true;
                    }
                }
            }
        }

        public static void Render(string frame = "update")
        {
            if (frame == "update")
            {
                //Clearing the pause message
                if (paused)
                {
                    Console.SetCursorPosition(2, 7);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("      ");
                    paused = false;
                }

                //This renders the score and level
                Console.SetCursorPosition(2, 10);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("Level:");
                Console.SetCursorPosition(2, 11);
                Console.Write(Program.level);
                Console.SetCursorPosition(2, 13);
                Console.Write("Score:");
                Console.SetCursorPosition(2, 14);
                Console.Write(Program.score);

                //This renders the held piece and the next pieces
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        string tile = "  ";
                        if (y == 0) { tile = "__"; }

                        if (Program.heldPiece != null)
                        {
                            if (Program.heldPiece.size == 4 | (x < 3 & y < 3))
                            {
                                Console.BackgroundColor = Program.heldPiece.blocks[x, y];
                            }
                            else { Console.BackgroundColor = ConsoleColor.Black; }

                            Console.SetCursorPosition(x * 2 + 3, 4 - y);
                            Console.Write(tile);
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            if (Program.nextPieces[i].size == 4 | (x < 3 & y < 3))
                            {
                                Console.BackgroundColor = Program.nextPieces[i].blocks[x, y];
                            }
                            else { Console.BackgroundColor = ConsoleColor.Black; }

                            Console.SetCursorPosition(x * 2 + 33, (4 * (1 + i)) - y);
                            Console.Write(tile);
                        }
                    }
                }

                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 24; y++)
                    {
                        if (state[x, y] != previousState[x, y] | y == 20)
                        {
                            previousState[x, y] = state[x, y];
                            Console.SetCursorPosition(x * 2 + 12, 24 - y);
                            Console.BackgroundColor = state[x, y];
                            if (y == 20 & Console.BackgroundColor == ConsoleColor.Black) { Console.BackgroundColor = ConsoleColor.DarkRed; }
                            if (y == 0) { Console.Write("__"); }
                            else { Console.Write("  ");}
                        }
                    }
                }
            }
            else if (frame == "paused")
            {
                Console.SetCursorPosition(2, 7);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("Paused");
                paused = true;
            }
        }
    }
}
