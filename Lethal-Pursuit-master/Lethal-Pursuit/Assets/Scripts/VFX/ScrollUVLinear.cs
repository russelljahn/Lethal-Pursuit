using UnityEngine;
using System.Collections;

public class ScrollUVLinear : MonoBehaviour {

	public Material materialToScroll;
	public float xRate = 1.0f;
	public float yRate = 1.0f;


	void Update () {
		Vector2 newOffset = new Vector2(xRate*Time.time, yRate*Time.time);
		materialToScroll.SetTextureOffset("_MainTex", newOffset); 
	}

}
