using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Jint;

public class JS_Runner : MonoBehaviour
{

    private Engine JSengine;
    // Start is called before the first frame update
    void Start()
    {

        JSengine = new Engine();
        JSengine.SetValue("log", new Action<object>(msg => Debug.Log(msg)));
        JSengine.Execute(File.ReadAllText("./Assets/FlowJS/flow.js"));

        JSengine.Execute(@"
        var myVariable = 108;
        log('Hello from Javascript! myVariable = '+myVariable);
      ");

    }

   
}
