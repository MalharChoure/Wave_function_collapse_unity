using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;

public class Threading : MonoBehaviour
{
    // Start is called before the first frame update

    private bool flag = true;

    void Start()
    {
        Thread t1 = new Thread(Func1);
        Thread t2 = new Thread(Func2);

        t1.Start();
        t2.Start();
    }

    void Func1()
    {
        for (int i = 0; i < 100; i++)
        {
            Debug.Log(i);
        }
     
        if (flag)
        {
            flag = false;
            Debug.Log("set the flag to false in func 1");
        }
    }

    void Func2()
    {
        for (int i = 100; i >= 0; i--)
        {
            Debug.Log(i);
        }
        if (flag)
        {
            flag = false;
            Debug.Log("set the flag to false in func 2");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
