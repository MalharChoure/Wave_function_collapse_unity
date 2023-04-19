using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    [SerializeField]
    private int size;
    private tile[,] grid;
    public GameObject[] tiles=new GameObject[16];
    private System.Random rnd = new System.Random();

    class tile
    {
        private enum state {collapsed,uncollapsed};
        private state current_state;
        private GameObject obj;
        private int[] coordinates;
        private Vector3 pos = new Vector3(0, 0, 0);
        private GameObject created;



        tile(GameObject obj, int[] coordinates)
        {
            current_state = state.uncollapsed;
            this.obj = obj;
            this.coordinates[0] = coordinates[0];
            this.coordinates[1] = coordinates[1];
            pos.x = coordinates[0];
            pos.y = coordinates[1];
        }

        public bool check_collapsed()
        {
            return current_state == state.collapsed;
        }

        public void collapse()
        {
            current_state = state.collapsed;
            created=Instantiate(obj,pos,Quaternion.identity);
        }

        public void uncollapse()
        {
            current_state=state.uncollapsed;
            Destroy(created);
        }
    };


    public void create_room(int x,int z)
    {
        int height = rnd.Next(1,6);
        int width = rnd.Next(1,8);
        int width_start = rnd.Next(0,width);
        int[] coordinates = { x, z };
        for (int i=width_start;i<width-1; i++)
        {
            //grid[i, z] = new tile(tiles[4],  );
        }
        for (int i = width_start; i >=0; i--)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = new tile[size, size];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
