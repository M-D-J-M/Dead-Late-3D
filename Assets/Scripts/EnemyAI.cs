using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    // Animations //
    public Animator enemyanim;

    // NavMesh
    public NavMeshAgent agent;

    // Found Player
    public Transform player;
    public bool foundplayer;
    public float followplayerrange;

    //Random Patrol
    public float range; //radius of sphere
    public Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area

    // Timer
    public float beginpatrolmaxtime;
    public float beginpatroltime;
    public bool beginpatrol;

    // Vision
    public GameObject visioncone;
    public int playerseen;

    // Hearing
    public GameObject hearingzone;
    public int playerheard;


    private void Start()
    {
        beginpatroltime = beginpatrolmaxtime;
    }


    // Need to run if sees player if not then normal patrol, if patrol then countdown and if countdown <0 then set new destination, if reach then idle and reset timer


    void Update()
    {
        // Input from hearing and vision
        playerseen = visioncone.GetComponent<VisionCone>().playerseen;
        playerheard = hearingzone.GetComponent<HearingZone>().playerheard;
        //print("EnemySeenPlayer: " + playerseen + " EnemyHeardPlayer: " + playerheard);


        // If player seen then follow imediately
        if (playerseen > 0 || playerheard > 0)
        {
            agent.SetDestination(player.position);
            enemyanim.Play("Run");
        }

        // If player not seen then normal patrol
        else
        { 
            PatrolRandom();
        }
    }


    void PatrolRandom()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) //done with path and get new point
        {
            // Standing still
            enemyanim.Play("Idle");

            // Countdown to doing this
            BeginPatrolTimer();
            if (beginpatrol == true) // if timer is 0 then get new point and follow
            {
                Vector3 point;
                if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
                {
                    Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                    agent.SetDestination(point); // set destination to new point and move

                    // reset timer
                    beginpatrol = false;
                    beginpatroltime = beginpatrolmaxtime;
                }
            }
        }
        else
        {
            enemyanim.Play("Walk");
        }
    }


    // Just Gets a random point
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }









    void BeginPatrolTimer()
    {
        if (beginpatroltime <= 0)
        {
            beginpatrol = true;

        }
        else
        {
            beginpatroltime -= Time.deltaTime;

        }
    }

} 

