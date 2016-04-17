using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour {

	float angle=0;
	float speed=0;
	Vector3 axis;
	// Use this for initialization
	void Start () {
		speed = Random.Range (-50f, 50f);
		axis=new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f));
	}
	
	// Update is called once per frame
	void Update () {
		angle = speed * Time.deltaTime;
		if (angle > 360) {
			angle-=360;
		}
		transform.Rotate(axis,angle);
	}
}
