using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationEvent : MonoBehaviour
{
    public Unit main;

    public void OnMeleeStart()
    {
        main.movement.MoveTo(main.TargetPos);
        main.movement.agent.speed = main.movement.sprintSpeed;
    }

    public void OnMelee()
    {
        main.combat.Melee();
    }

    public void OnMeleeFinish()
    {
        main.movement.agent.speed = main.movement.DefaultSpeed;

    }

    public void StopMovement()
    {
        main.movement.ToggleMovement(false);
    }

    public void ResumeMovement()
    {
        main.movement.ToggleMovement(true);
    }

    public void FinishReload()
    {
        main.combat.FinishReload();
    }

    public void Ragdoll()
    {
        main.body.RagDoll();
        main.coll.enabled = false;

    }

    public void DropHand()
    {
        Weapon.DropHand(main.combat.inHand);
    }
    public void RotateCover(float rotationTime)
    {
        StartCoroutine(RotateRoutine(main.movement.coverPoint.Rotation, rotationTime));
    }

    IEnumerator RotateRoutine(Quaternion rot, float rotationTime)
    {
        float t = 0f;
        Quaternion startRot = transform.rotation;
        while (t < 1f)
        {
            main.transform.rotation = Quaternion.Lerp(startRot, rot, t);
            t += Time.deltaTime / rotationTime;

            yield return null;
        }
    }

    IEnumerator FollowRotationRoutine(Transform target, float rotationTime)
    {
        float t = 0f;
        while (t < 1f)
        {
            Vector3 toTarget = target.transform.position - main.transform.position;
            toTarget.y = 0f;
            toTarget.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(toTarget);
            main.transform.rotation = Quaternion.Lerp(main.transform.rotation, targetRotation, t);
            t += Time.deltaTime / rotationTime;

            yield return null;
        }
    }

    IEnumerator FollowRotationRoutine(float rotationTime)
    {
        float t = 0f;
        while (t < 1f)
        {
            main.movement.RotateToTarget();

            yield return null;
        }
    }


    public void Shoot()
    {
        main.combat.Fire();
    }

    public void CoverToShoot()
    {

        main.body.Play("InCover", false);

        float duration = Random.Range(2f, 3f);
        StartCoroutine(CoverPeeking(duration));
    }

    IEnumerator CoverPeeking(float duration)
    {
        yield return new WaitForSeconds(.3f);
        StartCoroutine(FollowRotationRoutine(duration - .3f));

        CoverPoint cover = main.movement.coverPoint;
        main.movement.MoveTo(cover.peekLocation.position);
        yield return new WaitForSeconds(duration);
        main.body.Play("InCover", true);
        string side = cover.coverDirection == CoverDirection.Right ? "CoverRight" : "CoverLeft";
        main.body.Play(side);
        ShootToCover();
    }
    public void ShootToCover()
    {
        main.movement.MoveTo(main.movement.coverPoint.transform.position);
    }

    public void Reload()
    {
        main.body.animator.SetLayerWeight(2, 1f);
    }
}
