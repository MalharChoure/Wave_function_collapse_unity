using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using Pathfinding;
public class chunk_script : MonoBehaviour
{//harsh
    // added by harsh to animate the blocks and tiles

    // blocks
    private static int curr_block_list_index = 0;
    private static int max_block_list_index = CreateChildGameObjects.childGameObjectsCount;

    private static int curr_block_count = 0;
    private static int blocks_index_change_threshold = 100;

    // tiles
    private static int curr_tile_list_index = 0;
    private static int max_tile_list_index = CreateTilesChildGO.childGameObjectsCount;

    private static int curr_tile_count = 0;
    private static int tiles_index_change_threshold = 50;


    //The integers required to fill out the room blocks.
    public int grid_size=10;
    public int padding=2;
    public int max_room_size;
    public int no_of_floors=2;
    public int max_no_of_rooms = 5;
    public int min_no_of_rooms = 3;

    //the blender blocks are called here
    public GameObject[] blend_blocks= new GameObject[6];

    //scale of the room
    public int room_x_scale = 1;
    public int room_y_scale = 1;
    public int room_z_scale = 1;

    //block_offset

    //parent object to save batches.

    // randomiser added under the name rnd and a random seed is invoked every time.
    private System.Random rnd = new System.Random();

    // central room 
    public int central_room_size=5;
    public int central_room_size_x = 5;
    public int central_room_size_y = 5;
    public int central_room_coordinate_x;

    //Grid creation
    private block[,,] chunks;

    //The tiles that are to be actually placed
    public GameObject[] tiles;
    List<GameObject> tiless;

    //This is to create a queue for the doors spawner
    public Queue<int[]> queue = new Queue<int[]>();

    public bool run = true;
    public bool run2 = true;

    int noOfCoroutinesRunning = 0;
    public bool isGridGenerated = false;
    class block
    {
        private int x;
        private int y;
        private int z;
        public GameObject current;
        public bool collapsed = false;
        int room_x;
        int room_y;
        int room_z;
        public GameObject created;

        public float block_offset_x = 0f;
        public float block_offset_y = 0f;
        public float block_offset_z = 0f;

        public int id;

        // modifying this block_parent to block_parent_list which will store all the child gameObject onto which the chunks will be combined
        public static GameObject block_parent_list = GameObject.Find("BlocksP");
        public static GameObject tile_parent_list = GameObject.Find("TilesP");

        //public static GameObject block_parent= GameObject.Find("Blocks");
        //public static GameObject ground_parent=GameObject.Find("Tiles");
        public static GameObject stairs_parent = GameObject.Find("Stairsv2");
        public static GameObject doors_parent = GameObject.Find("Doors");


        public block(int x,int y, int z, GameObject obj, int room_x, int room_y, int room_z, int identity)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            current = obj;
            this.room_x = room_x;
            this.room_y = room_y;
            this.room_z = room_z;
            id = identity;
        }

        public void set_collapsed()
        {
            collapsed = true;
            instantiate_prefab();
        }

        public void un_collapse()
        {
            collapsed= false;
            if(created!=null)
            Destroy(created);
        }

        private bool is_collapsed()
        {
            return collapsed;
        }

        //0 is wall 
        //1 is floor
        //2 is empty
        //3 is stairs 
        //4 is doors
        private void instantiate_prefab()
        {
            Vector3 a = new Vector3((x+block_offset_x)*room_x, (z+block_offset_z)*room_z, (y+block_offset_y)*room_y); //cause unity has 
            
            if (id == 1)
            {
                curr_tile_count++;

                if (curr_tile_count >= tiles_index_change_threshold)
                {
                    curr_tile_count = 0;
                    curr_tile_list_index = (curr_tile_list_index + 1) % max_tile_list_index;
                }

                created = Instantiate(current, a, Quaternion.identity, block.tile_parent_list.transform.Find(curr_tile_list_index.ToString()));
                Vector3 scale_of_object = created.transform.localScale;
                created.transform.localScale = new Vector3(room_x*scale_of_object.x, room_z*scale_of_object.z, room_y*scale_of_object.y);
            }
            else if (id == 0)
            {
                chunk_script.curr_block_count++;
                if (chunk_script.curr_block_count >= chunk_script.blocks_index_change_threshold)
                {
                    chunk_script.curr_block_count = 0;
                    chunk_script.curr_block_list_index = (chunk_script.curr_block_list_index + 1) % chunk_script.max_block_list_index;
                }

                created = Instantiate(current, a, Quaternion.identity, block.block_parent_list.transform.Find(chunk_script.curr_block_list_index.ToString()));
                Vector3 scale_of_object = created.transform.localScale;
                created.transform.localScale = new Vector3(room_x * scale_of_object.x, room_z * scale_of_object.z, room_y * scale_of_object.y);
            }
            else if(id==3)
            {
                //Debug.Log("HEre");
                created = Instantiate(current, a, Quaternion.Euler(new Vector3(-90,0,0)), block.stairs_parent.transform);
                Vector3 scale_of_object = created.transform.localScale;
                created.transform.localScale = new Vector3(room_x * scale_of_object.x, room_z * scale_of_object.z, room_y * scale_of_object.y);
            }
            else if(id==4)
            {
                created = Instantiate(current, a, Quaternion.Euler(new Vector3(-90, 0, 0)),block.doors_parent.transform);
                Vector3 scale_of_object = created.transform.localScale;
                created.transform.localScale = new Vector3(room_x * scale_of_object.x, room_z * scale_of_object.z, room_y * scale_of_object.y);
            }
            /*else if (id == 2)
            {
                Debug.Log("HEre");
                created = Instantiate(current, a, Quaternion.identity);
                //created.transform.localScale = new Vector3(room_x, room_z, room_y);
            }*/
            else
            {
            }
        }

        public void rotate(int rot_x,int rot_y,int rot_z)
        {
            created.transform.rotation = Quaternion.Euler(new Vector3(rot_x,rot_z,rot_y));
        }
    }

    private void Start()
    {
        //Debug.Log(GameObject.Find("BlocksP").transform.childCount);

        chunks = new block[grid_size, grid_size, no_of_floors];
        central_room_coordinate_x = Mathf.Abs(grid_size / 2);
        central_room_size = central_room_size > grid_size ?(grid_size/2)-padding:central_room_size;
        
        floors_creator();
        
        cover_rest();
        //central_room_creator();
        while (queue.Any())
        {
            //execute_queue();///copy the queue from here
            queueexecute();
        }
        //cover_rest();
        //change_orientation_of_blocks();
        central_room_creator();
        change_orientation_of_blocks();

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
        int step = 1;
        //Debug.Log(central_room_coordinate_x);
        for (int k = 0; k < no_of_floors; k++)
        {
            //Debug.Log("here 1");
            for (int i = central_room_coordinate_x - central_room_size_x /*- step*/ ; i < central_room_coordinate_x + central_room_size_x /*+step*/; i++)
            {
               // Debug.Log(i);
                for (int j = central_room_coordinate_x - central_room_size_y /*- step*/ ; j < central_room_coordinate_x + central_room_size_y /*+step*/; j++)
                {
                    //Debug.Log(j);
                    chunks[i, j, k].un_collapse();
                    chunks[i, j, k] = new block(i, j, k, tiles[k==0?1:2],room_x_scale,room_y_scale,room_z_scale, k == 0 ? 1 : 2);
                    chunks[i, j, k].set_collapsed();
                }
            }
            step = step + 1;
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

    private void room_collapse(int floor) // I am not at liberty to talk about this
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
                    chunks[i, j, floor] = new block(i, j, floor, tiles[1], room_x_scale, room_y_scale, room_z_scale,1);
                    chunks[i, j, floor].set_collapsed();
                }
            }
        }
    }

    public void execute_queue()
    {
        int[] coordinate = queue.Peek();
        float xdirection = ((grid_size / 2) - coordinate[0]) / (grid_size / 2);
        float ydirection = ((grid_size / 2) - coordinate[1]) / (grid_size / 2);
        int z_path = coordinate[2];
        int x_create_direction = xdirection >= 0 ? 1 : -1;
        int y_create_direction = ydirection >= 0 ? 1 : -1;
        int x_path = coordinate[0];
        int y_path = coordinate[1];
        int x_origin = coordinate[0];
        int y_origin = coordinate[1];
        int z_origin = coordinate[2];

        bool flip = false;
        while (x_path < grid_size - padding && x_path > padding)
        {
            if (chunks[x_path, coordinate[1],z_path].current == tiles[1] && flip)
            {
                break;
            }
            if (chunks[x_path, coordinate[1], z_path].current == tiles[1])
            {
                flip = true;
            }
               chunks[x_path, coordinate[1], z_path].un_collapse();
               chunks[x_path, coordinate[1], z_path] = new block(x_path, coordinate[1], z_path, tiles[1], room_x_scale, room_y_scale, room_z_scale, 1);
               chunks[x_path, coordinate[1], z_path].set_collapsed();
               x_path = x_path + x_create_direction;
            
        }
        flip = false;
        while (y_path < grid_size - padding && y_path > padding)
        {
            if (chunks[coordinate[0], y_path,z_path].current == tiles[1] && flip)
            {
                break;
            }
            if (chunks[coordinate[0], y_path,z_path].current == tiles[1])
            {
                flip = true;
            }
                chunks[coordinate[0], y_path, z_path].un_collapse();
                chunks[coordinate[0], y_path, z_path] = new block(coordinate[0], y_path, z_path, tiles[1], room_x_scale, room_y_scale, room_z_scale, 1);
                chunks[coordinate[0], y_path, z_path].set_collapsed();
                y_path = y_path + y_create_direction;
        }
        queue.Dequeue();

        if (z_origin != no_of_floors - 1)
        {
            if (chunks[x_origin, y_origin, z_origin + 1].id == 1)
            {
                //Debug.Log("This is executed");
                chunks[x_origin,y_origin, z_origin].un_collapse();
                chunks[x_origin, y_origin, z_origin] = new block(x_origin, y_origin, z_origin, tiles[3], room_x_scale, room_y_scale, room_z_scale, 3);
                chunks[x_origin, y_origin, z_origin].set_collapsed();
                chunks[x_origin, y_origin, z_origin + 1].un_collapse();
                chunks[x_origin, y_origin, z_origin + 1] = new block(x_origin, y_origin, z_origin+1, tiles[2], room_x_scale, room_y_scale, room_z_scale, 2);
                chunks[x_origin,y_origin, z_origin+1].set_collapsed();
            }
        }
    }


    private IEnumerator ChunkFall(Transform chunk)
    {
        float chunk_fall_speed = UnityEngine.Random.Range(40, 150);
        while (chunk.transform.position.y > 0)
        {
            chunk.position -= new Vector3(0, chunk_fall_speed * Time.deltaTime, 0);
            yield return null;
        }
        chunk.transform.position = Vector3.zero;
    }

    void MakeChunksFallFromSky()
    {
        GameObject BlocksP = GameObject.Find("BlocksP");

        foreach (Transform blocks in BlocksP.transform)
        {
            blocks.transform.position = new Vector3(0, 1000, 0);
            StartCoroutine(ChunkFall(blocks));
        }
    }

    IEnumerator CombineTilesAgainAfterFall()
    {
        GameObject TilesP2 = GameObject.Find("TilesP2");

        foreach (Transform t in TilesP2.transform)
        {
            t.GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
            yield return null;
        }
    }

    IEnumerator ScanGraph()
    {
        foreach (Progress p in AstarData.active.ScanAsync())
        {
            yield return null;
        }
    }

    IEnumerator TileFall(Transform tile)
    {
        noOfCoroutinesRunning++;
        float tile_fall_speed = UnityEngine.Random.Range(40, 150);
        while (tile.transform.position.y >= 0)
        {
            tile.position -= new Vector3(0, tile_fall_speed * Time.deltaTime, 0);
            yield return null;
        }
        tile.transform.position = Vector3.zero;
        noOfCoroutinesRunning--;
        if (noOfCoroutinesRunning==0)
        {

            StartCoroutine(ScanGraph());
            StartCoroutine(CombineTilesAgainAfterFall());

            isGridGenerated = true;
            GameObject.Find("PlatformForPlayer").transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void MakeFloorFallFromSky()
    {
        GameObject TilesP2 = GameObject.Find("TilesP2");

        foreach (Transform tile in TilesP2.transform)
        {
            tile.transform.position = new Vector3(0, 400, 0);
            StartCoroutine(TileFall(tile));
        }
    }




    void CubeSegments()
    {

        // since the default grid size is 100, i am making 100 (10 x 10) blocks so that they can fall from sky in blocked fashion instead of random.
        // this needs to be changed if the grid size if changed or if it is not a multiple of 10

        GameObject BlocksP2 = GameObject.Find("BlocksP2");
        GameObject TilesP2 = GameObject.Find("TilesP2");

        // this will run (grid_size x grid_size) times
        for (int i = 0; i < grid_size; i += 10)
        {
            for (int j = 0; j < grid_size; j += 10)
            {
                string id = (i.ToString() + "_" + j.ToString());

                GameObject go = new GameObject(id);
                go.transform.parent = BlocksP2.transform;
                go.AddComponent<Mesh_combiner_script_call>();

                GameObject go_floor = new GameObject(id);
                go_floor.transform.parent = TilesP2.transform;
                go_floor.AddComponent<Mesh_combiner_script_call>();

                for (int ii = i; ii < i + 10; ii++)
                {
                    for (int jj = j; jj < j + 10; jj++)
                    {
                        for (int k = 0; k < no_of_floors; k++)
                        {
                            if (chunks[ii, jj, k].id == 0)
                            {
                                GameObject created = chunks[ii, jj, k].created;
                                created.transform.parent = go.transform;
                            }else if (chunks[ii, jj, k].id == 1)
                            {
                                GameObject created = chunks[ii, jj, k].created;
                                created.transform.parent = go_floor.transform;
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator ChunkFallV2(Transform blockChunk)
    {
        float fallSpeed = UnityEngine.Random.Range(40, 150);

        blockChunk.position = new Vector3(0, 500, 0);

        while (blockChunk.position.y > 0)
        {
            blockChunk.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);
            yield return null;
        }
        blockChunk.transform.position = Vector3.zero;
    }

    void MakeChunksFallFromSkyV2()
    {
        GameObject BlocksV2 = GameObject.Find("BlocksP2");

        foreach (Transform BlockChunk in BlocksV2.transform)
        {
            StartCoroutine(ChunkFallV2(BlockChunk));
        }
    }

    void CombineCubeSegments()
    {
        GameObject BlocksP2 = GameObject.Find("BlocksP2");

        for (int i = 0; i < BlocksP2.transform.childCount; i++)
        {
            BlocksP2.transform.GetChild(i).gameObject.GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
        }

        

        /*for (int i = 0; i < max_tile_list_index; i++)
        {
            GameObject.Find("TilesP").transform.Find(i.ToString()).gameObject.GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
        }*/

        /*foreach (Transform cubeSegment in BlocksP2.transform)
        {
            cubeSegment.gameObject.GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
        }*/
    }

    private void Update()
    {

        if (run)
        {
            run = false;
            //call_mesh_combiner();
            //MakeChunksFallFromSky();
            //MakeFloorFallFromSky();

            // this will break the whole mesh into 10 x 10 cubes so that it looks good when they fall from the sky
            CubeSegments();
            CombineCubeSegments();
            MakeChunksFallFromSkyV2();
            MakeFloorFallFromSky();

        }
    }

    private void cover_rest()
    {
        for(int i=0;i<grid_size;i++)
        {
            for(int j=0;j<grid_size;j++)
            {
                for(int k=0;k<no_of_floors;k++)
                {
                    if (chunks[i, j,k] == null || chunks[i,j,k].id==0)
                    {
                        int[] orient = new int[4];
                        if(chunks[i - 1 > 0 ? i - 1 : i, j, k]==null)
                            orient[0] = 0;
                        else if (chunks[i-1>0?i-1:i,j,k].id==1)
                            orient[0] = 1;
                        else
                            orient[0] = 0;
                        if (chunks[i, j - 1 > 0 ? j - 1 : j, k] == null)
                            orient[1] = 0;
                        else if (chunks[i,j - 1 > 0 ? j - 1 : j, k].id == 1)
                            orient[1] = 1;
                        else
                            orient[1] = 0;
                        if (chunks[i + 1 < grid_size ? i + 1 : i, j, k] == null)
                            orient[2] = 0;
                        else if (chunks[i + 1 < grid_size ? i + 1 : i, j, k].id == 1)
                            orient[2] = 1;
                        else
                            orient[2] = 0;
                        if (chunks[i, j + 1 > grid_size ? j + 1 : j, k] == null)
                            orient[3] = 0;
                        else if (chunks[i, j + 1 > 0 ? j + 1 : j, k].id == 1)
                            orient[3] = 1;
                        else
                            orient[3] = 0;

                        int sum = orient[0] * 1000 + orient[1] * 100 + orient[2] * 10 + orient[3] ;
                        GameObject obj = blend_blocks[0];
                        int[] rot = new int[3];
                        //Debug.Log(sum);
                        switch (sum)
                        {
                            case 0000:
                                obj = blend_blocks[5];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            case 1000:
                                obj = blend_blocks[0];
                                rot[0] = 0;
                                rot[1] = 90;
                                rot[2] = 0; 
                                break;

                            case 0100:
                                obj = blend_blocks[0];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            case 0010:
                                obj = blend_blocks[0];
                                rot[0] = 0;
                                rot[1] = -90;
                                rot[2] = 0;
                                break;

                            case 0001:
                                obj = blend_blocks[0];
                                rot[0] = 0;
                                rot[1] = -180;
                                rot[2] = 0;
                                break;

                            case 1100:
                                obj = blend_blocks[1];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            case 0110:
                                obj = blend_blocks[1];
                                rot[0] = 0;
                                rot[1] = -90;
                                rot[2] = 0;
                                break;

                            case 0011:
                                obj = blend_blocks[1];
                                rot[0] = 0;
                                rot[1] = -180;
                                rot[2] = 0;
                                break;

                            case 1001:
                                obj = blend_blocks[1];
                                rot[0] = 0;
                                rot[1] = 90;
                                rot[2] = 0;
                                break;

                            case 1010:
                                obj = blend_blocks[4];
                                rot[0] = 0;
                                rot[1] = 90;
                                rot[2] = 0;
                                break;

                            case 0101:
                                obj = blend_blocks[4];
                                rot[0] = 0;
                                rot[1] = 90;
                                rot[2] = 0;
                                break;

                            case 1110:
                                obj = blend_blocks[2];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            case 0111:
                                obj = blend_blocks[2];
                                rot[0] = 0;
                                rot[1] = -90;
                                rot[2] = 0;
                                break;

                            case 1011:
                                obj = blend_blocks[2];
                                rot[0] = 0;
                                rot[1] = -180;
                                rot[2] = 0;
                                break;

                            case 1101:
                                obj = blend_blocks[2];
                                rot[0] = 0;
                                rot[1] = 90;
                                rot[2] = 0;
                                break;

                            case 1111:
                                obj = blend_blocks[3];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            default:
                                break;
                        }
                        chunks[i, j, k] = new block(i, j, k, obj, room_x_scale, room_y_scale, room_z_scale,0);
                        chunks[i, j, k].set_collapsed();
                        chunks[i, j, k].rotate(rot[0], rot[2], rot[1]);
                    }
                }
                
            }
        }      
    }
    
    private void call_mesh_combiner()
    {
        for (int i = 0; i < max_block_list_index; i++)
        {
            GameObject.Find("BlocksP").transform.Find(i.ToString()).gameObject.GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
        }

        for (int i = 0; i < max_tile_list_index; i++)
        {
            GameObject.Find("TilesP").transform.Find(i.ToString()).gameObject.GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
        }
        //GameObject.Find("Tiles").GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();

        //GameObject.Find("Stairs").GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
        //GameObject.Find("Doors").GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();

        //GameObject.Find("Stairsv2").GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
        //GameObject.Find("Doors").GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
        //GameObject.Find("Tiles").GetComponent<Mesh_combiner_script_call>().call_mesh_combiner();
    }

    IEnumerator call_plane_mesh_combiner()
    {
        new WaitForSeconds(1f);
        
        yield return null; 
    }

    private void change_orientation_of_blocks()
    {
        for (int i = 1; i < grid_size-1; i++)
        {
            for (int j = 1; j < grid_size-1; j++)
            {
                for (int k = 0; k < no_of_floors; k++)
                {
                    if (chunks[i, j, k].id == 0)
                    {
                        
                        int[] orient = new int[4];
                        /*if (chunks[i - 1 > 0 ? i - 1 : i, j, k] == null)
                            orient[0] = 0;*/
                        if (chunks[i - 1 > 0 ? i - 1 : i, j, k].id == 1)
                            orient[0] = 1;
                        else
                            orient[0] = 0;
                        /*if (chunks[i, j - 1 > 0 ? j - 1 : j, k] == null)
                            orient[1] = 0;*/
                        if (chunks[i, j - 1 > 0 ? j - 1 : j, k].id == 1)
                            orient[1] = 1;
                        else
                            orient[1] = 0;
                        /*if (chunks[i + 1 < grid_size ? i + 1 : i, j, k] == null)
                            orient[2] = 0;*/
                        if (chunks[i + 1 < grid_size ? i + 1 : i, j, k].id == 1)
                            orient[2] = 1;
                        else
                            orient[2] = 0;
                        /*if (chunks[i, j + 1 > grid_size ? j + 1 : j, k] == null)
                            orient[3] = 0;*/
                        if (chunks[i, j + 1 > 0 ? j + 1 : j, k].id == 1)
                            orient[3] = 1;
                        else
                            orient[3] = 0;
                        Debug.Log(orient[0] + "" + orient[1] + "" + orient[2] + "" + orient[3] + "");
                        int sum = orient[0] * 1000 + orient[1] * 100 + orient[2] * 10 + orient[3];
                        GameObject obj = blend_blocks[0];
                        int[] rot = new int[3];
                        
                        switch (sum)
                        {
                            case 0000:
                                obj = blend_blocks[5];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            case 1000:
                                obj = blend_blocks[0];
                                rot[0] = 0;
                                rot[1] = 90;
                                rot[2] = 0;
                                break;

                            case 0100:
                                obj = blend_blocks[0];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            case 0010:
                                obj = blend_blocks[0];
                                rot[0] = 0;
                                rot[1] = -90;
                                rot[2] = 0;
                                break;

                            case 0001:
                                obj = blend_blocks[0];
                                rot[0] = 0;
                                rot[1] = -180;
                                rot[2] = 0;
                                break;

                            case 1100:
                                obj = blend_blocks[1];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            case 0110:
                                obj = blend_blocks[1];
                                rot[0] = 0;
                                rot[1] = -90;
                                rot[2] = 0;
                                break;

                            case 0011:
                                obj = blend_blocks[1];
                                rot[0] = 0;
                                rot[1] = -180;
                                rot[2] = 0;
                                break;

                            case 1001:
                                obj = blend_blocks[1];
                                rot[0] = 0;
                                rot[1] = 90;
                                rot[2] = 0;
                                break;

                            case 1010:
                                obj = blend_blocks[4];
                                rot[0] = 0;
                                rot[1] = 90;
                                rot[2] = 0;
                                break;

                            case 0101:
                                obj = blend_blocks[4];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            case 1110:
                                obj = blend_blocks[2];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            case 0111:
                                obj = blend_blocks[2];
                                rot[0] = 0;
                                rot[1] = -90;
                                rot[2] = 0;
                                break;

                            case 1011:
                                obj = blend_blocks[2];
                                rot[0] = 0;
                                rot[1] = -180;
                                rot[2] = 0;
                                break;

                            case 1101:
                                obj = blend_blocks[2];
                                rot[0] = 0;
                                rot[1] = 90;
                                rot[2] = 0;
                                break;

                            case 1111:
                                obj = blend_blocks[3];
                                rot[0] = 0;
                                rot[1] = 0;
                                rot[2] = 0;
                                break;

                            default:
                                break;
                        }
                        chunks[i, j, k].un_collapse();
                        chunks[i, j, k] = new block(i, j, k, obj, room_x_scale, room_y_scale, room_z_scale, 0);
                        chunks[i, j, k].set_collapsed();
                        chunks[i, j, k].rotate(rot[0], rot[2], rot[1]);
                    }
                }

            }
        }
    }

    public void queueexecute()
    {
        int[] coordinate = queue.Peek();
        float xdirection = ((grid_size / 2) - coordinate[0]) / (grid_size / 2);
        float ydirection = ((grid_size / 2) - coordinate[1]) / (grid_size / 2);
        int x_create_direction = xdirection >= 0 ? 1 : -1;
        int y_create_direction = ydirection >= 0 ? 1 : -1;
        int x_path = coordinate[0];
        int y_path = coordinate[1];
        bool flip = false;
        while (x_path < grid_size - padding && x_path > padding)
        {
            if (chunks[x_path,coordinate[1],coordinate[2]].id!=1)
            {
                flip = true;
            }
            if(flip)
            {
                if(chunks[x_path, coordinate[1], coordinate[2]].id==1)
                {
                    /*chunks[x_path, coordinate[1], coordinate[2]].un_collapse();
                    chunks[x_path, coordinate[1], coordinate[2]] = new block(x_path, coordinate[1], coordinate[2], tiles[4], room_x_scale, room_y_scale, room_z_scale, 4);
                    chunks[x_path, coordinate[1], coordinate[2]].set_collapsed();
                    chunks[coordinate[0], y_path, coordinate[2]].rotate(-90, 0, x_create_direction==-1?180:0);
                    x_path = x_path + x_create_direction;*/
                    break;
                }
            }
            chunks[x_path, coordinate[1], coordinate[2]].un_collapse();

            chunks[x_path, coordinate[1], coordinate[2]] = new block(x_path, coordinate[1], coordinate[2], tiles[1], room_x_scale, room_y_scale, room_z_scale, 1);
            chunks[x_path, coordinate[1], coordinate[2]].set_collapsed();

            x_path = x_path + x_create_direction;
        }
        flip = false;

        while(y_path < grid_size - padding && y_path > padding)
        {
            if (chunks[coordinate[0], y_path, coordinate[2]].id != 1)
            {
                flip = true;
            }
            if (flip)
            {
                if (chunks[coordinate[0], y_path, coordinate[2]].id == 1)
                {
                    /*chunks[coordinate[0], y_path, coordinate[2]].un_collapse();
                    chunks[coordinate[0], y_path, coordinate[2]] = new block(coordinate[0], y_path, coordinate[2], tiles[4], room_x_scale, room_y_scale, room_z_scale, 4);
                    chunks[coordinate[0], y_path, coordinate[2]].set_collapsed();
                    chunks[coordinate[0], y_path, coordinate[2]].rotate(-90,0,-y_create_direction*90);*/
                    y_path = y_path + y_create_direction;
                    break;
                }
            }
            chunks[coordinate[0], y_path, coordinate[2]].un_collapse();
            chunks[coordinate[0], y_path, coordinate[2]] = new block(coordinate[0], y_path, coordinate[2], tiles[1], room_x_scale, room_y_scale, room_z_scale, 1);
            chunks[coordinate[0], y_path, coordinate[2]].set_collapsed();
            y_path = y_path + y_create_direction;
        }

        queue.Dequeue();

        if (coordinate[2] != no_of_floors - 1)
        {
            if (chunks[coordinate[0], coordinate[1], coordinate[2] + 1].id == 1)
            {
                //Debug.Log("This is executed");
                chunks[coordinate[0], coordinate[1], coordinate[2]].un_collapse();
                chunks[coordinate[0], coordinate[1], coordinate[2]] = new block(coordinate[0], coordinate[1], coordinate[2], tiles[3], room_x_scale, room_y_scale, room_z_scale, 3);
                chunks[coordinate[0], coordinate[1], coordinate[2]].set_collapsed();
                chunks[coordinate[0], coordinate[1], coordinate[2] + 1].un_collapse();
                chunks[coordinate[0], coordinate[1], coordinate[2] + 1] = new block(coordinate[0], coordinate[1], coordinate[2] + 1, tiles[2], room_x_scale, room_y_scale, room_z_scale, 2);
                chunks[coordinate[0], coordinate[1], coordinate[2] + 1].set_collapsed();
            }
        }
    }

    public List<GameObject> GetSpawnPoints(Transform player, int numberOfEnemies, int minEnemyDistance, int maxEnemeyDistance)
    {

        Vector3 playerWorldPoition = player.TransformPoint(Vector3.zero);
        int count = 0;
        tiless = new List<GameObject>();

        foreach (block b in chunks)
        {
            count++;
            if (b.id == 1 &&
                (Vector3.Distance(b.created.transform.position, playerWorldPoition) < maxEnemeyDistance && Vector3.Distance(b.created.transform.position, playerWorldPoition) > minEnemyDistance))
            {
                tiless.Add(b.created);
            }
        }

        List<float> distances = new List<float>();
        foreach (GameObject b in tiless)
        {
            float distance = Mathf.Abs(b.transform.position.y - playerWorldPoition.y);
            distances.Add(distance);
        }

        float maxDistance = distances.Count > 0 ? distances.Max() : 1f;
        List<float> normalizedDistances = distances.Select(d => maxDistance - d).ToList();


        List<float> weights = normalizedDistances.Select(d => Mathf.Exp(d)).ToList();
        List<GameObject> selectedObjects = new List<GameObject>();

        for (int i = 0; i < numberOfEnemies; i++)
        {
            float totalWeight = weights.Sum();
            float randNum = UnityEngine.Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;

            for (int j = 0; j < tiless.Count; j++)
            {
                cumulativeWeight += weights[j];
                if (randNum <= cumulativeWeight)
                {
                    selectedObjects.Add(tiless[j]);
                    break;
                }
            }
        }

        return selectedObjects;
    }

}