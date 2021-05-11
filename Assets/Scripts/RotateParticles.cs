using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateParticles : MonoBehaviour
{
    public float speed = 30f;
    private ParticleSystem.ShapeModule shape;

    void Start()
    {
        var particleSys = GetComponent<ParticleSystem>();
        shape = particleSys.shape;
    }

    void Update()
    {
        shape.rotation = new Vector3(shape.rotation.x, shape.rotation.y, shape.rotation.z + Time.deltaTime * speed);
    }
}
