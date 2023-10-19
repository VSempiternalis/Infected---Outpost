using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class aiInput : MonoBehaviour {
    // private NodeHandler currentNode;
    // private NodeHandler nextNode;
    // private NodeHandler previousNode;
    // private NavMeshAgent agent;
    private Character character;

    private string state;
    private string nextState;
    // private float speed = 3f;
    [SerializeField] private float nodeStopDist = 0.125f;
    
    private float nextSecUpdate = 1;

    private Vector2 waitTimes = new Vector2(0f, 5f);
    private float waitTime;
    private float waitTimer;

    // [SerializeField] private Transform nodes;
    private int currentNodeIndex = 0;
    private NodeHandler previousNode;
    // private NodeHandler currentNode;
    private NodeHandler nextNode;

    private void Start() {
        if(!PhotonNetwork.IsMasterClient) return;

        character = GetComponent<Character>();
        nextSecUpdate = Time.time + 1;
        // agent = GetComponent<NavMeshAgent>();
        // nodes = GameObject.Find("NODES").transform;

        //[] Get closest node
        NodeHandler closestNode = null;
        float closestDist = 1000;
        foreach(NodeHandler node in GameObject.FindObjectsOfType<NodeHandler>()) {
            float dist = Vector3.Distance(transform.position, node.transform.position);
            if(dist < closestDist) {
                closestNode = node;
                closestDist = dist;
            } 
        }
        // currentNode = closestNode;
        previousNode = closestNode;
        nextNode = closestNode;
        // nextNode = previousNode.GetComponent<NodeHandler>().GetNextNode();
        // state = "GettingRandomNode";
        waitTime = Time.time + Random.Range(waitTimes.x, waitTimes.y);
        state = "Waiting";
        // state = "Moving";

        // SetRandomWaitTime();
        // GetNextNode();

        //Get random state
        // int randInt = Random.Range(0, 2);
        // if(randInt == 0) state = "Idle";
        // else state = "Moving";
    }

    private void Update() {
        if(!PhotonNetwork.IsMasterClient) return;

        if(state == "Waiting") Wait();
        else if(state == "GettingNextNode") GetNextNode();
    }

    private void FixedUpdate() {
        if(state == "Moving") Movement();
    }

    private void Wait() {
        // print("Waiting: " + Time.time);
        if(Time.time >= waitTime) state = "GettingNextNode";
    }

    private void GetNextNode() {
        // print("Getting Next Node");
        //Set nodes and state
        previousNode = nextNode;
        if(nextNode == null) nextNode = previousNode;
        else nextNode = previousNode.GetComponent<NodeHandler>().GetNextNode();
        // previousNode = currentNode;
        state = "Moving";
    }

    private void Movement() {
        // print(name + "Moving to: " + nextNode.name + ". Dist: " + Vector3.Distance(transform.position, nextNode.transform.position) + "/" + nodeStopDist);

        // NPC has reached the waypoint
        if(Vector3.Distance(transform.position, nextNode.transform.position) < nodeStopDist) {
            // print("Reached next node!");
            waitTime = Time.time + Random.Range(waitTimes.x, waitTimes.y);
            
            // previousNode = nextNode;
            // nextNode = previousNode.GetComponent<NodeHandler>().GetNextNode();

            // print("State to waiting");
            state = "Waiting";
            // state = "Moving";
        } else {
            character.MoveHead(nextNode.transform.position);
            character.MoveTo(nextNode.transform.position);
        }
    }
}
