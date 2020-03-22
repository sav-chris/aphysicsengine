using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsEngine
{
    /// <summary>
    /// A utility class which aggregates the particle parameters simplifying the creation of particles.
    /// </summary>
    public class ParticleParameters
    {
        /// <summary>
        /// The initial position of the particle.
        /// </summary>
        private Vector3 initialPosition;
        
        /// <summary>
        /// The iniitial velocity of the particle.
        /// </summary>
        private Vector3 velocity;
        
        /// <summary>
        /// The iniital colour of the particle.
        /// </summary>
        private Color colour;
        
        /// <summary>
        /// The life time of the particle.
        /// </summary>
        private float lifeTime;
        
        /// <summary>
        /// The mass of the particle.
        /// </summary>
        private float mass;
        
        /// <summary>
        /// The gravity acting on the particle.
        /// </summary>
        private Vector3 gravity;
        
        /// <summary>
        /// The strength of air resistance on the particle.
        /// </summary>
        private float airResistance;

        /// <summary>
        /// Gets or sets the initial position of the particle.
        /// </summary>
        public Vector3 InitialPosition
        {
            get
            {
                return this.initialPosition;
            }
            
            set
            {
                this.initialPosition = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the velocity of the particle.
        /// </summary>
        public Vector3 Velocity
        {
            get
            {
                return this.velocity;
            }
            
            set
            {
                this.velocity = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the particle colour.
        /// </summary>
        public Color Colour
        {
            get
            {
                return this.colour;
            }
            
            set
            {
                this.colour = value;
            }
        }

        /// <summary>
        /// Gets or sets the life time of the particle.
        /// </summary>
        public float LifeTime
        {
            get
            {
                return this.lifeTime;
            }
            
            set
            {
                this.lifeTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the particle mass.
        /// </summary>
        public float Mass
        {
            get
            {
                return this.mass;
            }
            
            set
            {
                this.mass = value;
            }
        }

        /// <summary>
        /// Gets or sets the gravity applied to this particle.
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

        /// <summary>
        /// Gets or sets the air resistance applied to a particle.
        /// </summary>
        public float AirResistance
        {
            get
            {
                return this.airResistance;
            }
            
            set
            {
                this.airResistance = value;
            }
        }
    }
}