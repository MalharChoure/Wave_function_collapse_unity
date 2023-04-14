using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    // whether that cell is collapsed or not
    public bool collapsed = false;

    // each cell in the grid starts with an all the possible options.
    public List<int> options = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
}
