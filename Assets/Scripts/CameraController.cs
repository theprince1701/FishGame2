using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float smoothing;
    [SerializeField] private float additionalForce;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private Vector3 shakeOffset;
    private Transform target = null;

    private void Update()
    {
        Vector3 t = target == null ? Vector3.zero : target.position / 3.5f;

        transform.position = Vector3.Lerp(transform.position, t + offset + shakeOffset, Time.deltaTime * smoothing);
    }

    private IEnumerator Shake(float dur, float mag)
    {
        float t = 0.0f;

        while (t < dur)
        {
            float x = Random.Range(-1f, 1f) * mag;
            float y = Random.Range(-1f, 1f) * mag;

            shakeOffset = new Vector3(x, y);
            t += Time.deltaTime;

            yield return null;
        }

        shakeOffset = Vector3.zero;
    }

    public void SetTarget(Transform target) => this.target = target;

    public void ShakeCamera(float dur, float mag) => StartCoroutine(Shake(dur, mag));
}
