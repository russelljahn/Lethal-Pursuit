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
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

/// <summary>
/// Track BuildR Track holds all the point and curve data
/// It is responsible for holding other track based data
/// It also generates all the curve data in Recalculate Data which is used in Generator class
/// It has some functions to allow other scripts to access basic track information
/// </summary>

[System.Serializable]
public class TrackBuildRTrack
{
    [SerializeField]
    private List<TrackBuildRPoint> _points = new List<TrackBuildRPoint>();

    [SerializeField]
    private List<TrackBuildRCurve> _curves = new List<TrackBuildRCurve>();
    
    [SerializeField]
    private bool _looped = true;

    public float trackResolution = 1.0f;//tris per metre
    public Transform baseTransform;
    private const float CLIP_THREASHOLD = 0.5f;

    //public float boundaryHeight = 2.0f;
    [SerializeField]
    private bool _disconnectBoundary = false;

    [SerializeField]
    private bool _renderBoundaryWallReverse = false;

    public bool includeCollider = true;
    public bool includeColliderRoof = true;
    public float trackColliderWallHeight = 5.0f;

    public bool trackBumpers = true;
    public float bumperAngleThresold = 0.5f;
    public float bumperWidth = 0.5f;
    public float bumperHeight = 0.03f;

    [SerializeField]
    private bool _hasPitLane;
    [SerializeField]
    private TrackBuildRPitLane _pitlane = null;

//    public bool showWireframes = true;
    public bool drawMode = false;

    public GameObject diagramGO;
    public Mesh diagramMesh;
    public string diagramFilepath = "";
    public Material diagramMaterial = null;
    public float scale = 1.0f;
    public Vector3 scalePointA = Vector3.zero;
    public Vector3 scalePointB = Vector3.right;
    public bool showDiagram = false;
    public int assignedPoints = 0;

    [SerializeField]
    private bool _showWireframe = true;

    [SerializeField]
    private float _trackLength;

    [SerializeField]
    private float _meshResolution = 1.5f;//the world unit distance for mesh face sizes for nextNormIndex completed mesh
    public float editMeshResolution = 3.0f;//the world unit distance for mesh face sizes - used when editing the track to reduce redraw time

    [SerializeField]
    private List<TrackBuildRTexture> _textures = new List<TrackBuildRTexture>();

    [SerializeField]
    private bool _tangentsGenerated = false;
    [SerializeField]
    private bool _lightmapGenerated = false;
    [SerializeField]
    private bool _optimised = false;

    public void Init()
    {
        _textures.Add(new TrackBuildRTexture("Track Texture"));
        _textures.Add(new TrackBuildRTexture("Offroad Texture"));
        _textures.Add(new TrackBuildRTexture("Wall Texture"));
        _textures.Add(new TrackBuildRTexture("Bumper Texture"));
    }

    public TrackBuildRPoint this[int index]
    {
        get
        {
            if (_looped && index > _points.Count - 1)//loop value around
                index = index % _points.Count;
            if (index < 0)
                Debug.LogError("Index can't be minus");
            if (index >= _points.Count)
                Debug.LogError("Index out of range");
            return _points[index];
        }
    }

    public TrackBuildRCurve Curve(int index)
    {
        if (_curves.Count == 0)
            Debug.LogError("There are no curves to access");
        if (_looped && index > _curves.Count - 1)//loop value around
            index = index % _curves.Count;
        if (index < 0)
            Debug.LogError("Index can't be minus");
        if (index >= _curves.Count)
            Debug.LogError("Index out of range");
        return _curves[index];
    }

    public TrackBuildRTexture Texture(int index)
    {
        if (_textures.Count == 0)
            Debug.LogError("There are no textures to access");
        if (index < 0)
            Debug.LogError("Index can't be minus");
        if (index >= _textures.Count)
            Debug.LogError("Index out of range");
        return _textures[index];
    }

    public void AddTexture()
    {
        _textures.Add(new TrackBuildRTexture("new texture " + (_textures.Count+1)));
    }

    public void RemoveTexture(TrackBuildRTexture texture)
    {
        int textureIndex = _textures.IndexOf(texture);
        _textures.Remove(texture);
        int numberOfTextures = _textures.Count;
        //ensure that curves are not referenceing textures that no longer exist
        for(int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRCurve curve = _curves[i];

            if (curve.trackTextureStyleIndex > textureIndex)
                curve.trackTextureStyleIndex--;
            if (curve.offroadTextureStyleIndex > textureIndex)
                curve.offroadTextureStyleIndex--;
            if (curve.bumperTextureStyleIndex > textureIndex)
                curve.bumperTextureStyleIndex--;
            if (curve.boundaryTextureStyleIndex > textureIndex)
                curve.boundaryTextureStyleIndex--;

            if (curve.trackTextureStyleIndex >= numberOfTextures)
                curve.trackTextureStyleIndex = numberOfTextures - 1;
            if (curve.offroadTextureStyleIndex >= numberOfTextures)
                curve.offroadTextureStyleIndex = numberOfTextures - 1;
            if (curve.bumperTextureStyleIndex >= numberOfTextures)
                curve.bumperTextureStyleIndex = numberOfTextures - 1;
            if (curve.boundaryTextureStyleIndex >= numberOfTextures)
                curve.boundaryTextureStyleIndex = numberOfTextures - 1;
        }
    }

    public int numberOfTextures
    {
        get {return _textures.Count;}
    }

    public TrackBuildRTexture[] GetTexturesArray()
    {
        return _textures.ToArray();
    }

    public int numberOfPoints
    {
        get
        {
            if(_points.Count==0)
                return 0;
            return (_looped) ? _points.Count + 1 : _points.Count;
        }
    }

    public int realNumberOfPoints { get { return _points.Count; } }


    public bool tangentsGenerated { get { return _tangentsGenerated; } }
    public bool lightmapGenerated { get { return _lightmapGenerated; } }
    public bool optimised { get { return _optimised; } }

    public int numberOfCurves
    {
        get
        {
            if (_points.Count < 2)
                return 0;
            return numberOfPoints - 1;
        }
    }

    public bool loop
    {
        get { return _looped; }
        set
        {
            if(_looped!=value)
            {
                _looped = value;
                SetTrackDirty();
                GUI.changed = true;
            }
        }
    }

    public float trackLength { get { return _trackLength; } }

    public float meshResolution
    {
        get
        {
            return _meshResolution;
        }
        set
        {
            _meshResolution = value;
        }
    }

    public bool disconnectBoundary {get {return _disconnectBoundary;} 
        set
        {
            _disconnectBoundary = value;

            if(value)
            {
                foreach(TrackBuildRPoint point in _points)
                {
                    if (point.leftForwardControlPoint == Vector3.zero)
                        point.leftForwardControlPoint = point.forwardControlPoint;
                    if (point.rightForwardControlPoint == Vector3.zero)
                        point.rightForwardControlPoint = point.forwardControlPoint;
                }
            }
        }
    }

    public bool hasPitLane {get {return _hasPitLane;} set
    {
        if(_hasPitLane!=value)
        {
            SetTrackDirty();
            _hasPitLane = value;
        }
    }}

    public bool showWireframe
    {
        get
        {
            return _showWireframe;
        } 
        set
        {
#if UNITY_EDITOR
            if(_showWireframe!=value)
            {
                foreach(TrackBuildRCurve curve in _curves)
                {
                    foreach (MeshRenderer holderR in curve.holder.GetComponentsInChildren<MeshRenderer>())
                    {
                        
#if UNITY_EDITOR
                        EditorUtility.SetSelectedWireframeHidden(holderR, !value);
#endif
                    }
                }
                _showWireframe = value;
            }
#endif
        }
    }

    public TrackBuildRPitLane pitlane
    {
        get {return _pitlane ?? (_pitlane = new TrackBuildRPitLane());}
        set {_pitlane = value;}
    }

    public bool isDirty
    {
        get
        {
            foreach (TrackBuildRPoint point in _points)
                if (point.isDirty) return true;
            return false;
        }
    }

    public bool renderBoundaryWallReverse
    {
        get {return _renderBoundaryWallReverse;} 
        set
        {
            if (_renderBoundaryWallReverse != value)
            {
                ReRenderTrack();
                _renderBoundaryWallReverse = value;
            }
        }
    }

    public TrackBuildRPoint AddPoint(Vector3 position)
    {
        TrackBuildRPoint point = ScriptableObject.CreateInstance<TrackBuildRPoint>();
        point.baseTransform = baseTransform;
        point.position = position;
        point.isDirty = true;
        _points.Add(point);
        RecalculateCurves();
        return point;
    }

    public void AddPoint(TrackBuildRPoint point)
    {
        point.baseTransform = baseTransform;
        _points.Add(point);
        RecalculateCurves();
    }

    public void InsertPoint(TrackBuildRPoint point, int index)
    {
        point.baseTransform = baseTransform;
        _points.Insert(index, point);
        RecalculateCurves();
    }

    public TrackBuildRPoint InsertPoint(int index)
    {
        TrackBuildRPoint point = ScriptableObject.CreateInstance<TrackBuildRPoint>();
        point.baseTransform = baseTransform;
        _points.Insert(index, point);
        RecalculateCurves();
        return point;
    }

    public void RemovePoint(TrackBuildRPoint point)
    {
        if(_points.Count < 3)
        {
            Debug.Log("We can't see any point in allowing you to delete any more points so we're not going to do it.");
            return;
        }
        
        _points.Remove(point);
        RecalculateCurves();
    }

    //Sample nextNormIndex position on the entire curve based on time (0-1)
    public Vector3 GetTrackPosition(float t)
    {
        if(realNumberOfPoints<2)
        {
            Debug.LogError("Not enough points to define a curve");
            return Vector3.zero;
        }
        float curveT = 1.0f / numberOfCurves;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfCurves);
        TrackBuildRPoint pointA = GetPoint(point);
        TrackBuildRPoint pointB = GetPoint(point + 1);

        return SplineMaths.CalculateBezierPoint(ct, pointA.position, pointA.forwardControlPoint, pointB.backwardControlPoint, pointB.position);
    }

    public float GetTrackWidth(float t)
    {
        float curveT = 1.0f / numberOfCurves;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfCurves);
        float hermite = SplineMaths.CalculateHermite(ct);
        TrackBuildRPoint pointA = GetPoint(point);
        TrackBuildRPoint pointB = GetPoint(point + 1);
        return Mathf.Lerp(pointA.width, pointB.width, hermite);
    }

    public float GetTrackCant(float t)
    {
        float curveT = 1.0f / numberOfCurves;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfCurves);
        float hermite = SplineMaths.CalculateHermite(ct);
        TrackBuildRPoint pointA = GetPoint(point);
        TrackBuildRPoint pointB = GetPoint(point + 1);
        return Mathf.LerpAngle(pointA.cant, pointB.cant, hermite);
    }

    public float GetTrackCrownAngle(float t)
    {
        float curveT = 1.0f / numberOfCurves;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfCurves);
        float hermite = SplineMaths.CalculateHermite(ct);
        TrackBuildRPoint pointA = GetPoint(point);
        TrackBuildRPoint pointB = GetPoint(point + 1);
        return Mathf.LerpAngle(pointA.crownAngle, pointB.crownAngle, hermite);
    }

    public float GetTrackPercentage(TrackBuildRPoint point)
    {
        int index = _points.IndexOf(point);
        return index / (float)numberOfPoints;
    }

    public float GetTrackPercentage(int pointIndex)
    {
        return pointIndex / (float)numberOfPoints;
    }

    public int GetNearestPointIndex(float trackPercentage)
    {
        return Mathf.RoundToInt(numberOfCurves * trackPercentage);
    }

    public int GetLastPointIndex(float trackPercentage)
    {
        return Mathf.FloorToInt(numberOfCurves * trackPercentage);
    }

    public int GetNextPointIndex(float trackPercentage)
    {
        return Mathf.CeilToInt(numberOfCurves * trackPercentage);
    }


    //Sample nextNormIndex position on the entire curve based on time (0-1)
    public Vector3 GetLeftBoundaryPosition(float t)
    {
        float curveT = 1.0f / numberOfCurves;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfCurves);
        TrackBuildRPoint pointA = GetPoint(point);
        TrackBuildRPoint pointB = GetPoint(point + 1);

        return SplineMaths.CalculateBezierPoint(ct, pointA.leftTrackBoundary, pointA.leftForwardControlPoint, pointB.leftBackwardControlPoint, pointB.leftTrackBoundary);
    }
    //Sample nextNormIndex position on the entire curve based on time (0-1)
    public Vector3 GetRightBoundaryPosition(float t)
    {
        float curveT = 1.0f / numberOfCurves;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * numberOfCurves);
        TrackBuildRPoint pointA = GetPoint(point);
        TrackBuildRPoint pointB = GetPoint(point + 1);

        return SplineMaths.CalculateBezierPoint(ct, pointA.rightTrackBoundary, pointA.rightForwardControlPoint, pointB.rightBackwardControlPoint, pointB.rightTrackBoundary);
    }

    private void RebuildCurves()
    {
        if (numberOfCurves != _curves.Count)
        {
            List<TrackBuildRCurve> oldCurves = new List<TrackBuildRCurve>(_curves);
            _curves.Clear();
            for (int i = 0; i < numberOfCurves; i++)
            {
                TrackBuildRPoint pointA = _points[i];
                TrackBuildRPoint pointB = _points[(i + 1)%realNumberOfPoints];
                bool pointHasCurve = false;
                TrackBuildRCurve likelyCurve = null;
                foreach (TrackBuildRCurve oldCurve in oldCurves)
                {
                    if (oldCurve.pointA == pointA)
                    {
                        if (oldCurve.pointB == pointB)
                        {
                            _curves.Add(oldCurve);//old curve still exists
                            oldCurves.Remove(oldCurve);
                            pointHasCurve = true;
                        }
                        else
                        {
                            likelyCurve = oldCurve;
                        }
                        break;
                    }
                    if (oldCurve.pointB == pointB)
                    {
                        likelyCurve = oldCurve;
                    }
                }
                if (!pointHasCurve)//rebuild curve
                {
                    pointA.isDirty = true;
                    pointB.isDirty = true;
                    TrackBuildRCurve newCurve;
                    if (likelyCurve != null)
                    {
                        newCurve = (TrackBuildRCurve)Object.Instantiate(likelyCurve);
                    }
                    else
                    {
                        newCurve = ScriptableObject.CreateInstance<TrackBuildRCurve>();
                    }
                    _curves.Add(newCurve);
                    newCurve.name = "Curve " + (i + 1);
                    newCurve.pointA = pointA;
                    newCurve.pointB = pointB;
                }
            }
            while(oldCurves.Count>0)
            {
                TrackBuildRCurve oldCurve = oldCurves[0];
                Object.DestroyImmediate(oldCurve.holder);
                Object.DestroyImmediate(oldCurve);
                oldCurves.RemoveAt(0);
            }
        }
    }

    public void RecalculateCurves()
    {
        if (_points.Count < 2)//there is no track with only one point :o)
            return;

        RebuildCurves();

        List<TrackBuildRCurve> calcCurves = new List<TrackBuildRCurve>();
        calcCurves.AddRange(_curves);
//        for (int pc = 0; pc < pitlane.numberOfCurves; pc++)
//            calcCurves.Add(pitlane.Curve(pc));
        int calcCurveNumber = calcCurves.Count;

        if (isDirty)
        {
            _tangentsGenerated = false;
            _lightmapGenerated = false;
            _optimised = false;
        }

        //calculate approx bezier arc length per curve
        for (int i = 0; i < calcCurveNumber; i++)
        {
            TrackBuildRPoint pointA = calcCurves[i].pointA;
            TrackBuildRPoint pointB = calcCurves[i].pointB;
            float thisArcLength = 0;
            thisArcLength += Vector3.Distance(pointA.position, pointA.forwardControlPoint);
            thisArcLength += Vector3.Distance(pointA.forwardControlPoint, pointB.backwardControlPoint);
            thisArcLength += Vector3.Distance(pointB.backwardControlPoint, pointB.position);
            _curves[i].arcLength = thisArcLength;
        }

        //set track directions for point based calculations
        for (int i = 0; i < realNumberOfPoints; i++)
        {
            int pointAIndex = (i - 1);
            if (pointAIndex < 0)
                pointAIndex = realNumberOfPoints - 1;
            int pointBIndex = i;
            int pointCIndex = (i + 1) % realNumberOfPoints;
            TrackBuildRPoint pointA = _points[pointAIndex];
            TrackBuildRPoint pointB = _points[pointBIndex];
            TrackBuildRPoint pointC = _points[pointCIndex];
            Vector3 trackDirectionPointA = SplineMaths.CalculateBezierPoint(0.99f, pointA.position, pointA.forwardControlPoint, pointB.backwardControlPoint, pointB.position);
            Vector3 trackDirectionPointB = SplineMaths.CalculateBezierPoint(0.01f, pointB.position, pointB.forwardControlPoint, pointC.backwardControlPoint, pointC.position);
            Vector3 trackDirection = (trackDirectionPointB - trackDirectionPointA).normalized;
            if (trackDirection != pointB.trackDirection)
                pointB.isDirty = true;
            pointB.trackDirection = trackDirection;
        }
        //set track directions for the pit lane
        /*for (int i = 0; i < _pitlane.numberOfPoints; i++)
        {
            int pointAIndex = (i - 1);
            int pointBIndex = i;
            int pointCIndex = (i + 1);
            TrackBuildRPoint pointA, pointB, pointC;
            Vector3 trackDirectionPointA, trackDirectionPointB, trackDirection;
            pointB = _points[pointBIndex];
            if(i>0)
            {
                pointA = _points[pointAIndex];
                trackDirectionPointA = SplineMaths.CalculateBezierPoint(0.99f, pointA.position, pointA.forwardControlPoint, pointB.backwardControlPoint, pointB.position);
            }
            if (i < pitlane.numberOfPoints-1)
            {
                pointC = _points[pointCIndex];
                trackDirectionPointB = SplineMaths.CalculateBezierPoint(0.01f, pointB.position, pointB.forwardControlPoint, pointC.backwardControlPoint, pointC.position);  
            }
            trackDirection = (trackDirectionPointB - trackDirectionPointA).normalized;
            if (trackDirection != pointB.trackDirection)
                pointB.isDirty = true;
            pointB.trackDirection = trackDirection;
        }*/

        foreach (TrackBuildRCurve curve in _curves)
        {
            if (!curve.isDirty)//only recalculate modified points
                continue;

            if (curve.arcLength > 0)
            {
                TrackBuildRPoint pointA = curve.pointA;
                TrackBuildRPoint pointB = curve.pointB;

                //Build accurate arc length data into curve
                curve.center = Vector3.zero;
                int arcLengthResolution = Mathf.Max(Mathf.RoundToInt(curve.arcLength * 10), 1);
                float alTime = 1.0f / arcLengthResolution;
                float calculatedTotalArcLength = 0;
                curve.storedArcLengthsFull = new float[arcLengthResolution];
                curve.storedArcLengthsFull[0] = 0.0f;
                Vector3 pA = curve.pointA.position;
                for (int i = 0; i < arcLengthResolution - 1; i++)
                {
                    curve.center += pA;
                    float altB = alTime * (i + 1) + alTime;
                    Vector3 pB = SplineMaths.CalculateBezierPoint(altB, curve.pointA.position, curve.pointA.forwardControlPoint, curve.pointB.backwardControlPoint, curve.pointB.position);
                    float arcLength = Vector3.Distance(pA, pB);
                    calculatedTotalArcLength += arcLength;
                    curve.storedArcLengthsFull[i + 1] = calculatedTotalArcLength;
                    pA = pB;//switch over values so we only calculate the bezier once
                }
                curve.arcLength = calculatedTotalArcLength;
                curve.center /= arcLengthResolution;

                int storedPointSize = Mathf.RoundToInt(calculatedTotalArcLength / meshResolution);
                curve.storedPointSize = storedPointSize;
                curve.normalisedT = new float[storedPointSize];
                curve.targetSize = new float[storedPointSize];
                curve.midPointPerc = new float[storedPointSize];
                curve.prevNormIndex = new int[storedPointSize];
                curve.nextNormIndex = new int[storedPointSize];
                curve.clipArrayLeft = new bool[storedPointSize];
                curve.clipArrayRight = new bool[storedPointSize];

                //calculate normalised spline data
                int normalisedIndex = 0;
                curve.normalisedT[0] = 0;
                for (int p = 1; p < storedPointSize; p++)
                {
                    float t = p / (float)(storedPointSize - 1);
                    float targetLength = t * calculatedTotalArcLength;
                    curve.targetSize[p] = targetLength;
                    int it = 1000;
                    while (targetLength > curve.storedArcLengthsFull[normalisedIndex])
                    {
                        normalisedIndex++;
                        it--;
                        if (it < 0)
                        {
                            Debug.LogError("ERROR");
                            return;
                        }
                    }

                    normalisedIndex = Mathf.Min(normalisedIndex, arcLengthResolution);//ensure we've not exceeded the length

                    int prevNormIndex = Mathf.Max((normalisedIndex - 1), 0);
                    int nextNormIndex = normalisedIndex;

                    float lengthBefore = curve.storedArcLengthsFull[prevNormIndex];
                    float lengthAfter = curve.storedArcLengthsFull[nextNormIndex];
                    float midPointPercentage = (targetLength - lengthBefore) / (lengthAfter - lengthBefore);
                    curve.midPointPerc[p] = midPointPercentage;
                    curve.prevNormIndex[p] = prevNormIndex;
                    curve.nextNormIndex[p] = nextNormIndex;
                    float normalisedT = (normalisedIndex + midPointPercentage) / arcLengthResolution;//lerp between the values to get the exact normal T
                    curve.normalisedT[p] = normalisedT;
                }

                curve.sampledPoints = new Vector3[storedPointSize];
                curve.sampledLeftBoundaryPoints = new Vector3[storedPointSize];
                curve.sampledRightBoundaryPoints = new Vector3[storedPointSize];
                curve.sampledCants = new float[storedPointSize];
                curve.sampledWidths = new float[storedPointSize];
                curve.sampledCrowns = new float[storedPointSize];
                curve.sampledTrackDirections = new Vector3[storedPointSize];
                curve.sampledTrackUps = new Vector3[storedPointSize];
                curve.sampledTrackCrosses = new Vector3[storedPointSize];
                curve.sampledAngles = new float[storedPointSize];
                for (int p = 0; p < storedPointSize; p++)
                {
                    float tN = curve.normalisedT[p];
                    float tH = SplineMaths.CalculateHermite(tN);
                    curve.sampledPoints[p] = SplineMaths.CalculateBezierPoint(tN, pointA.position, pointA.forwardControlPoint, pointB.backwardControlPoint, pointB.position);
                    curve.sampledLeftBoundaryPoints[p] = SplineMaths.CalculateBezierPoint(tN, pointA.leftTrackBoundary, pointA.leftForwardControlPoint, pointB.leftBackwardControlPoint, pointB.leftTrackBoundary);
                    curve.sampledRightBoundaryPoints[p] = SplineMaths.CalculateBezierPoint(tN, pointA.rightTrackBoundary, pointA.rightForwardControlPoint, pointB.rightBackwardControlPoint, pointB.rightTrackBoundary);
                    float sampledCant = Mathf.LerpAngle(pointA.cant, pointB.cant, tH);
                    curve.sampledCants[p] = sampledCant;
                    curve.sampledWidths[p] = Mathf.LerpAngle(pointA.width, pointB.width, tH);
                    curve.sampledCrowns[p] = Mathf.LerpAngle(pointA.crownAngle, pointB.crownAngle, tH);
                }
            }
        }

        for (int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRCurve curve = _curves[i];
            if (!curve.isDirty)//only recalculate modified points
                continue;
            if (curve.arcLength > 0)
            {
                int lastCurveIndex = (i > 0) ? i - 1 : (_looped) ? numberOfCurves - 1 : 0;
                int nextCurveIndex = (i < numberOfCurves - 1) ? i + 1 : (_looped) ? 0 : numberOfCurves - 1;
                TrackBuildRCurve lastcurve = _curves[lastCurveIndex];
                TrackBuildRCurve nextcurve = _curves[nextCurveIndex];

                int storedPointSize = curve.storedPointSize;
                for (int p = 0; p < storedPointSize; p++)
                {
                    int pA = p - 1;
                    int pB = p;
                    int pC = p + 1;
                    if(pA < 0) pA = 0;
                    if(pC >= storedPointSize) pC = storedPointSize - 1;

                    Vector3 sampledPointA = curve.sampledPoints[pA];
                    Vector3 sampledPointB = curve.sampledPoints[pB];
                    Vector3 sampledPointC = curve.sampledPoints[pC];

                    if(p == 0 && lastcurve != null) sampledPointA = lastcurve.sampledPoints[lastcurve.storedPointSize - 2];//retrieve the penultimate point from the last curve
                    if(p == storedPointSize - 1 && nextcurve != null) sampledPointC = nextcurve.sampledPoints[1];//retrieve the second point from the next curve

                    Vector3 sampledTrackDirectionA = (sampledPointB - sampledPointA);
                    Vector3 sampledTrackDirectionB = (sampledPointC - sampledPointB);
                    Vector3 sampledTrackDirection = (sampledTrackDirectionA + sampledTrackDirectionB).normalized;
                    curve.sampledTrackDirections[pB] = sampledTrackDirection;
                    float sampledCantRad = curve.sampledCants[pB] * Mathf.Deg2Rad;
                    Vector3 sampledTrackUp = Quaternion.LookRotation(sampledTrackDirection) * new Vector3(Mathf.Sin(sampledCantRad), Mathf.Cos(sampledCantRad), 0);
                    if(sampledTrackUp == Vector3.zero)
                    {
                        Debug.Log(sampledTrackDirection);
                        Debug.Log(sampledCantRad);
                    }
                    curve.sampledTrackUps[pB] = sampledTrackUp;
                    curve.sampledTrackCrosses[pB] = Vector3.Cross(sampledTrackUp, sampledTrackDirection);
                    curve.sampledAngles[pB] = Vector3.Angle(sampledTrackDirectionA, sampledTrackDirectionB) * -Mathf.Sign(Vector3.Dot((sampledTrackDirectionB - sampledTrackDirectionA), curve.sampledTrackCrosses[pB]));
//Need to work out a successful method for this - if there is one...
//                    if (p > 0 && p < storedPointSize - 1)
//                    {
//                        //clip array
//                        //LEFT
//                        Vector3 sampledPointLeftA = curve.sampledLeftBoundaryPoints[pB];
//                        Vector3 sampledPointLeftB = curve.sampledLeftBoundaryPoints[pC];
//                        Vector3 leftBoundaryDirection = (sampledPointLeftB - sampledPointLeftA);
//                        curve.clipArrayLeft[pB] = Vector3.Dot(leftBoundaryDirection, sampledTrackDirection) > CLIP_THREASHOLD;
//                        //RIGHT
//                        Vector3 sampledPointRightA = curve.sampledRightBoundaryPoints[pB];
//                        Vector3 sampledPointRightB = curve.sampledRightBoundaryPoints[pC];
//                        Vector3 rightBoundaryDirection = (sampledPointRightB - sampledPointRightA);
//                        curve.clipArrayRight[pB] = Vector3.Dot(rightBoundaryDirection, sampledTrackDirection) > CLIP_THREASHOLD;
//                    }else
//                    {
                        curve.clipArrayLeft[pB] = true;
                        curve.clipArrayRight[pB] = true;
//                    }
                }
            }
        }

        foreach(TrackBuildRPoint point in _points)
        {
            if (point.isDirty)
                point.shouldReRender = true;//if nextNormIndex point was dirty, ensure it's rerendered
            point.isDirty = false;//reset all points - data is no longer considered dirty
        }

        //recalculate track length
        _trackLength = 0;
        foreach(TrackBuildRCurve curve in _curves)
        {
            _trackLength += curve.arcLength;
        }

//        pitlane.track = this;
//        pitlane.RecalculateCurves();
    }

    public float GetNearestPoint(Vector3 fromPostition)
    {
        int testPoints = 10;
        float testResolution = 1.0f / testPoints;
        float nearestPercentage = 0;
        float nearestPercentageSqrDistance = Mathf.Infinity;
        for (float i = 0; i < 1; i += testResolution)
        {
            Vector3 point = GetTrackPosition(i);
            Vector3 difference = point - fromPostition;
            float newSqrDistance = Vector3.SqrMagnitude(difference);
            if (nearestPercentageSqrDistance > newSqrDistance)
            {
                nearestPercentage = i;
                nearestPercentageSqrDistance = newSqrDistance;
            }
        }
        for (int r = 0; r < 2; r++)
        {
            float refinedResolution = testResolution / testPoints;
            float startSearch = nearestPercentage - testResolution / 2;
            float endSearch = nearestPercentage + testResolution / 2;
            for (float i = startSearch; i < endSearch; i += refinedResolution)
            {
                Vector3 point = GetTrackPosition(i);
                Vector3 difference = point - fromPostition;
                float newSqrDistance = Vector3.SqrMagnitude(difference);
                if (nearestPercentageSqrDistance > newSqrDistance)
                {
                    nearestPercentage = i;
                    nearestPercentageSqrDistance = newSqrDistance;
                }
            }
        }
        return nearestPercentage;
    }

    public void Clear()
    {

        int numCurves = numberOfCurves;
        for (int i = 0; i < numCurves; i++)
        {
            if (_curves[i].holder != null)
            {
                Object.DestroyImmediate(_curves[i].holder);
            }
            Object.DestroyImmediate(_curves[i]);
        }
        _curves.Clear();

        for (int i = 0; i < realNumberOfPoints; i++)
        {
            Object.DestroyImmediate(_points[i]);
        }
        _points.Clear();
    }


    public TrackBuildRPoint GetPoint(int index)
    {
        if (_points.Count == 0)
            return null;
        if (!_looped)
        {
            return _points[Mathf.Clamp(index, 0, numberOfCurves)];
        }
        if (index >= numberOfCurves)
            index = index - numberOfCurves;
        if (index < 0)
            index = index + numberOfCurves;

        return _points[index];
    }


    private float SignedAngle(Vector3 from, Vector3 to, Vector3 up)
    {
        Vector3 direction = (to - from).normalized;
        Vector3 cross = Vector3.Cross(up, direction);
        float dot = Vector3.Dot(from, cross);
        return Vector3.Angle(from, to) * Mathf.Sign(dot);
    }

    /// <summary>
    /// Set all point data as clean
    /// </summary>
    public void CleanDirt()
    {
        foreach (TrackBuildRPoint point in _points)
        {
            point.isDirty = false;
        }
        for (int i = 0; i < pitlane.numberOfPoints; i++ )
        {
            pitlane[i].isDirty = false;
        }
    }

    /// <summary>
    /// Set all point data as rendered
    /// </summary>
    public void TrackRendered()
    {
        foreach (TrackBuildRPoint point in _points)
        {
            point.shouldReRender = false;
        }
//        int numberOfPitlanPoints = pitlane.numberOfPoints;
//        for (int i = 0; i < numberOfPitlanPoints; i++)
//        {
//            pitlane[i].shouldReRender = false;
//        }
    }

    /// <summary>
    /// Mark the entire track as dirty so it will be recalculated/rebuilt
    /// </summary>
    public void SetTrackDirty()
    {
        foreach (TrackBuildRPoint point in _points)
        {
            point.isDirty = true;
        }
//        for (int i = 0; i < pitlane.numberOfPoints; i++)
//        {
//            pitlane[i].isDirty = true;
//        }
//        SetPitDirty();
    }

    /// <summary>
    /// Set all point data to rerender
    /// </summary>
    public void ReRenderTrack()
    {
        foreach (TrackBuildRPoint point in _points)
        {
            point.shouldReRender = true;
        }
//        for (int i = 0; i < pitlane.numberOfPoints; i++)
//        {
//            pitlane[i].shouldReRender = true;
//        }
//        ReRenderPit();
    }

//    /// <summary>
//    /// Mark the pit lane as dirty so it will be recalculated/rebuilt
//    /// </summary>
//    public void SetPitDirty()
//    {
//        for (int i = 0; i < pitlane.numberOfPoints; i++)
//        {
//            pitlane[i].isDirty = true;
//        }
//    }
//
//    /// <summary>
//    /// Set pit lane point data to rerender
//    /// </summary>
//    public void ReRenderPit()
//    {
//        for (int i = 0; i < pitlane.numberOfPoints; i++)
//        {
//            pitlane[i].shouldReRender = true;
//        }
//    }


    public void SolveTangents()
    {
        for (int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRCurve curve = Curve(i);
            curve.dynamicTrackMesh.SolveTangents();
            curve.dynamicOffroadMesh.SolveTangents();
            curve.dynamicBumperMesh.SolveTangents();
            curve.dynamicBoundaryMesh.SolveTangents();
        }
        _tangentsGenerated = true;
    }

    public void GenerateSecondaryUVSet()
    {
#if UNITY_EDITOR
        for (int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRCurve curve = Curve(i);

            for (int m = 0; m < curve.dynamicTrackMesh.meshCount; m++)
                Unwrapping.GenerateSecondaryUVSet(curve.dynamicTrackMesh[m].mesh);
            for (int m = 0; m < curve.dynamicOffroadMesh.meshCount; m++)
                Unwrapping.GenerateSecondaryUVSet(curve.dynamicOffroadMesh[m].mesh);
            for (int m = 0; m < curve.dynamicBumperMesh.meshCount; m++)
                Unwrapping.GenerateSecondaryUVSet(curve.dynamicBumperMesh[m].mesh);
            for (int m = 0; m < curve.dynamicBoundaryMesh.meshCount; m++)
                Unwrapping.GenerateSecondaryUVSet(curve.dynamicBoundaryMesh[m].mesh);
        }
        _lightmapGenerated = true;
#endif
    }

    public void OptimseMeshes()
    {
#if UNITY_EDITOR
        for (int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRCurve curve = Curve(i);

            for (int m = 0; m < curve.dynamicTrackMesh.meshCount; m++)
                MeshUtility.Optimize(curve.dynamicTrackMesh[m].mesh);
            for (int m = 0; m < curve.dynamicOffroadMesh.meshCount; m++)
                MeshUtility.Optimize(curve.dynamicOffroadMesh[m].mesh);
            for (int m = 0; m < curve.dynamicBumperMesh.meshCount; m++)
                MeshUtility.Optimize(curve.dynamicBumperMesh[m].mesh);
            for (int m = 0; m < curve.dynamicBoundaryMesh.meshCount; m++)
                MeshUtility.Optimize(curve.dynamicBoundaryMesh[m].mesh);
        }
        _optimised = true;
#endif
    }
}
