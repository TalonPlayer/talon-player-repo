using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
public class EntityBody : MonoBehaviour
{
    public Animator animator;
    public Animator thisAnimator;
    public Transform body;
    public Transform hand;
    public Transform otherHand;

    [Header("Body Parts")]
    public Transform bandana;
    public Transform face, hat, gloveR, gloveL, shoeR, shoeL, belt;
    [Header("Renderers")]
    public SkinnedMeshRenderer skin;
    public List<Clothing> clothes;
    public bool itemDropGravity = true;
    public List<GameObject> heldItems;
    private Entity entity;
    private EntityBrain brain;
    private List<Renderer> renderers;
    void Awake()
    {
        entity = GetComponentInParent<Entity>();
        brain = GetComponentInParent<EntityBrain>();
    }
    void Start()
    {
        if (brain.isHuman) skin.material.color = Helper.GetColor("#004DFF");
        else skin.material.color = Helper.GetColor("#35B900");
    }

    public void SetClothes(EntityObj info)
    {
        for (int i = 0; i < clothes.Count; i++)
            SetClothing(info, i);
    }

    public void SetClothing(EntityObj info, int index)
    {
        Clothing clothing = clothes[index];
        if (clothing)
        {
            if (clothing.hasPrint)
                clothing.SetPrint(info.shirtTexture);
            clothing.SetPrimary(info.primaryColors[index]);
            clothing.SetSecondary(info.secondaryColors[index]);
        }

        clothing.SetClothing();
    }
    public void SetMaterialOpacity(Renderer renderer, float opacity)
    {
        if (renderer == null) return;
        foreach (Material mat in renderer.materials)
        {
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_AlphaClip", 0f);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.SetFloat("_Blend", 1f);
            mat.SetInt("_SrcBlend", (int)BlendMode.One);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)RenderQueue.Transparent;
            mat.SetFloat("_ZWriteControl", 2f);
            mat.SetInt("_ZWrite", 0);
            if (mat.HasProperty("_BaseColor"))
            {
                Color c = mat.GetColor("_BaseColor");
                c.a = opacity;
                mat.SetColor("_BaseColor", c);
            }
            else if (mat.HasProperty("_Color"))
            {
                var c = mat.color;
                c.a = opacity;
                mat.color = c;
            }
        }
    }

    public void OnDeath()
    {
        renderers = GetComponentsInChildren<Renderer>().ToList();
        body.transform.parent = MyEntityManager.Instance.ragdollFolder;
        StartCoroutine(FadeRoutine(4f, 1.75f));
    }

    public IEnumerator FadeRoutine(float fadeTime, float delay = 0f)
    {

        DropAllItems();
        PlayRandom("IsDead", 6, true);

        foreach (Renderer renderer in renderers)
            renderer.shadowCastingMode = ShadowCastingMode.Off;

        yield return new WaitForSeconds(2f);

        RagDoll();

        yield return new WaitForEndOfFrame();

        thisAnimator.SetTrigger("Sink");

        yield return new WaitForEndOfFrame();

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;


        foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
            r.isKinematic = true;

        float start = 1f;
        float elapsed = 0f;

        yield return new WaitForEndOfFrame();

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;

            float opacity = Mathf.Lerp(start, 0f, t);
            foreach (Renderer renderer in renderers)
                SetMaterialOpacity(renderer, opacity);

            yield return null;
        }


        Destroy(gameObject);

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
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = false;
            r.velocity = Vector3.zero;
        }
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

            item.transform.parent = MyEntityManager.Instance.itemFolder;
            item.transform.localScale = item.transform.lossyScale;

            // Item is now affected by gravity and not attached to body
            Collider coll = item.GetComponent<Collider>();
            if (!coll) return;

            coll.enabled = true;
            Rigidbody rb = coll.attachedRigidbody;

            if (!rb) return;

            rb.useGravity = itemDropGravity;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;

            // Launches items in an upward arc between 30 and 80 degrees
            Vector3 direction = Helper.RandomDirection(30f, 80f);
            Vector3 torque = Helper.ApplyTorque(direction, 15f);
            rb.AddForce(direction * 2f, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }
    }
    /// <summary>
    /// Drops a random amount of items
    /// </summary>
    private void DropItemsRandom()
    {
        heldItems = Helper.ShuffleList(heldItems);

        int rand = Random.Range(0, heldItems.Count + 1);
        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < rand; i++)
        {
            temp.Add(heldItems[i]);
        }

        foreach (GameObject item in heldItems)
        {
            // creates a new item and puts it into the item folder

            item.transform.parent = MyEntityManager.Instance.itemFolder;
            item.transform.localScale = item.transform.lossyScale;

            // Item is now affected by gravity and not attached to body
            Collider coll = item.GetComponent<Collider>();
            if (!coll) return;

            coll.enabled = true;
            Rigidbody rb = coll.attachedRigidbody;

            if (!rb) return;

            rb.useGravity = itemDropGravity;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;

            // Launches items in an upward arc between 30 and 80 degrees
            Vector3 direction = Helper.RandomDirection(30f, 80f);
            Vector3 torque = Helper.ApplyTorque(direction, 15f);
            rb.AddForce(direction * 2f, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }
    }
}
