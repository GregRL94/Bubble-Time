using UnityEngine;

public class DamageByContact : MonoBehaviour
{
    [SerializeField] private LayerMask _eligibleLayers;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("DamageByContact: OnTriggerEnter2D with " + collider.gameObject.name);
        if (!(((1 << collider.gameObject.layer) & _eligibleLayers) != 0)) { return; }
        if (collider.gameObject.TryGetComponent(out IHit hit))
        {
            hit.OnHit();
        }
    }
}
