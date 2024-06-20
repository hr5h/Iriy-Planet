using UnityEngine;
using UnityEngine.Events;

public class BulletSensor : MonoBehaviour
{
    public UnityEvent<Bullet, Collider2D> BulletDetected;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Bullet obj))
        {
            //Logger.Debug("Bullet");
            BulletDetected?.Invoke(obj, collision);
        }
    }
}
