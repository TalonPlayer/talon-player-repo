using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum OffMeshLinkMoveMethod
{
    Teleport,
    NormalSpeed,
    Parabola,
    Curve
}

[RequireComponent(typeof(NavMeshAgent))]
public class AgentLinkMover : MonoBehaviour
{
    public OffMeshLinkMoveMethod m_Method = OffMeshLinkMoveMethod.Parabola;
    public AnimationCurve m_Curve = new AnimationCurve();
    public EnemyBody eBody;
    public UnitBody uBody;
    private NavMeshAgent agent;
    IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;

        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                if (m_Method == OffMeshLinkMoveMethod.NormalSpeed)
                {
                    // Rough estimate based on distance and speed
                    float t = Vector3.Distance(agent.transform.position,
                            agent.currentOffMeshLinkData.endPos) / Mathf.Max(0.01f, agent.speed);
                    ReserveEntranceForCurrentLink(Mathf.Clamp(t, 0.25f, 2.0f));
                    yield return StartCoroutine(NormalSpeed(agent));
                }
                else if (m_Method == OffMeshLinkMoveMethod.Parabola)
                {
                    ReserveEntranceForCurrentLink(0.5f);
                    yield return StartCoroutine(Parabola(agent, 2.0f, 0.5f));
                }
                else if (m_Method == OffMeshLinkMoveMethod.Curve)
                {
                    float duration = 0.5f;
                    ReserveEntranceForCurrentLink(duration);
                    yield return StartCoroutine(Curve(agent, duration));
                }

                agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }


    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        if (eBody) eBody.Play("Jump");
        if (uBody) uBody.Play("Jump");
        yield return new WaitForSeconds(0.35f);
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
    }


    IEnumerator Curve(NavMeshAgent agent, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = m_Curve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }

    // In AgentLinkMover
    JumpReserveObstacle FindNearbyBlocker(Vector3 pos, float radius = 1.2f)
    {
        var hits = Physics.OverlapSphere(pos, radius);
        foreach (var h in hits)
        {
            var blocker = h.GetComponentInParent<JumpReserveObstacle>();
            if (blocker) return blocker;
        }
        return null;
    }

    void ReserveEntranceForCurrentLink(float seconds)
    {
        var data = agent.currentOffMeshLinkData;
        // Prefer the start position so approaching agents get repelled
        var blocker = FindNearbyBlocker(data.startPos);
        if (!blocker)
            blocker = FindNearbyBlocker(data.endPos); // fallback if you only placed one on the far side

        if (blocker)
            blocker.Reserve(seconds + .25f); // small buffer
    }

}