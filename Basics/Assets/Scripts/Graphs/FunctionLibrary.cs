using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEngine.Mathf;

namespace Basics.Graphs
{
    public static class FunctionLibrary
    {
        public enum FunctionName
        {
            Wave,
            MultiWave,
            Ripple,
            Sphere
        }

        public delegate Vector3 Function(float u, float v, float t);
        private static Function[] functions = { Wave, MultiWave, Ripple, Sphere };

        /// <summary>
        /// Get a function from the function library
        /// </summary>
        public static Function GetFunction(FunctionName name) => functions[(int)name];

        /// <summary>
        /// Calculate a single sine wave
        /// </summary>
        public static Vector3 Wave(float u, float v, float t)
        {
            Vector3 p;

            // Construct the positions
            p.x = u;
            p.y = Sin(PI * (u + v + t));
            p.z = v;

            return p;
        }

        /// <summary>
        /// Calculate a multi-sine wave function
        /// </summary>
        public static Vector3 MultiWave(float u, float v, float t)
        {
            Vector3 p;

            p.x = u;

            // Calculate a single sine wave
            p.y = Sin(PI * (u + 0.5f * t));

            // Add another sine wave that has double the frequency of the first
            p.y += 0.5f * Sin(2f * PI * (v + t));

            // Add a third wave that travels along teh XZ diagonal
            p.y += Sin(PI * (u + v + 0.25f * t));

            p.z = v;

            // Divide by 2.5 to gaurantee the [-1, 1] range
            return p;
        }

        /// <summary>
        /// Create a ripple effect
        /// </summary>
        public static Vector3 Ripple(float u, float v, float t)
        {
            // Get the absolute value of x to get the distance from the center
            float d = Sqrt(u * u + v * v);

            Vector3 p;

            p.x = u;

            // Use the distance as the input for the sine function
            p.y = Sin(PI * (4f * d - t));

            // Decrease the amplitude of the wave
            p.y /= 1f + 10f * d;

            p.z = v;

            return p;
        }

        /// <summary>
        /// Generate a sphere
        /// </summary>
        public static Vector3 Sphere(float u, float v, float t)
        {
            Vector3 p;

            // Scale the radius based on time
            float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));

            // Calculate surface radius
            float s = r * Cos(0.5f * PI * v);

            p.x = s * Sin(PI * u);
            p.y = r * Sin(PI * 0.5f * v);
            p.z = s * Cos(PI * u);

            return p;
        }
    }
}
