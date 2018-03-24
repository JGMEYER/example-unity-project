using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazePiston : MonoBehaviour
{

    public float smooth = 1f;
    public float minHeight = 0f;
    public float maxHeight = 1f;

    private Vector3 _minScale;
    private Vector3 _maxScale;
    private Vector3 _targetScale;

    void Start()
    {
        Vector3 startScale = transform.localScale;

        // Cylinder scales are weird
        _minScale = new Vector3(startScale.x, minHeight / 2, startScale.z);
        _maxScale = new Vector3(startScale.x, maxHeight / 2, startScale.z);

        transform.localScale = _maxScale;
    }

    void Update()
    {
        CheckTarget();
        Vector3 newScale = Vector3.Lerp(transform.localScale, _targetScale, smooth * Time.deltaTime);
        transform.position += newScale - transform.localScale;
        transform.localScale = newScale;
    }

    void CheckTarget()
    {
        if (transform.localScale == _minScale)
        {
            _targetScale = _maxScale;
        }
        else if (transform.localScale == _maxScale)
        {
            _targetScale = _minScale;
        }
    }

}
