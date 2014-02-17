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
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TrackBuildR))]
public class TrackBuildREditor : Editor
{
    private const float LINE_RESOLUTION = 0.005f;
    private const float HANDLE_SCALE = 0.1f;

    private TrackBuildR _trackBuildR;
    private TrackBuildRTrack _track;
    private float _handleSize;
    [SerializeField]
    private int selectedPoint = 0;//selected track point

    [SerializeField]
    private int selectedCurveIndex = 0;//selected curve

    public static Texture2D[] _stageToolbarTextures;
    public static string[] trackModeString = new[] { "track", "boundary", "bumpers", "textures"/*, "pit lane"*/, "diagram", "options" };
    public static string[] pointModeString = new[] { "transform", "control points", "track point" };
    public static string[] boundaryModeString = new[] { "boundary transform", "boundary control points" };
    public static string[] pitModeString = new[] { "edit pit lane", "set start point", "set end point" };
    public const int numberOfMenuOptions = 6;

    void OnEnable()
    {
        if (target != null)
        {
            _trackBuildR = (TrackBuildR)target;
            _track = _trackBuildR.track;
        }

        _stageToolbarTextures = new Texture2D[numberOfMenuOptions];
        _stageToolbarTextures[0] = (Texture2D)Resources.Load("GUI/track");
        _stageToolbarTextures[1] = (Texture2D)Resources.Load("GUI/boundary");
        _stageToolbarTextures[2] = (Texture2D)Resources.Load("GUI/bumpers");
        _stageToolbarTextures[3] = (Texture2D)Resources.Load("GUI/textures");
        //_stageToolbarTextures[4] = (Texture2D)Resources.Load("GUI/pitin");
        _stageToolbarTextures[4] = (Texture2D)Resources.Load("GUI/diagram");
        _stageToolbarTextures[5] = (Texture2D)Resources.Load("GUI/options");

        //Preview Camera
        if (_trackBuildR.trackEditorPreview != null)
            DestroyImmediate(_trackBuildR.trackEditorPreview);
        if (!EditorApplication.isPlaying && SystemInfo.supportsRenderTextures)
        {
            _trackBuildR.trackEditorPreview = new GameObject("Track Preview Cam");
            _trackBuildR.trackEditorPreview.AddComponent<Camera>();
            _trackBuildR.trackEditorPreview.camera.fov = 80;
            //Retreive camera settings from the main camera
            Camera[] cams = Camera.allCameras;
            bool sceneHasCamera = cams.Length > 0;
            Camera sceneCamera = null;
            Skybox sceneCameraSkybox = null;
            if (Camera.mainCamera)
            {
                sceneCamera = Camera.mainCamera;
            }
            else if (sceneHasCamera)
            {
                sceneCamera = cams[0];
            }

            if (sceneCamera != null)
                if (sceneCameraSkybox == null)
                    sceneCameraSkybox = sceneCamera.GetComponent<Skybox>();
            if (sceneCamera != null)
            {
                _trackBuildR.trackEditorPreview.camera.backgroundColor = sceneCamera.backgroundColor;
                if (sceneCameraSkybox != null)
                    _trackBuildR.trackEditorPreview.AddComponent<Skybox>().material = sceneCameraSkybox.material;
                else
                    if (RenderSettings.skybox != null)
                        _trackBuildR.trackEditorPreview.AddComponent<Skybox>().material = RenderSettings.skybox;
            }
        }
    }

    void OnDisable()
    {
        CleanUp();
    }

    void OnDestroy()
    {
        CleanUp();
    }

    private void CleanUp()
    {
        TrackBuildREditorInspector.CleanUp();
        DestroyImmediate(_trackBuildR.trackEditorPreview);
    }

    /// <summary>
    /// This function renders and controls the layout track function in the menu
    /// Users can click place track points into the scene for a quick and easy creation of a track
    /// </summary>
    private void DrawTrack()
    {
        SceneView.focusedWindow.wantsMouseMove = true;
        _handleSize = HandleUtility.GetHandleSize(_trackBuildR.transform.position) * 0.1f;
        Plane buildingPlane = new Plane(Vector3.up, _trackBuildR.transform.forward);
        float distance;
        Vector3 mousePlanePoint = Vector3.zero;
        Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
        if (buildingPlane.Raycast(mouseRay, out distance))
            mousePlanePoint = mouseRay.GetPoint(distance);

        int numberOfPoints = _track.realNumberOfPoints;
        for (int i = 0; i < numberOfPoints; i++)
        {
            TrackBuildRPoint thisPoint = _track[i];
            Vector3 thisPosition = thisPoint.worldPosition;
            Vector3 lastPosition = (i > 0) ? _track[i - 1].worldPosition : thisPosition;
            Vector3 nextPosition = (i < numberOfPoints - 1) ? _track[i + 1].worldPosition : thisPosition;

            Vector3 backwardTrack = thisPosition - lastPosition;
            Vector3 forwardTrack = nextPosition - thisPosition;
            Vector3 controlDirection = (backwardTrack + forwardTrack).normalized;
            float controlMagnitude = (backwardTrack.magnitude + forwardTrack.magnitude) * 0.333f;

            thisPoint.forwardControlPoint = (controlDirection * controlMagnitude) + thisPosition;
        }

        //draw track outline
        int numberOfCurves = _track.numberOfCurves;
        Vector3 position = _trackBuildR.transform.position;
        for (int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRCurve curve = _track.Curve(i);
            int curvePoints = curve.storedPointSize;
            bool lastPoint = i == numberOfCurves - 1;

            Handles.color = (lastPoint && _track.loop) ? TrackBuildRColours.RED : TrackBuildRColours.GREEN;

            for (int p = 1; p < curvePoints; p++)
            {
                int indexA = p - 1;
                int indexB = p;

                Handles.DrawLine(curve.sampledPoints[indexA] + position, curve.sampledPoints[indexB] + position);
                Vector3 trackCrossWidth = curve.sampledTrackCrosses[indexA] * (curve.sampledWidths[indexA] * 0.5f);
                Handles.DrawLine(curve.sampledPoints[indexA] + trackCrossWidth + position, curve.sampledPoints[indexB] + trackCrossWidth + position);
                Handles.DrawLine(curve.sampledPoints[indexA] - trackCrossWidth + position, curve.sampledPoints[indexB] - trackCrossWidth + position);
            }
        }

        Handles.color = Color.green;
        if (Handles.Button(mousePlanePoint, Quaternion.identity, _handleSize, _handleSize, Handles.DotCap))
        {
            TrackBuildRPoint newTrackPoint = CreateInstance<TrackBuildRPoint>();
            newTrackPoint.baseTransform = _trackBuildR.transform;
            newTrackPoint.position = mousePlanePoint;
            _track.AddPoint(newTrackPoint);
        }

        if (_track.realNumberOfPoints > 0)
        {
            TrackBuildRPoint pointOne = _track[0];
            Handles.Label(pointOne.worldPosition, "Loop Track");
            if (Handles.Button(pointOne.worldPosition, Quaternion.identity, _handleSize, _handleSize, Handles.DotCap))
            {
                _track.drawMode = false;
                _trackBuildR.UpdateRender();
            }
        }

        if (GUI.changed)
        {
            UpdateGui();
        }
    }

    void OnSceneGUI()
    {
        if (_track.drawMode)
        {
            DrawTrack();
            return;
        }

        if (SceneView.focusedWindow != null)
            SceneView.focusedWindow.wantsMouseMove = false;
        Vector3 position = _trackBuildR.transform.position;
        Camera sceneCamera = Camera.current;
        _handleSize = HandleUtility.GetHandleSize(_trackBuildR.transform.position) * 0.1f;

        int realNumberOfPoints = _track.realNumberOfPoints;

        Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
        Quaternion mouseLookDirection = Quaternion.LookRotation(-mouseRay.direction);
        int numberOfCurves = _track.numberOfCurves;

        switch (_trackBuildR.mode)
        {
            case TrackBuildR.modes.track:
                Handles.color = TrackBuildRColours.RED;

                switch (_trackBuildR.pointMode)
                {
                    case TrackBuildR.pointModes.add:

                        if (SceneView.focusedWindow != null)
                            SceneView.focusedWindow.wantsMouseMove = true;

                        if (Event.current.type == EventType.MouseMove)
                            Repaint();

                        float mousePercentage = NearestmMousePercentage();// _track.GetNearestPoint(mousePlanePoint);
                        Vector3 mouseTrackPoint = _track.GetTrackPosition(mousePercentage) + position;
                        Handles.Label(mouseTrackPoint, "Add New Track Point");
                        float newPointHandleSize = HandleUtility.GetHandleSize(mouseTrackPoint) * HANDLE_SCALE;
                        if (Handles.Button(mouseTrackPoint, mouseLookDirection, newPointHandleSize, newPointHandleSize, Handles.DotCap))
                        {
                            Undo.RegisterSceneUndo("Create New Track Point");
                            int newPointIndex = _track.GetLastPointIndex(mousePercentage);
                            TrackBuildRPoint newPoint = _track.InsertPoint(newPointIndex + 1);
                            newPoint.worldPosition = mouseTrackPoint;
                            newPoint.width = _track.GetTrackWidth(mousePercentage);
                            newPoint.cant = _track.GetTrackCant(mousePercentage);
                            newPoint.crownAngle = _track.GetTrackCrownAngle(mousePercentage);
                            selectedPoint = newPointIndex + 1;
                            GUI.changed = true;
                            _trackBuildR.pointMode = TrackBuildR.pointModes.transform;
                        }
                        break;

                    case TrackBuildR.pointModes.remove:

                        if (SceneView.focusedWindow != null)
                            SceneView.focusedWindow.wantsMouseMove = true;

                        Handles.color = TrackBuildRColours.RED;
                        for (int i = 0; i < realNumberOfPoints; i++)
                        {
                            TrackBuildRPoint point = _track[i];

                            float pointHandleSize = HandleUtility.GetHandleSize(point.worldPosition) * HANDLE_SCALE;
                            Handles.Label(point.worldPosition, "Remove Track Point");
                            if (Handles.Button(point.worldPosition, mouseLookDirection, pointHandleSize, pointHandleSize, Handles.DotCap))
                            {
                                Undo.RegisterSceneUndo("Remove Track Point");
                                _track.RemovePoint(point);
                                GUI.changed = true;
                                _trackBuildR.pointMode = TrackBuildR.pointModes.transform;
                            }
                        }

                        break;

                    default:

                        SceneGUIPointBased();
                        break;

                }

                //draw track outline
                for (int i = 0; i < numberOfCurves; i++)
                {
                    TrackBuildRCurve curve = _track.Curve(i);
                    int curvePoints = curve.storedPointSize;

                    float dotPA = Vector3.Dot(sceneCamera.transform.forward, curve.pointA.worldPosition - sceneCamera.transform.position);
                    float dotPB = Vector3.Dot(sceneCamera.transform.forward, curve.pointB.worldPosition - sceneCamera.transform.position);

                    if (dotPA < 0 && dotPB < 0)
                        continue;

                    float curveDistance = Vector3.Distance(sceneCamera.transform.position, curve.center);
                    int pointJump = Mathf.Max((int)(curveDistance / 20.0f), 1);

                    for (int p = pointJump; p < curvePoints; p += pointJump)
                    {
                        int indexA = p - pointJump;
                        int indexB = p;

                        if (p + pointJump > curvePoints - 1)
                            indexB = curvePoints - 1;

                        Handles.color = new Color(0, 1, 0, 0.5f);
                        Handles.DrawLine(curve.sampledPoints[indexA] + position, curve.sampledPoints[indexB] + position);
                        Handles.color = Color.green;
                        Vector3 trackCrossWidth = curve.sampledTrackCrosses[indexA] * (curve.sampledWidths[indexA] * 0.5f);
                        Handles.DrawLine(curve.sampledPoints[indexA] + trackCrossWidth + position, curve.sampledPoints[indexB] + trackCrossWidth + position);
                        Handles.DrawLine(curve.sampledPoints[indexA] - trackCrossWidth + position, curve.sampledPoints[indexB] - trackCrossWidth + position);
                    }
                }
                break;

            case TrackBuildR.modes.boundary:

                //draw boundary outline
                for (int i = 0; i < numberOfCurves; i++)
                {
                    TrackBuildRCurve curve = _track.Curve(i);
                    int curvePoints = curve.storedPointSize;

                    float dotPA = Vector3.Dot(sceneCamera.transform.forward, curve.pointA.worldPosition - sceneCamera.transform.position);
                    float dotPB = Vector3.Dot(sceneCamera.transform.forward, curve.pointB.worldPosition - sceneCamera.transform.position);

                    if (dotPA < 0 && dotPB < 0)
                        continue;

                    float curveDistance = Vector3.Distance(sceneCamera.transform.position, curve.center);
                    int pointJump = Mathf.Max((int)(curveDistance / 20.0f), 1);

                    for (int p = pointJump; p < curvePoints; p += pointJump)
                    {

                        int indexA = p - pointJump;
                        int indexB = p;

                        if (p + pointJump > curvePoints - 1)
                            indexB = curvePoints - 1;

                        if (_track.disconnectBoundary)
                        {
                            Handles.color = TrackBuildRColours.BLUE;
                            Handles.DrawLine(curve.sampledLeftBoundaryPoints[indexA] + position, curve.sampledLeftBoundaryPoints[indexB] + position);
                            Handles.color = TrackBuildRColours.RED;
                            Handles.DrawLine(curve.sampledRightBoundaryPoints[indexA] + position, curve.sampledRightBoundaryPoints[indexB] + position);
                        }
                        else
                        {

                            Vector3 trackCrossWidth = curve.sampledTrackCrosses[indexA] * (curve.sampledWidths[indexA] * 0.5f);
                            Handles.color = TrackBuildRColours.BLUE;
                            Handles.DrawLine(curve.sampledPoints[indexA] + trackCrossWidth + position, curve.sampledPoints[indexB] + trackCrossWidth + position);
                            Handles.color = TrackBuildRColours.RED;
                            Handles.DrawLine(curve.sampledPoints[indexA] - trackCrossWidth + position, curve.sampledPoints[indexB] - trackCrossWidth + position);
                        }
                    }

                    SceneGUIPointBased();
                }
                break;

            /*case TrackBuildR.modes.pit:

            if (!_track.hasPitLane)
                break;

            //is legal?

            if (SceneView.focusedWindow != null)
                SceneView.focusedWindow.wantsMouseMove = true;

            //draw pit outline
            for (int i = 0; i < _track.pitlane.numberOfCurves; i++)
            {
                TrackBuildRCurve curve = _track.pitlane.Curve(i);
                int curvePoints = curve.storedPointSize;
                for (int p = 1; p < curvePoints; p++)
                {
                    int indexA = p - 1;
                    int indexB = p;

                    Handles.color = new Color(0, 1, 0, 0.5f);
                    Handles.DrawLine(curve.sampledPoints[indexA] + position, curve.sampledPoints[indexB] + position);
                    Handles.color = Color.green;
                    Vector3 trackCrossWidth = curve.sampledTrackCrosses[indexA] * (curve.sampledWidths[indexA] * 0.5f);
                    Handles.DrawLine(curve.sampledPoints[indexA] + trackCrossWidth + position, curve.sampledPoints[indexB] + trackCrossWidth + position);
                    Handles.DrawLine(curve.sampledPoints[indexA] - trackCrossWidth + position, curve.sampledPoints[indexB] - trackCrossWidth + position);
                }
            }

            switch (pitMode)
            {
                    case TrackBuildR.pitModes.editPitLane:

                    break;

                    case TrackBuildR.pitModes.setStartPoint:

                    break;

                    case TrackBuildR.pitModes.setEndPoint:

                    break;
            }

            break;*/

            case TrackBuildR.modes.textures:

                for (int i = 0; i < numberOfCurves; i++)
                {
                    TrackBuildRCurve thisCurve = _track.Curve(i);

                    float pointHandleSize = HandleUtility.GetHandleSize(thisCurve.center) * HANDLE_SCALE;
                    Handles.color = (i == selectedCurveIndex) ? TrackBuildRColours.RED : TrackBuildRColours.BLUE;
                    if (Handles.Button(thisCurve.center, Quaternion.identity, pointHandleSize, pointHandleSize, Handles.DotCap))
                    {
                        selectedCurveIndex = i;
                        GUIUtility.hotControl = 0;
                        GUIUtility.keyboardControl = 0;
                        GUI.changed = true;
                    }
                }

                Handles.color = TrackBuildRColours.RED;
                TrackBuildRCurve selectedCurve = _track.Curve(selectedCurveIndex);
                int numberOfSelectedCurvePoints = selectedCurve.storedPointSize;
                for (int i = 0; i < numberOfSelectedCurvePoints - 1; i++)
                {
                    Vector3 leftPointA = selectedCurve.sampledLeftBoundaryPoints[i];
                    Vector3 leftPointB = selectedCurve.sampledLeftBoundaryPoints[i + 1];
                    Vector3 rightPointA = selectedCurve.sampledRightBoundaryPoints[i];
                    Vector3 rightPointB = selectedCurve.sampledRightBoundaryPoints[i + 1];

                    Handles.DrawLine(leftPointA, leftPointB);
                    Handles.DrawLine(rightPointA, rightPointB);

                    if (i == 0)
                        Handles.DrawLine(leftPointA, rightPointA);
                    if (i == numberOfSelectedCurvePoints - 2)
                        Handles.DrawLine(leftPointB, rightPointB);
                }

                break;

            case TrackBuildR.modes.diagram:
                if (SceneView.focusedWindow != null)
                    SceneView.focusedWindow.wantsMouseMove = true;
                if (!_track.showDiagram)
                    break;
                Plane diagramPlane = new Plane(Vector3.up, position);
                float diagramDistance;
                float crossSize = _handleSize * 10;

                switch (_trackBuildR.track.assignedPoints)
                {
                    case 0://display the diagram scale points

                        Vector3 diagramPointA = _track.scalePointA;
                        Vector3 diagramPointB = _track.scalePointB;

                        if (diagramPointA != Vector3.zero || diagramPointB != Vector3.zero)
                        {

                            Handles.color = TrackBuildRColours.BLUE;
                            Handles.DrawLine(diagramPointA, diagramPointA + Vector3.left * crossSize);
                            Handles.DrawLine(diagramPointA, diagramPointA + Vector3.right * crossSize);
                            Handles.DrawLine(diagramPointA, diagramPointA + Vector3.forward * crossSize);
                            Handles.DrawLine(diagramPointA, diagramPointA + Vector3.back * crossSize);

                            Handles.color = TrackBuildRColours.GREEN;
                            Handles.DrawLine(diagramPointB, diagramPointB + Vector3.left * crossSize);
                            Handles.DrawLine(diagramPointB, diagramPointB + Vector3.right * crossSize);
                            Handles.DrawLine(diagramPointB, diagramPointB + Vector3.forward * crossSize);
                            Handles.DrawLine(diagramPointB, diagramPointB + Vector3.back * crossSize);

                            Handles.color = TrackBuildRColours.RED;
                            Handles.DrawLine(diagramPointA, diagramPointB);
                        }
                        break;

                    case 1://place the first of two scale points to define the diagram scale size

                        Ray diagramRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
                        if (diagramPlane.Raycast(diagramRay, out diagramDistance))
                        {
                            Vector3 diagramPlanePoint = diagramRay.GetPoint(diagramDistance);

                            Handles.color = TrackBuildRColours.BLUE;
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.left * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.right * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.forward * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.back * crossSize);

                            Handles.color = new Color(0, 0, 0, 0);
                            if (Handles.Button(diagramPlanePoint, Quaternion.identity, crossSize, crossSize, Handles.DotCap))
                            {
                                _track.scalePointA = diagramPlanePoint;
                                _track.assignedPoints = 2;
                            }
                        }

                        break;

                    case 2://place the second of two scale points to define the diagram scale

                        Vector3 diagramPoint1 = _track.scalePointA;
                        Handles.color = TrackBuildRColours.BLUE;
                        Handles.DrawLine(diagramPoint1, diagramPoint1 + Vector3.left * crossSize);
                        Handles.DrawLine(diagramPoint1, diagramPoint1 + Vector3.right * crossSize);
                        Handles.DrawLine(diagramPoint1, diagramPoint1 + Vector3.forward * crossSize);
                        Handles.DrawLine(diagramPoint1, diagramPoint1 + Vector3.back * crossSize);

                        Ray diagramRayB = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
                        if (diagramPlane.Raycast(diagramRayB, out diagramDistance))
                        {
                            Vector3 diagramPlanePoint = diagramRayB.GetPoint(diagramDistance);

                            Handles.color = TrackBuildRColours.RED;
                            Handles.DrawLine(diagramPlanePoint, diagramPoint1);

                            Handles.color = TrackBuildRColours.GREEN;
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.left * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.right * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.forward * crossSize);
                            Handles.DrawLine(diagramPlanePoint, diagramPlanePoint + Vector3.back * crossSize);

                            Handles.color = new Color(0, 0, 0, 0);
                            if (Handles.Button(diagramPlanePoint, Quaternion.identity, crossSize, crossSize, Handles.DotCap))
                            {
                                _track.scalePointB = diagramPlanePoint;
                                _track.assignedPoints = 0;
                                //wUpdateDiagram();
                            }
                        }
                        break;
                }
                break;

        }

        if (Input.GetMouseButtonDown(0))
        {
            // Register the undos when we press the Mouse button.
            Undo.CreateSnapshot();
            Undo.RegisterSnapshot();
        }

        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    GUI.changed = true;
                    break;
            }
        }

        if (GUI.changed)
        {
            UpdateGui();
        }
    }

    private void SceneGUIPointBased()
    {
        Vector3 position = _trackBuildR.transform.position;
        Camera sceneCamera = Camera.current;
        _handleSize = HandleUtility.GetHandleSize(_trackBuildR.transform.position) * 0.1f;
        int realNumberOfPoints = _track.realNumberOfPoints;
        Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - 30, 0));
        for (int i = 0; i < realNumberOfPoints; i++)
        {
            TrackBuildRPoint point = _track[i];


            if (Vector3.Dot(sceneCamera.transform.forward, point.worldPosition - sceneCamera.transform.position) < 0)
                continue;

            Handles.Label(point.worldPosition, "point " + (i + 1));
            float pointHandleSize = HandleUtility.GetHandleSize(point.worldPosition) * HANDLE_SCALE;
            //            bool selectedPointChanged = false;
            Handles.color = (i == selectedPoint) ? TrackBuildRColours.RED : TrackBuildRColours.GREEN;
            if (Handles.Button(point.worldPosition, Quaternion.identity, pointHandleSize, pointHandleSize, Handles.DotCap))
            {
                selectedPoint = i;
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
                GUI.changed = true;
                point.isDirty = true;
            }

            if (i == selectedPoint)
            {
                switch (_trackBuildR.mode)
                {
                    case TrackBuildR.modes.track:

                        switch (_trackBuildR.pointMode)
                        {
                            case TrackBuildR.pointModes.transform:
                                Undo.SetSnapshotTarget(point, "Track Point Moved");
                                Vector3 currentPosition = point.worldPosition;
                                currentPosition = Handles.DoPositionHandle(currentPosition, Quaternion.identity);
                                if (currentPosition != point.worldPosition)
                                {
                                    point.isDirty = true;
                                    point.worldPosition = currentPosition;
                                }
                                break;

                            case TrackBuildR.pointModes.controlpoint:
                                Undo.SetSnapshotTarget(point, "Track Control Point Moved");
                                if (Vector3.Dot(mouseRay.direction, point.worldPosition - mouseRay.origin) > 0)
                                    Handles.Label(point.forwardControlPoint, "point " + (i + 1) + " control point");
                                Handles.color = TrackBuildRColours.RED;
                                Handles.DrawLine(point.worldPosition, point.forwardControlPoint + position);
                                point.forwardControlPoint = Handles.DoPositionHandle(point.forwardControlPoint + position, Quaternion.identity) - position;

                                Handles.DrawLine(point.worldPosition, point.backwardControlPoint + position);
                                point.backwardControlPoint = Handles.DoPositionHandle(point.backwardControlPoint + position, Quaternion.identity) - position;
                                if (Vector3.Dot(mouseRay.direction, point.worldPosition - mouseRay.origin) > 0)
                                    Handles.Label(point.backwardControlPoint, "point " + (i + 1) + " reverse control point");
                                break;

                            case TrackBuildR.pointModes.trackpoint:

                                //Track Width
                                Handles.color = TrackBuildRColours.RED;
                                float pointWidth = point.width / 2;
                                Vector3 sliderPos = Handles.Slider(point.worldPosition + point.trackCross * pointWidth, point.trackCross);
                                float pointwidth = Vector3.Distance(sliderPos, point.worldPosition) * 2;
                                if (pointwidth != point.width)
                                {
                                    point.isDirty = true;
                                    Undo.RegisterUndo(point, "Modified Point Width");
                                    point.width = pointwidth;
                                }

                                //Cant
                                Handles.color = TrackBuildRColours.BLUE;
                                Quaternion rot = Quaternion.Inverse(Quaternion.Euler(new Vector3(point.cant, 0, 0)));
                                rot = Quaternion.Inverse(Handles.Disc(rot, point.worldPosition, point.trackDirection, point.width, false, 1));
                                point.cant = rot.eulerAngles.x;
                                if (point.cant > 270)
                                    point.cant += -360;
                                point.cant = Mathf.Clamp(point.cant, -90, 90);

                                //Crown
                                Handles.color = TrackBuildRColours.GREEN;
                                Vector3 crownPosition = point.worldPosition + point.trackUp * point.crownAngle;
                                Vector3 newCrownPosition = Handles.Slider(crownPosition, point.trackUp);
                                Vector3 crownDifference = newCrownPosition - point.worldPosition;
                                if (crownDifference.sqrMagnitude != 0)//Crown Modified
                                {
                                    point.isDirty = true;
                                    Undo.RegisterUndo(point, "Modified Point Crown");
                                    point.crownAngle = Vector3.Project(crownDifference, point.trackUp).magnitude * Mathf.Sign(Vector3.Dot(crownDifference, point.trackUp));
                                }

                                break;
                        }
                        break;


                    case TrackBuildR.modes.boundary:

                        if (_track.disconnectBoundary)
                        {
                            if (Vector3.Dot(mouseRay.direction, point.worldPosition - mouseRay.origin) > 0)
                            {
                                Handles.color = TrackBuildRColours.RED;
                                Handles.Label(point.leftTrackBoundary, "point " + (i + 1) + " left track boundary");
                                Handles.Label(point.rightTrackBoundary, "point " + (i + 1) + " right track boundary");
                                Handles.DrawLine(point.worldPosition, point.leftTrackBoundary);
                                Handles.DrawLine(point.worldPosition, point.rightTrackBoundary);
                            }
                            switch (_trackBuildR.boundaryMode)
                            {
                                case TrackBuildR.boundaryModes.transform:
                                    point.leftTrackBoundary = Handles.DoPositionHandle(point.leftTrackBoundary, Quaternion.identity);

                                    point.rightTrackBoundary = Handles.DoPositionHandle(point.rightTrackBoundary, Quaternion.identity);
                                    break;

                                case TrackBuildR.boundaryModes.controlpoint:
                                    Handles.color = TrackBuildRColours.RED;
                                    Handles.DrawLine(point.leftTrackBoundary, point.leftForwardControlPoint);
                                    point.leftForwardControlPoint = Handles.DoPositionHandle(point.leftForwardControlPoint, Quaternion.identity);

                                    Handles.DrawLine(point.leftTrackBoundary, point.leftBackwardControlPoint);
                                    point.leftBackwardControlPoint = Handles.DoPositionHandle(point.leftBackwardControlPoint, Quaternion.identity);

                                    Handles.DrawLine(point.rightTrackBoundary, point.rightForwardControlPoint);
                                    point.rightForwardControlPoint = Handles.DoPositionHandle(point.rightForwardControlPoint, Quaternion.identity);

                                    Handles.DrawLine(point.rightTrackBoundary, point.rightBackwardControlPoint);
                                    point.rightBackwardControlPoint = Handles.DoPositionHandle(point.rightBackwardControlPoint, Quaternion.identity);
                                    break;

                            }
                        }
                        break;

                    /*case TrackBuildR.modes.pit:

                    switch(pitMode)
                    {
                        case TrackBuildR.pitModes.setStartPoint:

                            if (selectedPointChanged)
                            {
                                _track.pitlane.pitStartPointIndex = selectedPoint;
                            }

                            break;

                        case TrackBuildR.pitModes.setEndPoint:

                            if (selectedPointChanged)
                            {
                                _track.pitlane.pitEndPointIndex = selectedPoint;
                            }

                            break;
                    }

                    break;*/
                }
            }
        }
    }

    /// <summary>
    /// Get the 2D screen position of the track curve from a specified percentage
    /// </summary>
    /// <param name="t">The percentage point of the whole track curve</param>
    /// <returns>A 2D Screen Space Coordinate</returns>
    private Vector2 ScreenTrackProjection(float t)
    {
        float curveT = 1.0f / _track.numberOfCurves;
        int point = Mathf.FloorToInt(t / curveT);
        float ct = Mathf.Clamp01((t - point * curveT) * _track.numberOfCurves);

        TrackBuildRPoint pointA = _track.GetPoint(point);
        TrackBuildRPoint pointB = _track.GetPoint(point + 1);

        Camera cam = Camera.current;

        float dotA = Vector3.Dot(cam.transform.forward, pointA.worldPosition - cam.transform.position);
        float dotB = Vector3.Dot(cam.transform.forward, pointB.worldPosition - cam.transform.position);

        if (dotA < 0 || dotB < 0)
            return Vector2.zero;

        Vector2 point2DPosA = cam.WorldToScreenPoint(pointA.worldPosition);
        Vector2 point2DPosFCPA = cam.WorldToScreenPoint(pointA.forwardControlPoint);
        Vector2 point2DPosBCPB = cam.WorldToScreenPoint(pointB.backwardControlPoint);
        Vector2 point2DPosB = cam.WorldToScreenPoint(pointB.worldPosition);

        Vector2 returnProjectionPoint = SplineMaths.CalculateBezierPoint(ct, point2DPosA, point2DPosFCPA, point2DPosBCPB, point2DPosB);
        float screenHeight = Camera.current.pixelHeight;
        returnProjectionPoint.y = screenHeight - returnProjectionPoint.y;
        return returnProjectionPoint;
    }

    /// <summary>
    /// Get the nearest point on the track curve to the  mouse position
    /// We essentailly project the track onto a 2D plane that is the editor camera and then find a point on that
    /// </summary>
    /// <returns>A percentage of the nearest point on the track curve to the nerest metre</returns>
    private float NearestmMousePercentage()
    {
        Vector2 mousePos = Event.current.mousePosition;
        int numberOfSearchPoints = (int)_track.trackLength;

        float nearestPointSqrMag = Vector2.SqrMagnitude(ScreenTrackProjection(0) - mousePos);
        float nearestT = 0;
        for (int i = 1; i < numberOfSearchPoints; i++)
        {
            float t = i / (float)numberOfSearchPoints;
            Vector2 point = ScreenTrackProjection(t);
            if (point == Vector2.zero)
                continue;
            float thisPointMag = Vector2.SqrMagnitude(point - mousePos);
            if (thisPointMag < nearestPointSqrMag)
            {
                nearestT = t;
                nearestPointSqrMag = thisPointMag;
            }
        }

        return nearestT;
    }

    public override void OnInspectorGUI()
    {
        TrackBuildREditorInspector.OnInspectorGUI(_trackBuildR, selectedPoint, selectedCurveIndex);

        if (GUI.changed)
        {
            UpdateGui();
        }
    }

    /// <summary>
    /// Handle GUI changes and repaint requests
    /// </summary>
    private void UpdateGui()
    {
        Repaint();
        HandleUtility.Repaint();
        SceneView.RepaintAll();
        _trackBuildR.UpdateRender();
        EditorUtility.SetDirty(_trackBuildR);
    }
}
