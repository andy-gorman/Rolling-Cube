using UnityEngine;
using System.Collections;

public class TeleportTile : GroundTile
{
	public TeleportTile OtherEnd;
	public Color color;
	private float time;
	private bool fadingOut;
	// Use this for initialization
	void Start ()
	{
		GetComponent<MeshRenderer> ().material.color = color;
		time = 0f;
		fadingOut = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		time += Time.deltaTime;
		if (fadingOut) {
			GetComponent<MeshRenderer> ().material.color = Color.Lerp (color, Color.white, time);
		} else {
			GetComponent<MeshRenderer> ().material.color = Color.Lerp (Color.white, color, time);
		}
		if(time > 1f) {
			time = 0f;
			fadingOut = !fadingOut;
		}
	}
}

