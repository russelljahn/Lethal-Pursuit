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

[System.Serializable]
public class TrackBuildRTexture
{

    public string name = "new trackTexture";
    public bool tiled = true;
    public bool patterned = false;
    public bool flipped = false;
    [SerializeField]
    private Vector2 _tileUnitUV = Vector2.one;//the UV coords of the end of nextNormIndex pattern in the trackTexture - used to match up textures to geometry
    [SerializeField]
    private Vector2 _textureUnitSize = Vector2.one;//the world numberOfPoints of the trackTexture - default 1m x 1m
    public int tiledX = 1;//the amount of times the trackTexture should be repeated along the x axis
    public int tiledY = 1;//the amount of times the trackTexture should be repeated along the y axis
    public Vector2 maxUVTile = Vector2.zero;//used for trackTexture atlasing
    public Vector2 minWorldUnits = Vector2.zero;//also used for atlasing
    public Vector2 maxWorldUnits = Vector2.zero;//also used for atlasing
    public Material material;

    public TrackBuildRTexture(string newName)
    {
        name = newName;
        material = new Material(Shader.Find("Diffuse"));
    }


    public TrackBuildRTexture Duplicate()
    {
        return Duplicate(name + " copy");
    }

    public TrackBuildRTexture Duplicate(string newName)
    {
        TrackBuildRTexture newTexture = new TrackBuildRTexture(newName);

        newTexture.tiled = true;
        newTexture.patterned = false;
        newTexture.tileUnitUV = _tileUnitUV;
        newTexture.textureUnitSize = _textureUnitSize;
        newTexture.tiledX = tiledX;
        newTexture.tiledY = tiledY;
        newTexture.maxUVTile = maxUVTile;
        newTexture.material = new Material(material);

        return newTexture;
    }

    public Texture2D texture
    {
        get
        {
            if (material.mainTexture == null)
                return null;
            return (Texture2D)material.mainTexture;
        }

        set
        {
            if (value == null)
                return;
            material.mainTexture = value;
        }
    }

    public Vector2 tileUnitUV
    {
        get { return _tileUnitUV; }
        set { _tileUnitUV = value; }
    }

    public Vector2 textureUnitSize
    {
        get { return _textureUnitSize; }
        set { _textureUnitSize = value; }
    }

    public void CheckMaxUV(Vector2 checkUV)
    {
        if (checkUV.x > maxUVTile.x)
        {
            maxUVTile.x = checkUV.x;
        }
        if (checkUV.y > maxUVTile.y)
        {
            maxUVTile.y = checkUV.y;
        }
    }

    public void MaxWorldUnitsFromUVs(Vector2 uv)
    {
        float xsize = uv.x * _textureUnitSize.x;
        float ysize = uv.y * _textureUnitSize.y;
        if (xsize > maxWorldUnits.x)
        {
            maxWorldUnits.x = xsize;
        }
        if (ysize > maxWorldUnits.y)
        {
            maxWorldUnits.y = ysize;
        }
    }
}
