using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float throwForce;
    public Transform spawnTransform;
    public GameObject linePrefab;
    [SerializeField] private float z = -3;

    public Transform armTransform;
    public float rotationSpeed = 100f; 
    public float minAngle = -45f;
    public float maxAngle = 45f; 

    private float currentAngle = 0f; 


    private FishingLineManager _currentLine;
    
    private void Update()
    {
        Vector3 pt = Input.mousePosition;
        pt.z = -z;
        pt = Camera.main.ScreenToWorldPoint(pt);
        pt.z = z;

        Vector2 dir = (pt - transform.position).normalized;
      //  armTransform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        
        if (Input.GetMouseButtonDown(0))
        {
            if (_currentLine)
            {
                Destroy(_currentLine.gameObject);
            }
            _currentLine = Instantiate(linePrefab, spawnTransform).GetComponent<FishingLineManager>();
            _currentLine.ThrowLine(spawnTransform,spawnTransform.forward, throwForce);
        }
        
        
    }
}
