using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class PlayerActionsController : MonoBehaviour
{
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private GameObject _aimReticle;
    [SerializeField] private float _aimReticleSpeed = 3f;
    [SerializeField] private float _aimReticleMinDistance = 1f;
    [SerializeField] private float _aimReticleMaxDistance = 10f;
    [SerializeField] private float _aimReticleScreenMargin = 1.5f;
    [SerializeField] private float _aimReticleKeepOnScreenSpeed = 5f;
    [SerializeField] private float _aimReticleKeepOnScreenSmoothTime = 1f;
    private PlayerInputHandler _inputHandler;
    private Vector3 _currentReticleVelocity;
    private float _offsetAngleDeg = -90f; // Décalage de l'angle pour que le gun soit orienté correctement


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputHandler = PlayerInputHandler.Instance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 aimInput = _inputHandler.GetLookInput();
        AimReticle(aimInput);
    }

    private void LateUpdate()
    {        
        KeepReticleInScreen(_aimReticleScreenMargin);
    }

    private void AimReticle(Vector2 aim)
    {
        float[] boundaries = MathUtils.ScreenBoundaries();
        _aimReticle.transform.Translate(_aimReticleSpeed * Time.deltaTime * aim);

        // Clamp la position du viseur aux limites de l'ecran
        _aimReticle.transform.position = new Vector3(
            Mathf.Clamp(_aimReticle.transform.position.x, boundaries[0], boundaries[2]),
            Mathf.Clamp(_aimReticle.transform.position.y, boundaries[1], boundaries[3]),
            _aimReticle.transform.position.z);

        Vector2 aimDirection = _aimReticle.transform.position - transform.position;
        if (aimDirection != Vector2.zero)
        {
            if (Vector2.Distance(_aimReticle.transform.position, transform.position) < _aimReticleMinDistance)
            {
                _aimReticle.transform.position = (Vector2)transform.position + aimDirection.normalized * _aimReticleMinDistance; // Empêche le viseur de se rapprocher trop du joueur.
            }

            if (Vector2.Distance(_aimReticle.transform.position, transform.position) > _aimReticleMaxDistance)
            {
                _aimReticle.transform.position = (Vector2)transform.position + aimDirection.normalized * _aimReticleMaxDistance; // Empêche le viseur de s'éloigner trop du joueur.
            }
            Aim(aimDirection);
        }
    }

    private void KeepReticleInScreen(float margin)
    {
        float[] screenBoundaries = MathUtils.ScreenBoundaries();

        float leftBoundary = screenBoundaries[0] + margin;
        float rightBoundary = screenBoundaries[2] - margin;
        float bottomBoundary = screenBoundaries[1] + margin;
        float topBoundary = screenBoundaries[3] - margin;

        Vector3 reticlePos = _aimReticle.transform.position;
        Vector3 playerDir = transform.position - reticlePos;

        if (reticlePos.x > rightBoundary || reticlePos.x < leftBoundary || reticlePos.y > topBoundary || reticlePos.y < bottomBoundary)
        {
            Vector3 adjustedPos = reticlePos + 1.4f * margin * playerDir.normalized;
            _aimReticle.transform.position = Vector3.SmoothDamp(reticlePos, adjustedPos, ref _currentReticleVelocity, _aimReticleKeepOnScreenSmoothTime, _aimReticleKeepOnScreenSpeed);
        }
    }

    private void Aim(Vector2 aimDirection)
    {
        Vector2 aimDir;
        aimDir = _aimReticle.transform.position - _shootingPoint.position;
        _shootingPoint.transform.eulerAngles = new Vector3(0f, 0f, MathUtils.DirToAngleRad(aimDir.x, aimDir.y, _offsetAngleDeg)); // Rotation du gun selon la direction de visee
    }
}
