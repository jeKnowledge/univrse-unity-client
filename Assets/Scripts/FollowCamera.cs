using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

    private float relativeDistance = 2/5f;
    private float smoothness = 2f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        float distance = (Camera.main.transform.position - transform.position).magnitude;
        Vector3 target = Camera.main.transform.position + Camera.main.transform.rotation * new Vector3(0,0,distance*relativeDistance);
        Quaternion targetAngle = Quaternion.LookRotation(target - transform.position) * Quaternion.AngleAxis(90, Vector3.right);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * smoothness);
    }
}
