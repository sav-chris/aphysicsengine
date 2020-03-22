using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// ConvexHull represents a closed convex orientable surface with outward facing normals which encloses an object for the
    /// purposes of collision detection. The hull is represented as a list of vertices and triangles which index into the vertex
    /// list. The orientation of a triangle ABC is given by n = AB x AC.
    /// </summary>
    public class ConvexSegment
    {
        /// <summary>
        /// The ray vertex data which makes up the object.
        /// </summary>
        private Vector3[] vertices;

        /// <summary>
        /// Each triangle is represented by three integers which index the verticies.
        /// </summary>
        private int[] triangles;

        /// <summary>
        /// Precalculated normals, avoids calculating them in IsPointInHull which profiling showed to
        /// be a bottleneck.
        /// </summary>
        private Vector3[] normals;

        /// <summary>
        /// Construct a ConvexHull from a set of vertices and triangles. This does not check to ensure the input data does inface
        /// represent a closed convex orientable surface with outward facing normals.
        /// </summary>
        /// <param name="vertices">The vertices in the hull.</param>
        /// <param name="triangles">The triangles in the hull represented as indices in the vertex list. Index starts at  0.</param>
        public ConvexSegment(Vector3[] vertices, int[] triangles)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.CalculateNormals();
        }

        /// <summary>
        /// Gets and sets the raw vertex data for this convex segment.
        /// </summary>
        public Vector3[] Vertices 
        { 
            get 
            { 
                return this.vertices; 
            }

            set
            {
                this.vertices = value;
            }
        }

        /// <summary>
        /// Gets and sets the raw triangle data for this convex segment
        /// </summary>
        public int[] Triangles
        {
            get
            {
                return this.triangles;
            }

            set
            {
                this.triangles = value;
            }
        }

        /// <summary>
        /// Tests if p is inside hull and returns the closestFace to p and the distance to that face. The point must be inside all
        /// faces of the convex hull.
        /// </summary>
        /// <param name="hull">The hull to test the point against.</param>
        /// <param name="p">The point being tested.</param>
        /// <param name="closestFaceNormal">If the point is in the hull this value is set to the normal of the closest face to the point.</param>
        /// <param name="closestDistance">If the point is in the hull this value is set to the distance to the closest face to 
        /// the point.</param>
        /// <returns>True if p is inside hull.</returns>
        public static bool IsPointInHull(ConvexSegment hull, Vector3 p, out Vector3 closestFaceNormal, out float closestDistance)
        {
            closestFaceNormal = new Vector3();
            closestDistance = float.PositiveInfinity;
            bool inside = true;

            // triStart is the starting point of a triangle in the list of triangles
            for (int triStart = 0; triStart < hull.triangles.Length / 3; ++triStart)
            {
                Vector3 a = hull.vertices[hull.triangles[3 * triStart]];
                Vector3 n = hull.normals[triStart];

                // If the angle between n and AP is larger then pi/2 then AP must be behind ABC
                Vector3 ap = p - a;
                
                // cosTheta is really |n||AP|cos(theta) but since |n| and |AP| are both positive it won't effect the result.
                float cosTheta = Vector3.Dot(n, ap);
                
                // distance = |<n, AP> n| = |<n, AP>||n| = |n||AP||cos(theta)|
                // However, since the point is outside the hull if cos(theta) < 0
                // distance = |n||AP|cos(theta)
                float distance = -cosTheta;
                
                inside &= cosTheta < 0;
                if (distance < closestDistance)
                {
                    closestFaceNormal = n;
                    closestDistance = distance;
                }
                
                if (!inside)
                {
                    break;
                }
            }

            return inside;
        }

        /// <summary>
        /// Transform all points in a ConvexSegment by transform and return the resulting convex segment.
        /// </summary>
        /// <param name="transform">The transform to apply to the convex segment.</param>
        /// <returns>The segment with the transform applied to all points.</returns>
        public ConvexSegment Transform(Matrix transform)
        {
            Vector3[] vertices = new Vector3[this.vertices.Length];
            Vector3.Transform(this.vertices, ref transform, vertices);
            return new ConvexSegment(vertices, this.triangles);
        }

        /// <summary>
        /// Calculate the normal vectors of the triangles in a convex segment. 
        /// </summary>
        private void CalculateNormals()
        {
            this.normals = new Vector3[this.triangles.Length / 3];
            Triangle face = new Triangle();
            for (int triStart = 0; triStart < this.triangles.Length / 3; ++triStart)
            {
                face.A = this.vertices[this.triangles[3 * triStart + 0]];
                face.B = this.vertices[this.triangles[3 * triStart + 1]];
                face.C = this.vertices[this.triangles[3 * triStart + 2]];
                this.normals[triStart] = Triangle.Normal(face);
            }
        }
    }
}
