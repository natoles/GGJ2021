using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    // PUBLIC
    public float percent; // Public only for debug purposes 

    // PRIVATE
    private float xmin;
    private float xmax;
    private static Texture2D _staticRectTextureRed;
    private static Texture2D _staticRectTextureGreen;
    private static GUIStyle _staticRectStyleRed;
    private static GUIStyle _staticRectStyleGreen;

 
    // Note that this function is only meant to be called from OnGUI() functions.
    public static void GUIDrawRect( Rect position, Color color )
    {
        if( _staticRectTextureRed == null ) _staticRectTextureRed = new Texture2D( 1, 1 );
        if( _staticRectStyleRed == null ) _staticRectStyleRed = new GUIStyle();
        if( _staticRectTextureGreen == null ) _staticRectTextureGreen = new Texture2D( 1, 1 );
        if( _staticRectStyleGreen == null ) _staticRectStyleGreen = new GUIStyle();
 
        _staticRectTextureRed.SetPixel( 0, 0, color );
        _staticRectTextureRed.Apply();
 
        _staticRectStyleRed.normal.background = _staticRectTextureRed;
 
        GUI.Box( position, GUIContent.none, _staticRectStyleRed );
    }
    public static void GUIInitRect( Rect position )
    {
        if( _staticRectTextureRed == null ) _staticRectTextureRed = new Texture2D( 1, 1 );
        if( _staticRectStyleRed == null ) _staticRectStyleRed = new GUIStyle();
        if( _staticRectTextureGreen == null ) _staticRectTextureGreen = new Texture2D( 1, 1 );
        if( _staticRectStyleGreen == null ) _staticRectStyleGreen = new GUIStyle();
 
        _staticRectTextureRed.SetPixel(0, 0, new Color(255, 0, 0, 255));
        _staticRectTextureRed.Apply();
 
        _staticRectStyleRed.normal.background = _staticRectTextureRed;
 
        GUI.Box( position, GUIContent.none, _staticRectStyleRed );
    }


    public void Show()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void Hide()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    public void SetPercent(float p)
    {
        percent = p;
        UpdateGraphic();
    }


    private void UpdateGraphic()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        percent = 0;    
    }
    
    //
    //// Update is called once per frame
    //void Update()
    //{
    //    
    //}
}
