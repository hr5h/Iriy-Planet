using UnityEngine;

public class OutlineRadius : MonoBehaviour
{
    public Material outline;
    public Material standart;
    [HideInInspector] public Collider2D collid;
    private void Awake()
    {
        collid = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Item obj))
        {
            obj.spriteRenderer.sharedMaterial = outline;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Item obj))
        {
            obj.spriteRenderer.sharedMaterial = standart;
        }
    }
}
