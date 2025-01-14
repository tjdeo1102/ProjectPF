using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_WhiteBoard : MonoBehaviourPun
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

    [PunRPC]
    public void Pun_UpdateTexture(int x, int y, int width, int height, float[] colorValues)
    {
        Color color = new Color(colorValues[0], colorValues[1], colorValues[2], colorValues[3]);

        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        texture.SetPixels(x, y, width, height, colors);
        texture.Apply();
    }
}
