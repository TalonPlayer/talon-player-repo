using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    Vector3 direction;
    public float lifeTime = .15f;
    public float speed = 55f;
    public void Launch(Vector3 dir)
    {
        direction = dir.normalized;
        StartCoroutine(LifeTime());
    }
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
    public virtual IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
