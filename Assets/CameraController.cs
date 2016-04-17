using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.ImageEffects;

public class CameraController : MonoBehaviour {

	FollowTarget ft;
	LookatTarget lt;
	public RocketController rocket;
	ScreenOverlay so;
	//public Material skyboxBlended;
	//public Material newSkybox;
	// Use this for initialization
	void Start () {
		ft = GetComponent<FollowTarget> ();
		lt = GetComponent<LookatTarget> ();
		so = GetComponent<ScreenOverlay> ();
		//newSkybox = Material.Instantiate (skyboxBlended);
		//RenderSettings.skybox = newSkybox;
	}
	
	// Update is called once per frame
	void Update () {


		if (transform.position.y > 400 && transform.position.y < 1000) {
			RenderSettings.skybox.SetFloat ("_Blend", (transform.position.y-400)/600);
		}
		if (transform.position.y <= 400) {
			RenderSettings.skybox.SetFloat ("_Blend", 0);
		}
		if (transform.position.y >= 1000) {
			RenderSettings.skybox.SetFloat ("_Blend", 1);
		}
		if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
			ft.offset.z += Input.GetAxis ("Mouse ScrollWheel")*20f;
			ft.offset.z = Mathf.Clamp(ft.offset.z,-60,-20);
		}

		if (transform.position.x > 2500 || transform.position.x < -2500) {
			so.intensity = (Mathf.Abs (transform.position.x) - 2500) / 250;
		} else if (transform.position.y > 6600) {
			so.intensity = (Mathf.Abs (transform.position.y) - 6600) / 500;
		}
		else{
			so.intensity = 0;
		}

	}
}
