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
            StaticSphere,
            ScalingSphere,
            VerticalBandSphere,
            HorizontalBandSphere,
            TwistingBandSphere,
            StaticTorus,
            RotatingTorus
        }

        public delegate Vector3 Function(float u, float v, float t);
        private readonly static Function[] functions = { 
            Wave, 
            MultiWave, 
            Ripple, 
            StaticSphere, 
            ScalingSphere, 
            VerticalBandSphere, 
            HorizontalBandSphere, 
            TwistingBandSphere, 
            StaticTorus, 
            RotatingTorus 
        };

        /// <summary>
        /// Get a function from the function library
        /// </summary>
        public static Function GetFunction(FunctionName name) => functions[(int)name];

        /// <summary>
        /// Get the next function name in the library
        /// </summary>
        public static FunctionName GetNextFunctionName(FunctionName name) => (int)name < functions.Length - 1 ? name + 1 : 0;

        /// <summary>
        /// Get a random function name from the library
        /// </summary>
        public static FunctionName GetRandomFunctionNameOtherThan(FunctionName name)
        {
            // Ensure index zero is never chosen at random
            FunctionName choice = (FunctionName)Random.Range(1, functions.Length);

            // Check if the choice equals the name to avoid
            // If so, return the first name, otherwise the chosen one
            // This allows us to substitute zero for the disallowed index
            // without introducing a selection bias
            return choice == name ? 0 : choice;
        }

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
        /// Generate a static sphere
        /// </summary>
        public static Vector3 StaticSphere(float u, float v, float t)
        {
            // Calculate the radius
            float r = Cos(0.5f * PI * v);

            Vector3 p;

            // Create a UV-sphere (dstribution of points isn't uniform because the
            // sphere is created by stacking circles with different radii)
            p.x = r * Sin(PI * u);
            p.y = Sin(PI * 0.5f * v);
            p.z = r * Sin(PI * u);

            return p;
        }

        /// <summary>
        /// Generate a sphere that scales in and out
        /// </summary>
        public static Vector3 ScalingSphere(float u, float v, float t)
        {
            // Calcualte the radius to scale with time
            float r = 0.5f + 0.5f * Sin(PI * t);

            // Calculate surface radius
            float s = r * Cos(0.5f * PI * v);

            Vector3 p;

            p.x = s * Sin(PI * u);
            p.y = r * Sin(0.5f * PI * v);
            p.z = s * Cos(PI * u);

            return p;
        }

        /// <summary>
        /// Generate a sphere with vertical bands
        /// </summary>
        public static Vector3 VerticalBandSphere(float u, float v, float t)
        {
            // Create a radius that creates vertical bands
            float r = 0.9f + 0.1f * Sin(8f * PI * u);

            // Calculate surface radius
            float s = r * Cos(0.5f * PI * v);

            Vector3 p;

            p.x = s * Sin(PI * u);
            p.y = r * Sin(0.5f * PI * v);
            p.z = s * Cos(PI * u);

            return p;
        }

        /// <summary>
        /// Generate a sphere with horizontal bands
        /// </summary>
        public static Vector3 HorizontalBandSphere(float u, float v, float t)
        {
            // Create a radius that creates horizontal bands
            float r = 0.9f + 0.1f * Sin(8f * PI * v);

            // Calculate surface radius
            float s = r * Cos(0.5f * PI * v);

            Vector3 p;

            p.x = s * Sin(PI * u);
            p.y = r * Sin(0.5f * PI * v);
            p.z = s * Cos(PI * u);

            return p;
        }

        /// <summary>
        /// Generate a sphere that rotates its twisting bands
        /// </summary>
        public static Vector3 TwistingBandSphere(float u, float v, float t)
        {
            // Scale the radius based on time and get both vertical and horizontal twisting bands
            float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));

            // Calculate surface radius
            float s = r * Cos(0.5f * PI * v);

            Vector3 p;

            p.x = s * Sin(PI * u);
            p.y = r * Sin(PI * 0.5f * v);
            p.z = s * Cos(PI * u);

            return p;
        }

        /// <summary>
        /// Generate a static torus
        /// </summary>
        public static Vector3 StaticTorus (float u, float v, float y)
        {
            // The major radius describes how far we pull the sphere apart to influence the
            // shape of the torus
            float r1 = 0.75f;

            // The minor radius determines the thickness of the ring
            float r2 = 0.25f;

            // Can create a torus from a sphere by pulling its vertical
            // half-circles away from each other and turning them into
            // full circles; then use v to describe the entire circle instead of
            // half fo the circle
            float s = 0.5f + r1 + r2 * Cos(PI * v);

            Vector3 p;

            p.x = s * Sin(PI * u);
            p.y = r2 * Sin(PI * v);
            p.z = s * Cos(PI * u);

            return p;
        }

        /// <summary>
        /// Generate a rotating torus with a star pattern and twisting ring
        /// </summary>
        public static Vector3 RotatingTorus(float u, float v, float t)
        {
            // The major radius describes how far we pull the sphere apart to influence the
            // shape of the torus
            float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));

            // The minor radius determines the thickness of the ring
            float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));

            // Can create a torus from a sphere by pulling its vertical
            // half-circles away from each other and turning them into
            // full circles; then use v to describe the entire circle instead of
            // half fo the circle
            float s = 0.5f + r1 + r2 * Cos(PI * v);

            Vector3 p;

            p.x = s * Sin(PI * u);
            p.y = r2 * Sin(PI * v);
            p.z = s * Cos(PI * u);

            return p;
        }

        /// <summary>
        /// Morph between two functions
        /// </summary>
        public static Vector3 Morph(float u, float v, float t, Function from, Function to, float progress)
        {
            return Vector3.LerpUnclamped(
                from(u, v, t), 
                to(u, v, t), 
                SmoothStep(0f, 1f, progress)
            );
        }
    }
}
