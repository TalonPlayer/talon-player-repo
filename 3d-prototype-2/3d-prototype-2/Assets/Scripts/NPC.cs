using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI text;
    public NavMeshAgent agent;
    public Animator animator;
    public List<Transform> destinations;
    public Vector3 targetPos;
    public Transform body;
    public float range;
    public bool inRange;
    public bool isStationery;
    public bool randomIdle;
    private bool ragdolled = false;
    private bool follower = false;
    private BodyRigs rig;
    private CapsuleCollider cc;
    public UnityEvent onInit;
    void Start()
    {
        if (!isStationery) GetDestination();
        animator.SetFloat("Offset", Random.Range(0f, .5f));
        
        if (randomIdle)
        {
            animator.SetInteger("Random Idle", Random.Range(0, 3));
        }
        onInit?.Invoke();
        canvas.gameObject.SetActive(false);
        animator.applyRootMotion = true;
        cc = GetComponent<CapsuleCollider>();
        rig = GetComponent<BodyRigs>();
    }
    void Update()
    {
        if (ragdolled) return;
        if (canvas)
            canvas.transform.LookAt(2 * Camera.main.transform.position - transform.position);
        if (!isStationery)
        {
            if (agent.hasPath && agent.velocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
                body.rotation = Quaternion.Slerp(body.rotation, targetRotation, Time.deltaTime * 5f);
            }


            if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                animator.SetBool("IsWalking", true);

            }
            else if (agent.remainingDistance <= agent.stoppingDistance)
            {
                animator.SetBool("IsWalking", false);
                if (!follower)
                    GetDestination();
            }

            if (follower)
            {
                targetPos = GameManager.Instance.player.transform.position;
                agent.destination = targetPos;
            }
        }

        float distance = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);
        inRange = distance <= range;

        if (rig != null && inRange)
        {
            rig.headRig.weight = Mathf.Lerp(rig.headRig.weight, 1f, Time.deltaTime * 5f);
        }
        else
        {
            rig.headRig.weight = Mathf.Lerp(rig.headRig.weight, 0f, Time.deltaTime * 5f);
        }
    }
    public void GetDestination()
    {
        targetPos = destinations[Random.Range(0, destinations.Count)].position;
        agent.destination = targetPos;
        animator.SetBool("IsWalking", true);
        animator.SetFloat("Offset", Random.Range(0f, .5f));
        animator.applyRootMotion = false;
    }

    public void RagDoll()
    {
        animator.enabled = false;
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = false;
            r.gameObject.layer = LayerMask.NameToLayer("Ragdoll");
            body.transform.parent = transform.parent;

        }

        isStationery = true;
        agent.enabled = false;
        ragdolled = true;

        Destroy(gameObject);
    }

    public void FollowPlayer()
    {
        follower = true;
        animator.applyRootMotion = false;
        isStationery = false;
        targetPos = GameManager.Instance.player.transform.position;
        agent.destination = targetPos;
    }

    public void DisableText(float time)
    {
        StartCoroutine(DisableTextRoutine(time));
    }

    public IEnumerator DisableTextRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        canvas.gameObject.SetActive(false);
    }
    
}
