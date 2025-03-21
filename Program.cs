using System.Collections.Generic;
using System;
using System.CodeDom.Compiler;

class Program
{
    class Board
    {
        Cell cell;
        public int Height;
        public int Width;

        public Board(int height, int width, Cell cell)
        {
            Height = height; Width = width; this.cell = cell;
        }

        internal void Draw()
        {
            Console.Clear();

            Console.Write("\n       ");
            for (int i = 1; i < Width + 1; i++)
            {
                if(i < 9)Console.Write(i + "  ");
                else Console.Write(i + " ");
            }

            Console.WriteLine("\n");
            for (int i = 1; i < Height + 1; i++)
            {
                if (i > 9) Console.Write("   " + i + " ");
                else Console.Write("   " + i + "  ");

                for (int j = 1; j < Width + 1; j++)
                {   
                    cell = cell.getCell(i, j);
                    if (cell.closed)
                    {
                        Console.Write("[] ");
                    }
                    else
                    {
                        switch (cell.state)
                        {
                            case Cell.State.bomb:
                                Console.Write("X");
                                break;
                            case Cell.State.num:
                                Console.Write("0");
                                break;
                            default:
                                continue;
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        internal void OpenCell(int x, int y)
        {
            foreach (Cell cell in Cell.cells)
            {
                if (cell.x == x && cell.y == y)
                {
                    cell.closed = false;
                }
            }
        }

        internal void OpenAllCells()
        {
            foreach (Cell cell in Cell.cells)
            {
                cell.closed = false;
            }
        }

    }

    class Cell
    {
        Board board;
        internal enum State
        {
            bomb,
            num
        }

        public bool closed = true;

        public static List<Cell> cells = new List<Cell>();

        internal int x;
        internal int y;

        Random rnd = new Random();

        internal State state;

        public Cell(int x, int y, bool closed, State state, Board board)
        {
            this.x = x; this.y = y; this.closed = closed; this.state = state; this.board = board;
        }

        public void setBoard(Board board)
        {
            this.board = board;
        }

        internal void Create(int bombСoefficient)
        {
            for (int i = 1; i < board.Height + 1; i++)
            {
                for (int j = 1; j < board.Width + 1; j++)
                {
                    Cell cell;
                    if (rnd.Next(0, 101) >= 100 - bombСoefficient)
                        cell = new Cell(i, j, true, State.bomb, board);
                    else
                        cell = new Cell(i, j, true, State.num, board);

                    cells.Add(cell);
                }
            }
        }

        public Cell getCell(int x, int y)
        {
            foreach (Cell cell in cells)
            {
                if (cell.x == x && cell.y == y)
                    return cell;
            }
            return null;
        }

    }

    class Game
    {
        Board board;
        Cell cell;

        public Game(Board board, Cell cell)
        {
            this.board = board; this.cell = cell;
        }

        internal void Initialization(int bombСoefficient) => cell.Create(bombСoefficient); // bombСoefficient% холста - бомбы

        internal void GamePlay()
        {
            Console.ForegroundColor = ConsoleColor.White;
            board.Draw();
            Console.Write("\n enter the coordinates as X Y:");
        }

    }

    static void Main(string[] args)
    {
        Cell cell = new Cell(0, 0, true, Cell.State.num, null);
        Board board = new Board(20, 30, cell);
        cell.setBoard(board);// отложенная инициализация
        Game game = new Game(board, cell);

        game.Initialization(15);
        game.GamePlay();
    }
}
