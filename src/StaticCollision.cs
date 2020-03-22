using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// A collsion handler that does nothing. Used for static entities. 
    /// </summary>
    public class StaticCollision : CollisionHandler
    {
        /// <summary>
        /// Respond to a collision between a static entity (left) and another entity (right). A static collision makes no
        /// changes to either collided entity as static entities do not move.
        /// </summary>
        /// <param name="left">The first (static) entity involved in the collision.</param>
        /// <param name="right">The second (potentially non-static) entity involved in the collision.</param>
        /// <param name="point">The point at which the two objects have collided.</param>
        /// <param name="normal">The normal of the left entity at the collision point.</param>
        /// <param name="dist">The distance the two entities have penetrated.</param>
        /// <param name="environment">The environment to which the two entities belong.</param>
        public override void Collide(Entity left, Entity right, Vector3 point, Vector3 normal, float dist, Environment environment)
        {
        }
    }
}
