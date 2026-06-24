using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    public Transform FillTransform;
    public Health Target;
    public Transform FollowTarget;
    public Vector3 WorldOffset = new Vector3(0f, 1.2f, 0f);

    private float fullWidth;

    public void Initialize(Health target, Transform followTarget)
    {
        Target = target;
        FollowTarget = followTarget;
        fullWidth = FillTransform.localScale.x;

        Target.OnDamaged += RefreshFill;
        Target.OnHealed += RefreshFill;
        RefreshFill();
    }

    private void LateUpdate()
    {
        if (FollowTarget != null)
            transform.position = FollowTarget.position + WorldOffset;
    }

    private void RefreshFill()
    {
        if (Target == null || FillTransform == null) return;

        float pct = Mathf.Clamp01((float)Target.CurrentHP / Target.MaxHP);
        Vector3 scale = FillTransform.localScale;
        float oldWidth = scale.x;
        scale.x = fullWidth * pct;
        FillTransform.localScale = scale;

        float widthDelta = oldWidth - scale.x;
        FillTransform.localPosition -= new Vector3(widthDelta / 2f, 0f, 0f);
    }
    private void OnDestroy()
    {
        if (Target != null)
        {
            Target.OnDamaged -= RefreshFill;
            Target.OnHealed -= RefreshFill;
        }
    }
}