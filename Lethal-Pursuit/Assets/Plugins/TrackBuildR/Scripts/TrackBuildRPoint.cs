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
/// Track BuildR Point holds the data concerning this point. The track is made up of a Bezier curve that is defined by these points.
/// It also holds a few precalculations to speed up track generation
/// </summary>

public class TrackBuildRPoint : ScriptableObject
{

    public Transform baseTransform;
    [SerializeField]
    private Vector3 _position;

    [SerializeField]
    private float _width = 15;//FIA Starting Grid regulation numberOfPoints
    [SerializeField]
    private float _cant = 0;//track tilt
    [SerializeField]
    private float _crownAngle = 0;//track camber
    public TrackBuildRCurve curveA;
    public TrackBuildRCurve curveB;

    //Bezier Control Points
    [SerializeField]
    private bool _splitControlPoints = false;
    [SerializeField]
    private Vector3 _forwardControlPoint;
    [SerializeField]
    private Vector3 _backwardControlPoint;

    //Boundary control points for borders
    [SerializeField]
    private Vector3 _leftTrackBoundary = Vector3.zero;
    [SerializeField]
    private Vector3 _leftForwardControlPoint;
    [SerializeField]
    private Vector3 _leftBackwardControlPoint;
    [SerializeField]
    private bool _leftSplitControlPoints = false;

    [SerializeField]
    private Vector3 _rightTrackBoundary = Vector3.zero;
    [SerializeField]
    private Vector3 _rightForwardControlPoint;
    [SerializeField]
    private Vector3 _rightBackwardControlPoint;
    [SerializeField]
    private bool _rightSplitControlPoints = false;

    //Internal stored calculations
    [SerializeField]
    private Vector3 _trackDirection = Vector3.forward;
    [SerializeField]
    private Vector3 _trackUp = Vector3.up;
    [SerializeField]
    private Vector3 _trackCross = Vector3.right;

    public bool isDirty = true;
    public bool shouldReRender = true;

    //public Vector3 position { get { return _position; } set { _position = value; } }
    public Vector3 position
    {
        get
        {
            return baseTransform.rotation * _position;
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            if (_position != newValue)
                isDirty = true;
            _position = newValue;
        }
    }
    public Vector3 worldPosition
    {
        get
        {
            return baseTransform.rotation * _position + baseTransform.position;
        }
        set
        {
            Vector3 newValue = value - baseTransform.position;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            if (_position != newValue)
            {
                isDirty = true;
                _position = newValue;
            }
        }
    }

    public Vector3 forwardControlPoint
    {
        get
        {
            return baseTransform.rotation * (_forwardControlPoint + _position);
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -_position;
            if (_forwardControlPoint != newValue)
            {
                isDirty = true;
                _forwardControlPoint = newValue;
            }
        }
    }

    public Vector3 backwardControlPoint
    {
        get
        {
            Vector3 controlPoint = (_splitControlPoints) ? _backwardControlPoint : -_forwardControlPoint;
            return baseTransform.rotation * (controlPoint + _position);
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -_position;
            if (_splitControlPoints)
            {
                if (backwardControlPoint != newValue)
                {
                    isDirty = true;
                    _backwardControlPoint = newValue;
                }
            }
            else
            {
                if (_forwardControlPoint != -newValue)
                {
                    isDirty = true;
                    _forwardControlPoint = -newValue;
                }
            }
        }
    }

    public bool splitControlPoints
    {
        get { return _splitControlPoints; }
        set
        {
            if (value != _splitControlPoints)
                _backwardControlPoint = -_forwardControlPoint;
            if (splitControlPoints != value)
            {
                isDirty = true;
                _splitControlPoints = value;
            }
        }
    }

    //LEFT BOUNDARY
    public Vector3 leftTrackBoundary
    {
        get
        {
            return baseTransform.rotation * (_leftTrackBoundary + _position - (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position - (trackCross * _width / 2));
            if (leftTrackBoundary != newValue)
            {
                isDirty = true;
                _leftTrackBoundary = newValue;
            }
        }
    }

    public Vector3 leftForwardControlPoint
    {
        get
        {
            return baseTransform.rotation * (_leftForwardControlPoint + _position + _leftTrackBoundary - (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + _leftTrackBoundary - (trackCross * _width / 2));
            if (_leftForwardControlPoint != newValue)
            {
                isDirty = true;
                _leftForwardControlPoint = newValue;
            }
        }
    }

    public Vector3 leftBackwardControlPoint
    {
        get
        {
            Vector3 controlPoint = (_leftSplitControlPoints) ? _leftBackwardControlPoint : -_leftForwardControlPoint;
            return baseTransform.rotation * (controlPoint + _position + _leftTrackBoundary - (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + _leftTrackBoundary - (trackCross * _width / 2));
            if (_leftSplitControlPoints)
            {
                if (_leftBackwardControlPoint != newValue)
                {
                    isDirty = true;
                    _leftBackwardControlPoint = newValue;
                }
            }
            else
            {
                if (_leftForwardControlPoint != -newValue)
                {
                    isDirty = true;
                    _leftForwardControlPoint = -newValue;
                }
            }
        }
    }

    public bool leftSplitControlPoints
    {
        get { return _leftSplitControlPoints; }
        set
        {
            if (value != _leftSplitControlPoints)
                _leftBackwardControlPoint = -_leftForwardControlPoint;
            if(_leftSplitControlPoints!=value)
            {
                isDirty = true; 
                _leftSplitControlPoints = value;
            }
        }
    }


    //RIGHT BOUNDARY
    public Vector3 rightTrackBoundary
    {
        get
        {
            return baseTransform.rotation * (_rightTrackBoundary + _position + (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + (trackCross * _width / 2));
            if(_rightTrackBoundary!=newValue)
            {
                isDirty = true; 
                _rightTrackBoundary = newValue;
            }
        }
    }

    public Vector3 rightForwardControlPoint
    {
        get
        {
            return baseTransform.rotation * (_rightForwardControlPoint + _position + _rightTrackBoundary + (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + _rightTrackBoundary + (trackCross * _width / 2));
            if(_rightForwardControlPoint!=newValue)
            {
                isDirty = true; 
                _rightForwardControlPoint = newValue;
            }
        }
    }

    public Vector3 rightBackwardControlPoint
    {
        get
        {
            Vector3 controlPoint = (_rightSplitControlPoints) ? _rightBackwardControlPoint : -_rightForwardControlPoint;
            return baseTransform.rotation * (controlPoint + _position + _rightTrackBoundary + (trackCross * _width / 2));
        }
        set
        {
            Vector3 newValue = value;
            newValue = Quaternion.Inverse(baseTransform.rotation) * newValue;
            newValue += -(_position + _rightTrackBoundary + (trackCross * _width / 2));
            if (_rightSplitControlPoints)
            {
                if(_rightBackwardControlPoint!=newValue)
                {
                    isDirty = true; 
                    _rightBackwardControlPoint = newValue;
                }
            }
            else
            {  if(_rightForwardControlPoint!=-newValue)
            {
                isDirty = true; 
                _rightForwardControlPoint = -newValue;
            }}
        }
    }

    public bool rightSplitControlPoints
    {
        get { return _rightSplitControlPoints; }
        set
        {
            if (value != _rightSplitControlPoints)
                _rightBackwardControlPoint = -_rightForwardControlPoint;
            if(_rightSplitControlPoints!=value)
            {
                isDirty = true; 
                _rightSplitControlPoints = value;
            }
        }
    }

    public Vector3 trackDirection
    {
        get
        {
            return _trackDirection;
        }
        set
        {
            if (value == Vector3.zero)
                return;
            _trackDirection = value.normalized;
            float cantRad = _cant * Mathf.Deg2Rad;
            _trackUp = Quaternion.LookRotation(_trackDirection) * new Vector3(Mathf.Sin(cantRad), Mathf.Cos(cantRad), 0).normalized;
            _trackCross = Vector3.Cross(trackUp, _trackDirection).normalized;
        }
    }

    public float width
    {
        get
        {
            return _width;
        }
        set
        {
            if (_width != value)
                isDirty = true;
            _width = value;
        }
    }

    public float cant
    {
        get { return _cant; }
        set
        {
            if (_cant != value)
                isDirty = true;
            _cant = value;
        }
    }

    public float crownAngle
    {
        get { return _crownAngle; }
        set
        {
            if (_crownAngle != value)
            {
                isDirty = true;
                _crownAngle = value;
            }
        }
    }

    public Vector3 trackUp {get {return _trackUp;}}

    public Vector3 trackCross {get {return _trackCross;}}

    public void MatchBoundaryValues()
    {
        leftForwardControlPoint = forwardControlPoint+(leftTrackBoundary-worldPosition);
        rightForwardControlPoint = forwardControlPoint+(rightTrackBoundary-worldPosition);
    }
}
