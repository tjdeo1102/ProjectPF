using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_WhiteBoard : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    public Color color = new();

    private void Start()
    {
        var r = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        r.material.mainTexture = texture;   
        ClearTexture();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearTexture();
        }
    }

    private void ClearTexture()
    {
        Color[] resetColors = new Color[texture.width * texture.height];
        for (int i = 0; i < resetColors.Length; i++)
        {
            resetColors[i] = color;
        }
        texture.SetPixels(resetColors);
        texture.Apply();
    }
}
