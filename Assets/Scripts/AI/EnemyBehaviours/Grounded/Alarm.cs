using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    [SerializeField] private List<Turret> _connectedTurrets = new List<Turret>();
    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _animator.SetTrigger("Ring");
        if (!collision.CompareTag("Player")) { return; }
        foreach (var turret in _connectedTurrets)
        {
            if (turret != null) { turret.Activate(); }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _animator.SetTrigger("Idle");
        if (!collision.CompareTag("Player")) { return; }
        foreach (var turret in _connectedTurrets)
        {
            if (turret != null) { turret.Deactivate(); }
        }
    }

}
