using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// Represents a entity with no forces acting on it but which may move as a result of collisions.
    /// </summary>
    public class NonForceEntity : Entity
    {
        /// <summary>
        /// Create a non-force entity.
        /// </summary>
        /// <param name="position">The initial position of the entity.</param>
        /// <param name="orientation">The initial orientation of the entity.</param>
        /// <param name="mass">The mass of the entity.</param>
        /// <param name="boundingRadius">The bounding radius of the entity.</param>
        /// <param name="hull">The entities collision detection hull.</param>
        /// <param name="collisionHandler">The entities collision handler.</param>
        public NonForceEntity(Vector3 position, Vector3 orientation, float mass, float boundingRadius, Hull hull, CollisionHandler collisionHandler)
            : base(position, orientation, mass, boundingRadius, hull, collisionHandler)
        {
        }

        /// <summary>
        /// Calculates the net force on the entity, this is always zero for a NonForceEntity.
        /// </summary>
        /// <returns>The net force on the entity, Zero.</returns>
        public override Vector3 Force()
        {
            return Vector3.Zero;
        }

        /// <summary>
        /// Calculates the net torque on the entity, this is always zero for a NonForceEntity.
        /// </summary>
        /// <returns>The net torque on the entity, Zero.</returns>
        public override Vector3 Torque()
        {
            return Vector3.Zero;
        }
    }
}
