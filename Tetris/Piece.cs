using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Piece
    {
        public int anchorX;
        public int anchorY;

        public int size;
        public ConsoleColor[,] blocks;

        public bool placementWarning;
        public bool secondWarning;

        public Piece()
        {
            placementWarning = false;
            secondWarning = false;

            int p = Program.rng.Next(0, 7);

            anchorX = 3;
            anchorY = 20;
            ConsoleColor color;

            switch (p)
            {
                case 0:
                    size = 4;
                    color = ConsoleColor.Yellow;
                    blocks = new ConsoleColor[4, 4] { { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, };
                    blocks[1, 1] = color;
                    blocks[1, 2] = color;
                    blocks[2, 1] = color;
                    blocks[2, 2] = color;
                    break;
                case 1:
                    size = 3;
                    color = ConsoleColor.Green;
                    blocks = new ConsoleColor[3, 3] { { ConsoleColor.Black , ConsoleColor.Black , ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, };
                    blocks[0, 1] = color;
                    blocks[1, 1] = color;
                    blocks[1, 2] = color;
                    blocks[2, 2] = color;
                    break;
                case 2:
                    size = 3;
                    color = ConsoleColor.Red;
                    blocks = new ConsoleColor[3, 3] { { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, };
                    blocks[2, 1] = color;
                    blocks[1, 1] = color;
                    blocks[1, 2] = color;
                    blocks[0, 2] = color;
                    break;
                case 3:
                    size = 3;
                    color = ConsoleColor.DarkYellow;
                    blocks = new ConsoleColor[3, 3] { { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, };
                    blocks[0, 1] = color;
                    blocks[1, 1] = color;
                    blocks[2, 1] = color;
                    blocks[2, 2] = color;
                    break;
                case 4:
                    size = 3;
                    color = ConsoleColor.Blue;
                    blocks = new ConsoleColor[3, 3] { { ConsoleColor.Black , ConsoleColor.Black , ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, };
                    blocks[0, 1] = color;
                    blocks[1, 1] = color;
                    blocks[2, 1] = color;
                    blocks[0, 2] = color;
                    break;
                case 5:
                    size = 3;
                    color = ConsoleColor.DarkMagenta;
                    blocks = new ConsoleColor[3, 3] { { ConsoleColor.Black , ConsoleColor.Black , ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, };
                    blocks[0, 1] = color;
                    blocks[1, 1] = color;
                    blocks[2, 1] = color;
                    blocks[1, 2] = color;
                    break;
                case 6:
                    size = 4;
                    color = ConsoleColor.Cyan;
                    blocks = new ConsoleColor[4, 4] { { ConsoleColor.Black , ConsoleColor.Black , ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, { ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black }, };
                    blocks[1, 0] = color;
                    blocks[1, 1] = color;
                    blocks[1, 2] = color;
                    blocks[1, 3] = color;
                    break;
            }
        }

        public void Rotate()
        {
            ClearPieceFromState();

            placementWarning = false;
            secondWarning = false;

            ConsoleColor[,] newBlocks = new ConsoleColor[size, size];

            float center = size / 2;

            int xAdjust = 0;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    newBlocks[x, y] = blocks[size - 1 - y, x];
                    if (newBlocks[x, y] != ConsoleColor.Black)
                    {
                        int tempx = x + anchorX;
                        if (tempx < 0 & -tempx > xAdjust) { xAdjust = -tempx; }
                        else if (tempx > 9 & 9 - tempx < xAdjust) { xAdjust = 9 - tempx; }
                    }
                }
            }

            anchorX += xAdjust;

            bool blocked = true;

            while (blocked)
            {
                blocked = false;

                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        if (newBlocks[x, y] != ConsoleColor.Black)
                        {
                            if (y + anchorY >= 0)
                            {
                                if (blocks[x, y] == ConsoleColor.Black & Board.state[x + anchorX, y + anchorY] != ConsoleColor.Black)
                                {
                                    blocked = true;
                                    break;
                                }
                            }
                            else { blocked = true; break;}
                        }
                    }
                    if (blocked) { break; }
                }

                if (blocked)
                {
                    anchorY++;
                }
            }

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    blocks[x, y] = newBlocks[x, y];
                }
            }

            AddPieceToState();
        }

        public bool Move(int dx, int dy)
        {
            ClearPieceFromState();

            bool blocked = CheckBlock(dx, dy);

            if (!blocked)
            {
                placementWarning = false;
                secondWarning = false;

                anchorX += dx;
                anchorY += dy;
            }

            AddPieceToState();
            return blocked;
        }

        public void Drop()
        {
            ClearPieceFromState();

            while (!CheckBlock(0, -1))
            {
                anchorY--;
            }

            AddPieceToState();
        }

        bool CheckBlock(int dx, int dy)
        {
            bool blocked = false;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (blocks[x, y] != ConsoleColor.Black)
                    {
                        if (x + anchorX + dx >= 0 & x + anchorX + dx <= 9 & y + anchorY + dy >= 0)
                        {
                            if (Board.state[x + anchorX + dx, y + anchorY + dy] != ConsoleColor.Black)
                            {
                                blocked = true;
                                break;
                            }
                        }
                        else { blocked = true; break; }
                    }
                }
                if (blocked) { break; }
            }

            if (blocked & dy == -1)
            {
                if (placementWarning) { secondWarning = true; }

                placementWarning = true;
            }

            return blocked;
        }

        public void ClearPieceFromState()
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (blocks[x, y] != ConsoleColor.Black)
                    {
                        Board.state[x + anchorX, y + anchorY] = ConsoleColor.Black;
                    }
                }
            }
        }

        void AddPieceToState()
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (blocks[x, y] != ConsoleColor.Black)
                    {
                        Board.state[x + anchorX, y + anchorY] = blocks[x, y];
                    }
                }
            }   
        }
    }
}
