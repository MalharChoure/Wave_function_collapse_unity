using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk_script : MonoBehaviour
{
    //The integers required to fill out the room blocks.
    public int grid_size=10;
    public int padding=2;
    public int max_room_size;
    public int no_of_floors=2;
    public int max_no_of_rooms = 5;
    public int min_no_of_rooms = 3;

    //scale of the room
    public int room_x_scale = 1;
    public int room_y_scale = 1;
    public int room_z_scale = 1;


    // randomiser added under the name rnd and a random seed is invoked every time.
    private System.Random rnd = new System.Random();

    // central room 
    public int central_room_size=5;
    public int central_room_coordinate_x;

    //Grid creation
    private block[,,] chunks;

    //The tiles that are to be actually placed
    public GameObject[] tiles;

    //This is to create a queue for the doors spawner
    Queue<int[]> queue = new Queue<int[]>();



    class block
    {
        private int x;
        private int y;
        private int z;
        public GameObject current;
        private bool collapsed = false;
        int room_x;
        int room_y;
        int room_z;

        public block(int x,int y, int z, GameObject obj, int room_x, int room_y, int room_z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            current = obj;
            this.room_x = room_x;
            this.room_y = room_y;
            this.room_z = room_z;
        }

        public void set_collapsed()
        {
            collapsed = true;
            instantiate_prefab();
        }

        public void un_collapse()
        {
            collapsed= false;
        }

        private bool is_collapsed()
        {
            return collapsed;
        }

        private void instantiate_prefab()
        {
            Vector3 a = new Vector3(x*room_x, z*room_y, y*room_z); //cause unity has 
            Instantiate(current,a,Quaternion.identity);
        }
    }

    private void Start()
    {
        chunks = new block[grid_size, grid_size, no_of_floors];
        central_room_coordinate_x = Mathf.Abs(grid_size / 2);
        central_room_size = central_room_size > grid_size ?(grid_size/2)-padding:central_room_size;
        central_room_creator();
        floors_creator();
    }

    private void addqueue(int x,int y, int z)
    {
        int[] arr = new int[3];
        arr[0] = x;
        arr[1] = y;
        arr[2] = z;
        queue.Enqueue(arr);
    }

    private void central_room_creator()
    {
        Debug.Log(central_room_coordinate_x);
        for (int k = 0; k < no_of_floors; k++)
        {
            Debug.Log("here 1");
            for (int i = central_room_coordinate_x - central_room_size ; i < central_room_coordinate_x + central_room_size ; i++)
            {
                Debug.Log(i);
                for (int j = central_room_coordinate_x - central_room_size ; j < central_room_coordinate_x + central_room_size; j++)
                {
                    Debug.Log(j);
                    chunks[i, j, k] = new block(i, j, k, tiles[k==0?1:2],room_x_scale,room_y_scale,room_z_scale);
                    chunks[i, j, k].set_collapsed();
                }
            }
        }
    }

    public void floors_creator()
    {
        for (int i=0;i<no_of_floors;i++)
        {
            int max = rnd.Next(min_no_of_rooms, max_no_of_rooms);
            for (int j=0;j<max;j++)
            {
                room_collapse(i);
            }
        }
    }

    private void room_collapse(int floor)
    {
        int grid_rnd_start_x = rnd.Next(padding, grid_size - padding);
        int grid_rnd_start_y = rnd.Next(padding, grid_size - padding);

        int grid_rnd_height = rnd.Next(1, padding);
        int grid_rnd_width = rnd.Next(1, padding);

        addqueue(grid_rnd_start_x + rnd.Next(1, grid_rnd_height), grid_rnd_start_y + rnd.Next(1, grid_rnd_width),floor);

        for (int i = grid_rnd_start_x; i < ((grid_rnd_start_x + grid_rnd_width) > grid_size ? grid_size : (grid_rnd_start_x + grid_rnd_width)); i++)
        {
            for (int j = grid_rnd_start_y; j < ((grid_rnd_start_y + grid_rnd_height) > grid_size ? grid_size : (grid_rnd_start_y + grid_rnd_height)); j++)
            {
                if (chunks[i, j, floor] == null)
                {
                    chunks[i, j, floor] = new block(i, j, floor, tiles[1], room_x_scale, room_y_scale, room_z_scale);
                    chunks[i, j, floor].set_collapsed();
                }
            }
        }
    }
}
