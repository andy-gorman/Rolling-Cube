﻿using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour {
	public Mesh SpikeMesh;
	public PlayerCube myCube;
	public float rollSpeed;
	public float delay = 2f;
	public float slideSpeed;
	public float sinkSpeed;


	private bool moving;
	public bool Moving
	{
		get{ return moving; }
	}

	private Direction lastMove_;
	public Direction LastMove
	{
		get {return lastMove_; }
	}
	private bool dead_;
	public bool Dead 
	{
		get { return dead_;}
	}

	void Start() 
	{
		moving = false;
	}

	void Update() 
	{
	}


	//TODO: Make this function less crappy.
	public void SetFaces() 
	{
		SetFace (myCube.Top, "Top");
		SetFace (myCube.Bottom, "Bottom");
		SetFace (myCube.PosX, "PosX");
		SetFace (myCube.NegX, "NegX");
		SetFace (myCube.PosZ, "PosZ");
		SetFace (myCube.NegZ, "NegZ");
	}

	private void SetFace(PlayerFaceType FaceType, string Face)
	{
		if (FaceType == PlayerFaceType.spikes) {
			transform.FindChild(Face).GetComponent<MeshFilter>().mesh = SpikeMesh;
		}
	}

	//BEGIN ROLLING METHODS

	public void RollLeft() 
	{
		myCube.roll(1);
		StartCoroutine (Roll(0, -.5f, -.5f, Vector3.forward, "left"));
		lastMove_ = Direction.negX;
	}

	public void RollRight() 
	{
		myCube.roll(2);
		StartCoroutine (Roll(0, -.5f, .5f, -Vector3.forward, "right"));
		lastMove_ = Direction.posX;
	}

	public void RollForward() 
	{
		myCube.roll(3);
		StartCoroutine (Roll (.5f, -.5f, 0, Vector3.right, "forward"));
		lastMove_ = Direction.posZ;
	}

	public void RollBackward() 
	{
		myCube.roll(4);
		StartCoroutine (Roll (-.5f, -.5f, 0, -Vector3.right, "backward"));
		lastMove_ = Direction.negZ;
	}

	IEnumerator Roll(float fwdWeight, float upWeight, float xWeight, Vector3 rotateAxis, string dir) {
		float totalRotation = 0f;
		moving = true;
		float startX = transform.position.x; float startZ = transform.position.z;
		while (totalRotation < 90f) {
			// we calc the spinamount but make sure it won't shoot over the 90 degrees
			float spinAmount = Mathf.Min (Time.deltaTime * rollSpeed, 90f - totalRotation);

			// we rotate around one of the edges of the cube (the stationary one of course)
			transform.RotateAround ((transform.position + (fwdWeight * Vector3.forward + upWeight * Vector3.up + xWeight * Vector3.right)), rotateAxis, spinAmount);
			
			// add to amount of spin in this update the total rotation
			totalRotation += spinAmount;
			Vector3 tmp = transform.position;
			tmp.y = 0.5f;
			transform.position = tmp;
			yield return 0;
		}
		Vector3 pos = transform.position;
		
		if (dir == "left") {
			pos.x = startX - 1.0f;
		} else if (dir == "right") {
			pos.x = startX + 1.0f;
		} else if (dir == "forward") {
			pos.z = startZ + 1.0f;
		} else  if (dir == "backward") {
			pos.z = startZ - 1.0f; 
		}
		pos.y = 0.5f; //Maybe change this eventually but its probably fine.
		transform.position = pos;
		moving = false;
	}

	//END ROLLING METHODS

	//BEGIN SLIDING METHODS

	public void SlidePosZ()
	{
		Vector3 pos = transform.position;
		StartCoroutine(Slide(pos.x, pos.z + 1f));
		lastMove_ = Direction.posZ;
	}

	public void SlideNegZ()
	{
		Vector3 pos = transform.position;
		StartCoroutine(Slide(pos.x, pos.z - 1f));
		lastMove_ = Direction.negZ;
	}

	public void SlidePosX()
	{
		Vector3 pos = transform.position;
		StartCoroutine(Slide(pos.x + 1f, pos.z));
		lastMove_ = Direction.posX;
	}

	public void SlideNegX()
	{
		Vector3 pos = transform.position;
		StartCoroutine(Slide(pos.x - 1f, pos.z));
		lastMove_ = Direction.negX;
	}

	IEnumerator Slide(float endX, float endZ) 
	{
		float curX = transform.position.x; float curZ = transform.position.z;
		float distToCover = Mathf.Sqrt((endX - curX) * (endX - curX) + (endZ - curZ) * (endZ - curZ));
		Vector3 translation = new Vector3(endX - curX, 0f, endZ - curZ);
		
		float distTraveled = 0f;
		moving = true;
		while(distTraveled < distToCover){
			float slideAmount = Mathf.Min (Time.deltaTime * slideSpeed, distToCover - distTraveled);
			Vector3 pos = transform.position;
			pos.x += slideAmount * translation.x;
			pos.z += slideAmount * translation.z;
			transform.position = pos;
			distTraveled += slideAmount;
			yield return 0;
		}
		//Clamp the position
		Vector3 tmp = transform.position;
		tmp.x = endX;
		tmp.z = endZ;
		transform.position = tmp;
		moving = false;
	}

	public void Sink()
	{
		//Burn
		GameObject fire = (GameObject)GameObject.Instantiate (Resources.Load ("fx_fire_a"));
		fire.transform.parent = gameObject.transform;
		fire.transform.localPosition = new Vector3 (0, 0, 0);

		//Start sinking
		StartCoroutine(SlowSink());
	}

	IEnumerator SlowSink()
	{
		moving = true;
		float distTraveled = 0f;
		float distToCover = 0.5f;
		while(distTraveled < distToCover)
		{
			float sinkAmount = Mathf.Min (Time.deltaTime * sinkSpeed, distToCover - distTraveled);
			Vector3 pos = transform.position;
			pos.y -= sinkAmount;
			transform.position = pos;
			distTraveled += sinkAmount;
			yield return 0;
		}
		//Kill it.
		dead_ = true;
	}

}
	