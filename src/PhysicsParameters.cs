using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// A wrapper for the values which can be used to customize the default physics behaviours.
    /// </summary>
    public class PhysicsParameters
    {
        /// <summary>
        /// Creates new Physics Parameters (Gravity, wind speed)
        /// </summary>
        /// <param name="gravity">A Vector in the direction of gravity, magnitude is strength of gravitational force.</param>
        /// <param name="windSpeed"></param>
        public PhysicsParameters(Vector3 gravity, Vector3 windSpeed)
        {
            this.gravity = gravity;
            this.windSpeed = windSpeed;
        }

        /// <summary>
        /// Default constructor for Physics Parameters (Zero gravity and wind)
        /// </summary>
        public PhysicsParameters() : this(Vector3.Zero, Vector3.Zero) { }

        /// <summary>
        /// The current wind speed in units distance/time.
        /// </summary>
        private Vector3 windSpeed;
        
        /// <summary>
        /// The current acceleration due to gravity.
        /// </summary>
        private Vector3 gravity;
        
        /// <summary>
        /// Gets or sets the current WindSpeed in units distance/time.
        /// </summary>
        public Vector3 WindSpeed
        {
            get
            {
                return this.windSpeed;
            }
            
            set
            {
                this.windSpeed = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the current Gravity in units distance/time^2.
        /// </summary>
        public Vector3 Gravity
        {
            get
            {
                return this.gravity;
            }
            
            set
            {
                this.gravity = value;
            }
        }
    }
}
