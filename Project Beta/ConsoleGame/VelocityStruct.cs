namespace ConsoleGame
{
    // Holds an (x,y) velocity
    public struct VelocityStruct
    {
        private int X;
        private int Y;

        // Constructors
        public VelocityStruct(int x_coord, int y_coord)
        {
            X = x_coord;
            Y = y_coord;
        }

        // ////////////////////////////////////////
        // Setting x velocity uses lower case letters
        public int x
        {
            get { return X; }
            set { X = value; }
        }

        // Setting y velocity 
        public int y
        {
            get { return Y; }
            set { Y = value; }
        }

        // Set both velocity
        public void Set(int new_x, int new_y)
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
    }
}
