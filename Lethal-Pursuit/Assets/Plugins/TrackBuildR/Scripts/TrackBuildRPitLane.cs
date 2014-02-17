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
/// This is not finished nor implemented - ignore for now...
/// </summary>

public class TrackBuildRPitLane
{
    public enum Sides
    {
        left,
        right
    }

    [SerializeField]
    private TrackBuildRTrack _track;

    [SerializeField]
    private Sides _side = Sides.left;

//    [SerializeField]
//    private bool _hasPitLane = false;
    [SerializeField]
    private List<TrackBuildRPoint> _points = new List<TrackBuildRPoint>();
    [SerializeField]
    private List<TrackBuildRCurve> _curves = new List<TrackBuildRCurve>();

    [SerializeField]
    private int _pitStartPointIndex = -1;
    [SerializeField]
    private int _pitEndPointIndex = -1;

    [SerializeField]
    private TrackBuildRPoint _pointStart = new TrackBuildRPoint();
    [SerializeField]
    private TrackBuildRPoint _pointEnd = new TrackBuildRPoint();

    [SerializeField]
    private TrackBuildRPoint _pointBoundaryStart = new TrackBuildRPoint();
    [SerializeField]
    private TrackBuildRPoint _pointBoundaryEnd = new TrackBuildRPoint();

    public TrackBuildRPoint this[int index]
    {
        get
        {
            if (index < 0)
                Debug.LogError("Index can't be minus");
            if (index >= numberOfPoints)
                Debug.LogError("Index out of range");
            return _points[index];
        }
    }

    public TrackBuildRCurve Curve(int index)
    {
        if (_curves.Count == 0)
            Debug.LogError("There are no curves to access");
        if (index < 0)
            Debug.LogError("Index can't be minus");
        if (index >= _curves.Count)
            Debug.LogError("Index out of range");
        return _curves[index];
    }

    public int numberOfPoints
    {
        get
        {
           return _points.Count;
        }
    }

    public int numberOfCurves { get { return numberOfPoints - 1; } }

    public bool isLegal
    {
        get { return true; }
    }

    public TrackBuildRTrack track
    {
        get
        {
            if (_track == null)
                Debug.Log("No Track Instance Set!!");
            return _track;
        }
        set { _track = value; }
    }

    public TrackBuildRPoint pointStart
    {
        get { return _pointStart; }
    }

    public TrackBuildRPoint pointEnd
    {
        get { return _pointEnd; }
    }

    public int pitStartPointIndex
    {
        get { return _pitStartPointIndex; }
        set
        {
            RecalculateCurves();
            _pitStartPointIndex = value;
        }
    }

    public int pitEndPointIndex
    {
        get { return _pitEndPointIndex; }
        set
        {
            RecalculateCurves();
            _pitEndPointIndex = value;

        }
    }

    public TrackBuildRPoint pointBoundaryStart {get {return _pointBoundaryStart;}}

    public TrackBuildRPoint pointBoundaryEnd {get {return _pointBoundaryEnd;}}

    public Sides side {get {return _side;} set {_side = value;}}

    public TrackBuildRPoint AddPoint(Vector3 position)
    {
        TrackBuildRPoint point = ScriptableObject.CreateInstance<TrackBuildRPoint>();
        point.baseTransform = track.baseTransform;
        point.position = position;
        _points.Add(point);
        track.RecalculateCurves();
        return point;
    }

    public void AddPoint(TrackBuildRPoint point)
    {
        point.baseTransform = track.baseTransform;
        _points.Add(point);
        track.RecalculateCurves();
    }

    public void InsertPoint(TrackBuildRPoint point, int index)
    {
        point.baseTransform = track.baseTransform;
        _points.Insert(index, point);
        track.RecalculateCurves();
    }

    public TrackBuildRPoint InsertPoint(int index)
    {
        TrackBuildRPoint point = ScriptableObject.CreateInstance<TrackBuildRPoint>();
        point.baseTransform = track.baseTransform;
        _points.Insert(index, point);
        track.RecalculateCurves();
        return point;
    }

    public void RemovePoint(TrackBuildRPoint point)
    {
        _points.Remove(point);
        track.RecalculateCurves();
    }

    private void RebuildCurves()
    {
        if (numberOfCurves != _curves.Count)
        {
            List<TrackBuildRCurve> oldCurves = new List<TrackBuildRCurve>(_curves);
            _curves.Clear();
            for (int i = 0; i < numberOfPoints - 1; i++)
            {
                TrackBuildRPoint pointA = _points[i];
                TrackBuildRPoint pointB = _points[i + 1];
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
                    newCurve.name = "Pit Curve " + (i + 1);
                    newCurve.pointA = pointA;
                    newCurve.pointB = pointB;
                }
            }
        }
    }

    public void RecalculateCurves()
    {
        if (_pitStartPointIndex == -1 || _pitEndPointIndex == -1)
            return;

//        _points.Clear();

        Debug.Log(_pointStart);
        Debug.Log(_points.Contains(_pointStart));
        if (!_points.Contains(_pointStart))
            InsertPoint(_pointStart,0);
        if (!_points.Contains(_pointEnd))
            InsertPoint(_pointEnd, numberOfPoints);

        _pointStart.position = _track[_pitStartPointIndex].position;
        _pointEnd.position = _track[_pitEndPointIndex].position;

        if(_track.disconnectBoundary)
        {
            if (!_points.Contains(_pointBoundaryStart))
                InsertPoint(_pointBoundaryStart,1);
            if (!_points.Contains(_pointBoundaryEnd))
                InsertPoint(_pointBoundaryEnd, numberOfPoints-1);
            if(_side == Sides.left)
            {
                _pointBoundaryStart.position = _track[_pitStartPointIndex].leftTrackBoundary;
                _pointBoundaryEnd.position = _track[_pitEndPointIndex].leftTrackBoundary;
            }
            else
            {
                _pointBoundaryStart.position = _track[_pitStartPointIndex].rightTrackBoundary;
                _pointBoundaryEnd.position = _track[_pitEndPointIndex].rightTrackBoundary;
            }
        }else
        {
            if (_points.Contains(_pointBoundaryStart))
                _points.Remove(_pointBoundaryStart);
            if (_points.Contains(_pointBoundaryEnd))
                _points.Remove(_pointBoundaryEnd);
        }

        if (_points.Count < 2)//there is no track with only one point :o)
            return;


        RebuildCurves();

        List<TrackBuildRCurve> calcCurves = new List<TrackBuildRCurve>();
        calcCurves.AddRange(_curves);
        int calcCurveNumber = calcCurves.Count;

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
        for (int i = 0; i < numberOfPoints; i++)
        {
            int pointAIndex = (i - 1);
            if (pointAIndex < 0)
                pointAIndex = numberOfPoints - 1;
            int pointBIndex = i;
            int pointCIndex = (i + 1) % numberOfPoints;
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

                int storedPointSize = Mathf.RoundToInt(calculatedTotalArcLength / _track.meshResolution);
                curve.storedPointSize = storedPointSize;
                curve.normalisedT = new float[storedPointSize];
                curve.targetSize = new float[storedPointSize];
                curve.midPointPerc = new float[storedPointSize];
                curve.prevNormIndex = new int[storedPointSize];
                curve.nextNormIndex = new int[storedPointSize];

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
                int lastCurveIndex = (i > 0) ? i - 1 : 0;
                int nextCurveIndex = (i < numberOfCurves - 1) ? i + 1 : 0;
                TrackBuildRCurve lastcurve = _curves[lastCurveIndex];
                TrackBuildRCurve nextcurve = _curves[nextCurveIndex];

                int storedPointSize = curve.storedPointSize;
                for (int p = 0; p < storedPointSize; p++)
                {
                    int pA = p - 1;
                    int pB = p;
                    int pC = p + 1;
                    if (pA < 0) pA = 0;
                    if (pC >= storedPointSize) pC = storedPointSize - 1;

                    Vector3 sampledPointA = curve.sampledPoints[pA];
                    Vector3 sampledPointB = curve.sampledPoints[pB];
                    Vector3 sampledPointC = curve.sampledPoints[pC];

                    if (p == 0 && lastcurve != null) sampledPointA = lastcurve.sampledPoints[lastcurve.storedPointSize - 2];//retrieve the penultimate point from the last curve
                    if (p == storedPointSize - 1 && nextcurve != null) sampledPointC = nextcurve.sampledPoints[1];//retrieve the second point from the next curve

                    Vector3 sampledTrackDirectionA = (sampledPointB - sampledPointA);
                    Vector3 sampledTrackDirectionB = (sampledPointC - sampledPointB);
                    Vector3 sampledTrackDirection = (sampledTrackDirectionA + sampledTrackDirectionB).normalized;
                    curve.sampledTrackDirections[pB] = sampledTrackDirection;
                    float sampledCantRad = curve.sampledCants[pB] * Mathf.Deg2Rad;
                    Vector3 sampledTrackUp = Quaternion.LookRotation(sampledTrackDirection) * new Vector3(Mathf.Sin(sampledCantRad), Mathf.Cos(sampledCantRad), 0);
                    if (sampledTrackUp == Vector3.zero)
                    {
                        Debug.Log(sampledTrackDirection);
                        Debug.Log(sampledCantRad);
                    }
                    curve.sampledTrackUps[pB] = sampledTrackUp;
                    curve.sampledTrackCrosses[pB] = Vector3.Cross(sampledTrackUp, sampledTrackDirection);
                    curve.sampledAngles[pB] = Vector3.Angle(sampledTrackDirectionA, sampledTrackDirectionB) * -Mathf.Sign(Vector3.Dot((sampledTrackDirectionB - sampledTrackDirectionA), curve.sampledTrackCrosses[pB]));
                }
            }
        }

        foreach (TrackBuildRPoint point in _points)
        {
            if (point.isDirty)
                point.shouldReRender = true;//if nextNormIndex point was dirty, ensure it's rerendered
            point.isDirty = false;//reset all points - data is no longer considered dirty
        }
    }
}
