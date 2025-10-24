using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class EnemyBody : MonoBehaviour
{
    public Animator animator;
    public Transform body;
    public bool itemDropGravity = true;
    private Enemy enemy;
    public List<GameObject> heldItems;
    public SkinnedMeshRenderer smr;
    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
    void Start()
    {
    }
    public void DelayOnDeath(float time)
    {
        Invoke(nameof(OnDeath), time);
    }
    public void OnDeath()
    {
        Material mat = smr.material;
        mat.SetFloat("_Surface", 1f);
        mat.SetFloat("_AlphaClip", 0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.SetFloat("_Blend", 1f);
        mat.SetInt("_SrcBlend", (int)BlendMode.One);
        mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int) RenderQueue.Transparent;
        mat.SetFloat("_ZWriteControl", 2f);
        mat.SetInt("_ZWrite", 0);
        if (mat.HasProperty("_BaseColor"))
        {
            Color c = mat.GetColor("_BaseColor");
            c.a = .3f;
            mat.SetColor("_BaseColor", c);
        }
        else if (mat.HasProperty("_Color"))
        {
            var c = mat.color;
            c.a = .3f;
            mat.color = c;
        }
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
        body.transform.parent = EntityManager.Instance.ragdollFolder;
        enemy.rb.isKinematic = false;
        foreach (Rigidbody r in rigidbodies)
            r.isKinematic = false;
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
            GameObject newItem = Instantiate(item, item.transform.position, Quaternion.identity,
            EntityManager.Instance.itemFolder);
            newItem.transform.localScale = item.transform.lossyScale;

            // Item is now affected by gravity and not attached to body
            Rigidbody rb = newItem.GetComponent<Rigidbody>();
            BoxCollider bc = newItem.GetComponent<BoxCollider>();
            SphereCollider sc = newItem.GetComponent<SphereCollider>();

            item.SetActive(false);

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
            GameObject newItem = Instantiate(item, item.transform.position, Quaternion.identity,
            EntityManager.Instance.itemFolder);
            newItem.transform.localScale = item.transform.lossyScale;
            Rigidbody rb = newItem.GetComponent<Rigidbody>();
            BoxCollider bc = newItem.GetComponent<BoxCollider>();
            SphereCollider sc = newItem.GetComponent<SphereCollider>();

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
