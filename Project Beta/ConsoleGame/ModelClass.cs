using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleGame
{
    public class ModelClass
    {
        public List<UFOClass> Galaxy;  // stores list of all UFOs
        static public Random randomGenerator = new Random(); // random number generator

        // Constructor: model class
        public ModelClass()
        {
            Galaxy = new List<UFOClass>();  //new list of UFO's
        }

        // ////////////////////////////////////////

        // remove a ufo from the galaxy
        public void RemoveUFO(UFOClass victim)
        {
            var victimUse = ActivityEnum.Target;
            Galaxy.Remove(victim);

            // Upon removal of a target
            if (victimUse == ActivityEnum.Target)
            {
                // aggitate the targets (use for-loop to access actual ufos)
                for (int j = 0; j < Galaxy.Count; j++)
                {
                    var item = Galaxy[j];
                    if (item.Use == ActivityEnum.Target)
                    {
                        item.aggressive = true;
                        item.guidanceSystem = UFOClass.GuidanceSystemFactory(ActivityEnum.Random);
                    }
                }
            }
        }

        // Make the UFO that is a missile
        public UFOClass CreateMissile(UFOClass source, char direction)
        {
            // Figure where weapon starts from
            int y_offset = 0;
            int x_offset = 0;
            int shipOffset = 0;

            switch (direction)
            {
                case '1':
                    y_offset = 1;
                    x_offset = -1;
                    break;

                case '2':
                    y_offset = 1;
                    break;

                case '3':
                    y_offset = 1;
                    x_offset = 1;
                    break;

                case '4':
                    x_offset = -1;
                    break;

                case '6':
                    x_offset = 1;
                    shipOffset = source.pieceWidth + 1;
                    break;

                case '7':
                    y_offset = -1;
                    x_offset = -1;
                    break;

                case '8':
                    y_offset = -1;
                    break;

                case '9':
                    y_offset = -1;
                    x_offset = 1;
                    break;

                default:
                    break;
            }

            // Find the initial velocity and position
            VelocityStruct velocity = new VelocityStruct(x_offset, y_offset);
            int x = source.Location.x + shipOffset + x_offset;
            int y = source.Location.y + y_offset;

            // Make the attacker
            UFOClass attacker = new UFOClass(x, y, "*", ConsoleColor.Red);
            attacker.Use = ActivityEnum.Missile;
            attacker.guidanceSystem = UFOClass.GuidanceSystemFactory(ActivityEnum.Missile);
            attacker.velocity = velocity;
            return attacker;
        }

        // Add ship to game
        public UFOClass AddShip(string givenSymbol = "<0>")
        {
            PointStruct point = UniqueUFOCoordinates();
            UFOClass ship = new UFOClass(point.x, point.y, givenSymbol);
            ship.Use = ActivityEnum.Ship;
            ship.guidanceSystem = UFOClass.GuidanceSystemFactory(ActivityEnum.None);
            Galaxy.Add(ship);
            return ship;
        }

        // Add target UFO to game
        public UFOClass AddSingleRandomTarget()
        {
            PointStruct point = UniqueUFOCoordinates();
            UFOClass target = new UFOClass(point.x, point.y, "<[X]>");
            target.Use = ActivityEnum.Target;
            target.guidanceSystem = UFOClass.GuidanceSystemFactory(ActivityEnum.Wandering);
            Galaxy.Add(target);
            return target;
        }

        // Add target(s) to game
        public void AddRandomTarget(int number = 1)
        {
            int repeat = number < 0 ? 1 : number;
            for (int j = 0; j < repeat; j++)
            {
                AddSingleRandomTarget();
            }
        }

        // If UFO is hit create a new one with a better guidance system
        public void MaybeMakeReplacementTarget()
        {
            int y = randomGenerator.Next(0, 9);
            if (y % 2 == 0)
            {
                var target = AddSingleRandomTarget();
                target.guidanceSystem = UFOClass.GuidanceSystemFactory(ActivityEnum.Random);
            }
        }

        // Removed destroyed/inactive UFOs and return the number of active targets
        public int RemoveDestroyedUFOs()
        {
            int targetCount = 0;
            int limit = Galaxy.Count - 1;
            if (limit >= 0)
            {
                for (int j = limit; j >= 0; j--)
                {
                    UFOClass doomedUfo = Galaxy[j];
                    if (!doomedUfo.IsActive)
                    {
                        RemoveUFO(doomedUfo);
                    }
                    else if (doomedUfo.Use == ActivityEnum.Target)
                    {
                        targetCount++;
                    }
                }
            }
            return targetCount;
        }

        // Manage an attack, return null if the attack should continue or list of victims
        public List<UFOClass> AttackResults(UFOClass attacker)
        {
            // Check for a collision
            List<UFOClass> list = CheckGalaxyForCollision(attacker);
            if (list.Count > 0)
            {
                // Everything hit will be destroyed later, check for a target replacement
                foreach (var ufo in list)
                {
                    ufo.IsActive = false;
                    MaybeMakeReplacementTarget();
                }
            }
            // Otherwise, move the attacker toward the first available victim
            else
            {
                var velocity = attacker.guidanceSystem(attacker, FirstTargetAvailable());
                attacker.MoveWithVelocity(velocity);
                if (attacker.Location.InTheWindow())
                {
                    Thread.Sleep(25);
                    return null;
                }
            }
            return list;
        }

        // Move UFO (maybe toward its victim), check if it hits another UFO
        public List<UFOClass> MoveUFO(UFOClass ufo, UFOClass optionalVictim)
        {
            // List of hits
            List<UFOClass> list = new List<UFOClass>();

            // Move active UFOs
            if (ufo.IsActive)
            {
                // Get the velocity and move the UFO
                VelocityStruct velocity = ufo.guidanceSystem(ufo, optionalVictim);
                ufo.MoveWithVelocity(velocity);

                // Correct for if it moved out of the window
                if (!ufo.Location.InTheWindow())
                {
                    ufo.Location = ufo.LastLocation;
                }
                // Look for a collision
                else if (ufo.Moved())
                {
                    bool hit = ufo.CheckForCollision(optionalVictim);
                    if (hit)
                    {
                        // Add items to the list
                        list.Add(optionalVictim);
                        list.Add(ufo);
                    }
                }
            }
            return list;
        }

        // Return the first target available to be attacked
        public UFOClass FirstTargetAvailable()
        {
            UFOClass ship = null;
            foreach (var ufo in Galaxy)
            {
                if (ufo.Use == ActivityEnum.Target) return ufo;
                if (ufo.Use == ActivityEnum.Ship) ship = ufo;
            }
            return ship;
        }

        // See if UFOs collide.  Return all colliding UFOs.
        public List<UFOClass> CheckGalaxyForCollision(UFOClass attacker)
        {
            // list of colliders
            List<UFOClass> colliders = new List<UFOClass>();

            // Check everything 
            foreach (var target in Galaxy)
            {
                if (attacker.CheckForCollision(target))
                {
                    colliders.Add(target);
                }
            }
            return colliders;
        }

        // Return a coordinate (x,y) pair that another object does not share
        public PointStruct UniqueUFOCoordinates()
        {
            // coordinates we will return
            PointStruct point;

            // search the list if there is somethinng to search for
            bool searching = true;
            do
            {
                // get a random pair of coordinates
                point = PointStruct.RandomPoint();

                // for each ufo subject in the list
                foreach (var subject in Galaxy)
                {
                    searching = subject.CheckForCollision(point);
                    if (!searching) break;
                }
                if (Galaxy.Count == 0) searching = false;
            } while (searching);
            return point;
        }
    }
}
