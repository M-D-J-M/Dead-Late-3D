using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public Transform[] startingPositions;


    public GameObject[] rooms; // 0:LR  1:LRB  2:LRT  3:LRBT 

    private int direction;
    public float moveAmount;

    private float timeBtwRoom;
    public float startTimeBtwRoom = 0.25f;

    public float maxX;
    public float minX;
    public float minZ;

    public bool stopGeneration = false;

    public LayerMask room;

    int downcounter;
    int clonenumber;


    private void Start()
    {
        // timer for generation
        timeBtwRoom = startTimeBtwRoom;

        int randStartingPos = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randStartingPos].position;
        Instantiate(rooms[0], transform.position, Quaternion.identity);

        direction = Random.Range(1, 6); //Random Between 1-5, 1-2 Left, 3-4 Right, 5 Down
    }

    private void Update()
    {
        if (timeBtwRoom <= 0 && stopGeneration == false)
        {
            Move();
            timeBtwRoom = startTimeBtwRoom;
        }
        else
        {
            timeBtwRoom -= Time.deltaTime;
        }
    }

    private void Move()
    {
        if (direction == 1 || direction == 2) // MOVE RIGHT
        {
            // reset down counter to 0 as double or triple downs not an issue here
            downcounter = 0;

            if(transform.position.x < maxX)  // Hasnt reached far right yet
            {

                // Transform position to newpos at the right
                Vector3 newPos = new Vector3(transform.position.x + moveAmount, transform.position.y, transform.position.z);
                transform.position = newPos;

                // Pick any room as all rooms have left and right
                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);


                // Gen direction again but change left to right and down
                direction = Random.Range(1, 6); 
                if(direction == 3)
                {
                    direction = 2;
                }
                if (direction == 4)
                {
                    direction = 2;
                }
            }
            else // reached Max right
            {
                direction = 5; // go down
            }

        }
        else if (direction == 3 || direction == 4)  // MOVE LEFT
        {
            // reset down counter to 0 as double or triple downs not an issue here
            downcounter = 0;

            if (transform.position.x > minX) // Hasnt reached far left yet
            {
                // Transform position to newpos at the right
                Vector3 newPos = new Vector3(transform.position.x - moveAmount, transform.position.y, transform.position.z);
                transform.position = newPos;

                // Pick any room as all rooms have left and right
                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                // Gen direction again but remove right(1,2)
                direction = Random.Range(3, 6);
            }
            else // Reached Max Left
            {
                direction = 5; // go down
            }


        }

        // When moving down need to make sure that the room under has top opening and above has bottom oening
        else if (direction == 5)  // MOVE DOWN
        {
            // Keep track of how many times moved down
            downcounter++;

            if (transform.position.z > minZ) // hasent reached bottom yet
            {
                // Detect room before moving down to check if has a bottom or not
                Collider[] roomDetection = Physics.OverlapSphere(transform.position, 1, room);

                // if room doesnt have a bottom
                if (roomDetection[roomDetection.Length - 1].GetComponent<RoomType>().type != 1 && roomDetection[roomDetection.Length - 1].GetComponent<RoomType>().type != 3)
                {
                    if(downcounter >= 2) // if level generator has moved down 2,3,4 etc times 
                    {
                        // destroy room
                        roomDetection[roomDetection.Length - 1].GetComponent<RoomType>().RoomDestruction();

                        // Replace with room with all openings
                        Instantiate(rooms[3],transform.position, Quaternion.identity);
                    }
                    else // if level has only moved down 1 time, so no top and bottom needed in the middle between 3 situation
                    {
                        // destroy room
                        roomDetection[roomDetection.Length - 1].GetComponent<RoomType>().RoomDestruction();

                        // Replace with room with all openings
                        Instantiate(rooms[3], transform.position, Quaternion.identity);

                        // CANT FIGURE OUT SO ALL OPEN
                        // Replace with one with a bottom
                        //int randBottomRoom = Random.Range(1, 4);
                        //if (randBottomRoom == 2)
                        //{
                        //    randBottomRoom = 1;
                        //}
                        //Instantiate(rooms[randBottomRoom], transform.position, Quaternion.identity);
                    }


                }

                // Transform position to newpos downwards
                Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - moveAmount);
                transform.position = newPos;

                // pick room that have top openings
                int rand = Random.Range(2, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(1, 6); // Move Gen again any direction
            }
            else
            {
                stopGeneration = true;
            }

        }

    }
}
