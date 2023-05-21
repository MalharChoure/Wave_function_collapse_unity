using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathfinding : MonoBehaviour
{
    public float speed = 5f;
    private int currentPointIndex = 0;
    // public static Astar1 a;
    public GameObject target;
    public List<Vector3> path = new List<Vector3>();
    private void Start()
    {
         path = aStarPath.FindPath(transform.position,target.transform.position);
        
        
    }

    private void Update()
    {
        
            if (path.Count > 0)
            {
                // Get the current target point
                Vector3 targetPoint = path[currentPointIndex];

                // Calculate the direction towards the target point
                Vector3 direction = targetPoint - transform.position;

                // Check if the object has reached the target point
                if (direction.magnitude < 0.1f)
                {
                    // Move to the next point in the array
                    if(currentPointIndex!=path.Count-1)
                        currentPointIndex = (currentPointIndex + 1) ;
                }
                else
                {
                    // Move towards the target point
                    transform.position += direction.normalized * speed * Time.deltaTime;
                }
            }
        
    }

}
