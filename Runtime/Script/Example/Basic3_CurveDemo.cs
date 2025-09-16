using UnityEngine;

public class Basic3_CurveDemo : MonoBehaviour
{
    [Header("Position-Y 이동 곡선 (0~1s에 0→3)")]
    public AnimationCurve posY = AnimationCurve.EaseInOut(0, 0, 1, 3);

    [Header("알파 페이드 곡선 (0~1s에 0→1)")]
    public AnimationCurve alpha = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float duration = 1f;
    public CanvasGroup canvasGroup;

    float t;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        t = Mathf.Clamp01(t + Time.deltaTime / duration);

        // Position-Y를 곡선으로
        var p = startPos;
        p.y += posY.Evaluate(t);
        transform.position = p;

        // UI 알파를 곡선으로
        if (canvasGroup)
            canvasGroup.alpha = Mathf.Clamp01(alpha.Evaluate(t));
    }
}
