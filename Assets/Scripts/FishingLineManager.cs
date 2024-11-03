using System;
using UnityEngine;

public class FishingLineManager : MonoBehaviour
{

    public void ThrowLine(Transform rodAnchor, Vector2 dir, float force)
    {
        GetComponent<Rigidbody2D>().AddForce(dir * force, ForceMode2D.Impulse);
        GetComponent<FishingLine>().rodAnchor = rodAnchor;
    }

}
