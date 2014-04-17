using UnityEngine;
using System.Collections;

public class RenderOnTop : MonoBehaviour {
	
	void Start () {
		this.renderer.material.renderQueue = 4000;
	}
	
}
