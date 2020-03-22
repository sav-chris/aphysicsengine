using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.IO;
using PhysicsEngine;

namespace PhysicsEngine
{
    /// <summary>
    /// Useful functions which are commonly used both within the physics engine and within games.
    /// </summary>
    public static class CommonFunctions
    {
        /// <summary>
        /// The largest number such that 1.0f + Epsilon = 1.0f.
        /// </summary>
        private static float epsilon = 5.96046447753906e-8f;
        
        /// <summary>
        /// Decompose a vector p into a component parralel to n and a component orthogonal to n.
        /// </summary>
        /// <param name="p">The vector to decompose.</param>
        /// <param name="n">Assumed normalized.</param>
        /// <param name="p_n">The size of the component along n.</param>
        /// <param name="p_t">The resulting vector tangential to p after the component along n is removed.</param>
        public static void GramSchmidt(Vector3 p, Vector3 n, out float p_n, out Vector3 p_t)
        {
            p_n = Vector3.Dot(p, n);
            p_t = p - p_n * n;
        }

        /// <summary>
        /// Load a convex hull from the given stream. The convex hull should use the .hull file format.
        /// </summary>
        /// <param name="reader">The stream to read the hull from.</param>
        /// <returns>The ConvexSegment representing the hull.</returns>
        public static ConvexSegment LoadConvexHull(StreamReader reader)
        {
            string line = reader.ReadLine();
            
            // Read the number of vertices and triangles from the first line.
            string[] nums = Split(line, ' ');
            int numVerts = int.Parse(nums[0]);
            int numTris = int.Parse(nums[1]);
            
            Vector3[] vertices = LoadObjVertices(reader, numVerts, 0);
            int[] triangles = LoadObjTriangles(reader, numTris, 0, 0);

            return new ConvexSegment(vertices, triangles);
        }

        //TO DO: probably replace this method
        //TO DO: Add binary reader
        /// <summary>
        /// Loads a convex hull file made up of several convex segments from a .hull file generated in .obj format by 
        /// ConvexDecomposition utility.
        /// </summary>
        /// <param name="reader">The stream from which to load the convex hulls.</param>
        /// <returns>An array containing the loaded ConvexSegment's.</returns>
        public static ConvexSegment[] LoadMultipleConvexHulls(StreamReader reader)
        {
            // 32 is the default number of hulls generated
            const int NumHulls = 32;
            
            string line;
            int verticesSoFar = 0;

            ConvexSegment[] hulls = new ConvexSegment[NumHulls];
           
            // Read through unnecessary lines
            for (int i = 1; i <= 10; i++)
            {
                line = reader.ReadLine();
            }

            for (int currentHull = 0; currentHull < NumHulls; ++currentHull)
            {
                line = reader.ReadLine();
                line = reader.ReadLine();

                string[] lineSplit = Split(line, ' ');
                int numVerts = int.Parse(lineSplit[5]);
                int numTriangles = int.Parse(lineSplit[8]);

                line = reader.ReadLine();
                Vector3[] vertices = LoadObjVertices(reader, numVerts, 1);
                int[] triangles = LoadObjTriangles(reader, numTriangles, verticesSoFar, 1);
                verticesSoFar += vertices.Length;

                hulls[currentHull] = new ConvexSegment(vertices, triangles);
            }

            return hulls;
        }
        
        /// <summary>
        /// Compare floating point numbers for equality using a relative epsilon.
        /// </summary>
        /// <param name="left">The first floating point value to compare.</param>
        /// <param name="right">The second floating point value to compare.</param>
        /// <returns>True if the relative error is less then the machine epsilon.</returns>
        public static bool FloatEqual(float left, float right)
        {
            return System.Math.Abs((left - right) / right) < epsilon && 
                   System.Math.Abs((left - right) / left) < epsilon;
        }

        /// <summary>
        /// Split a string into segments delimited by chr and eliminate empty results.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="chr">The delimiting char.</param>
        /// <returns>The string segments.</returns>
        private static string[] Split(string str, char chr)
        {
            char[] chrs = new char[] { chr };
            return str.Split(chrs, System.StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Read a list of vertices from an obj stream.
        /// </summary>
        /// <param name="reader">The stream to read the vertices from.</param>
        /// <param name="numVerts">The number of vertices to read.</param>
        /// <param name="offset">The position within a line of the first component which corresponds to vertex data.</param>
        /// <returns>Exactly numVerts vertices loaded from reader.</returns>
        private static Vector3[] LoadObjVertices(StreamReader reader, int numVerts, int offset)
        {
            Vector3[] vertices = new Vector3[numVerts];

            for (int i = 0; i < numVerts; ++i)
            {
                // One vertex per line, components seperated by spaces.
                string[] nums = Split(reader.ReadLine(), ' ');
                float x = float.Parse(nums[offset + 0]);
                float y = float.Parse(nums[offset + 1]);
                float z = float.Parse(nums[offset + 2]);
                vertices[i] = new Vector3(x, y, z);
            }
            return vertices;
        }

        /// <summary>
        /// Load a list of trianlges from an obj stream.
        /// </summary>
        /// <param name="reader">The stream to read the triangles from.</param>
        /// <param name="numTris">The number of triangles to read.</param>
        /// <param name="verticesSoFar">The number of vertices which have already been processed when loading multiple hulls.
        /// </param>
        /// <param name="offset">The offset within a line at which triangle data begins.</param>
        /// <returns>A list of indices with every triple representing a triangle.</returns>
        private static int[] LoadObjTriangles(StreamReader reader, int numTris, int verticesSoFar, int offset)
        {
            int[] triangles = new int[numTris * 3];

            for (int currentIndex = 0; currentIndex < 3 * numTris; currentIndex += 3)
            {
                // One triangle per line, vertices seperated by spaces.
                string[] nums = Split(reader.ReadLine(), ' ');
                triangles[currentIndex] = int.Parse(nums[offset + 0]) /*- 1*/ - verticesSoFar;
                triangles[currentIndex + 1] = int.Parse(nums[offset + 1]) /*- 1*/ - verticesSoFar;
                triangles[currentIndex + 2] = int.Parse(nums[offset + 2]) /*- 1*/ - verticesSoFar;
            }

            return triangles;
        }
    }
}