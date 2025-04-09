using TMPro;
using UnityEngine;

namespace Basics.UI
{
    public class FrameRateCounter : MonoBehaviour
    {
        public enum DisplayMode
        {
            FPS,
            MS
        }

        [Header("References")]
        [SerializeField] private TextMeshProUGUI display;

        [Header("Fields")]
        [SerializeField] private DisplayMode displayMode = DisplayMode.FPS;
        [SerializeField, Range(0.1f, 2f)] float sampleDuration = 1f;
        private float bestDuration = float.MaxValue;
        private float worstDuration = 0f;

        private int frames;
        private float duration;

        private void Awake()
        {
            // Get the display component
            display = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            // Get the frame duration (how much time has elapsed)
            float frameDuration = Time.unscaledDeltaTime;

            // Update the frame count and total duration for average frame rate
            frames++;
            duration += frameDuration;

            // Check if the frame duration is less than the best duration
            if(frameDuration < bestDuration)
            {
                // Update the best duration
                bestDuration = frameDuration;
            }

            if(frameDuration > worstDuration)
            {
                // Update the worst duration
                worstDuration = frameDuration;
            }

            // Check if the accumulated duration equals or exceeds the
            // configured sample duration
            if(duration >= sampleDuration)
            {
                if(displayMode == DisplayMode.FPS)
                {
                    // Display the performance in frame rate
                    display.SetText(
                        "FPS\n{0:0}\n{1:0}\n{2:0}",
                        1f / bestDuration,
                        frames / duration,
                        1f / worstDuration
                    );
                } else
                {
                    // Display the performance in milliseconds
                    display.SetText(
                        "MS\n{0:1}\n{1:1}\n{2:1}",
                        1000f * bestDuration,
                        1000f * duration / frames,
                        1000f * worstDuration
                    );
                }

                // Reset the average
                frames = 0;
                duration = 0f;

                // Reset duration achievements
                bestDuration = float.MaxValue;
                worstDuration = 0f;
            }
        }
    }
}
