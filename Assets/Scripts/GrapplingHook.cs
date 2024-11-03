using System.Collections;
using UnityEngine;

public enum GrappleStates
{
    Shooting,
    Retracting,
    Idle
}

public class GrapplingHook : MonoBehaviour
{
 public GrappleStates state { get; private set; }

    [SerializeField] private int percision = 20;
    [SerializeField] private float waveSize = 1.4f;
    [SerializeField] private float launchSpeedMultiplier = 15f;
    [SerializeField] private float straightenLineSpeed = 10f;

    [SerializeField] private Transform shootTransform;
    [SerializeField] private Transform retractTransform;
    [SerializeField] private AnimationCurve ropeAnimationCurve;
    [SerializeField] private AnimationCurve ropeLaunchSpeedCurve;
    [SerializeField] private Animator armAnimator;


    private LineRenderer lineRenderer = null;
    private Vector3 grappleTarget = Vector3.zero;
    private float moveTime;

    private float currentWaveSize = 0.0f;
    private GrappleCollision grappleCollision;
    private CameraController camController;
    private MiniGame _miniGame;
    private AudioSource audioSource;

    private IEnumerator _resetGrappleCoroutine;
    
    private void Awake()
    {
        grappleCollision = new GameObject("GrappleCollision").AddComponent<GrappleCollision>();
        grappleCollision.gameObject.AddComponent<BoxCollider2D>();
        grappleCollision.gameObject.AddComponent<Rigidbody2D>();
        grappleCollision.Initialize(this);
        camController = FindObjectOfType<CameraController>();
        lineRenderer = GetComponent<LineRenderer>();
        _miniGame = FindObjectOfType<MiniGame>();
        audioSource = GetComponent<AudioSource>();

        lineRenderer.positionCount = percision;
        state = GrappleStates.Idle;
        currentWaveSize = waveSize;
    }

    private IEnumerator ResetGrapple()
    {
        yield return new WaitForSeconds(1.5f);
        OnHooked();
    }

    private void Update()
    {
        if (state != GrappleStates.Idle)
            moveTime += Time.deltaTime;

        bool shootGrapple = Input.GetMouseButtonDown(0) && state == GrappleStates.Idle;

        if (shootGrapple && !_miniGame.isInMiniGame)
        {
            Fire();
            _resetGrappleCoroutine = ResetGrapple();
            StartCoroutine(_resetGrappleCoroutine);
        }

        if (state == GrappleStates.Shooting)
        {
            if (lineRenderer.GetPosition(percision - 1).x != grappleTarget.x)
                UpdateShoot();
            else
            {
                if (currentWaveSize > 0)
                {
                    currentWaveSize -= Time.deltaTime * straightenLineSpeed;
                    UpdateShoot();
                }
                else
                {
                    currentWaveSize = waveSize;
                    grappleTarget = shootTransform.position;
                    state = GrappleStates.Retracting;
                    moveTime = 0.0f;

                    Vector3 p2 = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, p2);
                }
            }
        }
        else if (state == GrappleStates.Retracting)
        {
            UpdateRetract();
        }
    }

    private void Fire()
    {
        state = GrappleStates.Shooting;
        grappleTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        moveTime = 0.0f;
        lineRenderer.enabled = true;
      //  armAnimator.SetTrigger("Throw");
       // audioSource.Play();
    }

    public void OnHookedBeforeMiniGame()
    {
        StopCoroutine(_resetGrappleCoroutine);
    }

    private void UpdateShoot()
    {
        for (int i = 0; i < percision; i++)
        {
            float delta = (float)i / ((float)percision - 1f);
            Vector2 offset = Vector2.Perpendicular(shootTransform.position - grappleTarget).normalized * ropeAnimationCurve.Evaluate(delta) * currentWaveSize;
            Vector2 targetPosition = Vector2.Lerp(shootTransform.position, grappleTarget, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(shootTransform.position, targetPosition, ropeLaunchSpeedCurve.Evaluate(moveTime) * launchSpeedMultiplier);

            lineRenderer.SetPosition(i, currentPosition);
        }

        grappleCollision.transform.position = lineRenderer.GetPosition(percision - 1);
        camController.SetTarget(grappleCollision.transform);
    }

    private void UpdateRetract()
    {
        Vector3 target = shootTransform.position;

        if((lineRenderer.GetPosition(lineRenderer.positionCount-1) - target).magnitude > 0.5f)
        {
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, Vector2.Lerp(lineRenderer.GetPosition(lineRenderer.positionCount - 1), shootTransform.position, Time.deltaTime * 5f));
        }
        else
        {
            lineRenderer.positionCount = percision;
            state = GrappleStates.Idle;
            lineRenderer.enabled = false;

            if (grappleCollision.fish != null)
                grappleCollision.fish.DestroyFish();
        }


        grappleCollision.transform.position = GetEndPosition();
        camController.SetTarget(grappleCollision.transform);
    }

    public void OnHooked()
    {
        if (state != GrappleStates.Retracting)
            state = GrappleStates.Retracting;
    }

    public Vector3 GetEndPosition() => lineRenderer.GetPosition(lineRenderer.positionCount - 1);
    public Vector3 GetOrigin() => shootTransform.position;
}
