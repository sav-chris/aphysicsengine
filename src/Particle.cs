using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsEngine
{
    /// <summary>
    /// Represents a single particle in a particle system.
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// The current position of the particle.
        /// </summary>
        private Vector3 _position;
        
        /// <summary>
        /// The current velocity of the particle.
        /// </summary>
        private Vector3 velocity;
        
        /// <summary>
        /// The current colour of the particle.
        /// </summary>
        private Color _colour;
        
        /// <summary>
        /// The time remaining until this particle dies.
        /// </summary>
        private float _timeToDeath;
        
        /// <summary>
        /// The mass of the particle.
        /// </summary>
        private float mass;
        
        /// <summary>
        /// The strength of gravity acting on this particle.
        /// </summary>
        private Vector3 gravity;
        
        /// <summary>
        /// The strength of air resistance on this particle.
        /// </summary>
        private float airResistance;

        /// <summary>
        /// Creates a particle for use by a particle system.
        /// </summary>
        /// <param name="pp">A list of supplied parameters.</param>
        public Particle(ParticleParameters pp)
        {
            this._position = pp.InitialPosition;
            this.velocity = pp.Velocity;
            this._colour = pp.Colour;
            this._timeToDeath = pp.LifeTime;
            this.mass = pp.Mass;
            this.gravity = pp.Gravity;
            this.airResistance = pp.AirResistance;
        }

        /// <summary>
        /// Creates a particle for use by a particle system.
        /// </summary>
        public Particle() : this(new ParticleParameters()) {}

        /// <summary>
        /// Gets the current position of the particle.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return this._position;
            }
        }

        /// <summary>
        /// Gets the colour of the particle.
        /// </summary>
        public Color Colour
        {
            get
            {
                return this._colour;
            }
        }

        /// <summary>
        /// Gets the time until the particle dies.
        /// </summary>
        public float TimeToDeath
        {
            get
            {
                return this._timeToDeath;
            }
        }

        /// <summary>
        /// Updates the position of the particle and disables the particle if its lifetime has ended.
        /// </summary>
        /// <param name="elapsedTime">The time elapsed since the last update.</param>
        public void Update(float elapsedTime)
        {
            this._timeToDeath -= elapsedTime;
            Vector3 force = -this.airResistance * this.velocity + this.mass * this.gravity;
            this.velocity += elapsedTime * force / this.mass;
            this._position += this.velocity * elapsedTime;
        }
    }
}