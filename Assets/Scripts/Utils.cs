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
    }
}