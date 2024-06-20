using UnityEngine;

public class ScopeUI : MonoBehaviour
{
    public Human player = null;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Camera mainCamera;
    public Sprite spr;
    private Transform _transform;
    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        _transform = GetComponent<Transform>();
    }

    void Update()
    {
        if (player != null)
        {
            if (player.weapon != null)
            {
                var cursor = mainCamera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                var shotPlace = player.weapon.shotPlace;
                var dist = Vector2.Distance(shotPlace.position, cursor);

                _transform.localScale = dist * Mathf.Tan(player.weapon.spreadCurrent * Mathf.Deg2Rad) * new Vector3(1, 1, 1) / 150;
                _transform.position = cursor;
                spriteRenderer.sprite = spr;
            }
            else
            {
                spriteRenderer.sprite = null;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
