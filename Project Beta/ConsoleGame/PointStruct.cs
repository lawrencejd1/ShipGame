using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame
{
    // Holds a 2D point
    public struct PointStruct
    {
        static public Random randomGenerator = new Random(); // random number generator

        private int X;
        private int Y;

        // Constructor
        public PointStruct(int _x, int _y )
        {
            (int x_max, int y_max) = DisplayLimits(smallerLimit: true);
            X = _x < 0 ? 0 : _x > x_max ? x_max : _x;
            Y= _y < 0 ? 0 : _y > y_max ? y_max : _y;
        }

        // ////////////////////////////////////////
        // return point high limits based on the window
        static public (int x, int y) DisplayLimits( bool smallerLimit = false)
        {
            int x_limit = Math.Min(Console.BufferWidth, Console.WindowWidth);
            int y_limit = Math.Min(Console.WindowHeight, Console.BufferHeight);
            if ( smallerLimit )
            {
                x_limit--;
                y_limit--;
            }
            return (x_limit, y_limit);
        }

        //setting x postion uses lower case letters
        public int x
        {
            get { return X; }
            set { X = value; }
        }

        //setting y position 
        public int y
        {
            get { return Y; }
            set { Y = value; }
        }

        // Set both positions
        public void Set( int new_x, int new_y)
        {
            x = new_x;
            y = new_y;
        }

        // Set x and y to zero
        public void SetoZeroes()
        {
            X = 0;
            Y = 0;
        }

        // Add a velocity to the point
        public void Add( VelocityStruct velocity)
        {
            x = X + velocity.x;
            y = Y + velocity.y;
        }

        // Check if out of window limits
        public bool InTheWindow()
        {
            (int x_limit, int y_limit) = DisplayLimits(smallerLimit: true);
            if (x < 0) return false;
            if (y < 0) return false;
            if (x > x_limit) return false;
            if (y > y_limit) return false;
            return true;
        }

        // Keep in window limits
        public void KeepInWindow()
        {
            (int x_limit, int y_limit) = DisplayLimits(smallerLimit: true); ;
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x > x_limit) x = x_limit;
            if (y > y_limit) y = y_limit;
        }        
        
        // return a random point that is on the map
        public static PointStruct RandomPoint()
        {
            (int x_coord, int y_coord) = DisplayLimits();
            x_coord = randomGenerator.Next(0, x_coord);
            y_coord = randomGenerator.Next(0, y_coord);
            return new PointStruct(x_coord, y_coord);
        }
    }
}
