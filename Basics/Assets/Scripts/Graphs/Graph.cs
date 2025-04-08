using Basics.Instantiation;
using UnityEngine;

namespace Basics.Graphs
{
    public class Graph : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform pointPrefab;
        private PointPool pointPool;

        [Header("Fields")]
        [SerializeField, Range(10, 100)] private int resolution;
        [SerializeField] private FunctionLibrary.FunctionName function;

        private Transform[] points;

        private void Awake()
        {
            // Create the point pool
            pointPool = new PointPool(pointPrefab, transform);

            // Instantiate the points array
            points = new Transform[resolution * resolution];

            // Create the graph
            CreateGraph();
        }

        private void Update()
        {
            // Cache the current time
            float time = Time.time;

            // Calculate the resolution step
            float step = 2f / resolution;

            // Retrieve the current function
            FunctionLibrary.Function function = FunctionLibrary.GetFunction(this.function);

            // Set a default v value (v only needs to be recalculated when z changes)
            float v = 0.5f * step - 1f;

            // Iterate through each point
            for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
            {
                // Check if we have finished a "row" in the array
                if (x == resolution)
                {
                    // Reset x back to 0
                    x = 0;

                    // Offset along the z dimension
                    z += 1;

                    // Recalculate v
                    v = (z + 0.5f) * step - 1f;
                }

                // Calculate the parametric values
                float u = (x + 0.5f) * step - 1f;

                // Set the local position of the point
                points[i].localPosition = function(u, v, time);
            }
        }

        private void OnDestroy()
        {
            // Destroy the graph
            DestroyGraph();
        }

        /// <summary>
        /// Create the graph
        /// </summary>
        private void CreateGraph()
        {
            // Calculate the size of each step according to the resolution
            float step = 2f / resolution;

            // Establish the scale of the points
            Vector3 scale = Vector3.one * step;

            // Iterate over the number of points to create
            for (int i = 0; i < points.Length; i++)
            {
                // Get a point from the pool
                Transform point = pointPool.Get();

                // Set the point's scale
                point.localScale = scale;

                // Add the point to the array
                points[i] = point;
            }
        }

        /// <summary>
        /// Destroy the graph
        /// </summary>
        private void DestroyGraph()
        {
            // Iterate over the point list
            for (int i = 0; i < points.Length; i++)
            {
                // Release the point back into the pool
                pointPool.Release(points[i]);

                // Set the point to null
                points[i] = null;
            }
        }
    }
}
