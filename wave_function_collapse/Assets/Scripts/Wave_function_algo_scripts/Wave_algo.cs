using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Xml.Schema;
using UnityEngine;

public class Wave_algo : MonoBehaviour
{
    [SerializeField]
    private int grid_size = 10;
    private block[,] grid;
    private int[,] set_rules = { { 1, 1, 1, 1 }, { 0, 1, 1, 1 }, { 1, 0, 1, 1 }, { 1, 1, 0, 1 }, { 1, 1, 1, 0 }, { 1, 0, 1, 0 }, { 0, 1, 0, 1 }, { 0, 1, 1, 0 }, { 0, 0, 1, 1 }, { 1, 0, 0, 1 }, { 1, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 }, { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } };
    [SerializeField]
    private GameObject[] blocks = new GameObject[16];
    //private int[,] queue;
    Queue<int[]> queue = new Queue<int[]>();
    private System.Random rnd = new System.Random();
    class block
    {
        private enum state {collapsed,uncollapsed};
        private state current;
        private GameObject prefab;
        private Vector3 position = new Vector3(0, 0, 0);
        private int[] rules=new int[4];
        private int identity;
        public block(int x,int z,GameObject fab, int[] arr,int id)
        {
            position.x = x;
            position.z = z;
            prefab = fab;
            current=state.uncollapsed;
            rules = arr;
            identity = id;
        }
        
        public bool check_collapsed()
        {
            return current == state.collapsed ? true : false;
        }

        public void set_collapsed()
        {
            current = state.collapsed;
            Instantiate(prefab, position, Quaternion.identity);
        }

        public int identify()
        {
            return identity;
        }
    };



    void Start()
    {
        grid = new block[grid_size, grid_size];
        set_seed();
    }

    // Update is called once per frame
    void Update()
    {
        if (queue.Any())
        {   
        execute_queue();
        }
    }

    public int[] return_dimension(int[,] arr,int index)
    {
        int[] temp = new int[4];
        for (int i = 0; i < 4; i++)
            temp[i] = arr[4,i];
        return temp;
    }

    public void set_seed()
    {
        int x = (int)Mathf.Ceil(grid_size / 2);
        int z = (int)Mathf.Ceil(grid_size / 2);
        grid[x, z] = new block(x, z, blocks[0], return_dimension(set_rules, 0), 0);
        grid[x, z].set_collapsed();
        grid[x+1, z] = new block(x+1, z, blocks[0], return_dimension(set_rules, 0), 0);
        grid[x+1, z].set_collapsed();
        addQueue(x + 1,z);
        addQueue(x,z + 1);
        addQueue(x - 1,z);
        addQueue(x,z - 1);
    }

    public void addQueue(int x,int z)
    {
        if (x < grid_size && x >= 0 && z < grid_size && z >= 0)
        {
            if (grid[x, z] == null)
            { 
                int[] arr = new int[3];
                arr[0] = x;
                arr[1] = z;
                queue.Enqueue(arr);
            }
        }
    }

    public void execute_queue()
    {
        int[] co_ordinates=queue.Peek();
        List<int> possibilities = check_and_return_identiy(co_ordinates[0], co_ordinates[1]);
        int block_no=rnd.Next(0, possibilities.Count);
        grid[co_ordinates[0],co_ordinates[1]]= new block(co_ordinates[0], co_ordinates[1], blocks[block_no], return_dimension(set_rules, block_no), block_no);
        grid[co_ordinates[0], co_ordinates[1]].set_collapsed();
        addQueue(co_ordinates[0]+1, co_ordinates[1]);
        addQueue(co_ordinates[0], co_ordinates[1]+1);
        addQueue(co_ordinates[0]-1, co_ordinates[1]);
        addQueue(co_ordinates[0], co_ordinates[1]-1);
        queue.Dequeue();
    }
    
    public List<int> check_and_return_identiy(int x,int z)
    {
        int[] check_side = new int[4];
        if (z+1<grid_size)
        {
            if (grid[x,z+1]!=null)
            {
                check_side[0] = set_rules[grid[x, z + 1].identify(),2];
            }
            else
            {
                check_side[0] = 1;
            }
        }
        else
        {
            check_side[0] = 0;
        }

        if (x + 1 <grid_size)
        {
            if (grid[x+1, z ] != null)
            {
                check_side[1] = set_rules[grid[x+1, z].identify(), 3];
            }
            else
            {
                check_side[1] = 1;
            }
        }
        else
        {
            check_side[1] = 0;
        }

        if (z - 1 >= 0)
        {
            if (grid[x, z - 1] != null)
            {
                check_side[2] = set_rules[grid[x, z -1].identify(), 0];
            }
            else
            {
                check_side[2] = 1;
            }
        }
        else
        {
            check_side[2] = 0;
        }

        if (x-1 >= 0)
        {
            if (grid[x-1, z] != null)
            {
                check_side[3] = set_rules[grid[x-1, z].identify(), 1];
            }
            else
            {
                check_side[3] = 1;
            }
        }
        else
        {
            check_side[3] = 0;
        }

        int counter = 0;
        for(int i=0;i<4;i++)
        {
            counter += check_side[i];
        }

        List<int> valid_blocks = new List<int>();

        for(int i=0;i<16;i++)
        {
            int temp_counter = 0;
            for (int j=0;j<4;j++)
            {
                /*if (check_side[j]==1)
                {
                    break;
                }*/
                
                if(check_side[j]==0 && set_rules[i,j]==0)
                {
                    temp_counter ++;
                }
            }
            if(temp_counter==counter)
            {
                valid_blocks.Add(i);
            }
        }
        return valid_blocks;
    }
}
