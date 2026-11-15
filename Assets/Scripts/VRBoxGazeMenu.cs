using UnityEngine;

// Gaze interaction for basic VR Box viewers with no tracked hand controllers.
// Look at a button for dwellSeconds to activate it; no screen touch required.
public class VRBoxGazeMenu : MonoBehaviour
{
    public VRBoxDemo game;
    public Transform head;
    public float dwellSeconds = 1.6f;
    public float maximumDistance = 8f;
    private VRBoxGazeButton focused;
    private float focusStart;
    private TextMesh status;
    private Transform reticle;

    void Start()
    {
        if (!head || !game) { Debug.LogWarning("Gaze menu needs a head pivot and VRBoxDemo."); enabled = false; return; }
        BuildMenu();
        BuildReticle();
    }

    void BuildMenu()
    {
        string[] labels = { "PAUSE / PLAY", "RESTART", "DANCE STYLE", "RECENTER" };
        for (int i = 0; i < labels.Length; i++)
        {
            var button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.name = "Gaze Button - " + labels[i];
            button.transform.position = new Vector3(-2.25f + i * 1.5f, 1.2f, 0.9f);
            button.transform.localScale = new Vector3(1.35f, .48f, .12f);
            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(.16f, .22f, .48f);
            button.GetComponent<Renderer>().material = mat;
            var target = button.AddComponent<VRBoxGazeButton>();
            target.action = i;
            target.game = game;
            target.baseColor = mat.color;
            var label = new GameObject("Label").AddComponent<TextMesh>();
            label.transform.SetParent(button.transform, false);
            label.transform.localPosition = new Vector3(0, 0, -.56f);
            label.transform.localRotation = Quaternion.Euler(0, 180, 0);
            label.transform.localScale = new Vector3(.13f, .35f, 1);
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.fontSize = 28;
            label.text = labels[i];
            label.color = Color.white;
        }
        var statusObj = new GameObject("Gaze Status");
        statusObj.transform.position = new Vector3(0, .7f, .85f);
        statusObj.transform.rotation = Quaternion.Euler(0, 180, 0);
        status = statusObj.AddComponent<TextMesh>();
        status.anchor = TextAnchor.MiddleCenter;
        status.fontSize = 35;
        status.characterSize = .05f;
        status.color = Color.cyan;
    }

    void BuildReticle()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Gaze Reticle";
        Destroy(go.GetComponent<Collider>());
        go.transform.SetParent(head, false);
        go.transform.localPosition = new Vector3(0, 0, 1.3f);
        go.transform.localScale = Vector3.one * .018f;
        var mat = new Material(Shader.Find("Unlit/Color")); mat.color = Color.white;
        go.GetComponent<Renderer>().material = mat;
        reticle = go.transform;
    }

    void Update()
    {
        VRBoxGazeButton candidate = null;
        RaycastHit hit;
        if (Physics.Raycast(head.position, head.forward, out hit, maximumDistance))
            candidate = hit.collider.GetComponent<VRBoxGazeButton>();
        if (candidate != focused)
        {
            if (focused) focused.SetHighlighted(false);
            focused = candidate;
            focusStart = Time.unscaledTime;
            if (focused) focused.SetHighlighted(true);
        }
        if (focused)
        {
            float progress = Mathf.Clamp01((Time.unscaledTime - focusStart) / Mathf.Max(.5f, dwellSeconds));
            if (status) status.text = "LOOK TO SELECT: " + Mathf.RoundToInt(progress * 100) + "%";
            if (progress >= 1f)
            {
                focused.Activate();
                focused.SetHighlighted(false);
                focused = null;
                focusStart = Time.unscaledTime;
            }
        }
        else if (status) status.text = "LOOK AT A BUTTON TO SELECT";
    }
}

public class VRBoxGazeButton : MonoBehaviour
{
    public VRBoxDemo game;
    public int action;
    public Color baseColor;
    public void SetHighlighted(bool value)
    {
        var renderer = GetComponent<Renderer>();
        if (renderer) renderer.material.color = value ? Color.yellow : baseColor;
    }
    public void Activate()
    {
        if (!game) return;
        switch (action)
        {
            case 0: game.TogglePause(); break;
            case 1: game.PlayTrack(); break;
            case 2: game.NextStyle(); break;
            case 3: game.Recenter(); break;
        }
    }
}
