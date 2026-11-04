using UnityEngine;


public interface IProjectile
{
    public void SetupProjectile(float speed, float damage, float lifetime, LayerMask impactLayerMask);
}
