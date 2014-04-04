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

public class TrackBuildRMonitor : MonoBehaviour
{

    public TrackBuildR track = null;
    private TrackBuildRTrack trackData;
//    private int curveIndex = 0;
//    private int curvePointIndex = 0;

    public void Awake()
    {
        if (track == null)
        {
            Debug.LogWarning("Track BuildR Monitor has not had nextNormIndex track assigned to it - disabling");
            enabled = false;
        }
    }

    public void Start()
    {
        //find nearest curve
        trackData = track.track;
        float nearestCurveDistance = Mathf.Infinity;
        int numberOfCurves = trackData.numberOfCurves;
        for (int i = 0; i < numberOfCurves; i++)
        {
            Vector3 curveCenter = trackData.Curve(i).center;
            float thisDistance = (curveCenter - transform.position).sqrMagnitude;
            if (thisDistance < nearestCurveDistance)
            {
//                curveIndex = i;
                nearestCurveDistance = thisDistance;
            }
        }

        //        TrackBuildRCurve curve = trackData.Curve(curveIndex);

    }

    public void Update()
    {
        //        Debug.Log("Monitor " + trackData.GetNearestPoint(transform.position));
    }
}
