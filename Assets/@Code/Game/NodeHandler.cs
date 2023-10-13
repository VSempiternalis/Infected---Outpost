using UnityEngine;
using System.Collections.Generic;

public class NodeHandler : MonoBehaviour {
    [SerializeField] private List<NodeHandler> connections;

    private void Start() {
        
    }

    private void Update() {
        
    }

    public NodeHandler GetNextNode() {
        // print("Get next node");
        int randInt = Random.Range(0, connections.Count);
        NodeHandler nextNode = connections[randInt];

        return nextNode;
    }

    private void OnDrawGizmos() {
        // Check if the currentNode is assigned
        if(connections.Count > 0) {
            foreach(NodeHandler connection in connections) {
            // Draw a line between the AI's position and the currentNode's position
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(transform.position, connection.transform.position);
            }
        }
    }
}
