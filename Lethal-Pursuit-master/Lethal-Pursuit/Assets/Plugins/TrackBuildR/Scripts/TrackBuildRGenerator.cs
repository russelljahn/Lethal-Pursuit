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

[ExecuteInEditMode]
public class TrackBuildRGenerator : MonoBehaviour
{
    public TrackBuildRTrack track;
    public List<GameObject> meshHolders = new List<GameObject>();
    public List<GameObject> colliderHolders = new List<GameObject>();

    public void Update()
    {
        int colliders = colliderHolders.Count;
        for (int i = 0; i < colliders; i++)
        {
            //colliderHolders[i].SetActive(Application.isPlaying);
        }
    }

    public void UpdateRender()
    {
        if (track.numberOfCurves == 0)
            return;

        //        float timeStart = Time.realtimeSinceStartup;
        track.RecalculateCurves();
        //        float trackLength = track.trackLength;
        float trackMeshRes = track.meshResolution;

        float bumperDistanceA = 0;
        float bumperDistanceB = 0;


        int numberOfCurves = track.numberOfCurves;
        for (int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRCurve curve = track.Curve(i);
            if (curve.shouldReRender)
            {
                DynamicMeshGenericMultiMaterialMesh dynamicTrackMesh = curve.dynamicTrackMesh;
                DynamicMeshGenericMultiMaterialMesh dynamicBoundaryMesh = curve.dynamicBoundaryMesh;
                DynamicMeshGenericMultiMaterialMesh dynamicOffroadMesh = curve.dynamicOffroadMesh;
                DynamicMeshGenericMultiMaterialMesh dynamicBumperMesh = curve.dynamicBumperMesh;
                DynamicMeshGenericMultiMaterialMesh dynamicColliderMesh = curve.dynamicColliderMesh;

                dynamicTrackMesh.Clear();
                dynamicTrackMesh.subMeshCount = 1;
                dynamicBoundaryMesh.Clear();
                dynamicBoundaryMesh.subMeshCount = 1;
                dynamicColliderMesh.Clear();
                dynamicColliderMesh.subMeshCount = 1;
                dynamicOffroadMesh.Clear();
                dynamicOffroadMesh.subMeshCount = 1;
                dynamicBumperMesh.Clear();
                dynamicBumperMesh.subMeshCount = 1;

                dynamicTrackMesh.name = "curve " + i + " track mesh";
                dynamicBoundaryMesh.name = "curve " + i + " boundary mesh";
                dynamicColliderMesh.name = "curve " + i + " collider mesh";
                dynamicOffroadMesh.name = "curve " + i + " offroad mesh";
                dynamicBumperMesh.name = "curve " + i + " bumper mesh";

                bool trackTextureFlip = (track.numberOfTextures > 0) ? track.Texture(curve.trackTextureStyleIndex).flipped : false;
                //bool offroadTextureFlip = (track.numberOfTextures > 0) ? track.Texture(curve.offroadTextureStyleIndex).flipped : false;
                bool boundaryTextureFlip = (track.numberOfTextures > 0) ? track.Texture(curve.boundaryTextureStyleIndex).flipped : false;
                bool bumperTextureFlip = (track.numberOfTextures > 0) ? track.Texture(curve.bumperTextureStyleIndex).flipped : false;

                int storedPointSize = curve.storedPointSize;
                float curveLength = curve.arcLength;
                float res = 1.0f / (storedPointSize);
                //Store these points so we can use previous values when Bezier clips itself
                Vector3 leftPointA = curve.sampledLeftBoundaryPoints[0];
                Vector3 rightPointA = curve.sampledRightBoundaryPoints[0];
                Vector3 leftPointB = curve.sampledLeftBoundaryPoints[0];
                Vector3 rightPointB = curve.sampledRightBoundaryPoints[0];
                for (int p = 0; p < storedPointSize - 1; p++)
                {
                    float tA = curve.normalisedT[p];
                    int sampleIndexA = p;
                    int sampleIndexB = sampleIndexA + 1;
                    Vector3 pointA = curve.sampledPoints[sampleIndexA];
                    Vector3 pointB = curve.sampledPoints[sampleIndexB];
                    float trackWidthA = curve.sampledWidths[sampleIndexA] * 0.5f;
                    float trackWidthB = curve.sampledWidths[sampleIndexB] * 0.5f;
                    float trackCrownA = curve.sampledCrowns[sampleIndexA];
                    float trackCrownB = curve.sampledCrowns[sampleIndexB];
                    Vector3 trackUpA = curve.sampledTrackUps[sampleIndexA];
                    Vector3 trackUpB = curve.sampledTrackUps[sampleIndexB];
                    Vector3 trackCrossA = curve.sampledTrackCrosses[sampleIndexA];
                    Vector3 trackCrossB = curve.sampledTrackCrosses[sampleIndexB];
                    float trackAngle = curve.sampledAngles[sampleIndexA];

                    if (trackUpA == Vector3.zero || trackUpB == Vector3.zero)
                        return;

                    //TrackBuildRTexture texture = track.Texture(curve.trackTextureStyleIndex) ;// track.trackTexture;
                    int pointANumber = Mathf.Max(Mathf.CeilToInt(trackWidthA / trackMeshRes / 2) * 2, 2);//number of verts along line A
                    int pointBNumber = Mathf.Max(Mathf.CeilToInt(trackWidthB / trackMeshRes / 2) * 2, 2);//number of verts along line B
                    int numberOfNewVerts = pointANumber + pointBNumber;
                    Vector3[] uncrownedVerts = new Vector3[numberOfNewVerts];
                    if (curve.clipArrayLeft[sampleIndexA]) leftPointA = (pointA + (trackCrossA * -trackWidthA));
                    if (curve.clipArrayRight[sampleIndexA]) rightPointA = (pointA + (trackCrossA * trackWidthA));
                    float curveLengthA = (curveLength * tA) / trackWidthA;
                    float curveLengthB = (curveLength * (tA + res)) / trackWidthA;
                    float lerpASize = 1.0f / (pointANumber - 1);

                    //track vertex/uv data for point nextNormIndex
                    Vector3[] newAPoints = new Vector3[pointANumber];
                    Vector3[] newTrackPoints = new Vector3[pointANumber + pointBNumber];
                    Vector2[] newTrackUVs = new Vector2[pointANumber + pointBNumber];
                    for (int pa = 0; pa < pointANumber; pa++)
                    {
                        float lerpValue = lerpASize * pa;
                        Vector3 crownVector = Quaternion.LookRotation(trackUpA) * new Vector3(0, 0, Mathf.Sin(lerpValue * Mathf.PI) * trackCrownA);
                        Vector3 uncrownedVert = Vector3.Lerp(leftPointA, rightPointA, lerpValue);
                        uncrownedVerts[pa] = uncrownedVert;
                        Vector3 newVert = uncrownedVert + crownVector;
                        newAPoints[pa] = newVert;
                        newTrackPoints[pa] = newVert;
                        //Vector2 newUV = CalculateUV(pa, pointANumber, curveLengthA);
                        //Vector2 newUV = new Vector2(lerpValue, curveLengthA);
                        Vector2 newUV = (!trackTextureFlip) ? new Vector2(lerpValue, curveLengthA) : new Vector2(curveLengthA, lerpValue);
                        //                        newUV.x /= texture.textureUnitSize.x;
                        //                        newUV.y /= texture.textureUnitSize.y;
                        newTrackUVs[pa] = newUV;
                    }

                    //track vertex/uv data for point prevNormIndex
                    if (curve.clipArrayLeft[sampleIndexB]) leftPointB = (pointB + (trackCrossB * -trackWidthB));
                    if (curve.clipArrayRight[sampleIndexB]) rightPointB = (pointB + (trackCrossB * trackWidthB));
                    float lerpBSize = 1.0f / (pointBNumber - 1);
                    Vector3[] newBPoints = new Vector3[pointBNumber];
                    for (int pb = 0; pb < pointBNumber; pb++)
                    {
                        float lerpValue = lerpBSize * pb;
                        Vector3 crownVector = Quaternion.LookRotation(trackUpB) * new Vector3(0, 0, Mathf.Sin(lerpValue * Mathf.PI) * trackCrownB);
                        Vector3 uncrownedVert = Vector3.Lerp(leftPointB, rightPointB, lerpValue);
                        uncrownedVerts[pb + pointANumber] = uncrownedVert;
                        Vector3 newVert = uncrownedVert + crownVector;
                        newBPoints[pb] = newVert;
                        newTrackPoints[pb + pointANumber] = newVert;
                        //                        Vector2 newUV = CalculateUV(pb, pointBNumber, curveLengthB);
                        Vector2 newUV = (!trackTextureFlip) ? new Vector2(lerpValue, curveLengthB) : new Vector2(curveLengthB, lerpValue);
                        //                        newUV.x /= texture.textureUnitSize.x;
                        //                        newUV.y /= texture.textureUnitSize.y;
                        newTrackUVs[pb + pointANumber] = newUV;
                    }
                    int baseTriPointA = 0;
                    int baseTriPointB = pointANumber;
                    int triPointA = baseTriPointA;
                    int triPointB = baseTriPointB;
                    int newTriPointCountA = 1;
                    int newTriPointCountB = 1;
                    int[] newTrackTris = new int[(numberOfNewVerts - 2) * 3];
                    for (int v = 0; v < numberOfNewVerts - 2; v++)
                    {
                        int newTriPointA = baseTriPointA + newTriPointCountA;
                        int newTriPointB = baseTriPointB + newTriPointCountB;

                        float newTriPointADist, newTriPointBDist;
                        if (newTriPointA >= baseTriPointA + pointANumber)
                            newTriPointADist = float.PositiveInfinity;
                        else
                            newTriPointADist = Vector3.SqrMagnitude(uncrownedVerts[newTriPointA] - uncrownedVerts[baseTriPointA]);

                        if (newTriPointB >= baseTriPointB + pointBNumber)
                            newTriPointBDist = float.PositiveInfinity;
                        else
                            newTriPointBDist = Vector3.SqrMagnitude(uncrownedVerts[newTriPointB] - uncrownedVerts[baseTriPointB]);

                        if (newTriPointADist < newTriPointBDist)
                        {
                            newTrackTris[v * 3] = triPointA;
                            newTrackTris[v * 3 + 1] = triPointB;
                            newTrackTris[v * 3 + 2] = newTriPointA;
                            triPointA = newTriPointA;
                            newTriPointCountA++;
                        }
                        else
                        {
                            newTrackTris[v * 3] = triPointA;
                            newTrackTris[v * 3 + 1] = triPointB;
                            newTrackTris[v * 3 + 2] = newTriPointB;
                            triPointB = newTriPointB;
                            newTriPointCountB++;
                        }
                    }
                    dynamicTrackMesh.AddData(newTrackPoints, newTrackUVs, newTrackTris, 0);
                    dynamicColliderMesh.AddData(newTrackPoints, newTrackUVs, newTrackTris, 0);

                    //Boundary
                    float trackBoundaryWallHeight = curve.boundaryHeight;// track.boundaryHeight;

                    Vector3 leftBoundaryPointA, leftBoundaryPointB, rightBoundaryPointA, rightBoundaryPointB;
                    if (track.disconnectBoundary)
                    {
                        leftBoundaryPointA = curve.sampledLeftBoundaryPoints[sampleIndexA];
                        leftBoundaryPointB = curve.sampledLeftBoundaryPoints[sampleIndexB];
                        rightBoundaryPointA = curve.sampledRightBoundaryPoints[sampleIndexA];
                        rightBoundaryPointB = curve.sampledRightBoundaryPoints[sampleIndexB];
                    }
                    else
                    {
                        leftBoundaryPointA = leftPointA;
                        leftBoundaryPointB = leftPointB;
                        rightBoundaryPointA = rightPointA;
                        rightBoundaryPointB = rightPointB;
                    }

                    Vector3[] newWallVerts;
                    Vector2[] newWallUVs;
                    int[] newWallTris;

                    //Boundary Render Mesh
                    //LEFT
                    newWallVerts = new[] { leftBoundaryPointA, leftBoundaryPointB, leftBoundaryPointA + trackUpA * trackBoundaryWallHeight, leftBoundaryPointB + trackUpB * trackBoundaryWallHeight };

                    if (!boundaryTextureFlip)
                        newWallUVs = new[] { new Vector2(curveLengthA, 0), new Vector2(curveLengthB, 0), new Vector2(curveLengthA, 1), new Vector2(curveLengthB, 1), };
                    else
                        newWallUVs = new[] { new Vector2(1, curveLengthA), new Vector2(1, curveLengthB), new Vector2(0, curveLengthA), new Vector2(0, curveLengthB), };
                    newWallTris = new[] { 1, 0, 2, 1, 2, 3 };
                    //                    newWallTris = (boundaryTextureFlip) ? (new[] { 1, 0, 2, 1, 2, 3 }) : (new[] { 0,2,1,2,3,1 });
                    //                    newWallTris = (!track.renderBoundaryWallReverse) ? new[] { 1, 0, 2, 1, 2, 3 } : new[] { 1, 0, 2, 1, 2, 3, 0, 1, 2, 2, 1, 3 };
                    dynamicBoundaryMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                    if (track.renderBoundaryWallReverse)
                    {
                        newWallTris = new[] { 0, 1, 2, 2, 1, 3 };
                        //                        newWallTris = (boundaryTextureFlip) ? (new[] { 0, 1, 2, 2, 1, 3 }) : (new[] { 0, 2, 1, 2, 3, 1 });
                        dynamicBoundaryMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                    }

                    //RIGHT
                    newWallVerts = (new[] { rightBoundaryPointA, rightBoundaryPointB, rightBoundaryPointA + trackUpA * trackBoundaryWallHeight, rightBoundaryPointB + trackUpB * trackBoundaryWallHeight });
                    //newWallUVs = new[] { new Vector2(curveLengthA, 0), new Vector2(curveLengthB, 0), new Vector2(curveLengthA, 1), new Vector2(curveLengthB, 1), };
                    if (!boundaryTextureFlip)
                        newWallUVs = new[] { new Vector2(curveLengthA, 0), new Vector2(curveLengthB, 0), new Vector2(curveLengthA, 1), new Vector2(curveLengthB, 1), };
                    else
                        newWallUVs = new[] { new Vector2(1, curveLengthA), new Vector2(1, curveLengthB), new Vector2(0, curveLengthA), new Vector2(0, curveLengthB), };

                    newWallTris = new[] { 0, 1, 2, 2, 1, 3 };
                    //newWallTris = (!track.renderBoundaryWallReverse) ? new[] { 0, 1, 2, 2, 1, 3 } : new[] { 1, 0, 2, 1, 2, 3, 0, 1, 2, 2, 1, 3 };
                    dynamicBoundaryMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                    if (track.renderBoundaryWallReverse)
                    {
                        newWallTris = new[] { 1, 0, 2, 1, 2, 3 };
                        dynamicBoundaryMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                    }

                    //COLLIDER walls for on track border
                    float trackColliderWallHeight = track.trackColliderWallHeight;

                    newWallVerts = (new[] { leftBoundaryPointA, leftBoundaryPointB, leftBoundaryPointA + trackUpA * trackColliderWallHeight, leftBoundaryPointB + trackUpB * trackColliderWallHeight });
                    newWallUVs = (new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero });
                    newWallTris = (new[] { 1, 0, 2, 1, 2, 3 });
                    dynamicColliderMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                    newWallVerts = (new[] { rightBoundaryPointA, rightBoundaryPointB, rightBoundaryPointA + trackUpA * trackColliderWallHeight, rightBoundaryPointB + trackUpB * trackColliderWallHeight });
                    newWallUVs = (new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero });
                    newWallTris = (new[] { 0, 1, 2, 2, 1, 3 });
                    dynamicColliderMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);


                    //offroad bits
                    if (track.disconnectBoundary)
                    {
                        Vector2 offroadTextureSize = Vector2.one;
                        if (track.numberOfTextures > 0)
                            offroadTextureSize = track.Texture(curve.offroadTextureStyleIndex).textureUnitSize;// track.offroadTexture.textureUnitSize;
                        newWallVerts = (new[] { leftPointA, leftPointB, leftBoundaryPointA, leftBoundaryPointB });
                        newWallUVs = (new[] { new Vector2(leftPointA.x / offroadTextureSize.x, leftPointA.z / offroadTextureSize.y), new Vector2(leftPointB.x / offroadTextureSize.x, leftPointB.z / offroadTextureSize.y), new Vector2(leftBoundaryPointA.x / offroadTextureSize.x, leftBoundaryPointA.z / offroadTextureSize.y), new Vector2(leftBoundaryPointB.x / offroadTextureSize.x, leftBoundaryPointB.z / offroadTextureSize.y) });
                        newWallTris = (new[] { 1, 0, 2, 1, 2, 3 });
                        dynamicOffroadMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);

                        newWallVerts = (new[] { rightPointA, rightPointB, rightBoundaryPointA, rightBoundaryPointB });
                        newWallUVs = (new[] { new Vector2(rightPointA.x / offroadTextureSize.x, rightPointA.z / offroadTextureSize.y), new Vector2(rightPointB.x / offroadTextureSize.x, rightPointB.z / offroadTextureSize.y), new Vector2(rightBoundaryPointA.x / offroadTextureSize.x, rightBoundaryPointA.z / offroadTextureSize.y), new Vector2(rightBoundaryPointB.x / offroadTextureSize.x, rightBoundaryPointB.z / offroadTextureSize.y) });
                        newWallTris = (new[] { 0, 1, 2, 2, 1, 3 });
                        dynamicOffroadMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);

                        newWallVerts = (new[] { leftPointA, leftPointB, leftBoundaryPointA, leftBoundaryPointB });
                        newWallUVs = (new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero });
                        newWallTris = (new[] { 1, 0, 2, 1, 2, 3 });
                        dynamicColliderMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                        newWallVerts = (new[] { rightPointA, rightPointB, rightBoundaryPointA, rightBoundaryPointB });
                        newWallUVs = (new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero });
                        newWallTris = (new[] { 0, 1, 2, 2, 1, 3 });
                        dynamicColliderMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                    }

                    if (track.includeColliderRoof)
                    {
                        newWallVerts = (new[] { leftBoundaryPointA + trackUpA * trackColliderWallHeight, leftBoundaryPointB + trackUpB * trackColliderWallHeight, rightBoundaryPointA + trackUpA * trackColliderWallHeight, rightBoundaryPointB + trackUpB * trackColliderWallHeight });
                        newWallUVs = (new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero });
                        newWallTris = (new[] { 1, 0, 2, 1, 2, 3 });
                        dynamicColliderMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                    }

                    if (track.trackBumpers)
                    {
                        float bumperWidth = track.bumperWidth;
                        float bumperHeight = track.bumperHeight;
                        Vector3 bumperRaisedA = trackUpA * bumperHeight;
                        Vector3 bumperRaisedB = trackUpB * bumperHeight;
                        float trackAngleThreashold = track.bumperAngleThresold;

                        //left bumpers
                        if (trackAngle >= trackAngleThreashold)
                        {
                            Vector3 offroadEdgeDirectionA = (leftBoundaryPointA - leftPointA).normalized;
                            Vector3 trackEdgeDirectionA = (newAPoints[1] - newAPoints[0]).normalized;
                            Vector3 bumperDirectionA = (Vector3.Project(offroadEdgeDirectionA, trackUpA) - trackEdgeDirectionA).normalized;
                            Vector3 offroadEdgeDirectionB = (leftBoundaryPointB - leftPointB).normalized;
                            Vector3 trackEdgeDirectionB = (newBPoints[1] - newBPoints[0]).normalized;
                            Vector3 bumperDirectionB = (Vector3.Project(offroadEdgeDirectionB, trackUpB) - trackEdgeDirectionB).normalized;
                            float trackEdgeA = Vector3.Distance(pointA, leftPointA);
                            float offroadEdgeA = Vector3.Distance(pointA, leftBoundaryPointA);
                            bool offroadBumper = (trackEdgeA < (offroadEdgeA - bumperWidth));
                            Vector3 bumperLeft0 = (offroadBumper ? leftPointA + bumperDirectionA * bumperWidth : leftBoundaryPointA) + bumperRaisedA;
                            Vector3 bumperLeft1 = (offroadBumper ? leftPointA : bumperLeft0 - (bumperDirectionA * bumperWidth) - bumperRaisedB);//bumperLeft0 + (trackEdgeDirectionA * bumperWidth)) - bumperRaisedB;

                            Vector3 bumperLeft2 = (offroadBumper ? leftPointB + bumperDirectionB * bumperWidth : leftBoundaryPointB) + bumperRaisedB;
                            Vector3 bumperLeft3 = (offroadBumper ? leftPointB : bumperLeft2 - (bumperDirectionB * bumperWidth) - bumperRaisedB);

                            float bumperSegmentDistanceA = Vector3.Distance(bumperLeft0, bumperLeft2);
                            float uvStartA, uvEndA;
                            if (track.numberOfTextures > 0)
                            {
                                uvStartA = bumperDistanceA / track.Texture(curve.bumperTextureStyleIndex).textureUnitSize.y;// track.bumperTexture.textureUnitSize.y;
                                uvEndA = (bumperDistanceA + bumperSegmentDistanceA) / track.Texture(curve.bumperTextureStyleIndex).textureUnitSize.y;// track.bumperTexture.textureUnitSize.y;
                            }
                            else
                            {
                                uvStartA = bumperDistanceA;// track.bumperTexture.textureUnitSize.y;
                                uvEndA = (bumperDistanceA + bumperSegmentDistanceA);// track.bumperTexture.textureUnitSize.y;  
                            }
                            newWallVerts = (new[] { bumperLeft0, bumperLeft1, bumperLeft2, bumperLeft3 });
                            if (!bumperTextureFlip)
                                newWallUVs = (new[] { new Vector2(uvStartA, 1), new Vector2(uvStartA, 0), new Vector2(uvEndA, 1), new Vector2(uvEndA, 0) });
                            else
                                newWallUVs = (new[] { new Vector2(1, uvStartA), new Vector2(0, uvStartA), new Vector2(1, uvEndA), new Vector2(0, uvEndA) });
                            newWallTris = (new[] { 1, 0, 2, 1, 2, 3 });
                            dynamicBumperMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                            bumperDistanceA += bumperSegmentDistanceA;

                            newWallVerts = (new[] { bumperLeft0, bumperLeft1, bumperLeft2, bumperLeft3 });
                            newWallUVs = (new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero });
                            newWallTris = (new[] { 1, 0, 2, 1, 2, 3 });
                            dynamicColliderMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                        }

                        //Right bumpers
                        if (trackAngle < -trackAngleThreashold)
                        {
                            Vector3 trackEdgeDirectionA = (newAPoints[pointANumber - 2] - newAPoints[pointANumber - 1]).normalized;
                            Vector3 trackEdgeDirectionB = (newBPoints[pointBNumber - 2] - newBPoints[pointBNumber - 1]).normalized;

                            Vector3 bumperRight0 = ((Vector3.Distance(pointA, rightPointA) < (Vector3.Distance(pointA, rightBoundaryPointA) - bumperWidth)) ? rightPointA : rightBoundaryPointA) + bumperRaisedA;
                            Vector3 bumperRight1 = bumperRight0 - (trackEdgeDirectionA * bumperWidth);

                            Vector3 bumperRight2 = ((Vector3.Distance(pointB, rightPointB) < (Vector3.Distance(pointB, rightBoundaryPointB) - bumperWidth)) ? rightPointB : rightBoundaryPointB) + bumperRaisedB;
                            Vector3 bumperRight3 = bumperRight2 - (trackEdgeDirectionB * bumperWidth);

                            float bumperSegmentDistanceB = Vector3.Distance(bumperRight0, bumperRight2);
                            //float bumperSegmentDistanceA = Vector3.Distance(bumperLeft0, bumperLeft2);

                            float uvStartB, uvEndB;
                            if (track.numberOfTextures > 0)
                            {
                                uvStartB = bumperDistanceB / track.Texture(curve.bumperTextureStyleIndex).textureUnitSize.y;// track.bumperTexture.textureUnitSize.y;
                                uvEndB = (bumperDistanceB + bumperSegmentDistanceB) / track.Texture(curve.bumperTextureStyleIndex).textureUnitSize.y;// track.bumperTexture.textureUnitSize.y;
                            }
                            else
                            {
                                uvStartB = bumperDistanceB;
                                uvEndB = (bumperDistanceB + bumperSegmentDistanceB);

                            }
                            newWallVerts = (new[] { bumperRight0, bumperRight1, bumperRight2, bumperRight3 });
                            if (!bumperTextureFlip)
                                newWallUVs = (new[] { new Vector2(uvStartB, 1), new Vector2(uvStartB, 0), new Vector2(uvEndB, 1), new Vector2(uvEndB, 0) });
                            else
                                newWallUVs = (new[] { new Vector2(1, uvStartB), new Vector2(0, uvStartB), new Vector2(1, uvEndB), new Vector2(0, uvEndB) });
                            newWallTris = (new[] { 1, 0, 2, 1, 2, 3 });
                            dynamicBumperMesh.AddData(newWallVerts, newWallUVs, newWallTris, 0);
                            bumperDistanceB += bumperSegmentDistanceB;
                        }
                    }
                }

                if (curve.holder != null)
                {
                    DestroyImmediate(curve.holder);
                }

                GameObject newCurveMeshHolder = new GameObject("curve " + (i + 1));
                newCurveMeshHolder.transform.parent = transform;
                newCurveMeshHolder.transform.localPosition = Vector3.zero;
                curve.holder = newCurveMeshHolder;

                dynamicTrackMesh.Build();
                int numberOfMeshes = dynamicTrackMesh.meshCount;
                for (int m = 0; m < numberOfMeshes; m++)
                {
                    //                    Debug.Log("dynamicTrackMesh " + dynamicTrackMesh.vertexCount);
                    GameObject newMeshHolder = new GameObject("model " + (m + 1));
                    newMeshHolder.transform.parent = curve.holder.transform;
                    newMeshHolder.transform.localPosition = Vector3.zero;
                    newMeshHolder.AddComponent<MeshFilter>().sharedMesh = dynamicTrackMesh[m].mesh;
                    if (track.numberOfTextures > 0)
                        newMeshHolder.AddComponent<MeshRenderer>().material = track.Texture(curve.trackTextureStyleIndex).material;// track.trackTexture.material;
#if UNITY_EDITOR
                    EditorUtility.SetSelectedWireframeHidden(newMeshHolder.renderer, !track.showWireframe);
#endif
                }

                dynamicBoundaryMesh.Build();
                numberOfMeshes = dynamicBoundaryMesh.meshCount;
                for (int m = 0; m < numberOfMeshes; m++)
                {
                    GameObject newMeshHolder = new GameObject("boundary " + (m + 1));
                    newMeshHolder.transform.parent = curve.holder.transform;
                    newMeshHolder.transform.localPosition = Vector3.zero;
                    newMeshHolder.AddComponent<MeshFilter>().sharedMesh = dynamicBoundaryMesh[m].mesh;
                    if (track.numberOfTextures > 0)
                        newMeshHolder.AddComponent<MeshRenderer>().material = track.Texture(curve.boundaryTextureStyleIndex).material;// track.trackTexture.material;
#if UNITY_EDITOR
                    EditorUtility.SetSelectedWireframeHidden(newMeshHolder.renderer, !track.showWireframe);
#endif
                }

                if (track.disconnectBoundary)
                {
                    dynamicOffroadMesh.Build();
                    numberOfMeshes = dynamicOffroadMesh.meshCount;
                    for (int m = 0; m < numberOfMeshes; m++)
                    {
                        GameObject newMeshHolder = new GameObject("offroad " + (m + 1));
                        newMeshHolder.transform.parent = curve.holder.transform;
                        newMeshHolder.transform.localPosition = Vector3.zero;
                        newMeshHolder.AddComponent<MeshFilter>().sharedMesh = dynamicOffroadMesh[m].mesh;
                        if (track.numberOfTextures > 0)
                            newMeshHolder.AddComponent<MeshRenderer>().material = track.Texture(curve.offroadTextureStyleIndex).material;// track.offroadTexture.material;
#if UNITY_EDITOR
                        EditorUtility.SetSelectedWireframeHidden(newMeshHolder.renderer, !track.showWireframe);
#endif
                    }
                }

                if (track.includeCollider)
                {
                    dynamicColliderMesh.Build();
                    int numberOfColliderMeshes = dynamicColliderMesh.meshCount;
                    for (int m = 0; m < numberOfColliderMeshes; m++)
                    {
                        GameObject newMeshHolder = new GameObject("collider " + (m + 1));
                        newMeshHolder.transform.parent = curve.holder.transform;
                        newMeshHolder.transform.localPosition = Vector3.zero;
                        newMeshHolder.AddComponent<MeshCollider>().sharedMesh = dynamicColliderMesh[m].mesh;
                        //collider mesh test
                        //                newMeshHolder.AddComponent<MeshFilter>().sharedMesh = dynamicColliderMesh[i].mesh;
                        //                newMeshHolder.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
                        //end test
                    }
                }

                if (track.trackBumpers)
                {
                    dynamicBumperMesh.Build();
                    numberOfMeshes = dynamicBumperMesh.meshCount;
                    for (int m = 0; m < numberOfMeshes; m++)
                    {
                        GameObject newMeshHolder = new GameObject("bumper " + (m + 1));
                        newMeshHolder.transform.parent = curve.holder.transform;
                        newMeshHolder.transform.localPosition = Vector3.zero;
                        newMeshHolder.AddComponent<MeshFilter>().sharedMesh = dynamicBumperMesh[m].mesh;
                        if (track.numberOfTextures > 0)
                            newMeshHolder.AddComponent<MeshRenderer>().material = track.Texture(curve.bumperTextureStyleIndex).material;// track.bumperTexture.material;
#if UNITY_EDITOR
                        EditorUtility.SetSelectedWireframeHidden(newMeshHolder.renderer, !track.showWireframe);
#endif
                    }
                }
            }
        }

        track.TrackRendered();

#if UNITY_EDITOR
        EditorUtility.UnloadUnusedAssets();
#endif

        //        Debug.Log("Generation Complete " + ((Time.realtimeSinceStartup - timeStart) * 1000) + " ms");
    }

    private Vector2 CalculateUV(int i, int pointNumber, float UVy)
    {
        Vector2 returnUV = Vector2.zero;
        if (pointNumber == 2)
        {

            float lerpASize = 1.0f / (pointNumber - 1);
            float lerpValue = lerpASize * i;
            returnUV = (new Vector2(lerpValue, UVy));
        }
        else
        {
            if (i == 0)
            {
                returnUV = (new Vector2(0.0f, UVy));
            }
            else if (i == 1)
            {
                returnUV = (new Vector2(0.333f, UVy));
            }

            else if (i == pointNumber - 2)
            {
                returnUV = (new Vector2(0.666f, UVy));
            }

            else if (i == pointNumber - 1)
            {
                returnUV = (new Vector2(1.0f, UVy));
            }
            else
            {
                returnUV = (new Vector2(0.333f + 0.333f * (1 - (i % 2)), UVy));
            }
        }
        return returnUV;
    }

    private float SignedAngle(Vector3 from, Vector3 to, Vector3 up)
    {
        Vector3 direction = (to - from).normalized;
        Vector3 cross = Vector3.Cross(up, direction);
        float dot = Vector3.Dot(from, cross);
        return Vector3.Angle(from, to) * Mathf.Sign(dot);
    }
}