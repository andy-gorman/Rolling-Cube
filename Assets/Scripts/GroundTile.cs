using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TerrainType {
	none,
	start,
	finish,
	fire,
	ice,
	teleport,
	temporary,
	null_exist
}

public class GroundTile : MonoBehaviour {

	public TerrainType TerrType;
	public AudioSource landSound;
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	public virtual void PlayerLand()
	{
		landSound.Play ();
	}

	public virtual void PlayerLeave()
	{

	}
}
