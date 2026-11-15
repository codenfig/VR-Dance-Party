#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CreateVRBoxScene
{
    [MenuItem("VR Dance Party/Create First Playable Scene")]
    public static void Create()
    {
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var floor=GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        floor.name="Dance Floor"; floor.transform.localScale=new Vector3(7,.12f,7);
        floor.GetComponent<Renderer>().sharedMaterial=MakeMat(new Color(.09f,.07f,.18f));
        var stage=GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stage.name="Stage"; stage.transform.position=new Vector3(0,.12f,3.5f);
        stage.transform.localScale=new Vector3(2,.22f,1.5f);
        stage.GetComponent<Renderer>().sharedMaterial=MakeMat(new Color(.32f,.12f,.46f));
        var dancer=new GameObject("Dancer (Procedural Placeholder)"); dancer.transform.position=new Vector3(0,.4f,3.5f);
        for(int i=0;i<8;i++)
        {
            float a=i*Mathf.PI/4;
            var lightGo=new GameObject("Club Light "+i);
            lightGo.transform.position=new Vector3(Mathf.Cos(a)*4,3,3.5f+Mathf.Sin(a)*4);
            var light=lightGo.AddComponent<Light>(); light.type=LightType.Point;
            light.range=7; light.intensity=2.0f;
            light.color=Color.HSVToRGB(i/8f,.85f,1);
        }
        var mainLight=new GameObject("Main Light"); mainLight.transform.rotation=Quaternion.Euler(45,-30,0);
        var dir=mainLight.AddComponent<Light>(); dir.type=LightType.Directional; dir.intensity=.5f;
        var head=new GameObject("Phone Head Pivot"); head.transform.position=new Vector3(0,1.65f,-2.2f);
        var left=new GameObject("Left Eye"); left.transform.SetParent(head.transform,false);
        var leftCamera=left.AddComponent<Camera>(); leftCamera.depth=0; leftCamera.tag="MainCamera";
        var listener=left.AddComponent<AudioListener>();
        var right=new GameObject("Right Eye"); right.transform.SetParent(head.transform,false);
        var rightCamera=right.AddComponent<Camera>(); rightCamera.depth=1;
        var root=new GameObject("VR Dance Party Game");
        var audio=root.AddComponent<AudioSource>(); audio.playOnAwake=false; audio.spatialBlend=0;
        var demo=root.AddComponent<VRBoxDemo>();
        demo.dancer=dancer.transform; demo.headPivot=head.transform; demo.leftEye=leftCamera;
        demo.rightEye=rightCamera; demo.audioSource=audio;
        RenderSettings.ambientLight=new Color(.3f,.3f,.45f);
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene,"Assets/Scenes/VRDanceParty_FirstScene.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/VRDanceParty_FirstScene.unity",true)};
        Selection.activeGameObject=root;
        Debug.Log("Created first playable scene. Press Play or build for Android.");
    }
    static Material MakeMat(Color color) { var m=new Material(Shader.Find("Standard")); m.color=color; return m; }
}
#endif
