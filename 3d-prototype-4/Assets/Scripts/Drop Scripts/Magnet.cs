using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public Type type = Type.Magnet;
    private PhysicalDrop gem;
    private int numOfKills = 0;
    public float gravForce = 20f;
    void Start()
    {
        switch (type)
        {
            case Type.Magnet:
                break;
            case Type.Slowfield:
                Invoke(nameof(DelayDestroy), 25f);
                break;
            case Type.BlackHole:
                gem = PlayerManager.Instance.gem;
                StartCoroutine(DelayDestroy(30f));
                break;
        }
    }
    public void DelayDestroy()
    {
        Destroy(transform.parent.gameObject);
    }
    public IEnumerator DelayDestroy(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        for (int i = 0; i < numOfKills; i++)
        {
            PhysicalDrop g = Instantiate(gem, transform.position, Quaternion.identity);
            g.LaunchUp();
            yield return new WaitForSeconds(.1f);
        }

        yield return new WaitForSeconds(1f);
        Destroy(transform.parent.gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        switch (type)
        {
            case Type.Magnet:
                if (other.CompareTag("Drop"))
                {
                    DropObject drop = other.GetComponent<DropObject>();

                    drop.moveSpeed = 5f;
                    drop.MoveTo(transform);
                }
                break;
            case Type.Slowfield:
                if (other.CompareTag("Enemy"))
                {
                    EnemyMovement enemy = other.GetComponent<EnemyMovement>();
                    enemy.AlterSpeed(.15f);
                }
                break;
            case Type.BlackHole:
                if (other.CompareTag("Enemy"))
                {
                    Enemy enemy = other.GetComponent<Enemy>();

                    if (enemy)
                    {
                        enemy.OnDeath();
                        GlobalSaveSystem.AddAchievementProgress("Black Hole_kills", 1);
                        if (Random.Range(0, 8) == 0) numOfKills++;
                        gravForce += .25f;
                        transform.localScale += Vector3.one * .03f;
                    }
                }

                if (other.CompareTag("Drop"))
                {
                    DropObject drop = other.GetComponent<DropObject>();

                    drop.moveSpeed = 5f;
                    drop.MoveTo(transform);

                    if (Random.Range(0, 5) == 0) numOfKills++;
                    gravForce += .1f;
                    transform.localScale += Vector3.one * .01f;
                }
                break;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        switch (type)
        {
            case Type.BlackHole:
                if (other.CompareTag("Drop"))
                {
                    if ((transform.position - other.transform.position).magnitude <= .45f)
                        Destroy(other.gameObject);
                    break;
                }
                if (other.gameObject.layer != LayerMask.NameToLayer("Ragdoll")) return;
                if (other.transform.root.CompareTag("Player") || other.transform.root.CompareTag("Unit")) return;
                Rigidbody rb = other.attachedRigidbody;
                if (rb == null) return;

                Vector3 dir = transform.position - rb.position;
                if (rb.name == "Knee.R" || rb.name == "Knee.L" || rb.name == "Leg.L" || rb.name == "Leg.R")
                {
                    rb.AddForce(dir.normalized * gravForce, ForceMode.Acceleration);
                }
                if (rb.transform.localScale.x >= 0.125f)
                {
                    rb.transform.localScale -= (Vector3.forward + Vector3.right) * Time.deltaTime * .15f;
                    rb.transform.localScale += Vector3.up * Time.deltaTime * .15f;
                }
                if (dir.magnitude <= 1f)
                {
                    EntityManager.Instance.RecycleRagdolls(8, 8);
                }


                break;


        }

    }
    public enum Type
    {
        Magnet,
        Slowfield,
        BlackHole
    }
}
