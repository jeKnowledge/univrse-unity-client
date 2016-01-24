using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {

    public GameObject orbitCenter;
    public float orbitPeriod;

    private Vector3 rotationAxis;
    private Vector3 orbitPosition;

	// Use this for initialization
	void Start () {
        
	}
	
    public void setupAxis(float angleXY, float angleYZ)
    {
        orbitPosition = transform.position - orbitCenter.transform.position;

        //Set rotation axis
        Vector3 firstRot = Quaternion.AngleAxis(angleXY, Vector3.up) * Vector3.forward;
        Vector3 secondRot = Quaternion.AngleAxis(angleYZ, Vector3.left) * firstRot;
        if (angleXY == 0){
            rotationAxis = Vector3.Cross(firstRot, Vector3.up);
        }else if(angleYZ == 0){
            rotationAxis = Vector3.Cross(Quaternion.AngleAxis(angleYZ, Vector3.right) * Vector3.forward, Vector3.right);
        }else{
            rotationAxis = Vector3.Cross(firstRot, secondRot);
        }
    }

	void FixedUpdate () {
        float tickRotation = 360 / orbitPeriod * Time.fixedDeltaTime;

        orbitPosition = Quaternion.AngleAxis(tickRotation, rotationAxis) * orbitPosition;
        transform.position = orbitCenter.transform.position + orbitPosition;
	}
}
