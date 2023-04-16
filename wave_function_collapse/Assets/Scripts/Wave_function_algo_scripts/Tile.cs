using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile
{
    public GameObject tile;

    // this stores the edges information of the current tile in the order UP, RIGHT, DOWN LEFT.
    public List<string> edges;

    // this stores all the tiles that can go to the respective sides of this tile
    public List<int> up = new List<int>();
    public List<int> right = new List<int>();
    public List<int> down = new List<int>();
    public List<int> left = new List<int>();

    public Tile(GameObject tile, List<string> edges)
    {
        this.tile = tile;
        this.edges = edges;
    }

    private static bool CompareString(string s1, string s2)
    {
        char[] s2Array = s2.ToCharArray();
        Array.Reverse(s2Array);
        string s2Reversed = new string(s2Array);

        return s1 == s2Reversed;
    }

    // this method will automatically generate the rules for each side of the tile figuring out what tiles can go with which edge of it.
    // for example it will compare the down edges of all the other tiles with it's up edges and if they match then it will consider both of them to be fit.
    public static void GenerateTileRules(List<Tile> allTiles)
    {
        for (int i = 0; i < allTiles.Count; i++)
        {
            Tile tile1 = allTiles[i];

            for (int j = 0; j < allTiles.Count; j++)
            {
                Tile tile2 = allTiles[j];

                /*if (tile1.edges[0] == tile2.edges[2])
                {
                    tile1.up.Add(j);
                }

                if (tile1.edges[1] == tile2.edges[3])
                {
                    tile1.right.Add(j);
                }

                if (tile1.edges[2] == tile2.edges[0])
                {
                    tile1.down.Add(j);
                }

                if (tile1.edges[3] == tile2.edges[1])
                {
                    tile1.left.Add(j);
                }*/

                if (CompareString(tile1.edges[0], tile2.edges[2]))
                {
                    tile1.up.Add(j);
                }

                if (CompareString(tile1.edges[1], tile2.edges[3]))
                {
                    tile1.right.Add(j);
                }

                if (CompareString(tile1.edges[2], tile2.edges[0]))
                {
                    tile1.down.Add(j);
                }

                if (CompareString(tile1.edges[3], tile2.edges[1]))
                {
                    tile1.left.Add(j);
                }
            }
        }
    }

    // this will return intersection of two tiles 
    // for example in {1, 2, 3} and {1, 2} it will return {1, 2}
    public static List<int> ReturnValidOptions(List<int> cellOptions, List<int> constraintOptions)
    {
        List<int> newValidOptions = new List<int>();

        for (int i = 0; i < cellOptions.Count; i++)
        {
            bool found = false;
            for (int j = 0; j < constraintOptions.Count; j++)
            {
                if (cellOptions[i] == constraintOptions[j])
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                newValidOptions.Add(cellOptions[i]);
            }
        }

        //Debug.Log(newValidOptions.Count == 0);
        return newValidOptions;
    }
}
