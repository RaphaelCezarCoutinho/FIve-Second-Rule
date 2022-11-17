using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

public class Flow_C : MonoBehaviour
{


    public TextAsset jsonFile;


    dynamic FlowFile;
    JToken publicNode;
    List<JToken> publicEvents;
    JToken globalNode;
    List<JToken> globalEvents;
    JToken flow_marker;

    public static Flow_C instance { get; private set; }


    private void Awake()
    {
        //instance = this ;
    }

    // Start is called before the first frame update
    void Start()
    {

        FlowFile= JsonConvert.DeserializeObject<dynamic>(jsonFile.text);

       // Debug.Log(FlowFile.GetType()+ " The type of flowfile");




        


        publicNode = findWhere( FlowFile["nodeDataArray"], "text", "public");
        publicEvents = nextNodesFrom(publicNode);

        

        globalNode = findWhere(FlowFile["nodeDataArray"], "text", "global");
        globalEvents = nextNodesFrom(globalNode);


        FlowStart();
    }


    void FlowStart() {

        JToken startNode = findWhere(FlowFile["nodeDataArray"], "category", "Start");

        List<JToken> nextNodes = nextNodesFrom(startNode);

        executeNodes(nextNodes);

    }

    JToken findWhere(dynamic countable_list, string property, string value) {



        for (int i = 0; i < countable_list.Count; i++) {

            

            if (countable_list[i][property].ToString() == value) return countable_list[i];
        }

        return null;
    }


    List<JToken> Where(JArray countable_list, string property, string value, string prop2=null, string val2=null)
    {

        var nodes_list = new List<JToken>();

       


        for (int i = 0; i < countable_list.Count; i++)
        {

            

            if(prop2==null && val2 == null)
            {
                

                if (countable_list[i][property].ToString() == value) nodes_list.Add(countable_list[i]);
            }

            else
            {
                if ((countable_list[i][property].ToString() == value) && (countable_list[i][prop2].ToString() == val2)) nodes_list.Add(countable_list[i]);

            }
        }

        return nodes_list;
    }


    List<JToken> pluck(List<JToken> list, string key)
    {

        var return_node_list = new List<JToken>();

        foreach(JToken node in list)
        {
            return_node_list.Add(node[key].ToString());

        }

        return return_node_list;
    }

    List<JToken> nextNodesFrom(JToken fromNode, string fromPort=null) {


        List<JToken> nextKeys;

        if (fromPort == null)
        {

            List<JToken> query_list = Where(FlowFile["linkDataArray"], "from", fromNode["key"].ToString());

            nextKeys = pluck(query_list, "to");
        }

        else {

            List<JToken> query_list = Where(FlowFile["linkDataArray"], "from", fromNode["key"].ToString(), "fromPort", fromPort);

            nextKeys = pluck(query_list, "to");

        }

        var returnNodes= new List<JToken>();

        foreach(string nodekey in nextKeys) {

            JToken temp = findWhere(FlowFile["nodeDataArray"], "key", nodekey);
            returnNodes.Add(temp);

        }

        return returnNodes;
    }

    void executeNodes(List<JToken> execNodes, object[] data=null, bool skipmarker=false) {

        if (execNodes.Count == 1) {

            JToken temp_node = execNodes[0];

            string category;

            if (temp_node["category"] == null) category = "";
            else category = temp_node["category"].ToString();

            

            switch (category)
            {

                

                case "Conditional":

                    if (!skipmarker)  flow_marker = temp_node["key"];

                    dynamic retValueCond = this.exec(temp_node["text"].ToString(), data);

                   

                    string fromPort;

                    if (retValueCond == true) fromPort = "R";
                    else fromPort = "B";

                    List<JToken> nextNodesF = nextNodesFrom(temp_node, fromPort);

                    executeNodes(nextNodesF, data, skipmarker);




                    break;

                case "End": break;

                default:

                    if (!skipmarker) flow_marker = temp_node["key"];

                    var retValue=this.exec( temp_node["text"].ToString(), data);

                  

                    // Don't continue if wait message
                    if (temp_node["text"].ToString() != "wait")
                    {
                        List<JToken> nextNodes = nextNodesFrom(temp_node);

                        executeNodes(nextNodes, data, skipmarker);
                    }
                    else
                    {
                        flow_marker = temp_node["key"];
                    }

                    break;


            }
        
        
        
        }
    
    }


    dynamic exec(string execText, object[] data=null) {


        if (execText.Length > 0) {

            string[] execParts = execText.Split(':');

            object[] parameters= null;

            if (execParts.Length > 1) {

                parameters = new object[] { execParts[1] };
            }

            if (parameters == null) parameters = data;

            Type thisType = this.GetType();
           
            MethodInfo theMethod = thisType
                .GetMethod(execParts[0], BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.Public);
            
            return theMethod.Invoke(this, parameters);

        }

        return null;
    
    }

    void executeEvent(string eventName, object [] data=null) {

        bool skipmarker = true;

        JToken eventNode= findWhere(publicEvents, "text", "event:"+eventName);

        if (eventNode == null) {

            eventNode = findWhere(globalEvents, "text", "event:" + eventName);

        }

        if (eventNode == null) {

            JToken startNode = findWhere(FlowFile["nodeDataArray"], "key", flow_marker.ToString());

            // Get available next nodes
            List<JToken> availNextNodes = nextNodesFrom(startNode);
         
            // Get event node if requested event matches any available ones
            eventNode = findWhere(availNextNodes, "text", "event:" + eventName);
            skipmarker = false;
        }

        if (eventNode!=null)
        {
            List<JToken> nextNodes = nextNodesFrom(eventNode);

            // Execute following nodes
            executeNodes(nextNodes, data, skipmarker);
        }
        else
        {
            Debug.LogError("Event " + eventName + " does not exist at position key " + flow_marker.ToString() + " EVENT NOT FOUND");
        }
    }

    void sayHello(dynamic num) {

        int numVal = Int32.Parse(num);
        

        Debug.Log("Hello number of type "+ numVal.GetType()+" " + numVal + " current flowKey is: " + flow_marker);
        
    }


    void sayGoodBye() {

        Debug.Log("Goodbye everybody" + " current flowKey is: " + flow_marker);
        
    }

    string sayReturnString() {

        Debug.Log("returning String: Hello World" + " current flowKey is: " + flow_marker);
        return "Hello World";
    }

    void throwEx() {

        Debug.Log(" THIS IS AN ERROR MESSAGE" + " current flowKey is: " + flow_marker);
    }

    void wait() {

        //no op

        Debug.Log("WAITING..."+ " current flowKey is: " + flow_marker);
        
    }

    bool branch() {

        bool b = false;
        Debug.Log("Branch: currently returning : "+b+ " current flowKey is: " + flow_marker);
        
        return b;
    }

    void autoNext(dynamic miliseconds) {

        Debug.Log("autoNext started with time of " + miliseconds);

        int numVal = Int32.Parse(miliseconds);

        StartCoroutine(WaitAndNext(numVal));
    
    }


    IEnumerator WaitAndNext( int miliseconds)
    {
        

        float time = (float)miliseconds;

        time = miliseconds / 1000;

        

        // suspend execution for 5 seconds
        yield return new WaitForSeconds(time);

        Debug.Log("WaitAndNext Fired after seconds: " + time + "  " + time.GetType());

        executeEvent("NEXT");
    }


   public void beforeEnd() {

        Debug.Log("Last Node before end " + " current flowKey is: " + flow_marker);
    }

}
