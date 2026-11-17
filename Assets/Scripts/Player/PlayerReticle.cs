using UnityEngine;

public class PlayerReticle : MonoBehaviour, IPlayerActionComponent
{
    [Header("General")]
    [SerializeField] private GameObject _aimReticle;
    [SerializeField] private float _aimReticleSpeed = 3f;
    [SerializeField] private bool _stabilized = true;

    [Header("Distance from player")]
    [SerializeField] private float _aimReticleMinDistance = 1f;
    [SerializeField] private float _aimReticleMaxDistance = 10f;

    [Header("Distance from screen edges")]
    [SerializeField] private float _aimReticleScreenMargin = 1.5f;
    [SerializeField] private float _aimReticleKeepOnScreenSpeed = 5f;
    [SerializeField] private float _aimReticleKeepOnScreenSmoothTime = 1f;

    private PlayerActionsController _actionsController;
    private Transform _reticleTransform;
    private Vector3 _currentReticleVelocity;
    private float _offsetAngleDeg = -90f; // Default offset. Math shenanigans.

    /// <summary>
    /// Aims the reticle and gun.
    /// </summary>
    /// <param name="aim">The aiming direction or position.</param>
    /// <param name="targetTransform">The transform of the target to aim at.</param>
    public void Aim(Vector2 aim, Transform targetTransform)
    {
        AimReticle(aim);
        AimGun(targetTransform);
    }

    private void AimReticle(Vector2 aim)
    {
        float[] boundaries = MathUtils.ScreenBoundaries();
        _reticleTransform.Translate(_aimReticleSpeed * Time.deltaTime * aim);

        // Clamp the position of the reticle to the screen boundaries
        _reticleTransform.position = new Vector3(
            Mathf.Clamp(_reticleTransform.position.x, boundaries[0], boundaries[2]),
            Mathf.Clamp(_reticleTransform.position.y, boundaries[1], boundaries[3]),
            _reticleTransform.position.z);

        Vector2 aimDirection = _reticleTransform.position - transform.position;
        if (aimDirection != Vector2.zero)
        {
            if (Vector2.Distance(_reticleTransform.position, transform.position) < _aimReticleMinDistance)
            {
                _reticleTransform.position = (Vector2)transform.position + aimDirection.normalized * _aimReticleMinDistance; // Keep the reticle at a minimum distance from the player.
            }

            if (Vector2.Distance(_reticleTransform.position, transform.position) > _aimReticleMaxDistance)
            {
                _reticleTransform.position = (Vector2)transform.position + aimDirection.normalized * _aimReticleMaxDistance; // Keep the reticle from getting too far from the player.
            }
        }
    }

    /// <summary>
    /// Keeps the aim reticle within the screen boundaries and applies a smooth adjustment when it moves outside the
    /// configured margin.
    /// </summary>
    public void KeepReticleInScreen()
    {
        float[] screenBoundaries = MathUtils.ScreenBoundaries();

        // Keep the reticle in the screen boundaries with a margin
        float leftBoundary = screenBoundaries[0] + _aimReticleScreenMargin;
        float rightBoundary = screenBoundaries[2] - _aimReticleScreenMargin;
        float bottomBoundary = screenBoundaries[1] + _aimReticleScreenMargin;
        float topBoundary = screenBoundaries[3] - _aimReticleScreenMargin;

        Vector3 reticlePos = _reticleTransform.position;
        Vector3 playerDir = transform.position - reticlePos;

        if (reticlePos.x > rightBoundary || reticlePos.x < leftBoundary || reticlePos.y > topBoundary || reticlePos.y < bottomBoundary)
        {
            Vector3 adjustedPos = reticlePos + 1.4f * _aimReticleScreenMargin * playerDir.normalized;
            _reticleTransform.position = Vector3.SmoothDamp(reticlePos, adjustedPos, ref _currentReticleVelocity, _aimReticleKeepOnScreenSmoothTime, _aimReticleKeepOnScreenSpeed);
        }
    }

    private void AimGun(Transform targetTransform)
    {
        Vector2 aimDir;
        aimDir = _reticleTransform.position - targetTransform.position;
        targetTransform.eulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(aimDir.x, aimDir.y, _offsetAngleDeg)); // Rotate the gun to face the reticle.
    }

    public void Initialize(PlayerActionsController actionsController)
    {
        _actionsController = actionsController;
        _reticleTransform = _aimReticle.transform;
        if (_stabilized)
        {
            _reticleTransform.position = transform.position + (transform.right * _aimReticleMinDistance); // Set the reticle at a minimum distance from the player.
            _reticleTransform.SetParent(null); // Detach the reticle from the player so it doesn't follow the player's movements.
        }
    }
}
