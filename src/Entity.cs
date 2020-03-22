using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// Entities are the primary object used to interact with the physics engine. Entities represent objects which have some
    /// physical representation and undergo collision detection and motion. Entity motion is calculates according to the
    /// application of a Force and Torque which are provided by abstract functions implimented in derived classes.
    /// </summary>
    /// <seealso cref="DefaultEntity"/>
    /// <seealso cref="NonForceEntity"/>
    /// <seealso cref="StaticEntity"/>
    public abstract class Entity
    {
        /// <summary>
        /// Velocity is stored implicitly as the previous position and current position, hence, when either is updated the
        /// opposite one should be updated as well to prevent changes to the velocity.
        /// </summary>
        private Vector3 _position;
        
        /// <summary>
        /// The _position of the object before the previous update. This value is required for verlet integration used in Update.
        /// </summary>
        private Vector3 previousPosition;
        
        /// <summary>
        /// The current object orientation.
        /// </summary>
        private Vector3 _orientation;
        
        /// <summary>
        /// The orientation before the previous update. As with previousPosition this value is required for verlet integtaion.
        /// </summary>
        private Vector3 previousOrientation;
        
        /// <summary>
        /// The smoothed value of elapsed time used in the previous update. Required to calculate velocity.
        /// </summary>
        private float elapsedTime;
        
        /// <summary>
        /// The mass of the entity.
        /// </summary>
        private float _mass;
        
        /// <summary>
        /// The minimum radius of the sphere entirly containing the object.
        /// </summary>
        private float _boundingRadius;
        
        /// <summary>
        /// The hull used for collision detection.
        /// </summary>
        private Hull _hull;
        
        /// <summary>
        /// The collision handler used in the case of a collision.
        /// </summary>
        private CollisionHandler collisionHandler;

        /// <summary>
        /// Constructs a Entity using some default values.
        /// </summary>
        /// <param name="position">The initial entity position.</param>
        /// <param name="orientation">The initial orientation of the entity.</param>
        /// <param name="mass">The mass of the entity.</param>
        /// <param name="boundingRadius">The bounding radius of the entity.</param>
        /// <param name="hull">The hull used for collision detection.</param>
        /// <param name="collisionHandler">The collision handler to use for collision responce.</param>
        public Entity(Vector3 position, Vector3 orientation, float mass, float boundingRadius, Hull hull, CollisionHandler collisionHandler)
        {
            this._position = position;
            this.previousPosition = position;
            this._orientation = orientation;
            this.previousOrientation = orientation;
            this._mass = mass;
            this._boundingRadius = boundingRadius;
            this._hull = hull;
            this.collisionHandler = collisionHandler;
            this.elapsedTime = 0.05f;
        }

        /// <summary>
        /// Gets or sets the current position of the object being careful not to implicitly alter the velocity.
        /// </summary>
        public Vector3 Position 
        { 
            get 
            { 
                return this._position; 
            } 
            
            set 
            {
                Vector3 vel = this.Velocity;
                this._position = value;
                this.previousPosition = this._position - vel * 2.0f * this.elapsedTime;
            }
        }
        
        /// <summary>
        /// Gets or sets the current orientation beging careful not to implicitly alter the angular velocity.
        /// </summary>
        public Vector3 Orientation 
        { 
            get 
            { 
                return this._orientation; 
            }
            
            set
            {
                Vector3 vel = this.AngularVelocity;
                
                this._orientation = value;
                
                this.previousOrientation = this._orientation - vel * 2.0f * this.elapsedTime;
            }
        }

        /// <summary>
        /// Gets or sets the objects velocity.
        /// </summary>
        public Vector3 Velocity 
        {
            get 
            { 
                return (this._position - this.previousPosition) / (2.0f * this.elapsedTime); 
            }
            
            set 
            { 
                this.previousPosition = this._position - 2.0f * value * this.elapsedTime; 
            }
        }
        
        /// <summary>
        /// Gets the object momentum.
        /// </summary>
        public Vector3 Momentum 
        { 
            get 
            { 
                return this.Velocity * this.Mass; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        public Vector3 AngularVelocity 
        {
            get 
            { 
                return (this._orientation - this.previousOrientation) / (2.0f * this.elapsedTime); 
            }
            
            set 
            { 
                this.previousOrientation = this._orientation - 2.0f * value * this.elapsedTime; 
            }
        }
        
        /// <summary>
        /// Gets the angular momentum.
        /// </summary>
        public Vector3 AngularMomentum 
        { 
            get 
            { 
                return this.Mass * this.AngularVelocity; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the object mass.
        /// </summary>
        public float Mass 
        { 
            get 
            { 
                return this._mass; 
            } 
            
            set 
            { 
                this._mass = value; 
            } 
        }
        
        /// <summary>
        /// Gets the object bounding radius.
        /// </summary>
        public float BoundingRadius 
        { 
            get 
            {  
                return this._boundingRadius; 
            } 
        }
        
        /// <summary>
        /// Gets the object hull.
        /// </summary>
        public Hull Hull 
        { 
            get 
            { 
                return this._hull; 
            } 
        }

        /// <summary>
        /// Gets an approximation to the objects moment of inertia.
        /// </summary>
        public Matrix MomentOfInertia
        {
            get
            {
                return Matrix.CreateScale(this.Mass * this.BoundingRadius * this.BoundingRadius);
            }
        }

        /// <summary>
        /// Gets a matrix representation of the objects current orientation.
        /// </summary>
        public Matrix Rotation
        {
            get
            {
                float angle = this._orientation.Length();
                Vector3 axis;
                if (angle == 0.0f)
                {
                    axis = Vector3.Zero;
                }
                else
                {
                    axis = this._orientation / angle;
                }
                
                return Matrix.CreateFromAxisAngle(axis, angle);
            }
        }

        /// <summary>
        /// Gets a matrix transform which converts vectors from object to world space.
        /// </summary>
        public Matrix Transform
        {
            get
            {
                //TO DO: Include scale matrix
                Matrix transform = this.Rotation;
                transform = transform * Matrix.CreateTranslation(this.Position);
                return transform;
            }
        }

        /// <summary>
        /// Tests if the left and right entity have collided.
        /// </summary>
        /// <param name="left">The first entity to test for collisions.</param>
        /// <param name="right">The second entity to test for collisions.</param>
        /// <returns>Information about where the two entities collided or null if they didn't collide. The left and right in the
        /// return value are not necesarily the same as the input left and right and should be used in their place after calling
        /// Collided.</returns>
        public static CollisionResult<Entity> Collided(Entity left, Entity right)
        {
            CollisionResult<ConvexHull> result = Hull.Collided(left._hull, left.Transform, right._hull, right.Transform);
            if (result != null)
            {
                return new CollisionResult<Entity>(left, right, result.Normal, result.Point, result.Distance);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Perform simulation for a single timestep of length elapsedTime.
        /// </summary>
        /// <param name="elapsedTime">The length of the timestep.</param>
        public void Update(float elapsedTime)
        {
            // Enforces a smooth change in the elapsed time.
            this.elapsedTime = 0.1f * elapsedTime + 0.9f * this.elapsedTime;

            Vector3 acceleration = this.Force() / this.Mass;
            Vector3 angularAcceleration = this.Torque() / this.Mass; //angular acceleration
            Vector3 nextPosition = 2.0f * this._position - this.previousPosition + acceleration * this.elapsedTime * this.elapsedTime;
            Vector3 nextOrientation = 2.0f * this._orientation - this.previousOrientation + angularAcceleration * this.elapsedTime * this.elapsedTime;
            this.previousPosition = this._position;
            this._position = nextPosition;
            this.previousOrientation = this._orientation;
            this._orientation = nextOrientation;
        }

        /// <summary>
        /// Calculate the current force acting on the object.
        /// </summary>
        /// <returns>The force on the object.</returns>
        public abstract Vector3 Force();
        
        /// <summary>
        /// Calculate the current torque acting on the object.
        /// </summary>
        /// <returns>The torque on the object.</returns>
        public abstract Vector3 Torque();

        /// <summary>
        /// Perform collision handling calculations for this entity.
        /// </summary>
        /// <param name="right">The second entity involved in the collision.</param>
        /// <param name="point">The point at which the collision occured.</param>
        /// <param name="normal">The normal at the point where the collision occured.</param>
        /// <param name="dist">The distance along the normal the two objects have penetrated.</param>
        /// <param name="environment">The enviroment to which the two entities belong.</param>
        public void Handle(Entity right, Vector3 point, Vector3 normal, float dist, Environment environment)
        {
            this.collisionHandler.Collide(this, right, point, normal, dist, environment);
        }

        /// <summary>
        /// Create a shallow clopy of this entity. This is used for providing consistent collision handling semantics.
        /// </summary>
        /// <returns>The new entity which is a shallow clopy of this.</returns>
        internal virtual Entity Clone()
        {
            return (Entity)this.MemberwiseClone();
        }
    }
}
