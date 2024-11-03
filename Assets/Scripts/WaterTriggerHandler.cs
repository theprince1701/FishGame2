using System;
using UnityEngine;

public class WaterTriggerHandler : MonoBehaviour
{
    public LayerMask waterMask;
    
    
    private EdgeCollider2D _edgeCollider2D;
    private Water _water;

    private void Awake()
    {
        _edgeCollider2D = GetComponent<EdgeCollider2D>();
        _water = GetComponent<Water>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if ((waterMask.value & (1 << other.gameObject.layer)) > 0)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb)
            {
                int multiplier = 1;

                if (rb.linearVelocity.y < 0)
                {
                    multiplier = -1;
                }
                else
                {
                    multiplier = 1;
                }

                float vel = rb.linearVelocity.y * _water.forceMultiplier;
                vel = Mathf.Clamp(Mathf.Abs(vel), 0f, _water.maxForce);
                vel *= multiplier;
                
                _water.Splash(other, vel);
            }
        }
    }
}
