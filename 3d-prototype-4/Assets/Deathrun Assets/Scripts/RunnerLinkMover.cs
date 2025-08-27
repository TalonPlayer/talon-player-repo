using UnityEngine;
using UnityEngine.AI;
using System.Collections;



[RequireComponent(typeof(NavMeshAgent))]
public class RunnerLinkMover : MonoBehaviour
{
    public OffMeshLinkMoveMethod m_Method = OffMeshLinkMoveMethod.Parabola;
    public AnimationCurve m_Curve = new AnimationCurve();
    public Runner runner;
    private NavMeshAgent agent;
    IEnumerator Start()
    {
        runner = GetComponent<Runner>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                if (m_Method == OffMeshLinkMoveMethod.NormalSpeed)
                    yield return StartCoroutine(NormalSpeed(agent));
                else if (m_Method == OffMeshLinkMoveMethod.Parabola)
                    yield return StartCoroutine(Parabola(agent, 2.0f, 0.5f));
                else if (m_Method == OffMeshLinkMoveMethod.Curve)
                    yield return StartCoroutine(Curve(agent, 0.5f));
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

        bool hasFailed = false;

        if (runner) runner.body.Play("Jump");

        hasFailed = Random.Range(0, 12) <= 0;

        yield return new WaitForSeconds(0.35f);
        float normalizedTime = 0.0f;
        float offsetHeight = Random.Range(2f, 4f);

        if (hasFailed)
        {
            endPos *= Random.Range(0.825f, .85f);
            offsetHeight = Random.Range(2f, 2.5f);
            Vector3 prevPos = agent.transform.position;
            Vector3 lastDelta = Vector3.zero;
            while (normalizedTime < .925f)
            {
                float yOffset = height * offsetHeight * (normalizedTime - normalizedTime * normalizedTime);
                Vector3 currPos = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;

                // Track movement delta this frame (horizontal + vertical included)
                lastDelta = currPos - prevPos;
                agent.transform.position = currPos;
                prevPos = currPos;

                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
            Vector3 pushDir = (endPos - prevPos);
            if (pushDir.sqrMagnitude < 1e-6f) pushDir = lastDelta;   // fallback
            if (pushDir.sqrMagnitude < 1e-6f) pushDir = agent.transform.forward; // final fallback
            runner.isAlive = false;
            runner.NoAnimDeath();
            runner.body.PushRagdoll(pushDir);
        }
        else
        {

            while (normalizedTime < 1.0f)
            {
                float yOffset = height * offsetHeight * (normalizedTime - normalizedTime * normalizedTime);
                agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                normalizedTime += Time.deltaTime / duration;

                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
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
}