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
	null_exist
}
public class GroundTile : MonoBehaviour {

	public TerrainType TerrType;
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}
}
