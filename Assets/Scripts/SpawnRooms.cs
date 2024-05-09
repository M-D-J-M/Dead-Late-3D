using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnRooms : MonoBehaviour
{
    public LayerMask whatIsRoom;
    public LevelGeneration levelGen;

    void Update()
    {
        
        Collider[] roomDetection = Physics.OverlapSphere(transform.position, 1, whatIsRoom);
        if (roomDetection.Length == 0 && levelGen.stopGeneration == true)// no room detected and main path finished
        {
            // Spawn Random Room
            int rand = Random.Range(0, levelGen.rooms.Length);
            Instantiate(levelGen.rooms[rand], transform.position, Quaternion.identity);
            Destroy(gameObject); // spawn one room then get destroyed
        }

    }
}
