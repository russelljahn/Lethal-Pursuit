/*
* Energy Bar Toolkit by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnergyBarToolkit {

public abstract class EnergyBar3DBase : EnergyBarBase {

#region Contants
    // how much depth values will be reserved for single energy bar
    public const int DepthSpace = 32;
#endregion

#region Fields public
    
    public TextureMode textureMode = TextureMode.Textures;
    
    // used only when texture mode is atlas
    public MadAtlas atlas;
    public AtlasTex[] atlasTexturesBackground;
    public AtlasTex[] atlasTexturesForeground;
    
    // label
    public MadFont labelFont;
    public float labelScale = 32;
    public Pivot labelPivot = Pivot.Center;
    
    // determines if this bar is selectable in the editor
    public bool editorSelectable = true;
    
#endregion
    
#region Fields private

    // created label sprite object
    [SerializeField]
    private MadText labelSprite;
    
    // created background sprite objects
    [SerializeField]
    private List<MadSprite> spriteObjectsBg = new List<MadSprite>();
    
    // created foreground sprite objects
    [SerializeField]
    private List<MadSprite> spriteObjectsFg = new List<MadSprite>();
    
#endregion

#region Properties
    
    protected bool useAtlas {
        get {
            return textureMode == TextureMode.TextureAtlas;
        }
    }
    
    public virtual Pivot pivot {
        get {
            return _pivot;
        }
        
        set {
            _pivot = value;
        }
    }
    [SerializeField]
    private Pivot _pivot = Pivot.Center;
#endregion 
    
    #if UNITY_EDITOR
    void OnDrawGizmos() {
        
        // Draw the gizmo
        Gizmos.matrix = transform.localToWorldMatrix;
        
        Gizmos.color = (UnityEditor.Selection.activeGameObject == gameObject)
            ? Color.green : new Color(1, 1, 1, 0.2f);
        
        var childSprites = MadTransform.FindChildren<MadSprite>(transform);
        Bounds totalBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool totalBoundsSet = false;
        
        foreach (var sprite in childSprites) {
            if (!sprite.CanDraw()) {
                return;
            }
        
            Rect boundsRect = sprite.GetBounds();
            boundsRect = MadMath.Translate(boundsRect, sprite.transform.localPosition);
            Bounds bounds = new Bounds(boundsRect.center, new Vector2(boundsRect.width, boundsRect.height));
            
            if (!totalBoundsSet) {
                totalBounds = bounds;
                totalBoundsSet = true;
            } else {
                totalBounds.Encapsulate(bounds);
            }
        }
        
        
        Gizmos.DrawWireCube(totalBounds.center, totalBounds.size);
        
        if (editorSelectable) {
            // Make the widget selectable
            Gizmos.color = Color.clear;
            Gizmos.DrawCube(totalBounds.center,
                            new Vector3(totalBounds.size.x, totalBounds.size.y, 0.01f * (guiDepth + 1)));
        }
    }
    #endif
    
#region Methods update
    
    protected override void Update() {
        base.Update();

        UpdateLabel();
        UpdateAnchor();
        UpdateColors();
        UpdatePivot();
    }
    
    void UpdateLabel() {
        if (labelSprite == null) {
            return;
        }
        
        labelSprite.scale = labelScale;
        labelSprite.pivotPoint = Translate(labelPivot);
        labelSprite.transform.localPosition = LabelPositionPixels;
        
        labelSprite.text = LabelFormatResolve(labelFormat);
        labelSprite.tint = ComputeColor(labelColor);
    }
    
    void UpdateAnchor() {
        // rewriting anchor objects to make them easy accessible
        var anchor = MadTransform.FindParent<MadAnchor>(transform, 1);
        if (anchor != null) {
            anchorObject = anchor.anchorObject;
            anchorCamera = anchor.anchorCamera;
        }
    }
    
    void UpdateColors() {
        if (textureMode == TextureMode.Textures) {
            UpdateTextureColors(spriteObjectsBg, texturesBackground);
            UpdateTextureColors(spriteObjectsFg, texturesForeground);
        } else {
            UpdateTextureColors(spriteObjectsBg, atlasTexturesBackground);
            UpdateTextureColors(spriteObjectsFg, atlasTexturesForeground);
        }
    }
    
    void UpdateTextureColors(List<MadSprite> sprites, AbstractTex[] textures) {
        for (int i = 0; i < sprites.Count; i++) {
            var sprite = sprites[i];
            var texture = textures[i];
            
            if (sprite == null) {
                continue;
            }
            
            sprite.visible = IsVisible();
            sprite.tint = ComputeColor(texture.color);
        }
    }
    
    void UpdatePivot() {
        var pivot = Translate(this.pivot);
        foreach (var sprite in spriteObjectsBg) {
            if (sprite != null) {
                sprite.pivotPoint = pivot;
            }
        }
        
        foreach (var sprite in spriteObjectsFg) {
            if (sprite != null) {
                sprite.pivotPoint = pivot;
            }
        }
    }
    
#endregion
#region Methods rebuild
    
    protected virtual void Rebuild() {
#if MAD_DEBUG
        Debug.Log("rebuilding " + this, this);
#endif

        if (spriteObjectsBg.Count == 0 && spriteObjectsFg.Count == 0) {
            // in previous version sprites were created without reference in spriteObjects
            // is spriteObjects is empty it's better to assume, that references are not created yet
            // and background objects may exist
            var children = MadTransform.FindChildren<MadSprite>(transform,
                    (s) => s.name.StartsWith("bg_") || s.name.StartsWith("fg_") || s.name == "label",
                    0);
            foreach (var child in children) {
                MadGameObject.SafeDestroy(child.gameObject);
            }
        } else {
            foreach (var sprite in spriteObjectsBg) {
                if (sprite != null) {
                    MadGameObject.SafeDestroy(sprite.gameObject);
                }
            }
            
            foreach (var sprite in spriteObjectsFg) {
                if (sprite != null) {
                    MadGameObject.SafeDestroy(sprite.gameObject);
                }
            }
            
            spriteObjectsBg.Clear();
            spriteObjectsFg.Clear();
        }
    }
    
    protected int BuildBackgroundTextures(int depth) {
        if (useAtlas) {
            return BuildTextures(atlasTexturesBackground, "bg_", depth, ref spriteObjectsBg);
        } else {
            return BuildTextures(texturesBackground, "bg_", depth, ref spriteObjectsBg);
        }
    }
    
    protected int BuildForegroundTextures(int depth) {
        if (useAtlas) {
            return BuildTextures(atlasTexturesForeground, "fg_", depth, ref spriteObjectsFg);
        } else {
            return BuildTextures(texturesForeground, "fg_", depth, ref spriteObjectsFg);
        }
    }
    
    int BuildTextures<T>(T[] textures, string prefix, int startDepth, ref List<MadSprite> sprites) {
        
        int counter = 0;
        foreach (var gTexture in textures) {
            Tex tex = gTexture as Tex;
            AtlasTex atlasTex = gTexture as AtlasTex;
            
            if ((tex != null && !tex.Valid) || (atlasTex != null && !atlasTex.Valid)) {
                continue;
            }
            
            string name = string.Format("{0}{1:D2}", prefix, counter + 1);
            var sprite = MadTransform.CreateChild<MadSprite>(transform, name);
            #if !MAD_DEBUG
            sprite.gameObject.hideFlags = HideFlags.HideInHierarchy;
            #endif
            
            sprite.guiDepth = startDepth + counter;
            
            if (tex != null) {
                sprite.texture = tex.texture;
            } else {
                sprite.inputType = MadSprite.InputType.TextureAtlas;
                sprite.textureAtlas = atlas;
                sprite.textureAtlasSpriteGUID = atlasTex.spriteGUID;
            }
            
            sprite.tint = tex != null ? tex.color : atlasTex.color;;
            
            sprites.Add(sprite);
            
            counter++;
        }
        
        return startDepth + counter;
    }
    
    protected int RebuildLabel(int depth) {
        if (labelSprite != null) {
            MadGameObject.SafeDestroy(labelSprite.gameObject);
        }
        
        if (labelEnabled && labelFont != null) {
            labelSprite = MadTransform.CreateChild<MadText>(transform, "label");
            labelSprite.font = labelFont;
            labelSprite.guiDepth = depth++;
            
            #if !MAD_DEBUG
            labelSprite.gameObject.hideFlags = HideFlags.HideInHierarchy;
            #endif
        }
        
        // after build we must update label at least once to make it visible
        UpdateLabel();
        
        return depth;
    }

#endregion

    #region Private Helper Methods

    /// <summary>
    /// Tells if one of given textures in valid, depending on if this bar is using atlases or not.
    /// </summary>
    protected bool TextureValid(Texture2D tex, string atlasTex) {
        if (useAtlas) {
            return AtlasTextureValid(atlasTex);
        } else {
            return tex != null;
        }
    }

    /// <summary>
    /// Assing regular texture or atlas texture based on current settings.
    /// </summary>
    protected void AssignTexture(MadSprite sprite, Texture2D tex, string atlasTex) {
        if (useAtlas) {
            sprite.inputType = MadSprite.InputType.TextureAtlas;
            sprite.textureAtlas = atlas;
            sprite.textureAtlasSpriteGUID = atlasTex;
            sprite.texture = null;
        } else {
            sprite.inputType = MadSprite.InputType.SingleTexture;
            sprite.textureAtlas = null;
            sprite.lastTextureAtlasSpriteGUID = null;
            sprite.texture = tex;
        }
    }

    /// <summary>
    /// Tells if given texture guid is valid, and it is valid only if atlas is set, and it contains the given texture.
    /// </summary>
    protected bool AtlasTextureValid(string guid) {
        if (atlas == null) {
            return false;
        }

        var item = atlas.GetItem(guid);

        if (item != null) {
            return true;
        } else {
            return false;
        }
    }

    #endregion

    // ===========================================================
    // Static Methods
    // ===========================================================

    protected static MadSprite.PivotPoint Translate(Pivot pivot) {
        switch (pivot) {
            case Pivot.Left:
                return MadSprite.PivotPoint.Left;
            case Pivot.Top:
                return MadSprite.PivotPoint.Top;
            case Pivot.Right:
                return MadSprite.PivotPoint.Right;
            case Pivot.Bottom:
                return MadSprite.PivotPoint.Bottom;
            case Pivot.TopLeft:
                return MadSprite.PivotPoint.TopLeft;
            case Pivot.TopRight:
                return MadSprite.PivotPoint.TopRight;
            case Pivot.BottomRight:
                return MadSprite.PivotPoint.BottomRight;
            case Pivot.BottomLeft:
                return MadSprite.PivotPoint.BottomLeft;
            case Pivot.Center:
                return MadSprite.PivotPoint.Center;
            default:
                Debug.Log("Unknown pivot point: " + pivot);
                return MadSprite.PivotPoint.Center;
        }
    }
    
    protected static Vector2 PivotOffset(Pivot pivot) {
        switch (pivot) {
            case Pivot.Left:
                return new Vector2(0f, -0.5f);
            case Pivot.Top:
                return new Vector2(-0.5f, -1f);
            case Pivot.Right:
                return new Vector2(-1f, -0.5f);
            case Pivot.Bottom:
                return new Vector2(-0.5f, 0f);
            case Pivot.TopLeft:
                return new Vector2(0f, -1f);
            case Pivot.TopRight:
                return new Vector2(-1f, -1f);
            case Pivot.BottomRight:
                return new Vector2(-1f, 0f);
            case Pivot.BottomLeft:
                return new Vector2(0f, 0f);
            case Pivot.Center:
                return new Vector2(-0.5f, -0.5f);
            default:
                Debug.Log("Unknown pivot point: " + pivot);
                return Vector2.zero;
        }
    }
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    // this intentionally shadows base declaration. The other Pivot order is just bad...
    public enum Pivot {
        Left,
        Top,
        Right,
        Bottom,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
        Center,
    }
    
    public enum BarType {
        Filled,
        Repeated,
    }
    
    public enum TextureMode {
        Textures,
        TextureAtlas,
    }
    
    [System.Serializable]
    public class AtlasTex : AbstractTex {
        public string spriteGUID;
        
        public bool Valid {
            get {
                return !string.IsNullOrEmpty(spriteGUID);
            }
        }
        
        public override int GetHashCode() {
            var hash = new MadHashCode();
            hash.Add(spriteGUID);
            hash.Add(color);
            
            return hash.GetHashCode();
        }
    }
}

} // namespace