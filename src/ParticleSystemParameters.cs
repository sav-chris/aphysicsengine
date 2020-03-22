using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhysicsEngine
{
    /// <summary>
    /// A utility class which aggregates the particle system parameters to simplify the creation of particle systems.
    /// </summary>
    public class ParticleSystemParameters
    {
        /// <summary>
        /// The initial number of particles in the particle system.
        /// </summary>
        private int initialParticles;
        
        /// <summary>
        /// The number of particles which 'die' per second.
        /// </summary>
        private int totalParticleDecreasePerSecond;
        
        /// <summary>
        /// The maximum number of particles.
        /// </summary>
        private int maxParticles;
        
        /// <summary>
        /// The position of the centre of the particle system.
        /// </summary>
        private Vector3 position;
        
        /// <summary>
        /// The average life time of particles.
        /// </summary>
        private float lifeTime;
        
        /// <summary>
        /// The allowed variance in particle life time.
        /// </summary>
        private float lifeTimeVariance;
        
        /// <summary>
        /// The average time between new particle births.
        /// </summary>
        private float birthTime;
        
        /// <summary>
        /// The allowed variance in the time between new particle births.
        /// </summary>
        private float birthTimeVariance;
        
        /// <summary>
        /// The average direction of particles.
        /// </summary>
        private Vector3 direction;
        
        /// <summary>
        /// The allowed variance in particle direction.
        /// </summary>
        private float directionVariance;
        
        /// <summary>
        /// The average speed of particles.
        /// </summary>
        private float speed;
        
        /// <summary>
        /// The allowed variance in particle speed.
        /// </summary>
        private float speedVariance;
        
        /// <summary>
        /// The average colour of the particles.
        /// </summary>
        private Color colour;
        
        /// <summary>
        /// The allowed variance in particle colour.
        /// </summary>
        private byte colourVariance;
        
        /// <summary>
        /// The average particle mass.
        /// </summary>
        private float mass;
        
        /// <summary>
        /// The allowed variance in particle mass.
        /// </summary>
        private float massVariance;
        
        /// <summary>
        /// The strength of gravity acting on the particles.
        /// </summary>
        private Vector3 gravity;
        
        /// <summary>
        /// The strength of air resistance acting on the particles.
        /// </summary>
        private float airResistance;

        /// <summary>
        /// Construct a empty set of particle system parameters.
        /// </summary>
        public ParticleSystemParameters()
        {
            this.totalParticleDecreasePerSecond = 1;
            this.gravity = Vector3.Zero;
            this.airResistance = 0.0f;
        }

        /// <summary>
        /// Gets or sets the initial number of particles in the particle system.
        /// </summary>
        public int InitialParticles
        {
            get
            {
                return this.initialParticles;
            }
            
            set
            {
                this.initialParticles = value;
            }
        }

        /// <summary>
        /// Gets or sets the total number of particles removed per second.
        /// </summary>
        public int TotalParticleDecreasePerSecond
        {
            get
            {
                return this.totalParticleDecreasePerSecond;
            }
            
            set
            {
                this.totalParticleDecreasePerSecond = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of particles in the particle system.
        /// </summary>
        public int MaxParticles
        {
            get
            {
                return this.maxParticles;
            }
            
            set
            {
                this.maxParticles = value;
            }
        }

        /// <summary>
        /// Gets or sets the average position of particles in the partile system.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return this.position;
            }
            
            set
            {
                this.position = value;
            }
        }

        /// <summary>
        /// Gets or sets the average life time of particles in the particle system.
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
        /// Gets or sets the allowed variance in particle lifetimes.
        /// </summary>
        public float LifeTimeVariance
        {
            get
            {
                return this.lifeTimeVariance;
            }
            
            set
            {
                this.lifeTimeVariance = value;
            }
        }

        /// <summary>
        /// Gets or sets the average time between particle births.
        /// </summary>
        public float BirthTime
        {
            get
            {
                return this.birthTime;
            }
            
            set
            {
                this.birthTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the allowed variance in particle birth times.
        /// </summary>
        public float BirthTimeVariance
        {
            get
            {
                return this.birthTimeVariance;
            }
            
            set
            {
                this.birthTimeVariance = value;
            }
        }

        /// <summary>
        /// Gets or sets the average direction of a particle.
        /// </summary>
        public Vector3 Direction
        {
            get
            {
                return this.direction;
            }
            
            set
            {
                this.direction = value;
            }
        }

        /// <summary>
        /// Gets or sets the allowed variance in particle direction.
        /// </summary>
        public float DirectionVariance
        {
            get
            {
                return this.directionVariance;
            }
            
            set
            {
                this.directionVariance = value;
            }
        }

        /// <summary>
        /// Gets or sets the average speed of the particles in the particle system.
        /// </summary>
        public float Speed
        {
            get
            {
                return this.speed;
            }
            
            set
            {
                this.speed = value;
            }
        }

        /// <summary>
        /// Gets or sets the allowed variance in particle speed.
        /// </summary>
        public float SpeedVariance
        {
            get
            {
                return this.speedVariance;
            }
            
            set
            {
                this.speedVariance = value;
            }
        }

        /// <summary>
        /// Gets or sets the average colour of particles in the particle system.
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
        /// Gets or sets the allowed variance in the particle colours.
        /// </summary>
        public byte ColourVariance
        {
            get
            {
                return this.colourVariance;
            }
            
            set
            {
                this.colourVariance = value;
            }
        }

        /// <summary>
        /// Gets or sets the average particle mass.
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
        /// Gets or sets the allowed variance in the particle mass.
        /// </summary>
        public float MassVariance
        {
            get
            {
                return this.massVariance;
            }
            
            set
            {
                this.massVariance = value;
            }
        }

        /// <summary>
        /// Gets or sets the strength of gravity acting on the particles.
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
        /// Gets or sets the strength of air resistance acting on particles.
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