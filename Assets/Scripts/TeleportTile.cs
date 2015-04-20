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
		fadeColor = Color.Lerp (color, Color.white, 0.5f);
	}

	// Update is called once per frame
	void Update ()
	{
		MeshRenderer mr = GetComponent<MeshRenderer> ();
		time += Time.deltaTime;
		if (fadingOut) {
			mr.material.color = Color.Lerp (color, fadeColor, time);
		} else {
			mr.material.color = Color.Lerp (fadeColor, color, time);
		}
		if(time > 1f) {
			time = 0f;
			fadingOut = !fadingOut;
			//fadeColor = GetComponent<MeshRenderer>().material.color;
		}
	}
}

