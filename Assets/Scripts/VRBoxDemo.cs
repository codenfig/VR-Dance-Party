using UnityEngine;

// Self-contained phone VR Box demonstration. No XR plugin is required.
public class VRBoxDemo : MonoBehaviour
{
    public Transform dancer;
    public Transform headPivot;
    public Camera leftEye, rightEye;
    public AudioSource audioSource;
    public float bpm = 120f;
    public float eyeSeparation = 0.064f;
    public bool stereo = true;
    private bool playing;
    private double startedAt;
    private double pausedAt;
    private int style;
    private Quaternion gyroCorrection = Quaternion.identity;
    private float phase;
    private Transform leftArm, rightArm, leftLeg, rightLeg;
    private bool gyroReady;

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        if (SystemInfo.supportsGyroscope) { Input.gyro.enabled = true; gyroReady = true; }
        SetupEyes();
        BuildDancer();
        CreateClickTrack();
        PlayTrack();
    }

    void SetupEyes()
    {
        if (!leftEye || !rightEye) return;
        leftEye.rect = stereo ? new Rect(0,0,0.5f,1) : new Rect(0,0,1,1);
        rightEye.rect = stereo ? new Rect(0.5f,0,0.5f,1) : new Rect(0,0,0,0);
        leftEye.transform.localPosition = new Vector3(-eyeSeparation/2,0,0);
        rightEye.transform.localPosition = new Vector3(eyeSeparation/2,0,0);
        leftEye.fieldOfView = rightEye.fieldOfView = 80;
    }

    void BuildDancer()
    {
        if (!dancer) return;
        // The primitive dancer is a placeholder; replace with a rigged character later.
        var mat = new Material(Shader.Find("Standard")); mat.color = new Color(0.1f,0.85f,0.9f);
        Part("Torso", dancer, PrimitiveType.Capsule, new Vector3(0,1.35f,0), new Vector3(.46f,.55f,.3f), mat);
        Part("Head", dancer, PrimitiveType.Sphere, new Vector3(0,2.12f,0), new Vector3(.38f,.38f,.38f), mat);
        leftArm = Joint("LeftArm",new Vector3(-.35f,1.72f,0));
        rightArm = Joint("RightArm",new Vector3(.35f,1.72f,0));
        leftLeg = Joint("LeftLeg",new Vector3(-.17f,1.0f,0));
        rightLeg = Joint("RightLeg",new Vector3(.17f,1.0f,0));
        Part("Left arm mesh",leftArm,PrimitiveType.Capsule,new Vector3(0,-.35f,0),new Vector3(.2f,.44f,.2f),mat);
        Part("Right arm mesh",rightArm,PrimitiveType.Capsule,new Vector3(0,-.35f,0),new Vector3(.2f,.44f,.2f),mat);
        Part("Left leg mesh",leftLeg,PrimitiveType.Capsule,new Vector3(0,-.45f,0),new Vector3(.24f,.5f,.24f),mat);
        Part("Right leg mesh",rightLeg,PrimitiveType.Capsule,new Vector3(0,-.45f,0),new Vector3(.24f,.5f,.24f),mat);
    }
    Transform Joint(string name, Vector3 pos) { var t=new GameObject(name).transform; t.SetParent(dancer,false); t.localPosition=pos; return t; }
    static void Part(string name,Transform parent,PrimitiveType shape,Vector3 pos,Vector3 scale,Material mat)
    { var go=GameObject.CreatePrimitive(shape); go.name=name; go.transform.SetParent(parent,false); go.transform.localPosition=pos; go.transform.localScale=scale; go.GetComponent<Renderer>().material=mat; }

    void CreateClickTrack()
    {
        // 16 seconds of ORIGINAL synthesized metronome, 120 BPM.
        int rate=22050, duration=16;
        float[] samples=new float[rate*duration];
        int interval=(int)(rate*60f/bpm);
        for (int start=0;start<samples.Length;start+=interval)
            for (int j=0;j<2200 && start+j<samples.Length;j++)
            { float env=Mathf.Exp(-j/350f); samples[start+j] += .24f*env*Mathf.Sin(2*Mathf.PI*880*j/rate); }
        AudioClip clip=AudioClip.Create("Original Demo Beat",samples.Length,1,rate,false);
        clip.SetData(samples,0); audioSource.clip=clip; audioSource.loop=true;
    }
    public void PlayTrack() { if (!audioSource) return; audioSource.Stop(); audioSource.Play(); startedAt=AudioSettings.dspTime; pausedAt=0; playing=true; }
    public void TogglePause()
    {
        if (!audioSource) return;
        if (playing) { pausedAt=AudioSettings.dspTime-startedAt; audioSource.Pause(); playing=false; }
        else { startedAt=AudioSettings.dspTime-pausedAt; audioSource.UnPause(); playing=true; }
    }
    public void NextStyle() { style=(style+1)%3; }
    public void ToggleStereo() { stereo=!stereo; SetupEyes(); }
    public void Recenter() { if (gyroReady) gyroCorrection=Quaternion.Inverse(ReadGyro()); }
    Quaternion ReadGyro()
    {
        // Unity's Android gyro attitude converted to Unity camera orientation.
        Quaternion q=Input.gyro.attitude;
        return Quaternion.Euler(90,0,0)*new Quaternion(q.x,q.y,-q.z,-q.w);
    }
    void Update()
    {
        if (headPivot && gyroReady) headPivot.localRotation=gyroCorrection*ReadGyro();
        else if (headPivot) headPivot.Rotate(0,Input.GetAxis("Horizontal")*55*Time.deltaTime,0,Space.World);
        if (!dancer || !playing || !audioSource) return;
        double elapsed=AudioSettings.dspTime-startedAt;
        phase=(float)((elapsed%(60.0/bpm))/(60.0/bpm));
        float wave=Mathf.Sin(phase*Mathf.PI*2);
        float movement=style==0?20:style==1?42:65;
        dancer.localPosition=new Vector3(0,Mathf.Max(0,wave)*.10f,0);
        dancer.localRotation=Quaternion.Euler(0,style==2?wave*12:0,0);
        if(leftArm) leftArm.localRotation=Quaternion.Euler(wave*movement,0,-20-wave*movement);
        if(rightArm) rightArm.localRotation=Quaternion.Euler(-wave*movement,0,20+wave*movement);
        if(leftLeg) leftLeg.localRotation=Quaternion.Euler(-wave*movement*.55f,0,0);
        if(rightLeg) rightLeg.localRotation=Quaternion.Euler(wave*movement*.55f,0,0);
    }
    void OnGUI()
    {
        // Touch buttons are for testing outside the viewer; a Bluetooth controller/gaze UI is a later sprint.
        float s=Mathf.Max(1f,Screen.height/700f);
        GUI.matrix=Matrix4x4.Scale(new Vector3(s,s,1));
        float w=Screen.width/s,h=Screen.height/s;
        if(GUI.Button(new Rect(12,h-58,100,45),playing?"Pause":"Resume")) TogglePause();
        if(GUI.Button(new Rect(118,h-58,100,45),"Restart")) PlayTrack();
        if(GUI.Button(new Rect(224,h-58,100,45),"Style "+(style+1))) NextStyle();
        if(GUI.Button(new Rect(330,h-58,100,45),"Recenter")) Recenter();
        if(GUI.Button(new Rect(436,h-58,110,45),stereo?"Mono test":"Stereo VR")) ToggleStereo();
        GUI.Label(new Rect(12,8,w-20,30),"VR DANCE PARTY | Original 120 BPM demo | "+(gyroReady?"Gyro active":"No gyro: PC arrow keys"));
    }
}
