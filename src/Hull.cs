using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// An agregation of multiple convex hull's used for collision detection.
    /// </summary>
    public class Hull
    {
        /// <summary>
        /// The component convex hulls which make up this hull.
        /// </summary>
        public ConvexHull[] convexHulls;

        /// <summary>
        /// Create a new hull.
        /// </summary>
        /// <param name="convexHulls">The component convex hulls.</param>
        public Hull(ConvexHull[] convexHulls)
        {
            this.convexHulls = convexHulls;
        }

        /// <summary>
        /// Test for collisions between two hulls.
        /// </summary>
        /// <param name="left">The first hull to test for collisions.</param>
        /// <param name="leftTransform">The transform taking objects from entity to world space for the left hull.</param>
        /// <param name="right">The second hull to test for collisions.</param>
        /// <param name="rightTransform">The transform taking objects from entity to world space for the right hull.</param>
        /// <returns>The result of the collision or null if no collision is found.</returns>
        public static CollisionResult<ConvexHull> Collided(Hull left, Matrix leftTransform, Hull right, Matrix rightTransform)
        {
            foreach (ConvexHull leftHull in left.convexHulls)
            {
                foreach (ConvexHull rightHull in right.convexHulls)
                {
                    CollisionResult<ConvexHull> result = ConvexHull.Collided(leftHull, leftTransform, rightHull, rightTransform);
                    if (result == null)
                    {
                        result = ConvexHull.Collided(rightHull, rightTransform, leftHull, leftTransform);
                        if (result != null)
                        {
                            result.Normal = -result.Normal;
                        }
                    }
                    
                    return result;
                }
            }

            return null;
        }
    }
}