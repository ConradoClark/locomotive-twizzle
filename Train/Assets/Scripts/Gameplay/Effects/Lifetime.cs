using UnityEngine;
using System.Collections;

public class Lifetime : MonoBehaviour
{
    public float LifeInSeconds;

    void Start()
    {
        Destroy(gameObject, LifeInSeconds);
    }
}
