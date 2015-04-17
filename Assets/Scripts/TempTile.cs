using UnityEngine;
using System.Collections;

public class TempTile : GroundTile {

	public int life;
	void Start () {
		GetComponent<Renderer>().material = 
			Resources.Load ("Crack_Face_" + life) as Material;
	}

	// Update is called once per frame
	void Update ()
	{
	}


	public override void PlayerLand()
	{
		life--;
		if(life >= 0)
		{
			GetComponent<Renderer>().material =
										Resources.Load("Crack_Face_" + life) as Material;
		}
	}
	public override void PlayerLeave()
	{
		if(life <= 0)
		{
			StartCoroutine(Fall());
			GetComponent<Rigidbody>().useGravity = true;
		}
	}
	IEnumerator Fall()
	{
		yield return new WaitForSeconds (5);
		Destroy(gameObject);
	}
}

