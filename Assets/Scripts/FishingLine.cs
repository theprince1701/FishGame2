using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishingLine : MonoBehaviour
{ 
    public Transform rodAnchor;           // The fishing rod's anchor point
    public Transform hook;                // The hook at the end of the rope
    public int segmentCount = 10;         // Number of segments in the rope
    public float segmentLength = 0.2f;    // Length of each rope segment
    public LineRenderer lineRenderer;     // LineRenderer to visualize the rope
    public float castForce = 5f;          // Initial upward force to simulate casting
    public float castDuration = 2f;       // Duration of the casting effect
    public float maxRopeLength = 3f;      // Maximum allowed length of the fishing line
    public float jiggleStrength = 0.1f;   // Strength of the jiggle effect
    public float jiggleDuration = 0.5f;   // Duration of the jiggle effect
    public float retractSpeed = 2f;       // Speed at which to retract the line

    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float castTimer;

    void Start()
    {
        // Initialize the rope segments
        Vector2 ropeStartPoint = rodAnchor.position;
        for (int i = 0; i < segmentCount; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= segmentLength;  // Place each segment slightly below the previous one
        }

        // Set the timer for casting
        castTimer = castDuration;
    }

    void Update()
    {
        DrawRope();
    }

    void FixedUpdate()
    {
        SimulateRopePhysics();
    }

    void SimulateRopePhysics()
    {
        Vector2 gravityForce = new Vector2(0f, -9.81f);

        // Apply physics to each segment
        for (int i = 1; i < ropeSegments.Count; i++)  // Skip the first segment since it's anchored
        {
            RopeSegment segment = ropeSegments[i];

            // Apply casting force if within casting time
            if (castTimer > 0)
            {
                segment.currentPosition += Vector2.up * castForce * Time.fixedDeltaTime;
            }
            else
            {
                segment.currentPosition += gravityForce * Time.fixedDeltaTime;
            }

            ropeSegments[i] = segment;
        }

        // Decrease cast timer
        if (castTimer > 0)
        {
            castTimer -= Time.fixedDeltaTime;
        }

        // Enforce rope length constraints
        for (int i = 0; i < 50; i++)  // Multiple passes to improve stability
        {
            ApplyConstraints();
        }

        // Enforce max rope length
        EnforceMaxLength();
    }

    void ApplyConstraints()
    {
        // Constraint: Keep the first segment at the rod anchor
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.currentPosition = rodAnchor.position;
        ropeSegments[0] = firstSegment;

        // Constraint: Keep the last segment at the hook
        RopeSegment lastSegment = ropeSegments[ropeSegments.Count - 1];
        lastSegment.currentPosition = hook.position;
        ropeSegments[ropeSegments.Count - 1] = lastSegment;

        // Enforce distance constraint between each pair of segments
        for (int i = 0; i < ropeSegments.Count - 1; i++)
        {
            RopeSegment segmentA = ropeSegments[i];
            RopeSegment segmentB = ropeSegments[i + 1];

            float distance = (segmentA.currentPosition - segmentB.currentPosition).magnitude;
            float error = distance - segmentLength;
            Vector2 changeDir = (segmentA.currentPosition - segmentB.currentPosition).normalized;
            Vector2 changeAmount = changeDir * error;

            if (i != 0)
            {
                segmentA.currentPosition -= changeAmount * 0.5f;
                ropeSegments[i] = segmentA;
            }
            segmentB.currentPosition += changeAmount * 0.5f;
            ropeSegments[i + 1] = segmentB;
        }
    }

    void EnforceMaxLength()
    {
        // Check distance from anchor to hook
        Vector2 anchorPosition = rodAnchor.position;
        Vector2 hookPosition = ropeSegments[ropeSegments.Count - 1].currentPosition;
        float currentRopeLength = (hookPosition - anchorPosition).magnitude;

        // If current length exceeds max length, adjust the hook position
        if (currentRopeLength > maxRopeLength)
        {
            Vector2 direction = (hookPosition - anchorPosition).normalized;
            Vector2 newHookPosition = anchorPosition + direction * maxRopeLength;

            RopeSegment lastSegment = ropeSegments[ropeSegments.Count - 1];
            lastSegment.currentPosition = newHookPosition;
            ropeSegments[ropeSegments.Count - 1] = lastSegment;
        }
    }

    public void JiggleLine()
    {
        StartCoroutine(JiggleCoroutine());
    }

    private IEnumerator JiggleCoroutine()
    {
        float jiggleEndTime = Time.time + jiggleDuration;

        while (Time.time < jiggleEndTime)
        {
            for (int i = 1; i < ropeSegments.Count; i++)
            {
                RopeSegment segment = ropeSegments[i];

                // Randomly adjust the position of the segment for jiggle effect
                float offsetX = Random.Range(-jiggleStrength, jiggleStrength);
                float offsetY = Random.Range(-jiggleStrength, jiggleStrength);
                segment.currentPosition += new Vector2(offsetX, offsetY);

                ropeSegments[i] = segment;
            }

            // Update the rope visualization
            DrawRope();

            // Wait for the next frame
            yield return null;
        }

        // Return the segments to their original positions
        for (int i = 1; i < ropeSegments.Count; i++)
        {
            RopeSegment segment = ropeSegments[i];
            segment.currentPosition = Vector2.Lerp(segment.currentPosition, ropeSegments[i].currentPosition, 0.5f);
            ropeSegments[i] = segment;
        }
    }

    public void RetractLine()
    {
        StartCoroutine(RetractCoroutine());
    }

    private IEnumerator RetractCoroutine()
    {
        Vector2 targetPosition = rodAnchor.position;
        float distanceToTarget = (ropeSegments[ropeSegments.Count - 1].currentPosition - targetPosition).magnitude;

        while (distanceToTarget > 0.1f) // Continue until close enough to the target
        {
            for (int i = 1; i < ropeSegments.Count; i++)
            {
                RopeSegment segment = ropeSegments[i];
                Vector2 direction = (targetPosition - segment.currentPosition).normalized;
                
                // Move each segment towards the rod anchor
                segment.currentPosition += direction * retractSpeed * Time.fixedDeltaTime;

                ropeSegments[i] = segment;
            }

            // Recalculate distance to target
            distanceToTarget = (ropeSegments[ropeSegments.Count - 1].currentPosition - targetPosition).magnitude;

            // Update the rope visualization
            DrawRope();

            yield return null; // Wait for the next frame
        }

        // Ensure the last segment is exactly at the rod anchor position
        RopeSegment lastSegment = ropeSegments[ropeSegments.Count - 1];
        lastSegment.currentPosition = targetPosition;
        ropeSegments[ropeSegments.Count - 1] = lastSegment;

        // Final draw call to ensure the line appears correctly retracted
        DrawRope();
    }

    void DrawRope()
    {
        lineRenderer.positionCount = ropeSegments.Count;
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            lineRenderer.SetPosition(i, ropeSegments[i].currentPosition);
        }
    }

    private struct RopeSegment
    {
        public Vector2 currentPosition;
        public Vector2 previousPosition;

        public RopeSegment(Vector2 position)
        {
            currentPosition = position;
            previousPosition = position;
        }
    }
}
