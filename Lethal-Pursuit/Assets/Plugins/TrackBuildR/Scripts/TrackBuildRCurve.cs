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

/// <summary>
/// Track BuildR Curve holds calculated data to speed up generation of the track
/// It is generated when between two points.
/// </summary>

public class TrackBuildRCurve : ScriptableObject
{

    public TrackBuildRPoint pointA;
    public TrackBuildRPoint pointB;

    public Vector3 center = Vector3.zero;
    public int storedPointSize = 0;
    public float arcLength = 0;
    public float[] storedArcLengths = null;
    public float[] storedArcLengthsFull = null;
    public int storedArcLengthArraySize = 750;
    public float[] normalisedT;
    public float boundaryHeight = 1.0f;
    /// ////
    public float[] midPointPerc;
    public float[] targetSize;
    public int[] prevNormIndex;
    public int[] nextNormIndex;

    public Vector3[] sampledPoints;
    public float[] sampledCants;
    public float[] sampledWidths;
    public float[] sampledCrowns;
    public Vector3[] sampledLeftBoundaryPoints;
    public Vector3[] sampledRightBoundaryPoints;
    public Vector3[] sampledTrackDirections;
    public Vector3[] sampledTrackUps;
    public Vector3[] sampledTrackCrosses;
    public float[] sampledAngles;
    public bool[] clipArrayLeft;
    public bool[] clipArrayRight;

    public GameObject holder = null;
    public DynamicMeshGenericMultiMaterialMesh dynamicTrackMesh = new DynamicMeshGenericMultiMaterialMesh();
    public DynamicMeshGenericMultiMaterialMesh dynamicBoundaryMesh = new DynamicMeshGenericMultiMaterialMesh();
    public DynamicMeshGenericMultiMaterialMesh dynamicOffroadMesh = new DynamicMeshGenericMultiMaterialMesh();
    public DynamicMeshGenericMultiMaterialMesh dynamicBumperMesh = new DynamicMeshGenericMultiMaterialMesh();
    public DynamicMeshGenericMultiMaterialMesh dynamicColliderMesh = new DynamicMeshGenericMultiMaterialMesh();

    //texture values
    public int trackTextureStyleIndex = 0;
    public int offroadTextureStyleIndex = 1;
    public int boundaryTextureStyleIndex = 2;
    public int bumperTextureStyleIndex = 3;

    public bool isDirty
    {
        get
        {
            if (pointA == null || pointB == null) return false;
            return pointA.isDirty || pointB.isDirty;
        }
    }

    public bool shouldReRender
    {
        get
        {
            if (pointA == null || pointB == null) return false;
            return pointA.shouldReRender || pointB.shouldReRender;
        }
    }

    public void SetReRender()
    {
        pointA.shouldReRender = true;
        pointB.shouldReRender = true;
    }
}
