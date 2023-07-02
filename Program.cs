using System.Numerics;
namespace Plinko;
public static class Program
{
    private static Board gameBoard;
    private readonly static Random random = new();
    private static int score = 0;

    private struct Board
    {
        public int width;
        public int length;
        public int ballSpeed;
        public int ballAmount;
        public int ballDistance;
        public bool[,] ballPositions;
        public int[] holePoints;
        public char barSymbol;
        public char ballSymbol;

        public Board(int width, int length, decimal ballSpeed, int ballAmount, int ballDistance, int[] holePoints = null, char barSymbol = '.', char ballSymbol = '*')
        {
            this.width = width;
            this.length = length;
            this.ballSpeed = (int) (ballSpeed * 1000);
            this.ballAmount = ballAmount;
            this.ballDistance = ballDistance;
            this.holePoints = holePoints;
            this.barSymbol = barSymbol;
            this.ballSymbol = ballSymbol;
            ballPositions = new bool[width, length];

            if(this.holePoints == null)
            {
                this.holePoints = new int[width];
                for (int i = 0; i < width; i++)
                {
                    this.holePoints[i] = random.Next(101);
                }
            }
        }
    }

    public static async Task Main()
    {
        Console.WriteLine("Game Started");
        if (GetBoard() == false)
        {
            Console.WriteLine("Game ended");
            return;
        }

        await StartGame();

        Console.Clear();
        Console.WriteLine($"Final score: {score}");
        Console.WriteLine("Game ended");
        Console.ReadKey();
    }

    private static async Task StartGame()
    {
        int distanceLeft = 0;
        for (int i = gameBoard.ballAmount; i > 0;)
        {
            if(distanceLeft == 0)
            {
                distanceLeft = gameBoard.ballDistance;
                i--;
                await UpdateBoard(true);
                continue;
            }

            distanceLeft--;
            await UpdateBoard(false);            
        }

        for (int i = 0; i < gameBoard.length; i++)
        {
            await UpdateBoard(false);
        }
    }

    private static void PlaceBall() => gameBoard.ballPositions[random.Next(0, gameBoard.width -1), 0] = true;
    private static void ScoreBall(int x) => score += gameBoard.holePoints[x];

    private static async Task UpdateBoard(bool isPlacingBall)
    {
        MoveBalls();
        if(isPlacingBall)
        {
            PlaceBall();
        }
        DrawBoard();
        await Task.Delay(gameBoard.ballSpeed);
    }

    private static void MoveBalls()
    {
        for(int y = gameBoard.length - 1; y >= 0; y--)
        {
            for (var x = 0; x < gameBoard.width; x++)
            {
                if(gameBoard.ballPositions[x, y] == false)
                {
                    continue;
                }

                gameBoard.ballPositions[x, y] = false;
                bool left = random.Next(2) == 1;

                if(x == 0)
                {
                    left = false;
                }
                else if(x == gameBoard.width - 1)
                {
                    left = true;
                }

                if(y == gameBoard.length - 1)
                {
                    ScoreBall(left ? x - 1 : x + 1);
                }
                else if(left)
                {
                    gameBoard.ballPositions[x-1, y+1] = true;
                }
                else
                {
                    gameBoard.ballPositions[x+1, y+1] = true;
                }
            }
        }
    }

    private static void DrawBoard()
    {
        System.Text.StringBuilder builder = new();
        for (int y = 0; y < gameBoard.length; y++)
        {
            if(y % 2 == 0)
            {
                builder.Append(' ');
            }

            for (int x = 0; x < gameBoard.width; x++)
            {
                if(gameBoard.ballPositions[x, y])
                {
                    builder.Append(gameBoard.ballSymbol);
                }
                else
                {
                    builder.Append(' ');
                }
                
                if(x == gameBoard.width - 1)
                {
                    break;
                }

                builder.Append(gameBoard.barSymbol);
            }
            builder.AppendLine();
        }
        builder.Append($"Current score: {score}");
        Console.SetCursorPosition(0, 0);
        Console.WriteLine(builder.ToString());
    }

    private static bool GetNumber<T>(T min, T max, string message, out T result) where T : struct, IComparable<T>, IConvertible, IEquatable<T>, INumber<T>
    {
        while(true)
        {
            Console.WriteLine(message);
            string x = Console.ReadLine();

            if(x == "exit")
            {
                result = default;
                return false;
            }
            
            try
            {
                T parsedValue = (T)Convert.ChangeType(x, typeof(T));

                if (Comparer<T>.Default.Compare(parsedValue, min) < 0 || Comparer<T>.Default.Compare(parsedValue, max) > 0)
                {
                    continue;
                }

                result = parsedValue;
                return true;
            }
            catch (Exception)
            {
                continue;
            }
        }
    }

    private static bool GetYesOrNo(string message, out bool result)
    {
        while(true)
        {
            Console.Write($"{message} y - yes, n - no, x - exit: ");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();
            result = default;

            switch (key.Key)
            {
                case ConsoleKey.X:
                {
                    return false;
                }

                case ConsoleKey.Y:
                {
                    result = true;
                    break;
                }

                case ConsoleKey.N:
                {
                    result = false;
                    break;
                }

                default:
                {
                    continue;
                }
            }

            break;
        }

        return true;
    }

    private static bool GetBoard()
    {
        if (!GetNumber(5, 100, "Specify a number from 5-100 for the board width, or type 'exit' to exit", out int _boardWidth))
        {
            return false;
        }

        if(!GetNumber(5, 70, "Specify a number from 5-70 for the board length, or type 'exit' to exit", out int _boardLength))
        {
            return false;
        }

        if(!GetNumber(0.01m, 10m, "Specify the time between ball movements, from 0,01-10, or type 'exit' to exit", out decimal _ballSpeed))
        {
            return false;
        }

        if(!GetNumber(1, 1000, "Specify amount of balls, from 1-1000, or type 'exit' to exit", out int _ballAmount))
        {
            return false;
        }

        if(!GetNumber(0, _boardLength, $"Specify the distance from each ball, from 0-{_boardLength}, or type 'exit' to exit", out int _ballDistance))
        {
            return false;
        }

        if(!GetYesOrNo("Do you want to specify point amount for each plinko hole?", out bool result))
        {
            return false;
        }
        
        int[] _pointHoles = null;

        if(result == true)
        {
            int[] pointHoles = new int[_boardWidth];
            bool cancelled = false;

            for (int i = 0; i < pointHoles.Length; i++)
            {
                if(GetNumber(
                    0, 
                    1_000_000, 
                    $"Specify points given by the hole-{i}, 0 - 1_000_000, or type 'exit' to generate random values.", 
                    out int x))
                {
                    pointHoles[i] = x;
                }
                else
                {
                    cancelled = true;
                    break;     
                }
            }

            if(!cancelled)
            {
                _pointHoles = pointHoles;
            }
        }

        if(!GetYesOrNo("Do you want to customise board appearance?", out bool _appearance))
        {
            return false;
        }

        if(_appearance)
        {   
            Console.Write($"provide a symbol for the plinko bar: ");
            char bar = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.Write($"provide a symbol for the plinko ball: ");
            char ball = Console.ReadKey().KeyChar;

            Console.Clear();
            gameBoard = new(_boardWidth, _boardLength, _ballSpeed, _ballAmount, _ballDistance, _pointHoles, bar, ball);
        }
        else
        {
            Console.Clear();
            gameBoard = new(_boardWidth, _boardLength, _ballSpeed, _ballAmount, _ballDistance, _pointHoles);
        }

        return true;
    }
}