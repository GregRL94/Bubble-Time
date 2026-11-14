using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerDash : MonoBehaviour, IPlayerMovementComponent
{
    [SerializeField] private float _dashSpeedFactor  = 3.0f;
    [SerializeField] private float _dashDuration = 0.5f;
    [SerializeField] private float _dashCooldown = 2.0f;

    private PlayerMovementController _movementController;
    private AfterImageEffect _afterImageEffect;
    private Coroutine _dashCoroutine;
    private float _dashCooldownTimer;


    public void Initialize(PlayerMovementController movementController)        
    {
        _movementController = movementController;
        TryGetComponent<AfterImageEffect>(out _afterImageEffect);
    }

    public void UpdateDash()
    {
        if (_dashCooldownTimer < _dashCooldown)
        {
            _dashCooldownTimer += Time.deltaTime;
        }
    }

    public void OnDash()
    {
        if (_dashCooldownTimer >= _dashCooldown && !_movementController.IsDashing)
        {
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
            }
            _dashCoroutine = StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        float startTime = Time.time;
        _movementController.IsDashing = true;
        _movementController.PlayerAnimator.SetTrigger("Dash");

        // AfterImageEffect
        _afterImageEffect?.PlayDashEffect(_dashDuration);

        // Génère une impulsion de caméra au début du dash si un CinemachineImpulseSource est attaché
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
        {
            impulseSource.GenerateImpulse();
        }

        _movementController.CurrentSpeed = _movementController.WalkSpeed * _dashSpeedFactor;
        while (Time.time < startTime + _dashDuration)
        {
            yield return null;
        }
        _movementController.CurrentSpeed = _movementController.WalkSpeed;
        _dashCooldownTimer = 0f;
        _movementController.IsDashing = false;
        yield break;
    }
}
