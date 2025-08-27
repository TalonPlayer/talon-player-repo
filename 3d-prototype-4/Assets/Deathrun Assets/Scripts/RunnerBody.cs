using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerBody : MonoBehaviour
{
    public Animator animator;
    public Transform body;
    public Renderer skin;
    public List<Renderer> eyes;
    public bool itemDropGravity = true;
    public int deathAnims;
    public int walkAnims;
    public int runningAnims;
    public int attackAnims;
    public int idleAnims;
    public float impulseStrength = 350f;
    private Runner runner;
    public List<GameObject> heldItems;
    private Rigidbody rootRb;
    private Color color;

    void Awake()
    {
        runner = GetComponent<Runner>();
    }
    void Start()
    {
        color = GetRandomColor();
        skin.material.color = color;
        Rigidbody[] rbs = body.GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < rbs.Length; i++)
        {
            if (rbs[i].name == "Root")
            {
                rootRb = rbs[i];
                break;
            }
        }
    }

    public Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    #region Animations
    /// <summary>
    /// Sets float parameter
    /// </summary>
    /// <param name="para"></param>
    /// <param name="val"></param>
    public void Play(string para, float val)
    {
        animator.SetFloat(para, val);
    }

    /// <summary>
    /// Sets integer parameter
    /// </summary>
    /// <param name="para"></param>
    /// <param name="val"></param>
    public void Play(string para, int val)
    {
        animator.SetInteger(para, val);
    }

    /// <summary>
    /// Sets boolean parameter
    /// </summary>
    /// <param name="para"></param>
    /// <param name="val"></param>
    public void Play(string para, bool val)
    {
        animator.SetBool(para, val);
    }

    /// <summary>
    /// Plays a trigger
    /// </summary>
    /// <param name="para"></param>
    public void Play(string para)
    {
        animator.SetTrigger(para);
    }

    /// <summary>
    /// Sets a trigger parameter and plays a random animation
    /// </summary>
    /// <param name="para">Parameter Trigger</param>
    /// <param name="count">Num of animations</param>
    public void PlayRandom(string para, int count)
    {
        Play("RandomInt", Random.Range(0, count));
        Play(para);
    }
    /// <summary>
    /// Sets a boolean parameter and plays a random animation
    /// </summary>
    /// <param name="para">Parameter of Boolean</param>
    /// <param name="count">Num of animations</param>
    /// <param name="active">Sets the boolean parameter</param>
    public void PlayRandom(string para, int count, bool active)
    {
        Play("RandomInt", Random.Range(0, count));
        Play(para, active);
    }
    #endregion
    /// <summary>
    /// Ragdoll the body
    /// </summary>
    public void RagDoll()
    {
        // Disable animator
        animator.enabled = false;

        // For each body of the part that has a joint, make them non kinematic and parent
        // the body to the ragdoll folder
        Rigidbody[] rigidbodies = body.GetComponentsInChildren<Rigidbody>();
        body.transform.parent = RunnerManager.Instance.ragdollFolder;
        runner.rb.isKinematic = false;
        foreach (Rigidbody r in rigidbodies)
            r.isKinematic = false;
    }
    public void CloseEyes()
    {
        foreach (Renderer eye in eyes)
            eye.material.color = color;
    }
    public void PushRagdoll(Vector3 direction)
    {
        // Ensure valid direction
        if (direction.sqrMagnitude > 1e-6f) direction.Normalize();
        else direction = transform.forward;

        // Switch to ragdoll first
        RagDoll();

        // Find the specific RB named "Root"
        Rigidbody rootRb = null;
        Rigidbody[] rbs = body.GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < rbs.Length; i++)
        {
            if (rbs[i].name == "Root")
            {
                rootRb = rbs[i];
                break;
            }
        }
        if (rootRb == null && rbs.Length > 0)
            rootRb = rbs[0]; // fallback to something reasonable

        Vector3 impulse = direction * impulseStrength;
        // Optional: tiny upward lift so it reads better
        // impulse += Vector3.up * 2f;

        if (rootRb != null)
        {
            rootRb.AddForce(impulse, ForceMode.Impulse);
        }
    }

    public void UnRagdoll()
    {
        // For each body of the part that has a joint, make them non kinematic and parent
        // the body to the ragdoll folder
        Rigidbody[] rigidbodies = body.GetComponentsInChildren<Rigidbody>();
        body.SetParent(runner.transform, true);
        body.position = Vector3.down;
        Vector3 localBodyOffset = new Vector3(0f, -1f, 0f);
        runner.transform.position = rootRb.position - runner.transform.rotation * localBodyOffset;

        body.localPosition = localBodyOffset;
        body.rotation = transform.rotation;

        runner.rb.isKinematic = true;
        foreach (Rigidbody r in rigidbodies)
            r.isKinematic = true;
        animator.enabled = true;

    }
    /// <summary>
    /// Delay Ragdoll
    /// </summary>
    /// <param name="time"></param>
    public void DelayRagdoll(float time)
    {
        Invoke(nameof(RagDoll), time);
    }

    /// <summary>
    /// Delay the drop of random items
    /// </summary>
    /// <param name="time"></param>
    public void DelayDropRandom(float time)
    {
        Invoke(nameof(DropItemsRandom), time);
    }

    /// <summary>
    /// Delay the drop of all items
    /// </summary>
    /// <param name="time"></param>
    public void DelayDropAll(float time)
    {
        Invoke(nameof(DropAllItems), time);
    }

    /// <summary>
    /// Drops all the items the enemy is holding
    /// </summary>
    private void DropAllItems()
    {
        foreach (GameObject item in heldItems)
        {
            // creates a new item and puts it into the item folder
            item.transform.parent = RunnerManager.Instance.itemFolder;
            item.transform.localScale = item.transform.lossyScale;

            // Item is now affected by gravity and not attached to body
            Rigidbody rb = item.GetComponent<Rigidbody>();
            BoxCollider bc = item.GetComponent<BoxCollider>();
            SphereCollider sc = item.GetComponent<SphereCollider>();

            rb.useGravity = itemDropGravity;
            rb.constraints = RigidbodyConstraints.None;

            if (bc) bc.enabled = true;
            if (sc) sc.enabled = true;

            // Launches items in an upward arc between 30 and 80 degrees
            Vector3 direction = RandExt.RandomDirection(30f, 80f); 
            Vector3 torque = RandExt.ApplyTorque(direction, 15f);
            rb.AddForce(direction * 2f, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }
    }
    /// <summary>
    /// Drops a random amount of items
    /// </summary>
    private void DropItemsRandom()
    {
        heldItems = RandExt.ShuffleList(heldItems);

        int rand = Random.Range(0, heldItems.Count + 1);
        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < rand; i++)
        {
            temp.Add(heldItems[i]);
        }

        foreach (GameObject item in temp)
        {
            item.transform.parent = RunnerManager.Instance.itemFolder;
            item.transform.localScale = item.transform.lossyScale;
            Rigidbody rb = item.GetComponent<Rigidbody>();
            BoxCollider bc = item.GetComponent<BoxCollider>();
            SphereCollider sc = item.GetComponent<SphereCollider>();

            item.SetActive(false);

            rb.useGravity = itemDropGravity;
            rb.constraints = RigidbodyConstraints.None;

            if (bc) bc.enabled = true;
            if (sc) sc.enabled = true;
            Vector3 direction = RandExt.RandomDirection(30f, 80f); // Upward arc between 30° and 80°
            Vector3 torque = RandExt.ApplyTorque(direction, 15f);
            rb.AddForce(direction * 2f, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }
    }
}
