using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fish : MonoBehaviour
{
    public float moveSpeed = 2f;
    public String fishName;
    public int fishScore = 1;
    
    private Rigidbody2D _rb;
    private Vector3 _target;

    private GrappleCollision _grappleCollision;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
        
        FishZone[] fishZones = FindObjectsByType<FishZone>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        int randomZones = Random.Range(0, fishZones.Length);
        _target = fishZones[randomZones].transform.position;
        moveSpeed = Random.Range(.4f, 1.1f);
    }

    private void Update()
    {
        if (_grappleCollision)
        {
            transform.position = _grappleCollision.grapple.GetEndPosition();
        }
    }

    private void FixedUpdate()
    {
        if (!_grappleCollision)
        {
            Vector3 dir = (_target - transform.position).normalized;

            Vector3 movePosition = transform.position + dir * moveSpeed * Time.fixedDeltaTime;
            movePosition = new Vector3(movePosition.x, transform.position.y, movePosition.z);
            _rb.MovePosition(movePosition);
        }
    }

    public void OnHooked(GrappleCollision collision)
    {
        _grappleCollision = collision;
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void OnMiniGameFailed()
    {
        _grappleCollision = null;
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void DestroyFish()
    {
        FindObjectOfType<ScoreUI>().OnFishCaught();
        Destroy(gameObject);
    }

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }
}
