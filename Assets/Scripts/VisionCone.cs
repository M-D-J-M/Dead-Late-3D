using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public Material VisionConeMaterial;
    public float VisionRange;
    public float VisionAngle;
    public LayerMask VisionObstructingLayer;//layer with objects that obstruct the enemy view, like walls, for example
    public LayerMask ObjectToDetectLayer;//layer with objects that are to be detected such as the player
    public int VisionConeResolution = 120;//the vision cone will be made up of triangles, the higher this value is the pretier the vision cone will be

    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;

    // Player detected 
    public int playerseen;
    public float beginscanmaxtime;
    public float beginscantime;
    public bool readytoscan = true;



    void Start()
    {
        beginscantime = beginscanmaxtime;

        transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        VisionAngle *= Mathf.Deg2Rad;
    }

    void Update()
    {

        if(playerseen > 0) 
        {
            PlayerScanCooldown();
        }
        

        DrawVisionCone();//calling the vision cone function everyframe just so the cone is updated every frame

        //print("Vision Seen: " + playerseen);
    }

    void DrawVisionCone()//this method creates the vision cone mesh
    {
    	int[] triangles = new int[(VisionConeResolution - 1) * 3];
    	Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -VisionAngle / 2;
        float angleIcrement = VisionAngle / (VisionConeResolution - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < VisionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);

            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                Vertices[i + 1] = VertForward * hit.distance;
            }

            else
            {
                Vertices[i + 1] = VertForward * VisionRange;
            }

            if (readytoscan) 
            {
                // Scan For Player
                if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hitdetect, VisionRange, ObjectToDetectLayer, QueryTriggerInteraction.Collide))
                {
                    // if player is detected
                    if (hitdetect.transform.gameObject.name == "Player")
                    {
                        playerseen = 1;
                    }
                }
            }


            Currentangle += angleIcrement;
        }

        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;
    }

    void PlayerScanCooldown()
    {
        if (beginscantime <= 0)
        {
            beginscantime = beginscanmaxtime;
            playerseen = 0;
            readytoscan = true;
        }
        else
        {
            readytoscan = false;
            beginscantime -= Time.deltaTime;
        }
    }
}

