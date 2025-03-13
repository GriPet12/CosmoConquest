using UnityEngine;

public class SpriteHoverEffectSmooth : MonoBehaviour
{
    private Vector3 originalScale;
    public float scaleFactor = 1.5f;
    public float speed = 5f;
    private bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isHovered)
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * scaleFactor, Time.deltaTime * speed);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * speed);
    }

    void OnMouseEnter()
    {
        isHovered = true;
    }

    void OnMouseExit()
    {
        isHovered = false;
    }
}