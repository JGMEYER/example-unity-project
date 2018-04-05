using System;
using UnityEngine;

public class GrowShrink : MonoBehaviour
{

    public float MinSizeRatio = 0.5f;
    public float MaxSizeRatio = 2f;
    public float AnimationSpeed = 2f;

    private float animationTimer = 0f;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnValidate()
    {
        if (MaxSizeRatio < MinSizeRatio)
        {
            MaxSizeRatio = MinSizeRatio;
            throw new ArgumentException("MaxSizeRatio cannot be less than MinSizeRatio");
        }
    }

    private void Update()
    {
        animationTimer += Time.deltaTime;

        float middle = ((MaxSizeRatio - MinSizeRatio) / 2);
        float sizeRatio = Mathf.Cos(animationTimer * AnimationSpeed) * middle + middle + MinSizeRatio;
        transform.localScale = originalScale * sizeRatio;
    }

}
