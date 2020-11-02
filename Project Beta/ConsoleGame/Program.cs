using System;
using System.Collections.Generic;

namespace ConsoleGame
{
    class Program
    {
        //Score Value
        static public int score = 0;
        // Determines when to quit playing
        static public bool playing = true;

        // Instantiate the view and model
        static public ModelClass Model = new ModelClass();
        static public ViewClass View = new ViewClass();
        static public UFOClass Ship;

        static void Main(string[] args)
        {
            // initialize window
            View.InitializeWindow();

            // initial graphics
            Ship = Model.AddShip("<[0]>");
            Model.AddRandomTarget(3);
            foreach (var ufo in Model.Galaxy)
            {
                View.Display(ufo);
            }

            // Loop until done
            do
            {
                int targetCount; // counts number of targets left

                // receive and determine the action for a ship command
                ConsoleKeyInfo response = View.GetAnyUserInput();
                char action = View.ConvertToAction(response);

                // act based on the action
                switch (action)
                {
                    case 'w':
                        Ship.Up();
                        break;

                    case 's':
                        Ship.Down();
                        break;

                    case 'a':
                        Ship.Left();
                        break;

                    case 'd':
                        Ship.Right();
                        break;

                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '6':
                    case '7':
                    case '8':
                        View.showInstructions();
                        break;
                    case '9':
                        // This new bullet takes all its moves at once
                        var bullet = Model.CreateMissile(Ship, action);

                        // Keep traveling until a hit or bullet goes off the window
                        bool traveling = bullet.Location.InTheWindow();
                        while (traveling)
                        {
                            // Display bullet 
                            View.Display(bullet);

                            // Move and check for any hit
                            var results = Model.AttackResults(bullet);
                            View.Erase(bullet);

                            // Everything hit blows up
                            if (results != null)
                            {
                                traveling = false;
                                score += 1;
                                View.showScore(score);
                                foreach (var ufo in results)
                                {
                                    View.Explode(ufo);
                                    ufo.IsActive = false;
                                }

                                // Removed everything that has been destroyed
                                targetCount = Model.RemoveDestroyedUFOs();
                            }
                        }
                        break;

                    default: // nothing to do
                        break;
                }

                // Move Ship 
                var moveResult = Model.MoveUFO(Ship, Ship);
                if (Ship.Moved() && Ship.IsActive)
                {
                    View.Erase(Ship);
                    View.Display(Ship);
                }

                // Both victim and target might blow up
                foreach (var doomedUfo in moveResult)
                {
                    View.Explode(doomedUfo);
                    doomedUfo.IsActive = false;
                }

                // Move ufos, check for a collision
                foreach (var ufo in Model.Galaxy)
                {
                    // Move ufo first, then display
                    moveResult = Model.MoveUFO(ufo, Ship);
                    if (ufo.Moved() && ufo.IsActive)
                    {
                        View.Erase(ufo);
                        View.Display(ufo);
                    }

                    // Both victim and target might blow up
                    foreach (var doomedUfo in moveResult)
                    {
                        View.Explode(doomedUfo);
                        doomedUfo.IsActive = false;
                    }
                }

                // Removed everything that has been destroyed
                targetCount = Model.RemoveDestroyedUFOs();

                // Stop when the ship is the only one left or is destroyed
                playing = (targetCount > 0 && Ship.IsActive && playing);

            } while (playing);

            // Quitting, Win or Lose
            if (Ship.IsActive)
            {
                if (Ship == Model.FirstTargetAvailable())
                {
                    View.Win();
                }
                else
                {
                    View.QuitEarly();
                }
            }
            else
            {
                View.Lose();
            }
            Console.ReadLine();
        }
    }
}
