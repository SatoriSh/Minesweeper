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

        internal void DrawAndUpdate()
        {
            Console.Clear();

            Console.Write("\n       ");
            for (int i = 1; i < Width + 1; i++)
            {
                if (i < 9) Console.Write(i + "  ");
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
                        Console.Write(cell.closedView);
                    }
                    else
                    {
                        switch (cell.state)
                        {
                            case Cell.State.empty:
                                Console.Write("   ");
                                break;
                            case Cell.State.bomb:
                                Console.Write("X  ");
                                break;
                            case Cell.State.num:
                                Console.Write($"{cell.numView}  ");
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
                    cell.closedView = string.Empty;
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

        internal void Initialization()
        {
            foreach (Cell cell in Cell.cells)
            {
                if (cell.state != Cell.State.bomb) cell.CheckTheCellNeighbors(cell);
            }
        }
    }

    class Cell
    {
        Board board;
        internal enum State
        {
            empty,
            bomb,
            num
        }

        public bool closed = true;
        public string closedView = "[] ";
        public int numView = 0;

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
                        cell = new Cell(i, j, true, State.empty, board);

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

        public void CheckTheCellNeighbors(Cell cell)
        {
            int bombCount = 0;

            // top left
            if (getCell(x, y).y - 1 <= board.Width && getCell(x, y).y - 1 > 0 && getCell(x, y).x - 1 <= board.Height && getCell(x, y).x - 1 > 0)
                if (getCell(x - 1,y - 1).state == State.bomb) bombCount++;

            // top
            if (getCell(x, y).x - 1 <= board.Height && getCell(x, y).x - 1 > 0)
                if (getCell(x - 1, y).state == State.bomb) bombCount++;

            // top right
            if (getCell(x, y).y + 1 <= board.Width && getCell(x, y).y + 1 > 0 && getCell(x, y).x - 1 <= board.Height && getCell(x, y).x - 1 > 0)
                if (getCell(x - 1, y + 1).state == State.bomb) bombCount++;

            // right
            if (getCell(x, y).y + 1 <= board.Width && getCell(x, y).y + 1 > 0)
                if (getCell(x, y + 1).state == State.bomb) bombCount++;

            // bottom left
            if (getCell(x, y).y - 1 <= board.Width && getCell(x, y).y - 1 > 0 && getCell(x, y).x + 1 <= board.Height && getCell(x, y).x + 1 > 0)
                if (getCell(x + 1, y - 1).state == State.bomb) bombCount++;

            // bottom
            if (getCell(x, y).x + 1 <= board.Height)
                if (getCell(x + 1, y).state == State.bomb) bombCount++;

            // bottom right
            if (getCell(x, y).y + 1 <= board.Width && getCell(x, y).y + 1 > 0 && getCell(x, y).x + 1 <= board.Height && getCell(x, y).x + 1 > 0)
                if (getCell(x + 1, y + 1).state == State.bomb) bombCount++;

            //left
            if (getCell(x, y).y - 1 <= board.Width && getCell(x, y).y - 1 > 0)
                if (getCell(x, y - 1).state == State.bomb) bombCount++;

            if (bombCount > 0) cell.state = State.num;
            cell.numView = bombCount;
        }

        public void AutoOpenEmptyCells(int x, int y) 
        {
            getCell(x, y).closed = false;

            // top left
            if (getCell(x, y).y - 1 <= board.Width && getCell(x, y).y - 1 > 0 && getCell(x, y).x - 1 <= board.Height && getCell(x, y).x - 1 > 0)
                if (getCell(x - 1, y - 1).state == State.num) getCell(x - 1, y - 1).closed = false;
                else if (getCell(x - 1, y - 1).state == State.empty && getCell(x - 1, y - 1).closed) AutoOpenEmptyCells(x - 1,y - 1);

            // top
            if (getCell(x, y).x - 1 <= board.Height && getCell(x, y).x - 1 > 0)
                if (getCell(x - 1, y).state == State.num) getCell(x - 1, y).closed = false;
                else if (getCell(x - 1, y).state == State.empty && getCell(x - 1, y).closed) AutoOpenEmptyCells(x - 1, y);

            // top right
            if (getCell(x, y).y + 1 <= board.Width && getCell(x, y).y + 1 > 0 && getCell(x, y).x - 1 <= board.Height && getCell(x, y).x - 1 > 0)
                if (getCell(x - 1, y + 1).state == State.num) getCell(x - 1, y + 1).closed = false;
                else if (getCell(x - 1, y + 1).state == State.empty && getCell(x - 1, y + 1).closed) AutoOpenEmptyCells(x - 1, y + 1);

            // right
            if (getCell(x, y).y + 1 <= board.Width && getCell(x, y).y + 1 > 0)
                if (getCell(x, y + 1).state == State.num) getCell(x, y + 1).closed = false;
                else if (getCell(x, y + 1).state == State.empty && getCell(x, y + 1).closed) AutoOpenEmptyCells(x, y + 1);

            // bottom left
            if (getCell(x, y).y - 1 <= board.Width && getCell(x, y).y - 1 > 0 && getCell(x, y).x + 1 <= board.Height && getCell(x, y).x + 1 > 0)
                if (getCell(x + 1, y - 1).state == State.num) getCell(x + 1, y - 1).closed = false;
                else if (getCell(x + 1, y - 1).state == State.empty && getCell(x + 1, y - 1).closed) AutoOpenEmptyCells(x + 1, y - 1);

            // bottom
            if (getCell(x, y).x + 1 <= board.Height)
                if (getCell(x + 1, y).state == State.num) getCell(x + 1, y).closed = false;
                else if (getCell(x + 1, y).state == State.empty && getCell(x + 1, y).closed) AutoOpenEmptyCells(x + 1, y);

            // bottom right
            if (getCell(x, y).y + 1 <= board.Width && getCell(x, y).y + 1 > 0 && getCell(x, y).x + 1 <= board.Height && getCell(x, y).x + 1 > 0)
                if (getCell(x + 1, y + 1).state == State.num) getCell(x + 1, y + 1).closed = false;
                else if (getCell(x + 1, y + 1).state == State.empty && getCell(x + 1, y + 1).closed) AutoOpenEmptyCells(x + 1, y + 1);

            //left
            if (getCell(x, y).y - 1 <= board.Width && getCell(x, y).y - 1 > 0)
                if (getCell(x, y - 1).state == State.num) getCell(x, y - 1).closed = false;
                else if (getCell(x, y - 1).state == State.empty && getCell(x, y - 1).closed) AutoOpenEmptyCells(x, y - 1);
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
            string coordinateXstr = string.Empty;
            string coordinateYstr = string.Empty;

            int coordinateX = int.MinValue;
            int coordinateY = int.MinValue;

            board.Initialization();

            while (true)
            {
                board.DrawAndUpdate();
                Console.Write("\n\t enter the coordinates (as X Y): ");

                if (Console.ReadLine() is string coordinates && !string.IsNullOrEmpty(coordinates))
                {
                    if (coordinates.Split(' ').Length == 2)
                    {
                        coordinateXstr = coordinates.Split(' ')[0];
                        coordinateYstr = coordinates.Split(' ')[1];
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\t you entered incorrect values.");
                        Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }
                    try
                    {
                        if (int.Parse(coordinateXstr) <= board.Height && int.Parse(coordinateYstr) <= board.Width && coordinateXstr.Length < 1000 && coordinateYstr.Length < 1000 && int.Parse(coordinateXstr) > 0 && int.Parse(coordinateYstr) > 0)
                        {
                            coordinateX = int.Parse(coordinateXstr);
                            coordinateY = int.Parse(coordinateYstr);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n\t you entered incorrect values.");
                            Console.ReadLine();
                            Console.ForegroundColor = ConsoleColor.White;
                            continue;
                        }
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\t you entered incorrect values.");
                        Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }

                    board.DrawAndUpdate();

                    board.OpenCell(coordinateX, coordinateY);
                    if (cell.getCell(coordinateX, coordinateY).state == Cell.State.bomb)
                    {
                        board.DrawAndUpdate();
                        gameover();
                    }
                    if (cell.getCell(coordinateX, coordinateY).state == Cell.State.empty)
                        cell.AutoOpenEmptyCells(coordinateX, coordinateY);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n\t you entered incorrect values.");
                    Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
            }
        }

        internal void gameover()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nYou've lost");
            Console.ForegroundColor = ConsoleColor.White;
            bool openCells = false;

            while (true)
            {
                if (!openCells) Console.WriteLine("(1) Open all cells");
                Console.WriteLine("(2) Exit");
                try
                {
                    int playerChoice = int.Parse(Console.ReadLine());

                    switch (playerChoice)
                    {
                        case 1:
                            board.OpenAllCells();
                            board.DrawAndUpdate();
                            openCells = true;
                            break;
                        case 2:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Error");
                            continue;
                    }
                }
                catch
                {
                    Console.WriteLine("Error");
                    Console.ReadLine();
                }
                Console.ReadLine();
            }
        }
    }

    static void Main(string[] args)
    {
        Cell cell = new Cell(0, 0, true, Cell.State.num, null);
        Board board = new Board(20, 30, cell);
        cell.setBoard(board);// delayed initialization
        Game game = new Game(board, cell);

        game.Initialization(10);
        game.GamePlay();
    }
}