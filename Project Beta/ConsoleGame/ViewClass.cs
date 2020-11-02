using System;
using System.Threading;

namespace ConsoleGame
{
    public class ViewClass
    {
        // Set up to leave user input periodically
        public ConsoleKeyInfo GetAnyUserInput()
        {
            ConsoleKeyInfo response = new ConsoleKeyInfo(); // our return value

            // Maximum wait is 500 milliseconds
            DateTime second = DateTime.Now.AddMilliseconds(500);
            while (second > DateTime.Now)
            {
                // Grab any available input or sleep
                if (Console.KeyAvailable)
                {
                    response = Console.ReadKey(true); // use (true) to not echo the input character
                    break;
                }
                else
                {
                    Thread.Sleep(20); // wait briefly then try to get user input again
                }
            }
            return response;
        }

        // Initialize window characteristics
        public void InitializeWindow()
        {
            try
            {
                Console.SetWindowSize(80, 30);
            }
            catch // MacOS does not allow setting the window size
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Black;

                int width = Console.WindowWidth;
                int height = Console.WindowHeight;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(" ");
                    }
                }

            }
            Console.CursorVisible = false;
        }

        // Convert user response into an action
        public char ConvertToAction(ConsoleKeyInfo response)
        {
            char action = response.KeyChar;

            // check for special keys to convert to a letter
            switch (response.Key)
            {
                case ConsoleKey.UpArrow:
                    action = 'w';
                    break;

                case ConsoleKey.DownArrow:
                    action = 's';
                    break;

                case ConsoleKey.LeftArrow:
                    action = 'a';
                    break;

                case ConsoleKey.RightArrow:
                    action = 'd';
                    break;

                default: // don't change the action on anything else
                    break;
            }
            return action;
        }

        //Show score
        public void showScore(int score)
        {
            Console.WriteLine($"Score {score}");
        }

        // Show game instruction
        public void showInstructions()
        {
            Console.WriteLine("Move with arrows");

        }

        // Win
        public void Win()
        {
            Console.WriteLine("You WIN!!");

        }

        //if users ship explodes game over
        public void Lose()
        {
            Console.WriteLine("You died!");

        }

        // if the user leaves early
        public void QuitEarly()
        {
            Console.WriteLine("Game quit early, bye");

        }

        // erase UFO 
        public void Erase(UFOClass ufo)
        {
            Console.SetCursorPosition(ufo.LastLocation.x, ufo.LastLocation.y);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(ufo.erasure);

        }

        // erase UFO 
        public void EraseAtLocation(UFOClass ufo, ConsoleColor color = ConsoleColor.Black)
        {
            Console.SetCursorPosition(ufo.Location.x, ufo.Location.y);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = color;
            Console.Write(ufo.erasure);

        }

        // display UFO 
        public void Display(UFOClass ufo)
        {
            Console.SetCursorPosition(ufo.Location.x, ufo.Location.y);
            Console.ForegroundColor = ufo.symbolColor;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(ufo.symbol);

        }

        // Explode UFO 
        public void Explode(UFOClass ufo)
        {
            Console.SetCursorPosition(ufo.Location.x, ufo.Location.y);
            Console.Write("BOOM");

        }
    }
}