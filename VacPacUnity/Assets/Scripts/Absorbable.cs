using UnityEngine;
using UnityEngine.Events;

public class Absorbable : MonoBehaviour
{
    [Header("Rewards")]
    public int points = 10;
    public float healthRestore = 5f;
    public bool destroyOnAbsorb = true;

    [Header("Events")]
    public UnityEvent<VacPac> onAbsorbed = new UnityEvent<VacPac>();

    [Header("Visual Feedback")]
    public float absorbScaleMultiplier = 1.5f;
    public float scaleTime = 0.18f;

    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    // 🔑 THE FIX: Prevent multiple absorbs!
    private bool isBeingAbsorbed = false;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Absorbable"))
            gameObject.layer = LayerMask.NameToLayer("Absorbable");
    }

    public void Absorb(VacPac vacPac)
    {
        // 🔑 PREVENT MULTIPLE CALLS!
        if (isBeingAbsorbed) return;
        isBeingAbsorbed = true;

        // Stop previous coroutine
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScalePopAndDie(vacPac));
    }

    private System.Collections.IEnumerator ScalePopAndDie(VacPac vacPac)
    {
        // VFX + Events
        if (vacPac.absorbImpactVFX != null)
            Instantiate(vacPac.absorbImpactVFX, transform.position, Quaternion.identity);
        onAbsorbed?.Invoke(vacPac);

        // Scale animation
        Vector3 startScale = originalScale;
        Vector3 peakScale = originalScale * absorbScaleMultiplier;
        float halfTime = scaleTime * 0.5f;
        float timer = 0f;

        // Punch up
        while (timer < halfTime)
        {
            timer += Time.deltaTime;
            float t = timer / halfTime;
            t = 1f - (1f - t) * (1f - t); // Ease out
            transform.localScale = Vector3.LerpUnclamped(startScale, peakScale, t);
            yield return null;
        }

        // Shrink
        timer = 0f;
        while (timer < halfTime)
        {
            timer += Time.deltaTime;
            float t = timer / halfTime;
            t = t * t; // Ease in
            transform.localScale = Vector3.LerpUnclamped(peakScale, Vector3.one * 0.05f, t);
            yield return null;
        }

        // FINAL DESTROY
        if (destroyOnAbsorb)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}