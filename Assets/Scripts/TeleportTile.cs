using UnityEngine;
using System.Collections;

public class TeleportTile : GroundTile
{
	public TeleportTile OtherEnd;
	public Color color;
	private float time;
	private bool fadingOut;
	private Color fadeColor;
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
		time += Time.deltaTime * 0.5f;
		if (fadingOut) {
			GetComponent<MeshRenderer> ().material.color = Color.Lerp (color, Color.white, time);
		} else {
			GetComponent<MeshRenderer> ().material.color = Color.Lerp (fadeColor, color, time);
		}
		if(time > 0.5f) {
			time = 0f;
			fadingOut = !fadingOut;
			fadeColor = GetComponent<MeshRenderer>().material.color;
		}
	}
}

