using UnityEngine;
using System.Collections;

public class RenderTransparencyOnBottom : MonoBehaviour {
	
	void Start () {
		this.renderer.material.renderQueue = 3000;
	}
	
}
