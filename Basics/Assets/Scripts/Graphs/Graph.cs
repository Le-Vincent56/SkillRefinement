using Basics.Instantiation;
using UnityEngine;

namespace Basics.Graphs
{
    public enum TransitionMode
    {
        Cycle,
        Random
    }

    public class Graph : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform pointPrefab;
        private PointPool pointPool;

        [Header("Fields")]
        [SerializeField, Range(10, 200)] private int resolution;
        [SerializeField] private FunctionLibrary.FunctionName function;
        private FunctionLibrary.FunctionName transitionFunction;

        [SerializeField] private TransitionMode transitionMode;
        [SerializeField, Min(0f)] private float functionDuration = 1f;
        [SerializeField, Min(0f)] private float transitionDuration = 1f;
        private float duration;
        private bool transitioning;

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
            // Update the function
            UpdateFunction();

            // Check if transitioning between functions
            if(transitioning)
            {
                // Update the graph through the transition
                UpdateGraphTransition();
            } else
            {
                // Otherwise, update the graph normally
                UpdateGraph();
            }
        }

        /// <summary>
        /// Pick the next function depending on the transition mode
        /// </summary>
        private void PickNextFunction()
        {
            // Pick the next function depending on the transition mode
            function = transitionMode == TransitionMode.Cycle
                ? FunctionLibrary.GetNextFunctionName(function)
                : FunctionLibrary.GetRandomFunctionNameOtherThan(function);
        }

        /// <summary>
        /// Update the function statically
        /// </summary>
        private void UpdateFunction()
        {
            // Add time to the duration
            duration += Time.deltaTime;

            // Check if transitioning between functions
            if (transitioning)
            {
                // Check if the duration exceeds the transition duration
                if (duration >= transitionDuration)
                {
                    // Deduct the transition duration from the current duration
                    duration -= transitionDuration;

                    // Switch back to the normal function mode
                    transitioning = false;
                }
            }
            // Otherwise, check if the duration exceeds the function duration
            else if (duration >= functionDuration)
            {
                // Deduct the extra time from the duration of the next function
                // to stay more synchronized with the excepted timing of function switches
                duration -= functionDuration;

                // Set to transitioning
                transitioning = true;

                // Set the transition function
                transitionFunction = function;

                // Pick the next function
                PickNextFunction();
            }
        }

        private void UpdateGraphTransition()
        {
            // Get the transition functions
            FunctionLibrary.Function from = FunctionLibrary.GetFunction(transitionFunction);
            FunctionLibrary.Function to = FunctionLibrary.GetFunction(function);

            // Get the current transition progress
            float progress = duration / transitionDuration;

            // Cache the current time
            float time = Time.time;

            // Calculate the resolution step
            float step = 2f / resolution;

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
                points[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
            }
        }

        /// <summary>
        /// Update the graph points
        /// </summary>
        private void UpdateGraph()
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
