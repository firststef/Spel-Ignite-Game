using UnityEngine;

public class WaterBall : MonoBehaviour
{
    private Vector3 direction;
    public float moveSpeed = 10f;

    public void Setup(Vector3 direction)
    {
        this.direction = direction;
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
