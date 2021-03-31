using UnityEngine;

public class OrbSkill : MonoBehaviour
{
    private Vector3 direction;
    public float moveSpeed = 3f;

    public static float GetAngleFromVectorFloat(Vector3 dir){
         dir = dir.normalized;
         float n = Mathf.Atan2(dir.y, dir.x)* Mathf.Rad2Deg;
         if (n<0) n += 360;
         return n;
    }

    public void Setup(Vector3 direction)
    {
        this.direction = direction;
        transform.eulerAngles = new Vector3(0,0,GetAngleFromVectorFloat(direction));
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
