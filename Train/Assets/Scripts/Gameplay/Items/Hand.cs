using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour
{
    private ItemState state;

    void Start()
    {
        this.state = this.GetComponent<ItemState>();
        this.state.ReferenceName = Constants.Items.Hand;
    }

    void Update()
    {

    }
}
