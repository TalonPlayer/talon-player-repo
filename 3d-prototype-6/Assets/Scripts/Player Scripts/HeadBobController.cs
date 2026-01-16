using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool _enable = true;
    [SerializeField, Range(0, 0.1f)] private float _walkAmplitude = 0.003f;
    [SerializeField, Range(0, 30f)] private float _walkFrequency = 14.0f;
    [SerializeField, Range(0, 0.1f)] private float _sprintAmplitude = 0.05f;
    [SerializeField, Range(0, 30f)] private float _sprintFrequency = 18.0f;
    [SerializeField] private Transform _cameraHolder;
    public Vector3 bobMotion;
    private float _toggleSpeed = 3.0f;
    private Vector3 _startPos;
    private float _amplitude;
    private float _frequency;
    private CharacterController _controller;
    private PlayerMovement move;
    private PlayerCombat combat;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        move = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        _startPos = _cameraHolder.localPosition;
    }

    void Update()
    {
        if (!_enable) return;

        EvaluateMotion();

        CheckMotion();

        ResetPosition();

        _cameraHolder.LookAt(FocusTarget());
    }

    private void EvaluateMotion()
    {
        _startPos.y = move.targetCamLevel;

        switch (move.position)
        {
            case PlayerMovement.PositionState.Stand:
                if (move.isSprinting)
                {
                    _amplitude = _sprintAmplitude;
                    _frequency = _sprintFrequency;
                }
                else
                {
                    _amplitude = _walkAmplitude;
                    _frequency = _walkFrequency;
                }
                break;
            case PlayerMovement.PositionState.Crouch:
                _amplitude = _walkAmplitude;
                _frequency = _walkFrequency;
                break;
            case PlayerMovement.PositionState.Prone:
                _amplitude = _walkAmplitude;
                _frequency = _walkFrequency;
                break;
        }
    }

    private void PlayMotion(Vector3 motion)
    {

        if (combat.isAiming)
            bobMotion = Vector3.zero;
        else
            bobMotion = motion;
        _cameraHolder.localPosition += motion;
    }

    private void CheckMotion()
    {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

        if (speed < _toggleSpeed) return;
        if (!_controller.isGrounded) return;

        PlayMotion(FootStepMotion());
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;

        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude * 2;

        return pos;
    }

    private void ResetPosition()
    {
        if (_cameraHolder.localPosition == _startPos) return;

        _cameraHolder.localPosition = Vector3.Lerp(
            _cameraHolder.localPosition,
            _startPos,
            1 * Time.deltaTime
        );
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(
            transform.position.x,
            transform.position.y + _cameraHolder.localPosition.y,
            transform.position.z
        );

        pos += _cameraHolder.forward * 15.0f;

        return pos;
    }
}
