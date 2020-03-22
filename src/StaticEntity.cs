using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// Represents a entity which does not move, for example a building.
    /// </summary>
    public class StaticEntity : NonForceEntity
    {
        /// <summary>
        /// Creates a new static entity.
        /// </summary>
        /// <param name="position">The position of the static entity.</param>
        /// <param name="orientation">The orientation of the static entity.</param>
        /// <param name="boundingRadius">The bounding radius of the static entity.</param>
        /// <param name="hull">The hull used for collision detection with the static entity.</param>
        public StaticEntity(Vector3 position, Vector3 orientation, float boundingRadius, Hull hull)
            : base(position, orientation, 1.0e10f, boundingRadius, hull, new StaticCollision())
        {
        }
    }
}
