using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class aStarPath 
{
   private static List<Vector3> openList = new List<Vector3>();
   private static List<Vector3> closedList = new List<Vector3>();
   private static Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
   private static Dictionary<Vector3, int> gScore = new Dictionary<Vector3, int>();
   private static Dictionary<Vector3, int> hScore = new Dictionary<Vector3, int>();
   private static Dictionary<Vector3, int> fScore = new Dictionary<Vector3, int>();
    static int layerMask = 1 << 6;

    private static void CalculateGHF(Vector3 node,Vector3 destination)
    {
        
        gScore.Add(node, gScore[cameFrom[node]] + 1);

        hScore.Add(node, (int)(Mathf.Abs(destination.x-node.x)+ Mathf.Abs(destination.y - node.y)+ Mathf.Abs(destination.z - node.z)));
        
        fScore.Add(node, gScore[node] + hScore[node]);
        
        
    }
    private static bool IsWalkable(Vector3 point)
    {
        Ray ray = new Ray(point, Vector3.down);
        if(Physics.Raycast(ray,2f,192))
        {
            return true;
        }
        return false;
    }
    private static bool IsObstacle(Vector3 point,Vector3 current)
    {
        Debug.Log(LayerMask.LayerToName(6));
        if (point.x < -36 || point.x > 36 || point.z < -36 || point.z > 36 || point.y < 0 || point.y > 10) return true;
        
        if (Physics.OverlapSphere(point, 0.5f,layerMask).Length>0)
        {
            Debug.Log(point + "obstacle");
           /* Gizmos.DrawWireSphere(point, 1);
            CoroutineRunner.StartCoroutine(WaitFor());*/
            return true;
        }

        return false;
    }
    private static void FindNeighbours(Vector3 point,Vector3 destination)
    {
        Vector3[] neighbours = new Vector3[6]
       {
            new Vector3 (point.x + 1, point.y, point.z),
            new Vector3 (point.x - 1, point.y, point.z),
            new Vector3 (point.x, point.y, point.z + 1),
            new Vector3 (point.x, point.y, point.z - 1),
            new Vector3 (point.x, point.y+1, point.z),
            new Vector3 (point.x, point.y-1, point.z),
       };
        
        
        foreach(Vector3 node in neighbours)
        {
            if (IsObstacle(node,point) || closedList.Contains(node)) continue;
            if (IsWalkable(node))
            {
                if (openList.Contains(node))
                {
                    int tempGcost = gScore[point] + 1;
                    int tempHcost = (int)(Mathf.Abs(destination.x - node.x) + Mathf.Abs(destination.y - node.y) + Mathf.Abs(destination.z - node.z));
                    int tempFcost = tempGcost + tempHcost;

                    if (tempFcost < fScore[node])
                    {
                        cameFrom[node] = point;
                        gScore[node] = tempGcost;
                        hScore[node] = tempHcost;
                        fScore[node] = tempFcost;
                        continue;
                    }
                    continue;
                }

                cameFrom.Add(node, point);
                CalculateGHF(node, destination);
                openList.Add(node);
            }
        }
        

    }
    public static List<Vector3> FindPath(Vector3 source, Vector3 destination)
    {
        openList.Add(source);

        //CalculateGHF(source, destination);
        gScore.Add(source, 0);

        hScore.Add(source, (int)(Mathf.Abs(destination.x - source.x) + Mathf.Abs(destination.y - source.y) + Mathf.Abs(destination.z - source.z)));

        fScore.Add(source, gScore[source] + hScore[source]);

        cameFrom.Add(source, source);

        float count = 0;

        while (openList.Count>0)
        {
            count += Time.deltaTime;
           

            

            Vector3 nodeWithLeastFCost = Vector3.zero;
            int leastFcount = int.MaxValue;

            foreach(Vector3 node in openList)
            {
                if(fScore[node]<leastFcount)
                {
                    leastFcount = fScore[node];
                    nodeWithLeastFCost = node;
                    //Debug.Log("Current"+nodeWithLeastFCost);
                }
            }

            openList.Remove(nodeWithLeastFCost);
            closedList.Add(nodeWithLeastFCost);

            if (nodeWithLeastFCost == destination)
            {
                //pathFound
                Debug.Log("path found");
                Debug.Log(count);
                //closedList.Reverse();
                List<Vector3> result = new List<Vector3>();
                Vector3 current = destination;
                while(current!=source)
                {
                    result.Add(current);
                    current = cameFrom[current];
                }
                result.Add(source);
                result.Reverse();
                DrawLines(result);
                return result;
            }

            FindNeighbours(nodeWithLeastFCost, destination);
        }
        Debug.Log("NoPath");
        return null;
    }

    private static void DrawLines(List<Vector3>path)
    {
        for(int i=1;i<path.Count;i++)
        {
            Debug.DrawLine(path[i - 1], path[i], Color.white, 100f);
        }
    }

    private static IEnumerator WaitFor()
    {
        Debug.Log("stated");
        yield return new WaitForSeconds(2);
    }
    

}
