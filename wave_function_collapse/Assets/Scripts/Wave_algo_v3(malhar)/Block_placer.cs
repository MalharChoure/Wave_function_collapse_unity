using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_placer : MonoBehaviour
{

    [SerializeField]
    private int grid_size = 100;
    private tile[,] grid_maker;
    [SerializeField]
    private GameObject[] blocks = new GameObject[2];
    private System.Random rnd = new System.Random();
    public int padding=10;
    class tile
    {
        private enum state { collapsed, uncollapsed };
        private state current_state;
        private GameObject obj;
        private Vector3 pos = new Vector3(0, 0, 0);
        private GameObject created;



        public tile(GameObject obj, int x,int z)
        {
            current_state = state.uncollapsed;
            this.obj = obj;
            pos.x = x; 
            pos.z = z;
        }

        public bool check_collapsed()
        {
            return current_state == state.collapsed;
        }

        public void collapse()
        {
            current_state = state.collapsed;
            created = Instantiate(obj, pos, Quaternion.identity);
        }

        public void uncollapse()
        {
            current_state = state.uncollapsed;
            Destroy(created);
        }
    };
    // Start is called before the first frame update
    void Start()
    {
        grid_maker = new tile[grid_size, grid_size];
        //main_room_collapse();
        decide_max_covered_tile();
        cover_rest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void main_room_collapse()
    {
        int grid_rnd_start_x=rnd.Next(padding, grid_size - padding);//height and width should not exceed padding
        int grid_rnd_start_z= rnd.Next(padding, grid_size - padding);
        int grid_rnd_height = rnd.Next(1, 10);
        int grid_rnd_width = rnd.Next(1, 10);
        Debug.Log(grid_rnd_height);
        Debug.Log(grid_rnd_width);
        for(int i=grid_rnd_start_x;i< ((grid_rnd_start_x+grid_rnd_width)>grid_size?grid_size: (grid_rnd_start_x + grid_rnd_width)); i++)
        {
            for(int j=grid_rnd_start_z;j< ((grid_rnd_start_z + grid_rnd_height) > grid_size ? grid_size : (grid_rnd_start_z + grid_rnd_height)); j++)
            {
                if (grid_maker[i, j] == null)
                {
                    grid_maker[i, j] = new tile(blocks[1], i, j);
                    grid_maker[i, j].collapse();
                }
                /*else
                {
                    
                }*/
            }
        }
    }

    public void decide_max_covered_tile()
    {
        int max = rnd.Next(30, 40);
        for (int i=0;i<max;i++)
        {
            main_room_collapse();
        }
    }

    public void cover_rest()
    {
        for(int i=0;i<grid_size;i++)

        {
            for(int j=0;j<grid_size;j++)
            {
                if (grid_maker[i,j]==null)
                {
                    grid_maker[i, j] = new tile(blocks[0], i, j);
                    grid_maker[i, j].collapse();
                }
            }    
        }
    }
}
