/*
* Energy Bar Toolkit by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EnergyBarToolkit;

public abstract class EnergyBarBase : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================
    
    // ===========================================================
    // Fields
    // ===========================================================
    
    [SerializeField]
    protected int version = 169;  // EBT version number to help updating properties
    
    public Tex[] texturesBackground = new Tex[0];
    public Tex[] texturesForeground = new Tex[0];
    
    public int guiDepth = 1;
    
    public GameObject anchorObject;
    public Camera anchorCamera; // camera on which anchor should be visible. if null then Camera.main
    
    // Label
    public bool labelEnabled;
    public GUISkin labelSkin;
    public Vector2 labelPosition;
    public bool labelPositionNormalized = true;
    
    public string labelFormat = "{cur}/{max}";
    public Color labelColor = Color.white;
    
    // smooth effect
    public bool effectSmoothChange = false;          // smooth change value display over time
    public float effectSmoothChangeSpeed = 0.5f;    // value bar width percentage per second

    // burn effect
    public bool effectBurn = false;                 // bar draining will display 'burn' effect
    public Texture2D effectBurnTextureBar;
    public string atlasEffectBurnTextureBarGUID;
    public Color effectBurnTextureBarColor = Color.red;

    // reference to actual bar component    
    protected EnergyBar energyBar;

    protected float ValueFBurn;
    protected float ValueF2;
    
    // ===========================================================
    // Getters / Setters
    // ===========================================================

    public abstract Rect DrawAreaRect { get; }
    
    protected float ValueF {
        get {
            return energyBar.ValueF;
        }
    }
    
    protected Vector2 LabelPositionPixels {
        get {
            var rect = DrawAreaRect;
            Vector2 v;
            if (labelPositionNormalized) {
                v = new Vector2(rect.x + labelPosition.x * rect.width, rect.y + labelPosition.y * rect.height);
            } else {
                v = new Vector2(rect.x + labelPosition.x, rect.y + labelPosition.y);
            }
            
            return v;
        }
    }
    
    
    /// <summary>
    /// Global opacity value.
    /// </summary>
    public float opacity {
        get {
            return _tint.a;
        }
        set {
            _tint.a = Mathf.Clamp01(value);
        }
    }
    
    /// <summary>
    /// Global tint value
    /// </summary>
    public Color tint {
        get {
            return _tint;
        }
        set {
            _tint = value;
        }
    }
    [SerializeField]
    Color _tint = Color.white;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    protected virtual void OnEnable() {
        energyBar = GetComponent<EnergyBar>();
        MadDebug.Assert(energyBar != null, "Cannot access energy bar?!");
    }
    
    protected virtual void OnDisable() {
        // do nothing
    }
    
    protected virtual void Start() {
        // do nothing
    }

    protected virtual void Update() {
        UpdateAnimations();
    }

    void UpdateAnimations() {
        UpdateBarValue();
        UpdateBurnValue();
    }

    void UpdateBurnValue() {
        EnergyBarCommons.SmoothDisplayValue(
                       ref ValueFBurn, ValueF2, effectSmoothChangeSpeed);
        ValueFBurn = Mathf.Max(ValueFBurn, ValueF2);
    }

    void UpdateBarValue() {
        if (effectBurn) {
            if (effectSmoothChange) {
                // in burn mode smooth primary bar only when it's increasing
                if (ValueF > ValueF2) {
                    EnergyBarCommons.SmoothDisplayValue(ref ValueF2, ValueF, effectSmoothChangeSpeed);
                } else {
                    ValueF2 = energyBar.ValueF;
                }
            } else {
                ValueF2 = energyBar.ValueF;
            }

        } else {
            if (effectSmoothChange) {
                EnergyBarCommons.SmoothDisplayValue(ref ValueF2, ValueF, effectSmoothChangeSpeed);
            } else {
                ValueF2 = energyBar.ValueF;
            }
        }
    }
    
    protected bool RepaintPhase() {
        return Event.current.type == EventType.Repaint;
    }
    
    
    protected string LabelFormatResolve(string format) {
        format = format.Replace("{cur}", "" + energyBar.valueCurrent);
        format = format.Replace("{min}", "" + energyBar.valueMin);
        format = format.Replace("{max}", "" + energyBar.valueMax);
        format = format.Replace("{cur%}", string.Format("{0:00}", energyBar.ValueF * 100));
        format = format.Replace("{cur2%}", string.Format("{0:00.0}", energyBar.ValueF * 100));
        
        return format;
    }
    
    protected Vector4 ToVector4(Rect r) {
        return new Vector4(r.xMin, r.yMax, r.xMax, r.yMin);
    }
    
    protected Vector2 Round(Vector2 v) {
        return new Vector2(Mathf.Round(v.x), Mathf.Round (v.y));
    }
    
    protected bool IsVisible() {
        if (anchorObject != null) {
            Camera cam;
            if (anchorCamera != null) {
                cam = anchorCamera;
            } else {
                cam = Camera.main;
            }
            
            Vector3 heading = anchorObject.transform.position - cam.transform.position;
            float dot = Vector3.Dot(heading, cam.transform.forward);
            
            return dot >= 0;
        }
        
        if (opacity == 0) {
            return false;
        }
        
        return true;
    }
    
    protected Color PremultiplyAlpha(Color c) {
        return new Color(c.r * c.a, c.g * c.a, c.b * c.a, c.a);
    }
    
    protected virtual Color ComputeColor(Color localColor) {
        Color outColor =
            new Color(
                localColor.r * tint.r,
                localColor.g * tint.g,
                localColor.b * tint.b,
                localColor.a * tint.a);
    
        return outColor;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================
    
    protected Rect FindBounds(Texture2D texture) {
        
        int left = -1, top = -1, right = -1, bottom = -1;
        bool expanded = false;
        Color32[] pixels;
        try {
            pixels = texture.GetPixels32();
        } catch (UnityException) { // catch not readable
            return new Rect();
        }
            
        int w = texture.width;
        int h = texture.height;
        int x = 0, y = h - 1;
        for (int i = 0; i < pixels.Length; ++i) {
            var c = pixels[i];
            if (c.a != 0) {
                Expand(x, y, ref left, ref top, ref right, ref bottom);
                expanded = true;
            }
            
            if (++x == w) {
                y--;
                x = 0;
            }
        }
        
        
        MadDebug.Assert(expanded, "bar texture has no visible pixels");
        
        var pixelsRect = new Rect(left, top, right - left + 1, bottom - top + 1);
        var normalizedRect = new Rect(
            pixelsRect.xMin / texture.width,
            1 - pixelsRect.yMax / texture.height,
            pixelsRect.xMax / texture.width - pixelsRect.xMin / texture.width,
            1 - pixelsRect.yMin / texture.height - (1 - pixelsRect.yMax / texture.height));
            
        return normalizedRect;
    }
    
    protected void Expand(int x, int y, ref int left, ref int top, ref int right, ref int bottom) {
        if (left == -1) {
            left = right = x;
            top = bottom = y;
        } else {
            if (left > x) {
                left = x;
            } else if (right < x) {
                right = x;
            }
            
            if (top > y) {
                top = y;
            } else if (bottom == -1 || bottom < y) {
                bottom = y;
            }    
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum Pivot {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        Center,
    }
    
    [System.Serializable]
    public class Tex : AbstractTex {
        public virtual int width { get { return texture.width; } }
        public virtual int height { get { return texture.height; } }
        
        public virtual bool Valid {
            get {
                return texture != null;
            }
        }
    
        public Texture2D texture;
        
        public override int GetHashCode() {
            var hash = new MadHashCode();
            hash.Add(texture);
            hash.Add(color);
            
            return hash.GetHashCode();
        }
    }
    
    public class AbstractTex {
        public Color color = Color.black;
    }
    
    public enum GrowDirection {
        LeftToRight,
        RightToLeft,
        BottomToTop,
        TopToBottom,
        RadialCW,
        RadialCCW,
        ExpandHorizontal,
        ExpandVertical,
        ColorChange,
    }
          
    public enum ColorType {
        Solid,
        Gradient,
    }
    
}
