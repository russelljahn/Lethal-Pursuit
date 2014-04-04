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

public class TrackBuildR : MonoBehaviour
{
    public TrackBuildRTrack track = new TrackBuildRTrack();
    public TrackBuildRGenerator generator;
    public GameObject trackEditorPreview = null;

    //CUSTOM EDITOR VALUES
    public enum modes
    {
        track,
        boundary,
        bumpers,
        textures,
        //pit,
        diagram,
        options
    }

    public enum pointModes
    {
        transform,
        controlpoint,
        trackpoint,
        add,
        remove
    }

    public enum boundaryModes
    {
        transform,
        controlpoint
    }

    public enum textureModes
    {
        track,
        boundary,
        offroad,
        bumpers
    }

    public enum pitModes
    {
        editPitLane,
        setStartPoint,
        setEndPoint
    }

    public modes mode = modes.track;
    public pointModes pointMode = pointModes.transform;
    public boundaryModes boundaryMode = boundaryModes.transform;
    public pitModes pitMode = pitModes.editPitLane;
    public textureModes textureMode = textureModes.track;

    public float previewPercentage = 0;

    public bool tangentsGenerated {get {return track.tangentsGenerated;}}
    public bool lightmapGenerated {get {return track.lightmapGenerated;}}
    public bool optimised {get {return track.optimised;}}

    public void Init()
    {
        track.Init();
        track.baseTransform = transform;

        TrackBuildRPoint p0 = ScriptableObject.CreateInstance<TrackBuildRPoint>();
        TrackBuildRPoint p1 = ScriptableObject.CreateInstance<TrackBuildRPoint>();
        TrackBuildRPoint p2 = ScriptableObject.CreateInstance<TrackBuildRPoint>();
        TrackBuildRPoint p3 = ScriptableObject.CreateInstance<TrackBuildRPoint>();

        p0.baseTransform = transform;
        p1.baseTransform = transform;
        p2.baseTransform = transform;
        p3.baseTransform = transform;

        p0.position = new Vector3(-20, 0, -20);
        p1.position = new Vector3(20, 0, -20);
        p2.position = new Vector3(20, 0, 20);
        p3.position = new Vector3(-20, 0, 20);

        p0.forwardControlPoint = new Vector3(0, 0, -20);
        p1.forwardControlPoint = new Vector3(40, 0, -20);
        p2.forwardControlPoint = new Vector3(0, 0, 20);
        p3.forwardControlPoint = new Vector3(-40, 0, 20);

        p0.leftForwardControlPoint = new Vector3(-15, 0, -20);
        p1.leftForwardControlPoint = new Vector3(25, 0, -20);
        p2.leftForwardControlPoint = new Vector3(5, 0, 20);
        p3.leftForwardControlPoint = new Vector3(-35, 0, 20);

        p0.rightForwardControlPoint = new Vector3(15, 0, -20);
        p1.rightForwardControlPoint = new Vector3(55, 0, -20);
        p2.rightForwardControlPoint = new Vector3(-5, 0, 20);
        p3.rightForwardControlPoint = new Vector3(-45, 0, 20);

        track.AddPoint(p0);
        track.AddPoint(p1);
        track.AddPoint(p2);
        track.AddPoint(p3);

        generator = gameObject.AddComponent<TrackBuildRGenerator>();

        UpdateRender();

        track.diagramMesh = new Mesh();
        track.diagramMesh.vertices = new [] { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1)};
        track.diagramMesh.uv = new [] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0,1), new Vector2(1,1)};
        track.diagramMesh.triangles = new []{1,0,2,1,2,3};

        track.diagramGO = new GameObject("Diagram");
        track.diagramGO.transform.parent = transform;
        track.diagramGO.transform.localPosition = Vector3.zero;
        track.diagramGO.AddComponent<MeshFilter>().mesh = track.diagramMesh;
        track.diagramMaterial = new Material(Shader.Find("Unlit/Texture"));
        track.diagramGO.AddComponent<MeshRenderer>().material = track.diagramMaterial;
        track.diagramGO.AddComponent<MeshCollider>().sharedMesh = track.diagramMesh;
    }

    public void UpdateRender()
    {
        generator.track = track;
        generator.UpdateRender();

        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.isStatic = gameObject.isStatic;
        }

    }

    public void GenerateSecondaryUVSet()
    {
        track.GenerateSecondaryUVSet();
    }

    public void GenerateTangents()
    {
        track.SolveTangents();
    }

    public void OptimseMeshes()
    {
        track.OptimseMeshes();
    }

    void OnDestroy()
    {
        track.Clear();
    }

    void OnDrawGizmos()
    {
        int numberOfPoints = track.numberOfCurves;
        if (numberOfPoints < 1)
            return;
    }

    public void Clear()
    {
        track.Clear();
    }

    public void OnEnable()
    {
        track.diagramGO.SetActive(!Application.isPlaying&&track.showDiagram);
    }
}
