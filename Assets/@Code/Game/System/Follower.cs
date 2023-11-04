using UnityEngine;

public class Follower : MonoBehaviour {
    public Transform toFollow;
    [SerializeField] private bool isFollowPosX;
    [SerializeField] private bool isFollowPosY;
    [SerializeField] private bool isFollowPosZ;

    private void Start() {
        
    }

    private void LateUpdate() {
        if(!toFollow) return;

        Vector3 newPos = transform.position;
        if(isFollowPosX) newPos.x = toFollow.position.x;
        if(isFollowPosY) newPos.y = toFollow.position.y;
        if(isFollowPosZ) newPos.z = toFollow.position.z;
        transform.position = newPos;
    }
}
