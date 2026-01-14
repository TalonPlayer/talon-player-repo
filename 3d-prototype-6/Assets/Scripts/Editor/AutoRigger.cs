#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class AutoRigger : EditorWindow
{
    [System.Serializable]
    private class BoneEntry
    {
        public string label;
        public string pattern;   // case-insensitive "contains" search; '|' for alternatives
        public Transform found;
        public BoneEntry(string label, string pattern) { this.label = label; this.pattern = pattern; }
    }

    private Transform root;

    // --- Bones the tool will find (ADD WRISTS) ---
    private readonly List<BoneEntry> bones = new List<BoneEntry>
    {
        new BoneEntry("Pelvis",        "Root"),
        new BoneEntry("Left Hips",     "Leg.L"),
        new BoneEntry("Left Knee",     "Knee.L"),
        new BoneEntry("Left Foot",     "Ankle.L"),
        new BoneEntry("Right Hips",    "Leg.R"),
        new BoneEntry("Right Knee",    "Knee.R"),
        new BoneEntry("Right Foot",    "Ankle.R"),
        new BoneEntry("Left Arm",      "Arm.LL"),
        new BoneEntry("Left Elbow",    "Elbow.L"),
        new BoneEntry("Left Wrist",    "Wrist.L"),
        new BoneEntry("Right Arm",     "Arm.R"),
        new BoneEntry("Right Elbow",   "Elbow.R"),
        new BoneEntry("Right Wrist",   "Wrist.R"),
        new BoneEntry("Middle Spine",  "Spine.02"),
        new BoneEntry("Head",          "Head")
    };

    [MenuItem("Tools/Character Joint Auto Binder")]
    public static void Open() => GetWindow<AutoRigger>("Joint Auto Binder");

    private Vector2 scroll;

    private void OnGUI()
    {
        EditorGUILayout.Space();
        root = (Transform)EditorGUILayout.ObjectField("Root (drag model)", root, typeof(Transform), true);

        EditorGUILayout.HelpBox("Enter name text for each bone. Matching is case-insensitive and uses 'contains'. You can separate alternatives with '|'. Example: 'Leg.L|Thigh.L'", MessageType.Info);

        EditorGUILayout.Space();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (var b in bones)
        {
            EditorGUILayout.BeginVertical("box");
            b.pattern = EditorGUILayout.TextField(b.label, b.pattern);
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Found", b.found, typeof(Transform), true);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Find Bones")) FindBones();
            if (GUILayout.Button("Clear Results")) ClearResults();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Build Components", EditorStyles.boldLabel);
        if (GUILayout.Button("Add CapsuleCollider + Rigidbody + CharacterJoint"))
            CreateCharacterJoints();
    }

    private void FindBones()
    {
        if (!root)
        {
            EditorUtility.DisplayDialog("No Root", "Drag a model root first.", "OK");
            return;
        }

        var all = root.GetComponentsInChildren<Transform>(true);
        foreach (var b in bones)
        {
            b.found = null;
            var tokens = b.pattern.Split('|');
            foreach (var t in tokens)
            {
                string token = t.Trim();
                if (string.IsNullOrEmpty(token)) continue;

                foreach (var tr in all)
                {
                    if (NameMatches(tr.name, token))
                    {
                        b.found = tr;
                        break;
                    }
                }
                if (b.found) break;
            }
        }
    }

    private static bool NameMatches(string name, string token)
    {
        if (string.IsNullOrEmpty(name)) return false;
        return name.ToLowerInvariant().Contains(token.ToLowerInvariant());
    }

    private void ClearResults()
    {
        foreach (var b in bones) b.found = null;
    }

    private void CreateCharacterJoints()
    {
        // quick presence check
        foreach (var b in bones)
        {
            if (b.found == null)
            {
                EditorUtility.DisplayDialog("Missing Bone", $"'{b.label}' was not found. Run 'Find Bones' or edit the pattern.", "OK");
                return;
            }
        }

        // build a lookup
        var map = new Dictionary<string, Transform>();
        foreach (var b in bones) map[b.label] = b.found;

        // CharacterJoint connections (connect to parent body)
        var parentOf = new Dictionary<string, string>
        {
            { "Pelvis",        null },
            { "Middle Spine",  "Pelvis" },
            { "Head",          "Middle Spine" },

            { "Left Hips",     "Pelvis" },
            { "Left Knee",     "Left Hips" },
            { "Left Foot",     "Left Knee" },

            { "Right Hips",    "Pelvis" },
            { "Right Knee",    "Right Hips" },
            { "Right Foot",    "Right Knee" },

            { "Left Arm",      "Middle Spine" },
            { "Left Elbow",    "Left Arm" },
            { "Left Wrist",    "Left Elbow" },

            { "Right Arm",     "Middle Spine" },
            { "Right Elbow",   "Right Arm" },
            { "Right Wrist",   "Right Elbow" },
        };

        // Collider sizing should extend TOWARD the next joint
        var nextOf = new Dictionary<string, string>
        {
            { "Pelvis",        "Middle Spine" },
            { "Middle Spine",  "Head" },
            { "Head",          null },

            { "Left Hips",     "Left Knee" },
            { "Left Knee",     "Left Foot" },
            { "Left Foot",     null },

            { "Right Hips",    "Right Knee" },
            { "Right Knee",    "Right Foot" },
            { "Right Foot",    null },

            { "Left Arm",      "Left Elbow" },
            { "Left Elbow",    "Left Wrist" },
            { "Left Wrist",    null },

            { "Right Arm",     "Right Elbow" },
            { "Right Elbow",   "Right Wrist" },
            { "Right Wrist",   null },
        };

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        // ensure rigidbodies first
        foreach (var kv in parentOf)
            EnsureRB(map[kv.Key]);

        // add colliders + joints
        foreach (var kv in parentOf)
        {
            if (!map.ContainsKey(kv.Key)) continue; // safety
            Transform bone = map[kv.Key];

            Transform parent = null;
            if (kv.Value != null && map.TryGetValue(kv.Value, out var parentTr))
                parent = parentTr;

            Transform next = null;
            if (nextOf.TryGetValue(kv.Key, out var nextName) && nextName != null && map.TryGetValue(nextName, out var nextTr))
                next = nextTr;

            EnsureCapsule(bone, next);         // size toward NEXT joint
            EnsureJoint(bone, parent);
        }

        Undo.CollapseUndoOperations(group);
        EditorUtility.DisplayDialog("Done", "Character joints created.", "OK");
    }

    private static Rigidbody EnsureRB(Transform t)
    {
        var rb = t.GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = Undo.AddComponent<Rigidbody>(t.gameObject);
            rb.mass = 3f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        return rb;
    }

    private static CharacterJoint EnsureJoint(Transform t, Transform parent)
    {
        var cj = t.GetComponent<CharacterJoint>();
        if (!cj) cj = Undo.AddComponent<CharacterJoint>(t.gameObject);
        cj.connectedBody = parent ? parent.GetComponent<Rigidbody>() : null;
        cj.autoConfigureConnectedAnchor = true;
        cj.enableProjection = true;
        cj.swingLimitSpring = new SoftJointLimitSpring { spring = 20f, damper = 2f };
        cj.twistLimitSpring = new SoftJointLimitSpring { spring = 20f, damper = 2f };
        cj.lowTwistLimit = new SoftJointLimit { limit = -30f };
        cj.highTwistLimit = new SoftJointLimit { limit = 30f };
        cj.swing1Limit = new SoftJointLimit { limit = 45f };
        cj.swing2Limit = new SoftJointLimit { limit = 45f };
        return cj;
    }

    // Uses world length to NEXT joint, converts to local using lossyScale (correct with non-uniform scale)
    private static CapsuleCollider EnsureCapsule(Transform t, Transform childInChain)
    {
        var col = t.GetComponent<CapsuleCollider>();
        if (!col) col = Undo.AddComponent<CapsuleCollider>(t.gameObject);
        float heightLocal = 0f;
        float radiusLocal = 0f;
        // No child: tiny fallback sized by lossyScale
        if (childInChain == null)
        {
            col.center = Vector3.zero;
            col.direction = 1; // Y
            Vector3 s = t.lossyScale;

            float heightWorld = 0.25f;
            float radiusWorld = 0.06f;

            heightLocal = heightWorld / Mathf.Max(s.y, 1e-6f);
            radiusLocal = radiusWorld / Mathf.Max(Mathf.Max(s.x, s.z), 1e-6f);

            col.height = Mathf.Max(heightLocal, radiusLocal * 2f);
            col.radius = radiusLocal;
            return col;
        }

        // World-space measurements to the NEXT joint
        Vector3 a = t.position;
        Vector3 b = childInChain.position;
        Vector3 dirW = (b - a);
        float lenW = Mathf.Max(dirW.magnitude, 0.05f);

        // Pick collider axis from dominant local direction
        Vector3 dirL = t.InverseTransformDirection(dirW);
        int axis = LargestAxisIndex(dirL); // 0=X,1=Y,2=Z
        col.direction = axis;

        // Center at midpoint
        Vector3 midW = (a + b) * 0.5f;
        col.center = t.InverseTransformPoint(midW);

        // Desired world dims
        float radiusW = Mathf.Clamp(lenW * 0.25f, 0.01f, lenW * 0.45f);
        float heightW = Mathf.Max(lenW, radiusW * 2f);

        // Convert world -> local using lossyScale
        Vector3 sL = t.lossyScale;

        float axisScale =
            (axis == 0) ? Mathf.Max(sL.x, 1e-6f) :
            (axis == 1) ? Mathf.Max(sL.y, 1e-6f) :
                          Mathf.Max(sL.z, 1e-6f);

        float perpScale =
            (axis == 0) ? Mathf.Max(sL.y, sL.z) :
            (axis == 1) ? Mathf.Max(sL.x, sL.z) :
                          Mathf.Max(sL.x, sL.y);
        perpScale = Mathf.Max(perpScale, 1e-6f);

        heightLocal = heightW / axisScale;
        radiusLocal = radiusW / perpScale;

        // Unity requires height >= 2*radius
        col.height = Mathf.Max(heightLocal, radiusLocal * 2f);
        col.radius = radiusLocal;

        return col;
    }



    private static int LargestAxisIndex(Vector3 v)
    {
        v = new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        if (v.x > v.y && v.x > v.z) return 0;
        if (v.z > v.y) return 2;
        return 1; // default Y
    }
}
#endif
