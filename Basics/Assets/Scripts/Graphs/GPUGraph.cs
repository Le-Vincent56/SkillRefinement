using Basics.Instantiation;
using UnityEngine;

namespace Basics.Graphs
{
    public class GPUGraph : MonoBehaviour
    {
        const int maxResolution = 1000;

        [Header("Fields")]
        [SerializeField] private ComputeShader computeShader;
        [SerializeField] private Material material;
        [SerializeField] private Mesh mesh;
        [SerializeField, Range(5, maxResolution)] private int resolution;
        [SerializeField] private FunctionLibrary.FunctionName function;
        private FunctionLibrary.FunctionName transitionFunction;

        [SerializeField] private TransitionMode transitionMode;
        [SerializeField, Min(0f)] private float functionDuration = 1f;
        [SerializeField, Min(0f)] private float transitionDuration = 1f;
        private float duration;
        private bool transitioning;

        private static readonly int positionsID = Shader.PropertyToID("_Positions");
        private static readonly int resolutionID = Shader.PropertyToID("_Resolution");
        private static readonly int stepID = Shader.PropertyToID("_Step");
        private static readonly int timeID = Shader.PropertyToID("_Time");

        private ComputeBuffer positionsBuffers;

        private void OnEnable()
        {
            // Create a Computer Buffer to allow positions to be stored on the GPU
            positionsBuffers = new ComputeBuffer(
                maxResolution * maxResolution,    // Amount of elements within the buffer
                3 * 4                       // Size of each element (3 floats for Vector3, 4 bytes each)   
            );
        }

        private void OnDisable()
        {
            // Free the GPU memory
            positionsBuffers.Release();

            // Since we won't use this specific object instance after this point,
            // explicitly set the field to reference null to allow garbage collection
            positionsBuffers = null;
        }

        private void Update()
        {
            UpdateFunctionOnGPU();
        }

        private void UpdateFunctionOnGPU()
        {
            // Calcualte the step
            float step = 2f / resolution;

            // Set the computer shader properties
            computeShader.SetInt(resolutionID, resolution);
            computeShader.SetFloat(stepID, step);
            computeShader.SetFloat(timeID, Time.time);

            // Get the current kernel index
            int kernelIndex = (int)function;

            // Set the positions buffer to link the buffer to the kernel
            // - The first argument is the index of the kernel function (a compute shader
            // can contain multiple kernels and buffers can be linked to specific ones)
            // - Can find a kernel index by invoking FindKernel() on the compute shader,
            // but single kernels always have an index of 0
            computeShader.SetBuffer(kernelIndex, positionsID, positionsBuffers);

            // Because of our fixed 8x8 group size the amount of groups
            // we need in the X and Y dimensions is equal to the resolution divided by 8, rounded up
            int groups = Mathf.CeilToInt(resolution / 8f);

            // Run our kernel
            // - The first argument is the kernel index
            // - The other three are the amount of groups to run again split per dimension
            // - Using 1 for all dimensions would mean only the first group of 8x8 positions
            // gets calculated
            computeShader.Dispatch(kernelIndex, groups, groups, 1);

            // Set the material's properties
            material.SetBuffer(positionsID, positionsBuffers);
            material.SetFloat(stepID, step);

            // Have to indicate a bounding box to tell Unity where in the scene to draw;
            // the graph sits at the origin and the points should remain inside a cube with size 2,
            // but points have a size as well, half of which could poke outside the bounds in all directions,
            // so we need to add the size of the points to the bounds
            Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));

            // Draw the mesh procedurally
            // - The sub-mesh index (second argument) is for when a mesh consists of multiple parts,
            // but because we do not, we can use index 0
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
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
    }
}
