using UnityEngine;
using System.Collections;

public class RotateParticle : MonoBehaviour
{
    public Vector3 Rotation;

    void Update()
    {
        this.transform.Rotate(Rotation);
    }
}
