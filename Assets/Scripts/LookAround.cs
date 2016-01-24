using UnityEngine;
using System.Collections;

public class LookAround : MonoBehaviour {
    public float smooth = 2.0F;
    public float tiltAngle = 30.0F;

    private Quaternion startRotation;

    // Use this for initialization
    void Start () {
        startRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        float tiltAroundY = Input.GetAxis("Horizontal") * tiltAngle;
        float tiltAroundX = - Input.GetAxis("Vertical") * tiltAngle;
        Quaternion target = Quaternion.Euler(startRotation.eulerAngles.x + tiltAroundX, startRotation.eulerAngles.y + tiltAroundY, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
    }
}
