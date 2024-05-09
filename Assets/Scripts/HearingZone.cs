using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HearingZone : MonoBehaviour
{
    public Material HearingZoneMaterial;
    public float VisionRange;
    public float VisionAngle;
    public LayerMask VisionObstructingLayer;//layer with objects that obstruct the enemy view, like walls, for example
    public LayerMask ObjectToDetectLayer;//layer with objects that are to be detected such as the player
    public int HearingZoneResolution = 120;//the vision cone will be made up of triangles, the higher this value is the pretier the vision cone will be
    Mesh HearingZoneMesh;
    MeshFilter MeshFilter_;

    // Player detected 
    public int playerheard;
    public float beginscanmaxtime;
    public float beginscantime;
    public bool readytoscan = true;


    void Start()
    {
        // For Scanning
        beginscantime = beginscanmaxtime;

        transform.AddComponent<MeshRenderer>().material = HearingZoneMaterial;
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        HearingZoneMesh = new Mesh();
        VisionAngle *= Mathf.Deg2Rad;
    }

    void Update()
    {

        if (playerheard > 0)
        {
            PlayerScanCooldown();
        }

        //print("Hearing Heard: " + playerheard);

        DrawHearingZone();//calling the vision cone function everyframe just so the cone is updated every frame
    }

    void DrawHearingZone()//this method creates the vision cone mesh
    {
        int[] triangles = new int[(HearingZoneResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[HearingZoneResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -VisionAngle / 2;
        float angleIcrement = VisionAngle / (HearingZoneResolution - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < HearingZoneResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);

            // Obstructions to hearing
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                Vertices[i + 1] = VertForward * hit.distance;
            }

            else
            {
                Vertices[i + 1] = VertForward * VisionRange;
            }

            if (readytoscan) // scan timer is 0
            {
                // If player is heard, needs only one hit to start following, then a timer, then checks again
                if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hitdetect, VisionRange, ObjectToDetectLayer, QueryTriggerInteraction.Collide))
                {
                    if (hitdetect.transform.gameObject.name == "Player")
                    {
                        if (hitdetect.transform.gameObject.GetComponent<PlayerMovement>().noiselevel > 0)
                        {
                            playerheard = 1;
                        }
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

        HearingZoneMesh.Clear();
        HearingZoneMesh.vertices = Vertices;
        HearingZoneMesh.triangles = triangles;
        MeshFilter_.mesh = HearingZoneMesh;
    }


    void PlayerScanCooldown()
    {
        if (beginscantime <= 0)
        {
            beginscantime = beginscanmaxtime;
            playerheard = 0;
            readytoscan = true;
        }
        else
        {
            readytoscan = false;
            beginscantime -= Time.deltaTime;
        }
    }
}
