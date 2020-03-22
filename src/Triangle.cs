using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// A utility class for representing triangles and some common operations on them.
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// The first point in the triangle.
        /// </summary>
        private Vector3 a;
        
        /// <summary>
        /// The second point in the triangle.
        /// </summary>
        private Vector3 b;
        
        /// <summary>
        /// The third point in the triangle.
        /// </summary>
        private Vector3 c;
        
        /// <summary>
        /// Gets or sets the first point in the triangle.
        /// </summary>
        public Vector3 A 
        { 
            get 
            { 
                return this.a; 
            } 
            
            set 
            { 
                this.a = value; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the second point in the triangle.
        /// </summary>
        public Vector3 B 
        { 
            get 
            { 
                return this.b; 
            } 
            
            set 
            { 
                this.b = value; 
            } 
        }
        
        /// <summary>
        /// Gets or sets the third point in the triangle.
        /// </summary>
        public Vector3 C 
        { 
            get 
            { 
                return this.c; 
            } 
            
            set 
            { 
                this.c = value; 
            }
        }

        /// <summary>
        /// Calculate the normal to the surface of the supplied triangle.
        /// </summary>
        /// <param name="tri">The triangle to calculate the normal of.</param>
        /// <returns>The normal of the surface on which the triangle lays.</returns>
        public static Vector3 Normal(Triangle tri)
        {
            return Vector3.Normalize(Vector3.Cross(tri.B - tri.A, tri.C - tri.A));
        }

        /// <summary>
        /// Apply the given transformation matrix to the triangle.
        /// </summary>
        /// <param name="triangle">The triangle to transform.</param>
        /// <param name="matrix">The transformation matrix.</param>
        /// <returns>The transformed triangle.</returns>
        public static Triangle Transform(Triangle triangle, Matrix matrix)
        {
            Triangle result = new Triangle();
            result.A = Vector3.Transform(triangle.A, matrix);
            result.B = Vector3.Transform(triangle.B, matrix);
            result.C = Vector3.Transform(triangle.C, matrix);
            return result;
        }
    }
}
