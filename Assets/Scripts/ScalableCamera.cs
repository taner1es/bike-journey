using UnityEngine;
public class ScalableCamera : MonoBehaviour
{
    private void Start()
    {
        float PIXELS_TO_UNITS = 30; // 1:1 ratio of pixels to units

        float TARGET_WIDTH = 1920f;
        float TARGET_HEIGHT = 1080.0f;

        float desiredRatio;
        float currentRatio;
        desiredRatio = TARGET_WIDTH / TARGET_HEIGHT;
        currentRatio = (float)Screen.width / (float)Screen.height;

        PIXELS_TO_UNITS = currentRatio * 10.11235955056f;
        if (currentRatio >= desiredRatio)
        {
            // Our resolution has plenty of width, so we just need to use the height to determine the camera size
            Camera.main.orthographicSize = TARGET_HEIGHT / 4 / PIXELS_TO_UNITS;
        }
        else
        {
            // Our camera needs to zoom out further than just fitting in the height of the image.
            // Determine how much bigger it needs to be, then apply that to our original algorithm.
            float differenceInSize = desiredRatio / currentRatio;
            Camera.main.orthographicSize = TARGET_HEIGHT / 4 / PIXELS_TO_UNITS * differenceInSize;
        }
    }
}
