using UnityEngine;

public class Spray : Projectile
{
    public SprayFragment fragmentPrefab;
    public int fragmentCount = 5;
    public float spreadAngle = 15f;

    public override void Launch(Vector3 dir)
    {
        base.Launch(dir);
        SpawnFragments();
    }

    /// <summary>
    /// Spawn Fragments in an offset random direction
    /// </summary>
    void SpawnFragments()
    {
        for (int i = 0; i < fragmentCount; i++)
        {
            // Calculate random spray direction
            Quaternion rotation = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0f
            );
            Vector3 sprayDir = rotation * direction;

            // Instantiate fragment
            SprayFragment frag = Instantiate(fragmentPrefab, transform.position, Quaternion.LookRotation(sprayDir), PlayerManager.Instance.bulletFolder);

            frag.Launch(sprayDir);
            frag.collateral = collateral;
            frag.damage = owner.hand.damage;
        }
    }
}
