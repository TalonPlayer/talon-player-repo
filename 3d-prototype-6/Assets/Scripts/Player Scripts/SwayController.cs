using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayController : MonoBehaviour
{
    [SerializeField] private PlayerMovement move;
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private HeadBobController bob;
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    public float smooth = 10f;
    public float stepRot = 0.01f;
    public float maxRotStep = 0.06f;
    public float smoothRot = 12f;
    private Vector3 swayPos;
    private Vector3 swayRot;
    void Update()
    {
        CompositeSway();

        Sway();
        SwayRotation();
    }

    private void CompositeSway()
    {
        Vector3 recoil = Vector3.zero; recoil.y = move.recoilPitchOffset * step / 2f;
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bob.bobMotion + recoil, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayRot + bob.bobMotion), Time.deltaTime * smoothRot);
    }

    private void Sway()
    {

        if (combat.isAiming) 
        {
            swayPos = Vector3.zero;
            return;
        }

        Vector3 invertLook = move.lookInput * -step;

        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance / 2f, maxStepDistance / 2f);

        swayPos = invertLook;
    }

    private void SwayRotation()
    {

        if (combat.isAiming) 
        {
            swayRot = Vector3.zero;
            return;
        }
        Vector3 invertLook = move.lookInput * -stepRot;

        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotStep, maxRotStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotStep, maxRotStep);

        swayRot = new Vector3(invertLook.y + move.recoilPitchOffset, invertLook.x, invertLook.x);
    }
}
