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
/// Buildr project U vs.
/// </summary>
public class ProjectUVs
{
    /// <summary>
    /// Project the specified Base UVs to find the appropriate 2D shape from 3D space - mainly used for angled roofs
    /// </summary>
    /// <param name='verts'>
    /// 3 verticies that define the polygon
    /// </param>
    /// <param name='baseUV'>
    /// The 3 source UV coordinates.
    /// </param>
    /// <param name='forward'>
    /// The direction of the facade forward normal
    /// </param>
    public static Vector2[] Project(Vector3[] verts, Vector2 baseUV, Vector3 forward)
    {
        int vertCount = verts.Length;
        Vector2[] uvs = new Vector2[vertCount];
        if (vertCount < 3)
            return null;
        List<Vector3> normals = new List<Vector3>();
        for (int i = 2; i < vertCount; i++)
        {
            normals.Add(Vector3.Cross(verts[0] - verts[i], verts[1] - verts[i]));
        }
        int normalCount = normals.Count;
        Vector3 planeNormal = normals[0];
        for (int n = 1; n < normalCount; n++)
            planeNormal += normals[1];
        planeNormal /= vertCount;

        Quaternion normalToFacFront = Quaternion.FromToRotation(planeNormal, forward);
        planeNormal = normalToFacFront * planeNormal;
        Quaternion normalToFront = Quaternion.FromToRotation(planeNormal, Vector3.forward);
        planeNormal = normalToFront * planeNormal;
        Quaternion moveFace = normalToFront * normalToFacFront;

        uvs[0] = baseUV;
        for (int p = 1; p < vertCount; p++)
        {
            Vector3 newRelativePosition = moveFace * (verts[p] - verts[0]);
            uvs[p] = new Vector2(newRelativePosition.x, newRelativePosition.y) + baseUV;
        }

        return uvs;
    }
}
