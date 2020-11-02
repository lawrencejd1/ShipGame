using System;

namespace ConsoleGame
{
    // Defines delegate for guidance systems
    public delegate VelocityStruct GuidanceSystem(UFOClass host, UFOClass victim); // delegate guidance system

    // Class for any kind of UFO
    public class UFOClass
    {
        static public Random randomGenerator = new Random(); // random number generator

        // 2D locations and velocity
        public PointStruct Location = new PointStruct(0, 0);
        public PointStruct LastLocation = new PointStruct(0, 0);
        public VelocityStruct velocity = new VelocityStruct(0, 0); // only used bu SHip\\\hip

        // setting the piece/symbol and its erasing
        private string ufoSymbol;
        public string symbol
        {
            get { return ufoSymbol; }
            set
            {
                ufoSymbol = value;
                erasure = "".PadRight(value.Length);
            }
        }

        // Symbol data
        public ConsoleColor symbolColor;
        public int pieceWidth { get; private set; }
        public string erasure; // matches length of symbol

        // Characteristics
        public bool aggressive { get; set; }
        public bool IsActive { get; set; }
        public Guid ID { get; private set; }
        public ActivityEnum Use { get; set; }

        // Guidance system
        public GuidanceSystem guidanceSystem;

        // constructor
        public UFOClass(int x, int y, string s, ConsoleColor color = ConsoleColor.White)
        {
            // setting the (x,y) positions
            Location.Set(x, y);
            LastLocation.Set(x, y);
            velocity.SetoZeroes();

            // Symbol for UFO
            symbol = s;
            pieceWidth = s.Length;
            symbolColor = color;

            // Characteristics
            aggressive = false;
            IsActive = true;
            Use = ActivityEnum.None;
            ID = Guid.NewGuid();
        }

        // Note if position has changed
        public bool Moved()
        {
            return (LastLocation.x != Location.x || LastLocation.y != Location.y);
        }

        // Move to a point
        public void MoveToPoint(int _x, int _y)
        {
            LastLocation = Location;
            Location.Set(_x, _y);
        }
        public void MoveToPoint(PointStruct position)
        {
            LastLocation = Location;
            Location.Set(position.x, position.y);
        }

        // Move using a velocity 
        public void MoveWithVelocity(VelocityStruct velocity)
        {
            LastLocation = Location;
            Location.Add(velocity);
        }

        //move user up
        public void Up()
        {
            velocity.y = -1;
        }

        //move user down 
        public void Down()
        {
            velocity.y = 1;
        }

        //move user left
        public void Left()
        {
            velocity.x = -1;
        }

        //move user right 
        public void Right()
        {
            velocity.x = 1;
        }

        //See if UFOs collide
        public bool CheckForCollision(UFOClass attacker)
        {
            // Can't collide with yourself
            if (attacker.ID != ID)
            {
                // Symbols must be on  the same y-axis
                if (Location.y == attacker.Location.y)
                {
                    // Check UFO's symbol against the attacker symbol
                    for (int j = 0; j < pieceWidth; j++)
                    {
                        int x = Location.x + j;
                        if (attacker.Location.x == x)
                        {
                            return true;
                        }
                    }

                    // Check attacker symbol against the UFO symbol
                    for (int j = 0; j < attacker.pieceWidth; j++)
                    {
                        int x = attacker.Location.x + j;
                        if (Location.x == x)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //See if UFOs collide using just the location point
        public bool CheckForCollision(PointStruct location)
        {
            // Symbols must be on  the same y-axis
            if (Location.y == location.y)
            {
                // Check UFO's symbol 
                for (int j = 0; j < pieceWidth; j++)
                {
                    int x = Location.x + j;
                    if (location.x == x)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Factory system to make guidance systems
        public static GuidanceSystem GuidanceSystemFactory(ActivityEnum choice)
        {
            // Determine the choice
            ActivityEnum selection = choice;

            // For random, choose it now
            if (selection == ActivityEnum.Random)
            {
                int number = randomGenerator.Next(0, 6);
                if (number < 4) selection = ActivityEnum.Aggressive;
                else selection = ActivityEnum.Smart;
            }

            // Return one of the guidance systems
            switch (selection)
            {
                case ActivityEnum.Wandering:
                case ActivityEnum.Missile:
                    return WanderingGuidanceSystem;

                case ActivityEnum.Smart:
                    return SmartGuidanceSystem;

                case ActivityEnum.Aggressive:
                    return AggresiveGuidanceSystem;

                case ActivityEnum.Aimed:
                case ActivityEnum.Laser:
                    return DirectionalGuidanceSystem;

                case ActivityEnum.Ship:
                    return NoGuidanceSystem;

                default: // no movement 
                    break;
            }
            return NoGuidanceSystem;

            // /////////////////////////////////////////////////////////////////////
            // NoGuidanceSystem returns the UFOs own velocity (which is set each time)
            VelocityStruct NoGuidanceSystem(UFOClass ufo, UFOClass __)
            {
                VelocityStruct velocity = ufo.velocity;
                ufo.velocity.SetoZeroes();
                return velocity;
            }

            // Directional guidance keep the ufo's velocity and returns it
            VelocityStruct DirectionalGuidanceSystem(UFOClass ufo, UFOClass __)
            {
                VelocityStruct velocity = ufo.velocity;
                return velocity;
            }

            // SmartGuidanceSystem dynamically returns a straight-line velocity aimed at the victim 
            VelocityStruct SmartGuidanceSystem(UFOClass host, UFOClass _victim)
            {

                return AggresiveGuidanceSystem( host, _victim);
            }

            // the Guidance system returns a Velocity toward a victim
            VelocityStruct AggresiveGuidanceSystem(UFOClass host, UFOClass _victim)
            {
                // ufo which is the target of this guidance system
                UFOClass victim = null;
                VelocityStruct velocity = new VelocityStruct();

                // Initialize the victim just the first time
                if (victim == null)
                {
                    victim = _victim;
                }

                // No movement if the victim is no more
                if (victim != null)
                {
                    // some randomness {-1, 0, 1} for a velocity
                    int x_random = randomGenerator.Next(0, 3) - 1;
                    int y_random = randomGenerator.Next(0, 3) - 1;

                    // targets distance each turn
                    int distance = 1;

                    // get distances on the axis
                    int x_distance = victim.Location.x - host.Location.x;
                    int y_distance = victim.Location.y - host.Location.y;

                    // UFO will plot a course toward its victim just changing x and y corrdinates
                    int x_offset = x_distance < -distance ? -distance : x_distance > distance ? distance : x_distance;
                    int y_offset = y_distance < -distance ? -distance : y_distance > distance ? distance : y_distance;
                    velocity = new VelocityStruct(x_offset, y_offset);
                }
                return velocity;
            }

            // the WanderingGuidanceSystem returns a Velocity to somewhere nearby
            VelocityStruct WanderingGuidanceSystem(UFOClass _, UFOClass __)
            {
                // some randomness {-1, 0, 1} for a velocity
                int x_random = randomGenerator.Next(0, 3) - 1;
                int y_random = randomGenerator.Next(0, 3) - 1;

                return new VelocityStruct(x_random, y_random);
            }
        }
    }
}
