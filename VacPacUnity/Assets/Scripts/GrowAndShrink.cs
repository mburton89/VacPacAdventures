using UnityEngine;
using System.Collections;

public class GrowAndShrink : MonoBehaviour
{
    [Header("Scale Settings")]
    public Vector3 targetScale = new Vector3(2f, 2f, 2f);
    public Vector3 endScale = Vector3.one;

    [Header("Timing")]
    public float growDuration = 0.3f;
    public float shrinkDuration = 0.4f;
    public float holdDuration = 0.1f;

    [Header("Easing")]
    public AnimationCurve growCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve shrinkCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine runningCoroutine;

    /// <summary>
    /// Call this public method to trigger the grow-then-shrink animation.
    /// Safe to call multiple times — cancels any previous animation.
    /// </summary>
    public void GrowThenShrink()
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);

        runningCoroutine = StartCoroutine(GrowShrinkRoutine());
    }

    /// <summary>
    /// Overload with custom durations
    /// </summary>
    public void GrowThenShrink(float growTime, float shrinkTime, float holdTime = 0.1f)
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);

        runningCoroutine = StartCoroutine(GrowShrinkRoutine(growTime, shrinkTime, holdTime));
    }

    private IEnumerator GrowShrinkRoutine(float? growTime = null, float? shrinkTime = null, float? holdTime = null)
    {
        Vector3 startScale = transform.localScale;

        float gt = growTime ?? growDuration;
        float st = shrinkTime ?? shrinkDuration;
        float ht = holdTime ?? holdDuration;

        // === GROW PHASE ===
        float growTimer = 0f;
        while (growTimer <= gt)
        {
            growTimer += Time.deltaTime;
            float t = Mathf.Clamp01(growTimer / gt);
            float curveValue = growCurve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveValue);
            yield return null;
        }

        // Ensure we end exactly at target scale
        transform.localScale = targetScale;

        // === HOLD AT MAX ===
        if (ht > 0f)
        {
            yield return new WaitForSeconds(ht);
        }

        // === SHRINK PHASE ===
        float shrinkTimer = 0f;
        while (shrinkTimer <= st)
        {
            shrinkTimer += Time.deltaTime;
            float t = Mathf.Clamp01(shrinkTimer / st);
            float curveValue = shrinkCurve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(targetScale, endScale, curveValue);
            yield return null;
        }

        // Ensure we end exactly at endScale
        transform.localScale = endScale;

        runningCoroutine = null;
    }

    // Optional: Reset scale when disabled (useful in object pools)
    private void OnDisable()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        transform.localScale = endScale;
    }
}