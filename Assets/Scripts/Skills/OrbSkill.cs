using UnityEngine;
using Utils;

public class OrbSkill : MonoBehaviour
{
    private Vector3 direction;
    private float moveSpeed = 0;

    public void Setup(Vector3 direction, float moveSpeed)
    {
        this.direction = direction;
        this.moveSpeed = moveSpeed;
        transform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction)); ;
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += moveSpeed * direction * Time.deltaTime;
    }
}
