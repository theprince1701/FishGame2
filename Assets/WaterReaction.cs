using System;
using System.Collections;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class WaterReaction : MonoBehaviour
{
   public float bobbingHeight = 0.5f;
   public float bobbingSpeed = 2f;
   public float waterDamping;

   private Vector3 _startPosition;
   private float _timeCounter;

   private bool _slowDownVelocity;
   private bool _startBobbing;

   private Rigidbody2D _rb;

   private void Awake()
   {
      _rb = GetComponent<Rigidbody2D>();
   }

   public void OnEnterWater()
   {
      _slowDownVelocity = true;
      _rb.linearDamping = waterDamping;
      StartCoroutine(StartBobbing());
   }

   private IEnumerator StartBobbing()
   {
      yield return new WaitForSeconds(0.25f);
      _slowDownVelocity = false;
      _startPosition = transform.position;
      _startBobbing = true;
   }

   private void FixedUpdate()
   {
      if (_slowDownVelocity)
      {
      }
      
      if (_startBobbing)
      {
         _timeCounter += Time.deltaTime * bobbingSpeed;

         float newY = _startPosition.y + Mathf.Sin(_timeCounter) * bobbingHeight;
         transform.position = Vector3.Lerp(transform.position,new Vector3(_startPosition.x, newY, _startPosition.z), Time.deltaTime * 20f);
      }
   }
}
