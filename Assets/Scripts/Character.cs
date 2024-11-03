using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float throwForce;
    public Transform spawnTransform;
    public GameObject linePrefab;
    
    public GrapplingHook grapplingHook; 
    public float z = -3;

    public Transform armTransform;
    public float rotationSpeed = 100f; 
    public float minAngle = -45f;
    public float maxAngle = 45f; 

    private float currentAngle = 0f; 


    private FishingLineManager _currentLine;
    
    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

        Vector3 difference = mousePosition - transform.position;
        difference.Normalize ();
        float rotZ=Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;

        rotZ = Mathf.Clamp(rotZ, minAngle, maxAngle);
        armTransform.rotation = Quaternion.Euler (0f, 0f, rotZ);

        
        if (Input.GetMouseButtonDown(0))
        {
           // grapplingHook.
            /*/
            if (_currentLine)
            {
                Destroy(_currentLine.gameObject);
            }
            _currentLine = Instantiate(linePrefab, spawnTransform).GetComponent<FishingLineManager>();
            _currentLine.ThrowLine(spawnTransform,spawnTransform.forward, throwForce);
            /*/
        }
        
        
    }
}
