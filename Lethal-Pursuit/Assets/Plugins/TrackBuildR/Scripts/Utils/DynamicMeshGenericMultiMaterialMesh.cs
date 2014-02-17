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

public class DynamicMeshGenericMultiMaterialMesh
{

    public string name = "mesh";
    [SerializeField]
    private List<DynamicMeshGenericMultiMaterial> _meshes = new List<DynamicMeshGenericMultiMaterial>();
    private int _subMeshCount;
    private int _vertexCount;

    public DynamicMeshGenericMultiMaterialMesh()
    {
        _meshes = new List<DynamicMeshGenericMultiMaterial>();
    }

    public DynamicMeshGenericMultiMaterial this[int index]
    {
        get {return _meshes[index];}
    }

    public int meshCount
    {
        get {return _meshes.Count;}
        set
        {
            if(_meshes.Count != value)
            {
                _meshes.Capacity = value;
                for(int i = 0; i < value; i++)
                {
                    if (_meshes[i] == null)
                        UseNewMesh(i);
                }
            }
        }
    }

    public Vector3[] vertices
    {
        get
        {
            List<Vector3> returnVerts = new List<Vector3>();
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                returnVerts.AddRange(mesh.vertices);
            return returnVerts.ToArray();
        }
    }

    public Vector2[] uv
    {
        get
        {
            List<Vector2> returnUVs = new List<Vector2>();
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                returnUVs.AddRange(mesh.uv);
            return returnUVs.ToArray();
        }
    }

    public int[] triangles
    {
        get
        {
            List<int> returnTris = new List<int>();
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                returnTris.AddRange(mesh.triangles);
            return returnTris.ToArray();
        }
    }

    public Vector2 MinWorldUvSize(int submesh)
    {
        Vector2 mainMinUV = _meshes[0].minWorldUvSize[0];
        foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
        {
            for (int i = 0; i < _subMeshCount; i++)
            {
                Vector2 meshMinUV = mesh.minWorldUvSize[i];
                if (meshMinUV.x < mainMinUV.x) mainMinUV.x = meshMinUV.x;
                if (meshMinUV.y < mainMinUV.y) mainMinUV.y = meshMinUV.y;
            }
        }
        return mainMinUV;
    }

    public Vector2 MaxWorldUvSize(int submesh)
    {
        Vector2 mainMinUV = _meshes[0].minWorldUvSize[0];
        foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
        {
            for (int i = 0; i < _subMeshCount; i++)
            {
                Vector2 meshMinUV = mesh.minWorldUvSize[i];
                if (meshMinUV.x > mainMinUV.x) mainMinUV.x = meshMinUV.x;
                if (meshMinUV.y > mainMinUV.y) mainMinUV.y = meshMinUV.y;
            }
        }
        return mainMinUV;
    }

    public void Build()
    {
        Build(false);
    }

    public void Build(bool calcTangents)
    {
        foreach(DynamicMeshGenericMultiMaterial mesh in _meshes)
        {
            mesh.Build(calcTangents);
        }
    }

    public void Clear()
    {
        foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
            mesh.Clear();
        _vertexCount = 0;
        _meshes.Clear();
    }

    public int vertexCount
    {
        get
        {
            return _vertexCount;
        }
    }

    public bool built
    {
        get
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                if (!mesh.built) return false;
            return true;
        }
    }

    public int triangleCount
    {
        get
        {
            int triCount = 0;
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                triCount += mesh.triangleCount;
            return triCount;
        }
    }

    public int subMeshCount
    {
        get
        {
            return _subMeshCount;
        }
        set
        {
            _subMeshCount = value;
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                mesh.subMeshCount = value;
        }
    }

    public bool hasTangents
    {
        get
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                if (!mesh.hasTangents) return false;
            return true;
        }
    }

    public bool lightmapUvsCalculated {
        get
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                if (!mesh.lightmapUvsCalculated) return false;
            return true;
        }
        set
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                mesh.lightmapUvsCalculated = true;
        }
    }

    public bool optimised
    {
        get
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                if (!mesh.optimised) return false;
            return true;
        }
        set
        {
            foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
                mesh.optimised = true;
        }
    }

//    public List<Vector2> minWorldUvSize {get {return _minWorldUVSize;}}

//    public List<Vector2> maxWorldUvSize {get {return _maxWorldUVSize;}}

    public void SolveTangents()
    {
        foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
            mesh.SolveTangents();
    }

    public int UseNewMesh()
    {
        return UseNewMesh(_meshes.Count);
    }

    public int UseNewMesh(int meshIndex)
    {
        DynamicMeshGenericMultiMaterial newMesh = new DynamicMeshGenericMultiMaterial();
        newMesh.name = name + " " + (meshIndex + 1);
        newMesh.subMeshCount = _subMeshCount;
        _meshes.Insert(meshIndex,newMesh);
        return meshIndex;
    }

    private int CheckMeshSize(int numberOfNewVerts)
    {
        int meshIndex = _meshes.Count-1;
        if(meshIndex<0)//check there is nextNormIndex mesh to begin with
        {
            _vertexCount += numberOfNewVerts;
            return UseNewMesh();
        }
        int newVertCount = numberOfNewVerts + _vertexCount - (meshIndex*65000);
        if(newVertCount >= 64990)
        {
            UseNewMesh();
            meshIndex++;
        }
//        if (newVertCount > 64990) Debug.Log(newVertCount + " " + numberOfNewVerts + " " + _vertexCount + " " + meshIndex+" "+(meshIndex*65000));
        _vertexCount += numberOfNewVerts;
        return meshIndex;
    }

    public void AddData(Vector3[] verts, Vector2[] uvs, int[] tris, int subMesh)
    {
        int useMeshIndex = CheckMeshSize(verts.Length);
        _meshes[useMeshIndex].AddData(verts,uvs,tris,subMesh);
    }

    public void AddTri(Vector3 p0, Vector3 p1, Vector3 p2, int subMesh)
    {
        int useMeshIndex = CheckMeshSize(3);
        _meshes[useMeshIndex].AddTri(p0,p1,p2,subMesh);
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
        int useMeshIndex = CheckMeshSize(4);
        _meshes[useMeshIndex].AddPlane(p0, p1, p2, p3, Vector2.zero, Vector2.one, subMesh);
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

        int useMeshIndex = CheckMeshSize(4);
        //if (_meshes[useMeshIndex].vertexCount > 64996) Debug.Log("usem " + useMeshIndex + " " + _meshes[useMeshIndex].vertexCount);
        _meshes[useMeshIndex].AddPlane(p0, p1, p2, p3, uv0, uv1, uv2, uv3, subMesh);
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
        int useMeshIndex = CheckMeshSize(4);
        _meshes[useMeshIndex].AddPlane(p0, p1, p2, p3, uv0, uv1, uv2, uv3, subMesh);
    }

    /// <summary>
    /// Checks the Max UV values used in this model for each trackTexture.
    /// </summary>
    /// <param name='data'>
    /// BuildR Data.
    /// </param>
    public void CheckMaxTextureUVs(TrackBuildRTexture[] textures)
    {
        foreach(DynamicMeshGenericMultiMaterial mesh in _meshes)
            mesh.CheckMaxTextureUVs(textures);
    }

    public void Atlas(Rect[] newTextureCoords, TrackBuildRTexture[] textures)
    {
        foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
            mesh.Atlas(newTextureCoords, textures);
    }

    public void Atlas(int[] modifySubmeshes, Rect[] newTextureCoords)
    {
        foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
            mesh.Atlas(modifySubmeshes, newTextureCoords);
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
        foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
            mesh.Atlas(modifySubmeshes, newTextureCoords, textures);
    }

    public void CollapseSubmeshes()
    {
        foreach (DynamicMeshGenericMultiMaterial mesh in _meshes)
            mesh.CollapseSubmeshes();
    }
}
