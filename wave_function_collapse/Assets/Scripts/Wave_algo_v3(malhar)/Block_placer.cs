using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    Queue<int[]> queue = new Queue<int[]>();

    class tile
    {
        private enum state { collapsed, uncollapsed };
        private state current_state;
        private GameObject obj;
        private Vector3 pos = new Vector3(0, 0, 0);
        private GameObject created;
        private int id = -1;



        public tile(GameObject obj, int x,int z,int id)
        {
            current_state = state.uncollapsed;
            this.obj = obj;
            pos.x = x; 
            pos.z = z;
            this.id = id;
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

        public int check_id()
        {
            return id;
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
        if (queue.Any())//&& Input.GetKeyDown("w"))
        {
            execute_queue();
        }

    }

    public void main_room_collapse()
    {
        int grid_rnd_start_x=rnd.Next(padding, grid_size - padding);//height and width should not exceed padding
        int grid_rnd_start_z= rnd.Next(padding, grid_size - padding);

        int grid_rnd_height = rnd.Next(1, padding);
        int grid_rnd_width = rnd.Next(1, padding);
        addqueue(grid_rnd_start_x+rnd.Next(1,grid_rnd_height), grid_rnd_start_z+ rnd.Next(1, grid_rnd_width));
        Debug.Log(grid_rnd_height);
        Debug.Log(grid_rnd_width);
        for(int i=grid_rnd_start_x;i< ((grid_rnd_start_x+grid_rnd_width)>grid_size?grid_size: (grid_rnd_start_x + grid_rnd_width)); i++)
        {
            for(int j=grid_rnd_start_z;j< ((grid_rnd_start_z + grid_rnd_height) > grid_size ? grid_size : (grid_rnd_start_z + grid_rnd_height)); j++)
            {
                if (grid_maker[i, j] == null)
                {
                    grid_maker[i, j] = new tile(blocks[1], i, j,1);
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
        int max = rnd.Next(20, 30);
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
                    grid_maker[i, j] = new tile(blocks[0], i, j,0);
                    grid_maker[i, j].collapse();
                }
            }    
        }
    }

    public void addqueue(int x,int z)
    {
        int[] arr = new int[2];
        arr[0] = x;
        arr[1] = z;
        queue.Enqueue(arr);
    }

    public void execute_queue()
    {
        int[] coordinate = queue.Peek();
        float xdirection = ((grid_size / 2) - coordinate[0]) / (grid_size / 2);
        float zdirection = ((grid_size / 2) - coordinate[1]) / (grid_size / 2);
        int x_create_direction = xdirection >= 0 ? 1 : -1;
        int z_create_direction = zdirection >= 0 ? 1 : -1;
        int x_path = coordinate[0];
        int z_path = coordinate[1];
        bool flip=false;
        while (x_path<grid_size-padding && x_path > padding)
        {
            if(grid_maker[x_path, coordinate[1]].check_id() == 1 && flip)
            {
                break;
            }
            if (grid_maker[x_path,coordinate[1]].check_id()==0)
            {
                flip = true;
            }
            grid_maker[x_path, coordinate[1]].uncollapse();
            grid_maker[x_path, coordinate[1]]=new tile(blocks[1], x_path, coordinate[1], 1);
            grid_maker[x_path, coordinate[1]].collapse();
            x_path = x_path + x_create_direction;
        }
        flip = false;
        while (z_path < grid_size-padding && z_path>padding)
        {
            if (grid_maker[coordinate[0],z_path].check_id() == 1 && flip)
            {
                break;
            }
            if (grid_maker[coordinate[0],z_path].check_id() == 0)
            {
                flip = true;
            }
            grid_maker[coordinate[0],z_path].uncollapse();
            grid_maker[coordinate[0], z_path] = new tile(blocks[1], coordinate[0], z_path, 1);
            grid_maker[coordinate[0], z_path].collapse();
            z_path =z_path+ z_create_direction;
        }
        queue.Dequeue();
    }
}
