using UnityEngine;
using System.Collections;

public class CosAnimation : MonoBehaviour
{
    public float Speed;
    public float WaveLength;
    public float WaveHeight;
    public bool InvertAxis;

    private float fCount = 0f;

    void Start()
    {

    }

    void Update()
    {
        Vector2 posOffset = new Vector2(InvertAxis ? 0 : Speed, InvertAxis ? Speed : 0);        
        float cosFunction = Mathf.Cos(fCount) * WaveHeight;
        fCount += (WaveLength * Time.deltaTime * Mathf.Deg2Rad) % (2 * Mathf.PI);

        if (!InvertAxis)
        {
            posOffset.y = cosFunction;
        }
        else
        {
            posOffset.x = cosFunction;
        }

        this.transform.localPosition += new Vector3(posOffset.x, posOffset.y);
    }
}
