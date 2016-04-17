using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour {

	public GameObject MainAfterburner;
	public GameObject LeftAfterburner;
	public GameObject RightAfterburner;
	public GameObject RocketBody;
	public float currentForce=0;
	public float MaxForce=1000;
	public float initialForce=500;
	public float MaxSteeringForce=200;
	public float currentSteeringForce=0;
	public float fuel=1000;
	float maxFuel=10000;
	public bool started=false;
	public float forceMultiplier=100;
	public float fuelConsumptionRate=1;
	public bool alive = true;
	public GameObject explosion;

	// Use this for initialization
	void Start () {
		scaleAfterburnerParticles(MainAfterburner, 0.01f);
		scaleAfterburnerParticles(LeftAfterburner, 0.01f);
		scaleAfterburnerParticles(RightAfterburner, 0.01f);
		maxFuel = fuel;
		GetComponent<Renderer> ().enabled = true;
		foreach(MeshRenderer r in RocketBody.GetComponentsInChildren<MeshRenderer>())
		{
			r.enabled=true;

		}
	}

	void OnCollisionEnter (Collision col)
	{
		if (GetComponent<Rigidbody> ().velocity.magnitude > 1) {
			die ();
		}
	}
	void OnCollisionStay (Collision col)
	{
		if (GetComponent<Rigidbody> ().velocity.magnitude > 1) {
			die ();
		}
	}

	public void die()
	{
		if (!alive) {
			return;
		}
		Start ();
		foreach(MeshRenderer r in RocketBody.GetComponentsInChildren<MeshRenderer>())
		{
			r.enabled=false;
			Debug.Log(r);

		}
		//RocketBody.SetActive (false);
		alive=false;

		currentForce = 0;
		currentSteeringForce = 0;
		this.fuel = 0;
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		GetComponent<Renderer> ().enabled = false;
		GameObject go = (GameObject)GameObject.Instantiate (explosion, this.transform.position,Quaternion.identity);
		Destroy (go, 5);
	}

	void reset()
	{
		Start ();
		currentForce = 0;
		currentSteeringForce = 0;
		this.fuel = 1000;
		GetComponent<Rigidbody>().isKinematic=true;
		started = false;
		this.transform.position =new Vector3(0, 13, 0);
		this.transform.eulerAngles = Vector3.zero;
		alive = true;

	}

	void scaleAfterburnerParticles(GameObject AfterburnerParticleSystem, float scale)
	{
		if (scale <= 0.01f) {
			scale=0.01f;
		}
		AfterburnerParticleSystem.GetComponent<ParticleSystem> ().startSize = scale*Random.Range(1.2f,1.4f);
		AfterburnerParticleSystem.transform.FindChild ("Glow").GetComponent<ParticleSystem> ().startSize =  scale*Random.Range(4f,6f);
	}

	void OnGUI () {
		string str = "Altitude: " + transform.position.y + " m\n" +
			"Speed: " + GetComponent<Rigidbody> ().velocity.magnitude + " m/s\n" +
			"Thrust: " + currentForce + "/" + MaxForce + "\n" +
			"\n";
		if (!started) {
			str+="Reach 7000 m\n" +
				"space - start engine\n" +
				"R - release rocket\n"+
				"W/S - increase/decrease thrust\n"+
				"A/D - steering\n" +
				"Scroll Wheel - zoom";
		}
		if (Mathf.Abs (transform.position.x)>2500) {
			str+="Hazardous radiation detected - Turn around!\n";
		}
		
		GUI.TextField (new Rect (10, 10, 200, 150), 
		               str);

		if (transform.position.y >= 7000) {
			GUI.TextField (new Rect ((Screen.width/2)-150, (Screen.height/2)-15, 300, 30), 
			               "Congratulations, you've reached the Sun!");
		}
		if (!alive&&transform.position.y < 7000) {
			GUI.TextField (new Rect ((Screen.width/2)-150, (Screen.height/2)-15, 300, 30), 
			               "You died - press R to restart!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		RenderSettings.skybox.SetFloat("Blend",0.5f);
		if (alive) {
			if (Input.GetButtonDown ("Release")) {
				GetComponent<Rigidbody> ().isKinematic = false;
			}
			if (Input.GetButtonDown ("StartEngine")) {
				currentForce = initialForce;
				started = true;
			}
			if (started) {
				if (Input.GetButtonDown ("MaxThrust")) {
					currentForce = initialForce;
				}
				if (Input.GetButtonDown ("MinThrust")) {
					currentForce = 0;
				}

				currentSteeringForce = Input.GetAxis ("Horizontal") * MaxSteeringForce;

				currentForce += Input.GetAxis ("Vertical") * forceMultiplier;
				currentForce = Mathf.Clamp (currentForce, 0, MaxForce);

				fuel = 1;//fuel-((currentSteeringForce+currentForce)*fuelConsumptionRate);
				if (fuel <= 0) {
					currentForce = 0;
					currentSteeringForce = 0;
				}
				float MainAfterburnerScale = (currentForce / MaxForce);
				float LeftAfterburnerScale = (currentSteeringForce / MaxSteeringForce) / 3;
				float RightAfterburnerScale = -(currentSteeringForce / MaxSteeringForce) / 3;
				Mathf.Clamp (MainAfterburnerScale, 0.01f, 1);
				Mathf.Clamp (LeftAfterburnerScale, 0.01f, 1);
				Mathf.Clamp (RightAfterburnerScale, 0.01f, 1);

				scaleAfterburnerParticles (MainAfterburner, MainAfterburnerScale);
				scaleAfterburnerParticles (LeftAfterburner, LeftAfterburnerScale);
				scaleAfterburnerParticles (RightAfterburner, RightAfterburnerScale);
				GetComponent<Rigidbody> ().AddForceAtPosition (currentForce * transform.up, transform.position - transform.up, ForceMode.Acceleration);
				GetComponent<Rigidbody> ().AddForceAtPosition (currentSteeringForce * transform.right, transform.up + transform.position, ForceMode.Acceleration);
				GetComponent<Rigidbody> ().velocity = new Vector3 (
				Mathf.Clamp (GetComponent<Rigidbody> ().velocity.x, -80, 80),
				Mathf.Clamp (GetComponent<Rigidbody> ().velocity.y, -80, 80),
				Mathf.Clamp (GetComponent<Rigidbody> ().velocity.z, -80, 80)

				);
				if (this.transform.position.x < -3000) {
					die ();//this.transform.position = new Vector3 (3000, this.transform.position.y, this.transform.position.z);
				}
				if (this.transform.position.x > 3000) {
					die ();//this.transform.position = new Vector3 (-3000, this.transform.position.y, this.transform.position.z);
				}

				if (transform.position.y >= 7050) {
					GetComponent<Rigidbody>().isKinematic=true;
					die ();
				}
			}
		} else {
			if(Input.GetButtonDown("Release")){
				reset ();
			}
		}
	}
}
