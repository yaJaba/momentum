using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BombController : MonoBehaviour
{

    public float force = 20;
    [Range(0, 10)]
    public float zeroFalloffDistance = 5;
    [Range(0, 10)]
    public float falloffDistance = 2;

    public GameObject explosionPrefab;

    public float ExpandedMultiplier = 10f;

    private Vector3 originalScale;
    private Vector3 explodedScale;

    private Color originalColor;

    public float SolveForce(float distance)
    {
        // https://www.desmos.com/calculator/r4uuqtr9gw
        return Mathf.Max(0, force * (Mathf.Abs(distance - zeroFalloffDistance)) / (-distance + zeroFalloffDistance), -(force/Mathf.Pow(falloffDistance, 2)) * Mathf.Pow(distance - zeroFalloffDistance, 2) + force);
    }

    #if UNITY_EDITOR

    [CustomEditor(typeof(BombController))]
    public class BombControllerInspector : Editor
    {

         public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Texture2D tex = new Texture2D(301, 101);
            float xscale = 0.034f;
            float yscale = 2;
            for (int x = 0; x < 300; x++)
            {
                int y = (int)(((BombController)this.target).SolveForce(x * xscale) * yscale);
                tex.SetPixel(x, y, Color.black);
            }
            tex.Apply();
            GUILayout.Box(tex);
            
        }
    }
    #endif

    [Tooltip("Whether to nullify the y velocity of a player if they are falling and this explosive force is opposed to their fall. " +
             "Usefull for making pogos consistent and preventing having to use large forces (that may act horizintally too) to break a fall")]
    public bool nullifyFall = true;

    public float respawnTime = 0.7f;
    public float animationScaleTime = 0.6f;


    private bool detonated = false;
    private float detonatedTime;

    private List<Renderer> renderers; 

    void Awake()
    {
        //GetComponent<Renderer>().material.color = originalColor = Color.white;
        originalScale = gameObject.transform.localScale;
        explodedScale = originalScale * ExpandedMultiplier;
    }

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>().ToList();
    }

    void Update()
    {
        if (detonated)
        {
          //  float scaleTimePercent = (Time.time - detonatedTime) / animationScaleTime;
          //  gameObject.transform.localScale = Vector3.Lerp(originalScale, explodedScale, scaleTimePercent);
          //  Color currentColor = Color.red;
           // currentColor.a = Mathf.Lerp(1f, 0f, scaleTimePercent);
          //  GetComponent<Renderer>().material.color = currentColor;

           // GetComponent<Renderer>().enabled = true;

            if (Time.time > detonatedTime + respawnTime)
            {
                Respawn();
            }
        }
    }

    internal void Select()
    {
        GetComponentInChildren<Animator>().SetBool(Animator.StringToHash("Armed"), true);
    }

    internal void Deselect()
    {
        GetComponentInChildren<Animator>().SetBool(Animator.StringToHash("Armed"), false);
    }

    internal void Detonate()
    {

        detonated = true;
        detonatedTime = Time.time;

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        renderers.ForEach(q => q.enabled = false);
    }

    internal void Respawn()
    {
        detonated = false;
        gameObject.transform.localScale = originalScale;
        renderers.ForEach(q => q.enabled = true);
        GetComponentInChildren<Animator>().SetBool(Animator.StringToHash("Armed"), false);
    }

    public bool detonatable
    {
        get
        {
            return !detonated;
        }
    }
}
