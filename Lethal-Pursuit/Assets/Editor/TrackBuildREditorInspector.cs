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
using System.Collections.Generic;

public class TrackBuildREditorInspector
{
    [SerializeField]
    private static int selectedTexture;

    [SerializeField]
    private static RenderTexture pointPreviewTexture = null;
    private static float aspect = 1.7777f;
    private static Vector3 previewCameraHeight = new Vector3(0, 1.8f, 0);
    private static int previewResolution = 800;

    public static void OnInspectorGUI(TrackBuildR _trackBuildR, int selectedPointIndex, int selectedCurveIndex)
    {
        TrackBuildRTrack _track = _trackBuildR.track;

        GUILayout.BeginVertical(GUILayout.Width(400));

        //Track Preview Window
        EditorGUILayout.Space();
        RenderPreview(_trackBuildR);
        EditorGUILayout.LabelField("Track Lap Length approx. " + (_track.trackLength / 1000).ToString("F2") + "km / " + (_track.trackLength / 1609.34f).ToString("F2") + " miles");

        int currentTrackMode = (int)_trackBuildR.mode;
        GUIContent[] guiContent = new GUIContent[TrackBuildREditor.numberOfMenuOptions];
        if (TrackBuildREditor._stageToolbarTextures == null)
            return;
        for (int i = 0; i < TrackBuildREditor.numberOfMenuOptions; i++)
            guiContent[i] = new GUIContent(TrackBuildREditor._stageToolbarTextures[i], TrackBuildREditor.trackModeString[i]);
        int newTrackMode = GUILayout.Toolbar(currentTrackMode, guiContent, GUILayout.Width(400), GUILayout.Height(50));
        if (newTrackMode != currentTrackMode)
        {
            _trackBuildR.mode = (TrackBuildR.modes)newTrackMode;
            GUI.changed = true;
        }

        if (_track.numberOfTextures == 0)
            EditorGUILayout.HelpBox("There are no textures defined. Track will not render until this is done", MessageType.Error);

        TrackBuildRPoint point = null;
        if (_track.realNumberOfPoints > 0)
        {
            point = _trackBuildR.track[selectedPointIndex];
        }

        switch (_trackBuildR.mode)
        {
            case TrackBuildR.modes.track:

                EditorGUILayout.Space();
                Title("Track", TrackBuildRColours.GREEN);

                bool trackloop = EditorGUILayout.Toggle("Is Looped", _track.loop);
                if (_track.loop != trackloop)
                {
                    Undo.RegisterSceneUndo("Modified Track Loop Setting");
                    _track.loop = trackloop;
                }

                EditorGUILayout.BeginHorizontal();
                if (_trackBuildR.pointMode != TrackBuildR.pointModes.add)
                {
                    if (GUILayout.Button("Add New Point"))
                    _trackBuildR.pointMode = TrackBuildR.pointModes.add;
                }
                else
                {
                    if (GUILayout.Button("Cancel Add New Point"))
                    _trackBuildR.pointMode = TrackBuildR.pointModes.transform;
                }

                EditorGUI.BeginDisabledGroup(_track.realNumberOfPoints < 3);
                if(_trackBuildR.pointMode != TrackBuildR.pointModes.remove)
                {
                    if (GUILayout.Button("Remove Point"))
                        _trackBuildR.pointMode = TrackBuildR.pointModes.remove;
                }else
                {
                    if (GUILayout.Button("Cancel Remove Point"))
                        _trackBuildR.pointMode = TrackBuildR.pointModes.transform;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();

                if (!_track.drawMode)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Layout Track Points"))
                    {
                        if (EditorUtility.DisplayDialog("Discard Current Track?", "Do you wish to discard the current track layout?", "Yup", "Nope"))
                        {
                            _track.Clear();
                            _track.drawMode = true;
                        }
                    }
                    if (GUILayout.Button("?", GUILayout.Width(35)))
                    {
                        EditorUtility.DisplayDialog("Layout Track", "This allows you to click place points to define your track. It will erase the current track layout and start anew. Ideally used with a defined diagram to help you plot out the track", "Ok - got it!");

                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    if (GUILayout.Button("Stop Layout Track"))
                    {
                        _track.drawMode = false;
                    }
                    return;
                }

                float meshResolution = _track.meshResolution;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Track Mesh Resolution", GUILayout.Width(140));
                meshResolution = EditorGUILayout.Slider(meshResolution, 0.1f, 20.0f);
                EditorGUILayout.LabelField("metres", GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();
                if (meshResolution != _track.meshResolution)
                {
                    Undo.RegisterSceneUndo("Modified Mesh Resolution");
                    _track.SetTrackDirty();
                    _track.meshResolution = meshResolution;
                }

                if (_track.realNumberOfPoints == 0)
                {
                    EditorGUILayout.HelpBox("There are no track points defined, add nextNormIndex track point to begin", MessageType.Warning);
                    return;
                }


                EditorGUILayout.Space();
                Title("Track Point", TrackBuildRColours.RED);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Point " + (selectedPointIndex + 1) + " selected");
                if (GUILayout.Button("Goto Point"))
                    GotoScenePoint(point.position);
                EditorGUILayout.EndHorizontal();


                int currentMode = (int)_trackBuildR.pointMode;
                int newStage = GUILayout.Toolbar(currentMode, TrackBuildREditor.pointModeString);
                if (newStage != currentMode)
                {
                    _trackBuildR.pointMode = (TrackBuildR.pointModes)newStage;
                    GUI.changed = true;
                }

                switch (_trackBuildR.pointMode)
                {
                    case TrackBuildR.pointModes.transform:
                        Vector3 pointposition = EditorGUILayout.Vector3Field("Point Position", point.position);
                        if (pointposition != point.position)
                        {
                            Undo.RegisterUndo(point, "Modify Point Position");
                            point.position = pointposition;
                        }
                        break;

                    case TrackBuildR.pointModes.controlpoint:
                        bool pointsplitControlPoints = EditorGUILayout.Toggle("Split Control Points", point.splitControlPoints);
                        if (pointsplitControlPoints != point.splitControlPoints)
                        {
                            Undo.RegisterUndo(point, "Modify Split Control Points");
                            point.splitControlPoints = pointsplitControlPoints;
                        }
                        Vector3 pointforwardControlPoint = EditorGUILayout.Vector3Field("Control Point Position", point.forwardControlPoint);
                        if (pointforwardControlPoint != point.forwardControlPoint)
                        {
                            Undo.RegisterUndo(point, "Modify Point Forward Control");
                            point.forwardControlPoint = pointforwardControlPoint;
                        }
                        break;

                    case TrackBuildR.pointModes.trackpoint:

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Point Cant");
                        float pointcant = EditorGUILayout.Slider(point.cant, -90, 90);
                        if (pointcant != point.cant)
                        {
                            point.isDirty = true;
                            Undo.RegisterUndo(point, "Modify Point Cant");
                            point.cant = pointcant;
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Point Crown");
                        float pointcrownAngle = EditorGUILayout.Slider(point.crownAngle, -45, 45);
                        if (pointcrownAngle != point.crownAngle)
                        {
                            point.isDirty = true;
                            Undo.RegisterUndo(point, "Modify Point Crown");
                            point.crownAngle = pointcrownAngle;
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Point Width", GUILayout.Width(250));
                        float pointwidth = EditorGUILayout.FloatField(point.width);
                        if (pointwidth != point.width)
                        {
                            point.isDirty = true;
                            Undo.RegisterUndo(point, "Modify Point Width");
                            point.width = pointwidth;
                        }
                        EditorGUILayout.LabelField("metres", GUILayout.Width(75));
                        EditorGUILayout.EndHorizontal();
                        break;
                }
                break;

            case TrackBuildR.modes.boundary:

                EditorGUILayout.Space();
                Title("Track Boundary", TrackBuildRColours.GREEN);
                //Track Based Boundary Options
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Split Boundary from Track");
                bool trackdisconnectBoundary = EditorGUILayout.Toggle(_track.disconnectBoundary, GUILayout.Width(25));
                if (trackdisconnectBoundary != _track.disconnectBoundary)
                {
                    Undo.RegisterSceneUndo("Split Boundary from Track");
                    _track.disconnectBoundary = trackdisconnectBoundary;
                    GUI.changed = true;
                    _track.ReRenderTrack();
                }

                if (GUILayout.Button("Reset Boundary Points"))
                {
                    Undo.RegisterSceneUndo("Reset Boundary Points");
                    for (int i = 0; i < _track.numberOfPoints; i++)
                    {
                        _track[i].MatchBoundaryValues();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Render Both Sides of Boundary");
                bool renderBothSides = EditorGUILayout.Toggle(_track.renderBoundaryWallReverse, GUILayout.Width(50));
                if (_track.renderBoundaryWallReverse != renderBothSides)
                {
                    Undo.RegisterSceneUndo("Modified Render Both Sides of Boundary");
                    _track.renderBoundaryWallReverse = renderBothSides;
                    GUI.changed = true;
                    _track.ReRenderTrack();
                }
                EditorGUILayout.EndHorizontal();

                float newTrackColliderHeight = EditorGUILayout.FloatField("Track Collider Height", _track.trackColliderWallHeight);
                if (newTrackColliderHeight != _track.trackColliderWallHeight)
                {
                    Undo.RegisterSceneUndo("Modified Track Collider Height");
                    _track.trackColliderWallHeight = newTrackColliderHeight;
                    _track.ReRenderTrack();
                    GUI.changed = true;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Track Collider Should Have Roof");
                bool newRoofCooliderValue = EditorGUILayout.Toggle(_track.includeColliderRoof, GUILayout.Width(50));
                if (newRoofCooliderValue != _track.includeColliderRoof)
                {
                    Undo.RegisterSceneUndo("Modified Track Collider Roof");
                    _track.includeColliderRoof = newRoofCooliderValue;
                    _track.ReRenderTrack();
                    GUI.changed = true;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                Title("Point Boundary", TrackBuildRColours.RED);
                //Selected Point Boundary Options
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Point " + (selectedPointIndex + 1) + " selected");
                if (GUILayout.Button("Goto Point"))
                    GotoScenePoint(point.position);
                EditorGUILayout.EndHorizontal();

                int currentBoundaryMode = (int)_trackBuildR.boundaryMode;
                int newBoundaryMode = GUILayout.Toolbar(currentBoundaryMode, TrackBuildREditor.boundaryModeString);
                if (newBoundaryMode != currentBoundaryMode)
                {
                    _trackBuildR.boundaryMode = (TrackBuildR.boundaryModes)newBoundaryMode;
                    GUI.changed = true;
                }

                if (_track.realNumberOfPoints > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Split Boundary Control Points");
                    bool pointSplitControlPoints = EditorGUILayout.Toggle(point.leftSplitControlPoints, GUILayout.Width(50));
                    if (point.leftSplitControlPoints != pointSplitControlPoints)
                    {
                        Undo.RegisterUndo(point, "Split Boundary Control Point");
                        point.leftSplitControlPoints = pointSplitControlPoints;
                        point.rightSplitControlPoints = pointSplitControlPoints;
                        GUI.changed = true;
                        _track.SetTrackDirty();
                    }
                    EditorGUILayout.EndHorizontal();

                }

                EditorGUILayout.HelpBox("It is suggested that you disable the collider bounding box when working on Track BuildR\nYou can do this by clicking on 'gizmos' above the scene view and deselecting 'Mesh Collider'", MessageType.Info);
                break;

            case TrackBuildR.modes.bumpers:

                EditorGUILayout.Space();
                Title("Track Bumpers", TrackBuildRColours.RED);
                bool _tracktrackBumpers = EditorGUILayout.Toggle("Enable", _track.trackBumpers);
                if (_track.trackBumpers != _tracktrackBumpers)
                {
                    Undo.RegisterSceneUndo("Toggled Enable Bumpers");
                    _track.trackBumpers = _tracktrackBumpers;
                }
                EditorGUI.BeginDisabledGroup(!_track.trackBumpers);
                float bumperWidth = EditorGUILayout.Slider("Width", _track.bumperWidth, 0.1f, 2.0f);
                if (bumperWidth != _track.bumperWidth)
                {
                    Undo.RegisterSceneUndo("Modifed Bumper Width");
                    GUI.changed = true;
                    _track.bumperWidth = bumperWidth;
                }
                float bumperHeight = EditorGUILayout.Slider("Height", _track.bumperHeight, 0.01f, 0.2f);
                if (bumperHeight != _track.bumperHeight)
                {
                    Undo.RegisterSceneUndo("Modifed Bumper Height");
                    GUI.changed = true;
                    _track.bumperHeight = bumperHeight;
                }
                float bumperAngleThresold = EditorGUILayout.Slider("Threshold Angle", _track.bumperAngleThresold, 0.005f, 1.5f);
                if (bumperAngleThresold != _track.bumperAngleThresold)
                {
                    Undo.RegisterSceneUndo("Modifed Bumper Angle Threshold");
                    GUI.changed = true;
                    _track.bumperAngleThresold = bumperAngleThresold;
                }
                if (GUI.changed)//change on mouse up
                {
                    _track.ReRenderTrack();
                }
                EditorGUI.EndDisabledGroup();
                break;

            case TrackBuildR.modes.textures:

                EditorGUILayout.Space();
                Title("Curve Textures", TrackBuildRColours.BLUE);

                TrackBuildRTexture[] textures = _track.GetTexturesArray();
                int numberOfTextures = textures.Length;
                string[] textureNames = new string[numberOfTextures];
                for (int t = 0; t < numberOfTextures; t++)
                    textureNames[t] = textures[t].name;

                TrackBuildRCurve selectedCurve = _track.Curve(selectedCurveIndex);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Selected Curve: " + selectedCurve.name);

                if (GUILayout.Button("Goto Curve"))
                    GotoScenePoint(selectedCurve.center);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Boundary Height");
                float boundaryHeight = EditorGUILayout.Slider(selectedCurve.boundaryHeight, 0, 10);
                if (boundaryHeight != selectedCurve.boundaryHeight)
                {
                    GUI.changed = true;
                    selectedCurve.SetReRender();
                    selectedCurve.boundaryHeight = boundaryHeight;
                }
                EditorGUILayout.EndHorizontal();

                if (numberOfTextures > 0)
                {
                    int trackTextureStyleIndex = CurveTextureSelector(_track, selectedCurve.trackTextureStyleIndex, "Track Texture");
                    if (trackTextureStyleIndex != selectedCurve.trackTextureStyleIndex)
                    {
                        Undo.RegisterSceneUndo("Modified Track Texture");
                        selectedCurve.trackTextureStyleIndex = trackTextureStyleIndex;
                        GUI.changed = true;
                        _track.ReRenderTrack();
                    }
                    int offroadTextureStyleIndex = CurveTextureSelector(_track, selectedCurve.offroadTextureStyleIndex, "Offroad Texture");
                    if (offroadTextureStyleIndex != selectedCurve.offroadTextureStyleIndex)
                    {
                        Undo.RegisterSceneUndo("Modified Offroad Texture");
                        selectedCurve.offroadTextureStyleIndex = offroadTextureStyleIndex;
                        GUI.changed = true;
                        _track.ReRenderTrack();
                    }
                    int boundaryTextureStyleIndex = CurveTextureSelector(_track, selectedCurve.boundaryTextureStyleIndex, "Boundary Texture");
                    if (boundaryTextureStyleIndex != selectedCurve.boundaryTextureStyleIndex)
                    {
                        Undo.RegisterSceneUndo("Modified Wall Texture");
                        selectedCurve.boundaryTextureStyleIndex = boundaryTextureStyleIndex;
                        GUI.changed = true;
                        _track.ReRenderTrack();
                    }
                    int bumperTextureStyleIndex = CurveTextureSelector(_track, selectedCurve.bumperTextureStyleIndex, "Bumper Texture");
                    if (bumperTextureStyleIndex != selectedCurve.bumperTextureStyleIndex)
                    {
                        Undo.RegisterSceneUndo("Modified Bumper Texture");
                        selectedCurve.bumperTextureStyleIndex = bumperTextureStyleIndex;
                        GUI.changed = true;
                        _track.ReRenderTrack();
                    }
                }

                EditorGUILayout.Space();
                Title("Track Texture Library", TrackBuildRColours.RED);

                if (GUILayout.Button("Add New"))
                {
                    Undo.RegisterSceneUndo("Add Texture");
                    _track.AddTexture();
                    numberOfTextures++;
                    selectedTexture = numberOfTextures - 1;
                }
                if (numberOfTextures == 0)
                {
                    EditorGUILayout.HelpBox("There are no textures to show", MessageType.Info);
                    return;
                }

                if (numberOfTextures > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Texture", GUILayout.Width(75));

                    selectedTexture = EditorGUILayout.Popup(selectedTexture, textureNames);

                    TrackBuildRTexture trackBuildRTexture = _track.Texture(selectedTexture);

                    if (GUILayout.Button("Remove Texture"))
                    {
                        Undo.RegisterSceneUndo("Add Texture");
                        _track.RemoveTexture(trackBuildRTexture);
                        numberOfTextures--;
                        selectedTexture = 0;
                        trackBuildRTexture = _track.Texture(selectedTexture);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (TextureGUI(ref trackBuildRTexture))
                    {
                        _track.ReRenderTrack();
                    }
                }
                break;

            /*case TrackBuildR.modes.pit:

            _track.hasPitLane = EditorGUILayout.Toggle("Has Pit Lane", _track.hasPitLane);
//
//                bool legalPit = true;
//                if(_track.pitStartPointIndex == -1)
//                {
//                    EditorGUILayout.HelpBox("The pit start point has not been set", MessageType.Warning);
//                    legalPit = false;
//                }
//                if(_track.pitEndPointIndex == -1)
//                {
//                    EditorGUILayout.HelpBox("The pit end point has not been set", MessageType.Warning);
//                    legalPit = false;
//                }
//                if(_track.numberOfPitPoints == 0)
//                {
//                    EditorGUILayout.HelpBox("The pit has no points", MessageType.Warning);
//                    legalPit = false;
//                }

            EditorGUI.BeginDisabledGroup(!_track.hasPitLane);
                
            int currentPitMode = (int)pitMode;
            int newPitMode = GUILayout.Toolbar(currentPitMode, pitModeString);
            if (newPitMode != currentPitMode)
            {
                pitMode = (TrackBuildR.pitModes)newPitMode;
                GUI.changed = true;
            }

            switch (pitMode)
            {
                    case TrackBuildR.pitModes.editPitLane:
                    break;

                    case TrackBuildR.pitModes.setStartPoint:
                    _track.pitlane.pitStartPointIndex = EditorGUILayout.IntField("Pit Start Percentage", _track.pitlane.pitStartPointIndex);
                    break;

                    case TrackBuildR.pitModes.setEndPoint:
                    _track.pitlane.pitEndPointIndex = EditorGUILayout.IntField("Pit End Percentage", _track.pitlane.pitEndPointIndex);
                    break;
            }

            EditorGUI.EndDisabledGroup();

            break;*/

            case TrackBuildR.modes.diagram:

                EditorGUILayout.Space();
                Title("Diagram Image", TrackBuildRColours.RED);

                _track.showDiagram = EditorGUILayout.Toggle("Show Diagram", _track.showDiagram);
                _track.diagramGO.renderer.enabled = _track.showDiagram;

                EditorGUILayout.BeginHorizontal();
                if (_track.diagramMaterial.mainTexture != null)
                {
                    float height = _track.diagramMaterial.mainTexture.height * (200.0f / _track.diagramMaterial.mainTexture.width);
                    GUILayout.Label(_track.diagramMaterial.mainTexture, GUILayout.Width(200), GUILayout.Height(height));
                }
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("Load Diagram"))
                {
                    string newDiagramFilepath = EditorUtility.OpenFilePanel("Load Track Diagram", "/", "");
                    if (newDiagramFilepath != _track.diagramFilepath)
                    {
                        _track.diagramFilepath = newDiagramFilepath;
                        WWW www = new WWW("file:///" + newDiagramFilepath);
                        Texture2D newTexture = new Texture2D(100, 100);
                        www.LoadImageIntoTexture(newTexture);
                        _track.diagramMaterial.mainTexture = newTexture;

                        _track.diagramGO.transform.localScale = new Vector3(newTexture.width, 0, newTexture.height);
                        _track.showDiagram = true;
                    }
                }
                if (GUILayout.Button("Clear"))
                {
                    _track.diagramFilepath = "";
                    _track.diagramMaterial.mainTexture = null;
                    _track.showDiagram = false;
                }


                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Diagram Scale", GUILayout.Width(100));
                float newScale = EditorGUILayout.FloatField(_track.scale, GUILayout.Width(40));
                if (_track.scale != newScale)
                {
                    _track.scale = newScale;
                    UpdateDiagram(_track);
                }
                EditorGUILayout.LabelField("metres", GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (_track.assignedPoints == 0)
                {
                    if (GUILayout.Button("Draw Scale"))
                    {
                        _track.assignedPoints = 1;
                    }
                    if (GUILayout.Button("?", GUILayout.Width(25)))
                    {
                        EditorUtility.DisplayDialog("Draw Scale", "Once you load a diagram, use this to define the start and end of the diagram scale (I do hope your diagram has a scale...)", "ok");
                    }
                }
                else
                {
                    if (GUILayout.Button("Cancel Draw Scale"))
                    {
                        _track.assignedPoints = 0;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField(_track.diagramFilepath);


                break;

            case TrackBuildR.modes.options:

                EditorGUILayout.Space();
                Title("Generation Options", TrackBuildRColours.RED);

                //Toggle showing the wireframe when we have selected the model.
                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                EditorGUILayout.LabelField("Show Wireframe");
                _track.showWireframe = EditorGUILayout.Toggle(_track.showWireframe, GUILayout.Width(15));
                EditorGUILayout.EndHorizontal();

                //Tangent calculation
                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                EditorGUI.BeginDisabledGroup(_trackBuildR.tangentsGenerated);
                if (GUILayout.Button("Build Tangents", GUILayout.Height(38)))
                {
                    Undo.RegisterSceneUndo("Build Tangents");
                    _trackBuildR.GenerateTangents();
                    GUI.changed = false;
                }
                EditorGUI.EndDisabledGroup();
                if (!_trackBuildR.tangentsGenerated)
                    EditorGUILayout.HelpBox("The model doesn't have tangents", MessageType.Warning);
                EditorGUILayout.EndHorizontal();

                //Lightmap rendering
                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                EditorGUI.BeginDisabledGroup(_trackBuildR.lightmapGenerated);
                if (GUILayout.Button("Build Lightmap UVs", GUILayout.Height(38)))
                {
                    Undo.RegisterSceneUndo("Build Lightmap UVs");
                    _trackBuildR.GenerateSecondaryUVSet();
                    GUI.changed = false;
                }
                EditorGUI.EndDisabledGroup();
                if (!_trackBuildR.lightmapGenerated)
                    EditorGUILayout.HelpBox("The model doesn't have lightmap UVs", MessageType.Warning);
                EditorGUILayout.EndHorizontal();

                //Mesh Optimisation
                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
                EditorGUI.BeginDisabledGroup(_trackBuildR.optimised);
                if (GUILayout.Button("Optimise Mesh For Runtime", GUILayout.Height(38)))
                {
                    Undo.RegisterSceneUndo("Optimise Mesh");
                    _trackBuildR.OptimseMeshes();
                    GUI.changed = false;
                }
                EditorGUI.EndDisabledGroup();
                if (!_trackBuildR.optimised)
                    EditorGUILayout.HelpBox("The model is currently fully optimised for runtime", MessageType.Warning);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Force Full Rebuild of Track"))
                {
                    int numberOfPoints = _track.realNumberOfPoints;
                    for (int i = 0; i < numberOfPoints; i++)
                    {
                        _track[i].isDirty = true;
                    }
                    _track.RecalculateCurves();
                    _trackBuildR.UpdateRender();
                }

                break;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Register the undos when we press the Mouse button.
            Undo.CreateSnapshot();
            Undo.RegisterSnapshot();
        }

        GUILayout.EndVertical();
    }

    /// <summary>
    /// A GUI stub that displays the coloured titles in the inspector for Track BuildR
    /// </summary>
    /// <param name="titleString">The title to display</param>
    /// <param name="colour">Colour of the background</param>
    private static void Title(string titleString, Color colour)
    {
        //TITLE
        GUIStyle title = new GUIStyle(GUI.skin.label);
        title.fixedHeight = 60;
        title.fixedWidth = 400;
        title.alignment = TextAnchor.UpperCenter;
        title.fontStyle = FontStyle.Bold;
        title.normal.textColor = Color.white;
        EditorGUILayout.LabelField(titleString, title);
        Texture2D facadeTexture = new Texture2D(1, 1);
        facadeTexture.SetPixel(0, 0, colour);
        facadeTexture.Apply();
        Rect sqrPos = new Rect(0, 0, 0, 0);
        if (Event.current.type == EventType.Repaint)
            sqrPos = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(sqrPos, facadeTexture);
        EditorGUI.LabelField(sqrPos, titleString, title);
    }

    private static void RenderPreview(TrackBuildR _trackBuildR)
    {
        if (!SystemInfo.supportsRenderTextures)
            return;

        if (EditorApplication.isPlaying)
            return;

        TrackBuildRTrack _track = _trackBuildR.track;
        if (_track.realNumberOfPoints < 2)
            return;

        if (pointPreviewTexture == null)
            pointPreviewTexture = new RenderTexture(previewResolution, Mathf.RoundToInt(previewResolution / aspect), 24, RenderTextureFormat.RGB565);

        _track.diagramGO.renderer.enabled = false;
        GameObject trackEditorPreview = _trackBuildR.trackEditorPreview;
        trackEditorPreview.transform.position = _track.GetTrackPosition(_trackBuildR.previewPercentage) + previewCameraHeight;
        trackEditorPreview.transform.LookAt(_track.GetTrackPosition((_trackBuildR.previewPercentage + 0.0001f) % 1) + previewCameraHeight);
        trackEditorPreview.camera.targetTexture = pointPreviewTexture;
        trackEditorPreview.camera.Render();
        trackEditorPreview.camera.targetTexture = null;
        _track.diagramGO.renderer.enabled = _track.showDiagram;

        GUILayout.Label(pointPreviewTexture, GUILayout.Width(400), GUILayout.Height(225));

        EditorGUILayout.BeginHorizontal();
        _trackBuildR.previewPercentage = EditorGUILayout.Slider(_trackBuildR.previewPercentage, 0, 1);
        EditorGUILayout.LabelField("0-1", GUILayout.Width(25));
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// The Texture display GUI
    /// </summary>
    /// <param name="texture"></param>
    /// <returns>True if this texture was modified</returns>
    private static bool TextureGUI(ref TrackBuildRTexture texture)
    {
        bool isModified = false;
        string textureName = texture.name;
        textureName = EditorGUILayout.TextField("Name", textureName);
        if (texture.name != textureName)
        {
            texture.name = textureName;
        }

        UnityEditorInternal.InternalEditorUtility.SetupShaderMenu(texture.material);
        Shader[] tempshaders = (Shader[])Resources.FindObjectsOfTypeAll(typeof(Shader));
        List<Shader> shaders = new List<Shader>();
        foreach (Shader shader in tempshaders)
            if (!string.IsNullOrEmpty(shader.name) && !shader.name.StartsWith("__"))
                shaders.Add(shader);
        int selectedShader = shaders.IndexOf(texture.material.shader);
        int numberOfShaders = shaders.Count;
        string[] shaderNames = new string[numberOfShaders];
        for (int s = 0; s < numberOfShaders; s++)
            shaderNames[s] = shaders[s].name;
        int newSelectedShader = EditorGUILayout.Popup("Shader", selectedShader, shaderNames);
        if (selectedShader != newSelectedShader)
            texture.material.shader = shaders[newSelectedShader];


        texture.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", texture.texture, typeof(Texture2D), false);

        if (texture.texture == null)
            return isModified;

        if (texture.material.HasProperty("_BumpMap"))
        {
            Texture bumpMap = (Texture)EditorGUILayout.ObjectField("Bump Map", texture.material.GetTexture("_BumpMap"), typeof(Texture), false);
            if (bumpMap != null)
                texture.material.SetTexture("_BumpMap", bumpMap);
        }

        if (texture.material.HasProperty("_Color"))
            texture.material.SetColor("_Color", EditorGUILayout.ColorField("Colour", texture.material.GetColor("_Color")));

        if (texture.material.HasProperty("_SpecColor"))
            texture.material.SetColor("_SpecColor", EditorGUILayout.ColorField("Specular Colour", texture.material.GetColor("_SpecColor")));

        if (texture.material.HasProperty("_Shininess"))
            texture.material.SetFloat("_Shininess", EditorGUILayout.Slider("Shininess", texture.material.GetFloat("_Shininess"), 0, 1));

        bool textureflipped = EditorGUILayout.Toggle("Flip Clockwise", texture.flipped);
        if (textureflipped != texture.flipped)
        {
            isModified = true;
            texture.flipped = textureflipped;
        }

        Vector2 textureUnitSize = texture.textureUnitSize;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("trackTexture width", GUILayout.Width(75));//, GUILayout.Width(42));
        float textureUnitSizex = EditorGUILayout.FloatField(texture.textureUnitSize.x, GUILayout.Width(25));
        if (textureUnitSizex != textureUnitSize.x)
        {
            isModified = true;
            textureUnitSize.x = textureUnitSizex;
        }
        EditorGUILayout.LabelField("metres", GUILayout.Width(40));//, GUILayout.Width(42));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("trackTexture height", GUILayout.Width(75));//, GUILayout.Width(42));
        float textureUnitSizey = EditorGUILayout.FloatField(texture.textureUnitSize.y, GUILayout.Width(25));
        if (textureUnitSizey != textureUnitSize.y)
        {
            isModified = true;
            textureUnitSize.y = textureUnitSizey;
        }
        EditorGUILayout.LabelField("metres", GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();
        texture.textureUnitSize = textureUnitSize;

        const int previewTextureUnitSize = 120;
        const int previewPadding = 25;

        EditorGUILayout.LabelField("1 Metre Squared", GUILayout.Width(previewTextureUnitSize));
        GUILayout.Space(previewPadding);
        EditorGUILayout.Space();

        Rect texturePreviewPostion = new Rect(0, 0, 0, 0);
        if (Event.current.type == EventType.Repaint)
            texturePreviewPostion = GUILayoutUtility.GetLastRect();

        Rect previewRect = new Rect(texturePreviewPostion.x, texturePreviewPostion.y, previewTextureUnitSize, previewTextureUnitSize);
        Rect sourceRect = new Rect(0, 0, (1.0f / textureUnitSize.x), (1.0f / textureUnitSize.y));

        Graphics.DrawTexture(previewRect, texture.texture, sourceRect, 0, 0, 0, 0);
        GUILayout.Space(previewTextureUnitSize);

        if (isModified)
            GUI.changed = true;

        return isModified;
    }

    /// <summary>
    /// Deals with modifing the diagram used in track building
    /// </summary>
    private static void UpdateDiagram(TrackBuildRTrack _track)
    {
        //        Texture texture = _track.diagramMaterial.mainTexture;
        float scaleSize = Vector3.Distance(_track.scalePointB, _track.scalePointA);
        float diagramScale = _track.scale / scaleSize;
        _track.diagramGO.transform.localScale *= diagramScale;
        _track.scalePointA *= diagramScale;
        _track.scalePointB *= diagramScale;
    }

    /// <summary>
    /// A stub of GUI for selecting the texture for a specific part of the track on a specific curve
    /// IE. The track texture, the wall texture, etc...
    /// </summary>
    /// <param name="_track"></param>
    /// <param name="textureIndex"></param>
    /// <param name="label"></param>
    /// <returns></returns>
    private static int CurveTextureSelector(TrackBuildRTrack _track, int textureIndex, string label)
    {
        TrackBuildRTexture[] textures = _track.GetTexturesArray();
        int numberOfTextures = textures.Length;
        string[] textureNames = new string[numberOfTextures];
        for (int t = 0; t < numberOfTextures; t++)
            textureNames[t] = textures[t].name;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(label);
        textureIndex = EditorGUILayout.Popup(textureIndex, textureNames);
        TrackBuildRTexture tbrTexture = _track.Texture(textureIndex);
        EditorGUILayout.EndVertical();
        GUILayout.Label(tbrTexture.texture, GUILayout.Width(50), GUILayout.Height(50));
        EditorGUILayout.EndHorizontal();

        return textureIndex;
    }

    /// <summary>
    /// Called to ensure we're not leaking stuff into the Editor
    /// </summary>
    public static void CleanUp()
    {
        if (pointPreviewTexture)
        {
            pointPreviewTexture.DiscardContents();
            pointPreviewTexture.Release();
            Object.DestroyImmediate(pointPreviewTexture);
        }
    }
    
    /// <summary>
    /// A little hacking of the Unity Editor to allow us to focus on an arbitrary point in 3D Space
    /// We're replicating pressing the F button in scene view to focus on the selected object
    /// Here we can focus on a 3D point
    /// </summary>
    /// <param name="position">The 3D point we want to focus on</param>
    private static void GotoScenePoint(Vector3 position)
    {
        Object[] intialFocus = Selection.objects;
        GameObject tempFocusView = new GameObject("Temp Focus View");
        tempFocusView.transform.position = position;
        Selection.objects = new Object[] { tempFocusView };
        SceneView.lastActiveSceneView.FrameSelected();
        Selection.objects = intialFocus;
        Object.DestroyImmediate(tempFocusView);
    }
}
