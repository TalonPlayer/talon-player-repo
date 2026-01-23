using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
    [Header("Weapon Effects")]
    [Tooltip("For Player")] public RuntimeAnimatorController weaponAnimator;
    [Tooltip("For Unit")] public RuntimeAnimatorController unitWeaponAnimator;
    public Transform barrelPoint;
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactParticles;
    public TrailRenderer bulletTrail;
    public GameObject magazine;
    public Collider coll;
    public Rigidbody rb;

    public IEnumerator SpawnTrail(RaycastHit hit, Action onHit)
    {
        TrailRenderer trail = Instantiate(bulletTrail, barrelPoint.position, Quaternion.identity);
        trail.Clear();
        float t = 0;
        Vector3 startPosition = barrelPoint.position;

        while (t < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, t);
            t += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.Clear();
        Destroy(trail.gameObject);
        Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
        onHit?.Invoke();
    }

    public IEnumerator SpawnTrail(Vector3 hit)
    {
        TrailRenderer trail = Instantiate(bulletTrail, barrelPoint.position, Quaternion.identity);

        float t = 0;
        Vector3 startPosition = barrelPoint.position;
        trail.Clear();
        while (t < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit, t);
            t += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.Clear();
        Destroy(trail.gameObject);
        Instantiate(impactParticles, hit, Quaternion.LookRotation(hit));
    }

    public void PlayerPickUp(Player player)
    {
        Weapon wpn = GetComponent<Weapon>();
        wpn.fx.coll.GetComponent<Outline>().SetOutlineActive(false);

        RuntimeWeapon weapon = new RuntimeWeapon(wpn);

        player.combat.Equip(weapon);

        Destroy(gameObject);
    }

    // public int poolAmount = 20;
    // private Queue<TrailRenderer> pool;
    /*
    public void Init()
    {
        pool = new Queue<TrailRenderer>();
        for (int i = 0; i < poolAmount; i++)
        {
            TrailRenderer t = Instantiate(bulletTrail, barrelPoint.position, Quaternion.identity, barrelPoint);
            pool.Enqueue(t);
            t.gameObject.SetActive(false);
        }
    }
    private TrailRenderer GetAvailableTrail()
    {
        return pool.Dequeue();
    }
    */

}
