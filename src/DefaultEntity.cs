using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// An implimentation of the Entity class which provides default physical behaviours including gravity and air resistance.
    /// </summary>
    public class DefaultEntity : Entity
    {
        /// <summary>
        /// A local reference to the physics parameters for calculating default forces.
        /// </summary>
        private PhysicsParameters _parameters;
        
        /// <summary>
        /// The strength of air resistance, higher values mean more air resistance.
        /// </summary>
        private float _coefficientOfAirResistance;

        /// <summary>
        /// Construct a DefaultEntity.
        /// </summary>
        /// <param name="position">The initial position of the entity.</param>
        /// <param name="orientation">The initial orientation of the entity.</param>
        /// <param name="mass">The mass of the entity.</param>
        /// <param name="boundingRadius">The minimal radius of the sphere which entirly encloses the entity.</param>
        /// <param name="hull">The collision detection hull of the entity.</param>
        /// <param name="collisionHandler">The entity collision handler.</param>
        /// <param name="environment">The enviroment in which the entity resides, used to calculate gravity strength, etc.
        /// </param>
        /// <param name="coefficientOfAirResistance">The strength of air resistance, high values mean increased air resistance.
        /// </param>
        public DefaultEntity
        (
            Vector3 position,
            Vector3 orientation,
            float mass,
            float boundingRadius,
            Hull hull,
            CollisionHandler collisionHandler,
            PhysicsParameters physicsParameters,
            float coefficientOfAirResistance
        )
            : base(position, orientation, mass, boundingRadius, hull, collisionHandler)
        {
            this._parameters = physicsParameters;
            this._coefficientOfAirResistance = coefficientOfAirResistance;
        }

        /// <summary>
        /// Default Constructor for DefaultEntity
        /// </summary>
        public DefaultEntity() : this
        (
            Vector3.Zero, 
            Vector3.Zero, 
            0, 
            0, 
            null, 
            new ElasticCollision(1), 
            new PhysicsParameters(), 
            0
        ) {}

        /// <summary>
        /// Accessor to physics parameters for calculating default forces.
        /// </summary>
        public PhysicsParameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        /// <summary>
        /// Gets or sets the coefficient air resistance, higher values mean increased air resistance.
        /// </summary>
        public float CoefficientOfAirResistance
        {
            get 
            { 
                return this._coefficientOfAirResistance; 
            }
            
            set 
            { 
                this._coefficientOfAirResistance = value; 
            }
        }

        /// <summary>
        /// Calculates the force due to gravity acting on the object.
        /// </summary>
        /// <returns>The force due to gravity.</returns>
        public Vector3 Gravity()
        {
            return this.Mass * this._parameters.Gravity;
        }

        /// <summary>
        /// Calculates the force due to air resistance acting on the object.
        /// </summary>
        /// <returns>The force due to air resistance.</returns>
        public Vector3 AirResistance()
        {
            return -this._coefficientOfAirResistance * (this.Velocity + this._parameters.WindSpeed);
        }

        /// <summary>
        /// Calculates the torque due to air reesistance acting on the object.
        /// </summary>
        /// <returns>The torque due to air resistance.</returns>
        public Vector3 TorqueResistance()
        {
            return -this._coefficientOfAirResistance * this.AngularVelocity;
        }

        /// <summary>
        /// The force net acting on the object, includes gravity and air resistance.
        /// </summary>
        /// <returns>The force acting on the object.</returns>
        public override Vector3 Force()
        {
            return this.Gravity() + this.AirResistance();
        }

        /// <summary>
        /// The net torque acting on the object, includes air resistance. 
        /// </summary>
        /// <returns>The torque acting on the object.</returns>
        public override Vector3 Torque()
        {
            return this.TorqueResistance();
        }
    }
}
