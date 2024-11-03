using System;
using UnityEngine;

public class FishZone : MonoBehaviour
{
    public Transform oppositeZone;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            Fish fish = other.GetComponent<Fish>();
            fish.SetTarget(oppositeZone.position);
        }
    }
}
