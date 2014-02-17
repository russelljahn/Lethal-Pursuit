// Track BuildR
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://support.jasperstocker.com
// For support contact email@jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A class dealing with dynamic mesh generataion. Attempts to keep memory use low. 
/// Contains nextNormIndex variety of functions to help in the generation of meshes.
/// Uses generic lists to contain the mesh data allowing for the mesh to be of dynamic numberOfPoints
/// Creates nextNormIndex mesh that can support multiple materials
/// </summary>

public class DynamicMeshGenericMultiMaterial
{

    public string name = "";
    public Mesh mesh;
    public List<Vector3> vertices;
    public List<Vector2> uv;
    public List<int> triangles;
    private List<Vector2> _minWorldUVSize = new List<Vector2>();
    private List<Vector2> _maxWorldUVSize = new List<Vector2>();
    private int _subMeshes = 1;
    private Dictionary<int, List<int>> subTriangles;
    private Vector3[] tan1;
    private Vector3[] tan2;
    private Vector4[] tangents;
    private bool _built;
    private bool _hasTangents;
    private bool _optimised;

    public DynamicMeshGenericMultiMaterial()
    {
        mesh = new Mesh();
        vertices = new List<Vector3>();
        uv = new List<Vector2>();
        triangles = new List<int>();
        subTriangles = new Dictionary<int, List<int>>();
    }

    public void Build()
    {
        Build(false);
    }

    public void Build(bool calcTangents)
    {
        if (vertexCount > 65000)//Unity has an inbuilt limit of 65000 verticies. Use DynamicMeshGenericMultiMaterialMesh to handle more than 65000
        {
            Debug.LogWarning(name+ " is exceeding 65000 vertices - stop build");
            _built = false;
            return;
        }

        if(subMeshCount == 0)//USer needs to specify the amount of submeshes this mesh contains
        {
            Debug.LogWarning(name+ " has no submeshes - you need to define them pre build");
            _built = false;
            return;
        }

        mesh.Clear();
        mesh.name = name;
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.uv2 = new Vector2[0];

        mesh.subMeshCount = _subMeshes;
        List<int> setTris = new List<int>();
        foreach (KeyValuePair<int, List<int>> triData in subTriangles)
        {
            mesh.SetTriangles(triData.Value.ToArray(), triData.Key);
            setTris.AddRange(triData.Value);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        if (calcTangents)
        {
            SolveTangents();
        }
        else
        {
            _hasTangents = false;
            Vector4[] emptyTangents = new Vector4[size];
            mesh.tangents = emptyTangents;
        }

        optimised = false;
        _built = true;
        lightmapUvsCalculated = false;
    }

    /// <summary>
    /// Clears the mesh data, ready for nextNormIndex new mesh build
    /// </summary>
    public void Clear()
    {
        mesh.Clear();
        vertices.Clear();
        uv.Clear();
        triangles.Clear();
        subTriangles.Clear();
        _built = false;
        _subMeshes = 0;
    }

    public int vertexCount
    {
        get
        {
            return vertices.Count;
        }
    }

    public bool built
    {
        get { return _built; }
    }

    public int size
    {
        get { return vertices.Count; }
    }

    public int triangleCount
    {
        get { return triangles.Count; }
    }

    public int subMeshCount
    {
        get
        {
            return _subMeshes;
        }
        set
        {
            _subMeshes = value;
            //reset the largest/smallest UZ numberOfPoints monitors
            if(minWorldUvSize.Count > value)
                minWorldUvSize.Clear();
            if(maxWorldUvSize.Count > value)
                maxWorldUvSize.Clear();

            while (minWorldUvSize.Count <= value)
            {
                minWorldUvSize.Add(Vector2.zero);
                maxWorldUvSize.Add(Vector2.one);
            }
        }
    }

    public bool hasTangents { get { return _hasTangents; } }

    public bool lightmapUvsCalculated {get; set;}

    public bool optimised {get {return _optimised;} set {_optimised = value;}}

    public List<Vector2> minWorldUvSize {get {return _minWorldUVSize;}}

    public List<Vector2> maxWorldUvSize {get {return _maxWorldUVSize;}}


    /// <summary>
    /// Generate the Mesh tangents.
    /// These calculations are heavy and not idea for complex meshes at runtime
    /// </summary>
    public void SolveTangents()
    {
        tan1 = new Vector3[size];
        tan2 = new Vector3[size];
        tangents = new Vector4[size];
        int triangleCount = triangles.Count / 3;

        for (int a = 0; a < triangleCount; a += 3)
        {
            int i1 = triangles[a + 0];
            int i2 = triangles[a + 1];
            int i3 = triangles[a + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = uv[i1];
            Vector2 w2 = uv[i2];
            Vector2 w3 = uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }


        for (int a = 0; a < size; ++a)
        {
            Vector3 n = mesh.normals[a];
            Vector3 t = tan1[a];

            Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
            tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }
        mesh.tangents = tangents;
        _hasTangents = true;
    }

    /// <summary>
    /// Add new mesh data - all arrays are ordered together
    /// </summary>
    /// <param name="verts">And array of verticies</param>
    /// <param name="uvs">And array of uvs</param>
    /// <param name="tris">And array of triangles</param>
    /// <param name="subMesh">The submesh to add the data into</param>
    public void AddData(Vector3[] verts, Vector2[] uvs, int[] tris, int subMesh)
    {
        int indiceBase = vertices.Count;
        vertices.AddRange(verts);
        uv.AddRange(uvs);
        if (!subTriangles.ContainsKey(subMesh))
            subTriangles.Add(subMesh, new List<int>());

        int newTriCount = tris.Length;
        for (int t = 0; t < newTriCount; t++)
        {
            int newTri = (indiceBase + tris[t]);
            triangles.Add(newTri);
            subTriangles[subMesh].Add(newTri);
        }

        //calculate the bounds of the UV on the mesh
        Vector2 minWorldUVSize = minWorldUvSize[subMesh];
        Vector2 maxWorldUVSize = maxWorldUvSize[subMesh];

        int vertCount = verts.Length;
        for(int i = 0; i < vertCount-1; i++)
        {
            Vector2 thisuv = uvs[i];
            if (thisuv.x < minWorldUVSize.x)
                minWorldUVSize.x = thisuv.x;
            if (thisuv.y < minWorldUVSize.y)
                minWorldUVSize.y = thisuv.y;

            if (thisuv.x > maxWorldUVSize.x)
                maxWorldUVSize.x = thisuv.x;
            if (thisuv.y > maxWorldUVSize.y)
                maxWorldUVSize.y = thisuv.y;
        }
        minWorldUvSize[subMesh] = minWorldUVSize;
        maxWorldUvSize[subMesh] = maxWorldUVSize;

    }

    /// <summary>
    /// Add the new triangle to the mesh data
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="subMesh"></param>
    public void AddTri(Vector3 p0, Vector3 p1, Vector3 p2, int subMesh)
    {
        int indiceBase = vertices.Count;
        vertices.Add(p0);
        vertices.Add(p1);
        vertices.Add(p2);

        uv.Add(new Vector2(0, 0));
        uv.Add(new Vector2(1, 0));
        uv.Add(new Vector2(0, 1));

        if (!subTriangles.ContainsKey(subMesh))
            subTriangles.Add(subMesh, new List<int>());

        triangles.Add(indiceBase);
        triangles.Add(indiceBase + 2);
        triangles.Add(indiceBase + 1);

        subTriangles[subMesh].Add(indiceBase);
        subTriangles[subMesh].Add(indiceBase + 2);
        subTriangles[subMesh].Add(indiceBase + 1);
    }

    /// <summary>
    /// Adds the plane to the generic dynamic mesh without specifying UV coords.
    /// </summary>
    /// <param name='p0,p1,p2,p3'>
    /// 4 Verticies that define the plane
    /// <param name='subMesh'>
    /// The sub mesh to attch this plan to - in order of Texture library indicies
    /// </param>
    public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int subMesh)
    {
        AddPlane(p0, p1, p2, p3, Vector2.zero, Vector2.one, subMesh);
    }

    /// <summary>
    /// Adds the plane to the generic dynamic mesh by specifying min and max UV coords.
    /// </summary>
    /// <param name='p0,p1,p2,p3'>
    /// 4 Verticies that define the plane
    /// </param>
    /// <param name='minUV'>
    /// the minimum vertex UV coord.
    /// </param>
    /// </param>
    /// <param name='maxUV'>
    /// the maximum vertex UV coord.
    /// </param>
    /// <param name='subMesh'>
    /// The sub mesh to attch this plan to - in order of Texture library indicies
    /// </param>
    public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 minUV, Vector2 maxUV, int subMesh)
    {
        Vector2 uv0 = new Vector2(minUV.x, minUV.y);
        Vector2 uv1 = new Vector2(maxUV.x, minUV.y);
        Vector2 uv2 = new Vector2(minUV.x, maxUV.y);
        Vector2 uv3 = new Vector2(maxUV.x, maxUV.y);

        AddPlane(p0, p1, p2, p3, uv0, uv1, uv2, uv3, subMesh);
    }

    /// <summary>
    /// Adds the plane to the generic dynamic mesh.
    /// </summary>
    /// <param name='p0,p1,p2,p3'>
    /// 4 Verticies that define the plane
    /// </param>
    /// <param name='uv0,uv1,uv2,uv3'>
    /// the vertex UV coords.
    /// </param>
    /// <param name='subMesh'>
    /// The sub mesh to attch this plan to - in order of Texture library indicies
    /// </param>
    public void AddPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3, int subMesh)
    {
        int indiceBase = vertices.Count;
        vertices.Add(p0);
        vertices.Add(p1);
        vertices.Add(p2);
        vertices.Add(p3);

        uv.Add(uv0);
        uv.Add(uv1);
        uv.Add(uv2);
        uv.Add(uv3);

        if (!subTriangles.ContainsKey(subMesh))
            subTriangles.Add(subMesh, new List<int>());

        subTriangles[subMesh].Add(indiceBase);
        subTriangles[subMesh].Add(indiceBase + 2);
        subTriangles[subMesh].Add(indiceBase + 1);

        subTriangles[subMesh].Add(indiceBase + 1);
        subTriangles[subMesh].Add(indiceBase + 2);
        subTriangles[subMesh].Add(indiceBase + 3);

        triangles.Add(indiceBase);
        triangles.Add(indiceBase + 2);
        triangles.Add(indiceBase + 1);

        triangles.Add(indiceBase + 1);
        triangles.Add(indiceBase + 2);
        triangles.Add(indiceBase + 3);

        Vector2 minWorldUVSize = minWorldUvSize[subMesh];
        Vector2 maxWorldUVSize = maxWorldUvSize[subMesh];
        int vertCount = 4;
        Vector2[] uvs = new []{uv0,uv1,uv2,uv3};
        for (int i = 0; i < vertCount - 1; i++)
        {
            Vector2 thisuv = uvs[i];
            if (thisuv.x < minWorldUVSize.x)
                minWorldUVSize.x = thisuv.x;
            if (thisuv.y < minWorldUVSize.y)
                minWorldUVSize.y = thisuv.y;

            if (thisuv.x > maxWorldUVSize.x)
                maxWorldUVSize.x = thisuv.x;
            if (thisuv.y > maxWorldUVSize.y)
                maxWorldUVSize.y = thisuv.y;
        }
        minWorldUvSize[subMesh] = minWorldUVSize;
        maxWorldUvSize[subMesh] = maxWorldUVSize;
    }

    /// <summary>
    /// Checks the Max UV values used in this model for each trackTexture.
    /// </summary>
    /// <param name='data'>
    /// BuildR Data.
    /// </param>
    public void CheckMaxTextureUVs(TrackBuildRTexture[] textures)
    {
        Vector2[] subMeshUVOffsets = new Vector2[subMeshCount];
        int[] subMeshIDs = new List<int>(subTriangles.Keys).ToArray();
        int numberOfSubmeshIDs = subMeshIDs.Length;
        for (int sm = 0; sm < numberOfSubmeshIDs; sm++)
        {
            int subMeshID = subMeshIDs[sm];
            if (subTriangles.ContainsKey(subMeshID))
            {
                int[] submeshIndices = subTriangles[subMeshID].ToArray();
                subMeshUVOffsets[sm] = Vector2.zero;
                foreach (int index in submeshIndices)
                {
                    if (uv[index].x < subMeshUVOffsets[sm].x)
                        subMeshUVOffsets[sm].x = uv[index].x;
                    if (uv[index].y < subMeshUVOffsets[sm].y)
                        subMeshUVOffsets[sm].y = uv[index].y;
                }

                List<int> UVsOffset = new List<int>();
                foreach (int uvindex in subTriangles[subMeshID])
                {
                    if (!UVsOffset.Contains(uvindex))
                    {
                        uv[uvindex] += -subMeshUVOffsets[sm];//offset the UV to ensure it isn't negative
                        UVsOffset.Add(uvindex);
                    }
                    textures[subMeshID].CheckMaxUV(uv[uvindex]);
                }
            }
            else
            {
                Debug.Log("Mesh does not contain key for trackTexture " + textures[subMeshID].name);
            }
        }
    }

    /// <summary>
    ///  Atlas the entire mesh using newTextureCoords and textures.
    /// </summary>
    /// <param name="newTextureCoords"></param>
    /// <param name="textures"></param>
    public void Atlas(Rect[] newTextureCoords, TrackBuildRTexture[] textures)
    {
        List<int> keys = new List<int>(subTriangles.Keys);
        Atlas(keys.ToArray(), newTextureCoords, textures);
    }

    /// <summary>
    /// Atlas the specified modifySubmeshes using newTextureCoords and textures.
    /// </summary>
    /// <param name='modifySubmeshes'>
    /// Submeshes indicies to atlas.
    /// </param>
    /// <param name='newTextureCoords'>
    /// New trackTexture coords generated from Pack Textures.
    /// </param>
    /// <param name='textures'>
    /// BuildR Textures library reference.
    /// </param>
    public void Atlas(int[] modifySubmeshes, Rect[] newTextureCoords, TrackBuildRTexture[] textures)
    {
        if (modifySubmeshes.Length == 0)
        {
            Debug.Log("No submeshes to atlas!");
            return;
        }
        List<int> atlasedSubmesh = new List<int>();
        int numberOfSubmeshesToModify = modifySubmeshes.Length;
        for (int s = 0; s < numberOfSubmeshesToModify; s++)
        {
            int submeshIndex = modifySubmeshes[s];
            Rect textureRect = newTextureCoords[s];
            if (!subTriangles.ContainsKey(submeshIndex))
                continue;
            int[] submeshIndices = subTriangles[submeshIndex].ToArray();
            subTriangles.Remove(submeshIndex);
            atlasedSubmesh.AddRange(submeshIndices);

            TrackBuildRTexture bTexture = textures[submeshIndex];
            List<int> ModifiedUVs = new List<int>();
            foreach (int index in submeshIndices)
            {
                if (ModifiedUVs.Contains(index))
                    continue;//don't move the UV more than once
                Vector2 uvCoords = uv[index];
                float xUV = uvCoords.x / bTexture.maxUVTile.x;
                float yUV = uvCoords.y / bTexture.maxUVTile.y;
                if (xUV > 1)
                {
                    bTexture.maxUVTile.x = uvCoords.x;
                    xUV = 1.0f;
                }
                if (yUV > 1)
                {
                    bTexture.maxUVTile.y = uvCoords.y;
                    yUV = 1;
                }

                uvCoords.x = Mathf.Lerp(textureRect.xMin, textureRect.xMax, xUV);
                uvCoords.y = Mathf.Lerp(textureRect.yMin, textureRect.yMax, yUV);

                if (float.IsNaN(uvCoords.x))
                {
                    uvCoords.x = 1;
                }
                if (float.IsNaN(uvCoords.y))
                {
                    uvCoords.y = 1;
                }
                uv[index] = uvCoords;
                ModifiedUVs.Add(index);//keep nextNormIndex record of moved UVs
            }
        }
        subMeshCount = subMeshCount - modifySubmeshes.Length + 1;
        subTriangles.Add(modifySubmeshes[0], atlasedSubmesh);
    }

    /// <summary>
    /// Atlas the entire mesh, specifying specific submeshes that have been atlased
    /// </summary>
    /// <param name="modifySubmeshes">Specified submeshes for the atlased coords</param>
    /// <param name="newTextureCoords">New trackTexture coords generated from Pack Textures.</param>
    public void Atlas(int[] modifySubmeshes, Rect[] newTextureCoords)
    {
        if (modifySubmeshes.Length == 0)
        {
            Debug.Log("No submeshes to atlas!");
            return;
        }
        List<int> atlasedSubmesh = new List<int>();
        List<int> modifySubmeshList = new List<int>(modifySubmeshes);
        for (int s = 0; s < subMeshCount; s++)
        {
            if (!subTriangles.ContainsKey(s))
                continue;

            int[] submeshIndices = subTriangles[s].ToArray();
            subTriangles.Remove(s);
            atlasedSubmesh.AddRange(submeshIndices);

            if(modifySubmeshList.Contains(s))
            {
                Rect textureRect = newTextureCoords[s]; 
                List<int> ModifiedUVs = new List<int>();
                foreach (int index in submeshIndices)
                {
                    if (ModifiedUVs.Contains(index))
                        continue;//don't move the UV more than once
                    Vector2 uvCoords = uv[index];
                    uvCoords.x = Mathf.Lerp(textureRect.xMin, textureRect.xMax, uvCoords.x);
                    uvCoords.y = Mathf.Lerp(textureRect.yMin, textureRect.yMax, uvCoords.y);
                    uv[index] = uvCoords;
                    ModifiedUVs.Add(index);//keep nextNormIndex record of moved UVs
                }
            }else
            {
                List<int> ModifiedUVs = new List<int>();
                foreach (int index in submeshIndices)
                {
                    if (ModifiedUVs.Contains(index))
                        continue;//don't move the UV more than once
                    uv[index] = Vector2.zero;
                    ModifiedUVs.Add(index);//keep nextNormIndex record of moved UVs
                }
            }
        }
        //subMeshCount = subMeshCount - modifySubmeshes.Length + 1;
        subMeshCount = 1;
        subTriangles.Add(modifySubmeshes[0], atlasedSubmesh);
    }

    /// <summary>
    /// Collapse all the submeshes into nextNormIndex single submesh
    /// </summary>
    public void CollapseSubmeshes()
    {
        List<int> atlasedSubmesh = new List<int>();
        int numberOfSubmeshesToModify = subMeshCount;
        for (int s = 0; s < numberOfSubmeshesToModify; s++)
        {
            if (subTriangles.ContainsKey(s))
            {
                int[] submeshIndices = subTriangles[s].ToArray();
                atlasedSubmesh.AddRange(submeshIndices);
            }
        }
        subMeshCount = 1;
        subTriangles.Clear();
        subTriangles.Add(0, atlasedSubmesh);
    }

    public void RemoveRedundantVerticies()
    {
        List<Vector3> vertList = new List<Vector3>();
        int vertIndex = 0;
        int numberOfVerts = vertexCount;

        for (int i = 0; i < numberOfVerts; i++)
        {
            Vector3 vert = vertices[i];
            if (!vertList.Contains(vert))
            {
                //no redundancy - add
                vertList.Add(vert);
            }
            else
            {
                //possible redundancy
                int firstIndex = vertices.IndexOf(vert);
                int secondIndex = vertIndex;

                //check to see if these verticies are connected
                if (uv[firstIndex] == uv[secondIndex])
                {
                    //verticies are connected - merge them

                    int triIndex = 0;
                    while ((triIndex = triangles.IndexOf(secondIndex)) != -1)
                        triangles[triIndex] = firstIndex;

                    vertices.RemoveAt(secondIndex);
                    uv.RemoveAt(secondIndex);

                    numberOfVerts--;
                    i--;
                }
                else
                {
                    vertList.Add(vert);
                }
            }

            vertIndex++;
        }
    }
}
