using UnityEngine;
using System;
using System.Collections;

public class ExplodeMesh : MonoBehaviour {

	public float explosionForce = 100f;
	public float explosionRadius = 100f;
	public Vector3 triangleScale = Vector3.one;

	// Use this for initialization
	void Start () {
		StartCoroutine(SplitMesh());
	}


	public IEnumerator SplitMesh() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		Mesh mesh = meshFilter.mesh;

		Vector3 [] vertices = mesh.vertices;
		Vector3 [] normals = mesh.normals;
		Vector2 [] uvs = mesh.uv;
		
		for (int submeshID = 0; submeshID < mesh.subMeshCount; ++submeshID) {
			int [] indices = mesh.GetTriangles(submeshID);

			for (int i = 0; i < indices.Length; i += 3) {

				Vector3 [] newTriangleVertices = new Vector3[3];
				Vector3 [] newTriangleNormals = new Vector3[3];
				Vector2 [] newTriangleUVs = new Vector2[3];
	
				for (int n = 0; n < 3; ++n) {
					int index = indices[i + n];
					newTriangleVertices[n] = vertices[index];
					newTriangleUVs[n] = uvs[index];
					newTriangleNormals[n] = normals[index];					
				}

				Mesh newTriangleMesh = new Mesh();
				newTriangleMesh.vertices = newTriangleVertices;
				newTriangleMesh.normals = newTriangleNormals;
				newTriangleMesh.uv = newTriangleUVs;
	
				newTriangleMesh.triangles = new int [] {0, 1, 2};
//				newTriangleMesh.triangles = new int [] {0, 1, 2, 2, 1, 0}; // Use this instead if you also want to create backface triangles.
			
				GameObject newTriangle = new GameObject(String.Format("Triangle: {0}", (i/3)));
				newTriangle.transform.position = this.transform.position;
				newTriangle.transform.localScale = triangleScale;
				newTriangle.transform.localRotation = this.transform.localRotation;
				newTriangle.AddComponent<MeshFilter>().mesh = newTriangleMesh;
				newTriangle.AddComponent<MeshRenderer>().material = meshRenderer.materials[submeshID];
				newTriangle.AddComponent<SphereCollider>();
				newTriangle.AddComponent<Rigidbody>().AddExplosionForce(explosionForce, this.transform.position, explosionRadius);

				GameObject.Destroy(newTriangle, 5.0f + UnityEngine.Random.Range(0.0f, 5.0f));
			
			}
		}
		meshRenderer.enabled = false;
		Destroy(gameObject);
		yield break;
	}

}
