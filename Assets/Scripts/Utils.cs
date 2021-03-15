using UnityEngine;

namespace Utils
{
    public class UtilsClass : MonoBehaviour
    {
        public static Vector3 GetMousePosition2D()
        {
            return GetMousePosition2D(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMousePosition2D(Vector3 screenPosition, Camera camera)
        {
            return camera.ScreenToWorldPoint(screenPosition);
        }

        public static float HypotenuseLength(float sideALength, float sideBLength)
        {
            return Mathf.Sqrt(sideALength * sideALength + sideBLength * sideBLength);
        }
    }
}