using Photon.Pun;
using System.Linq;
using UnityEngine;

public class LSY_Marker : MonoBehaviourPun
{
    [SerializeField] Transform tip;
    [SerializeField] int penSize = 5;

    Renderer renderer;
    Color[] colors;
    float tipHeight;

    RaycastHit touch;
    LSY_WhiteBoard whiteBoard;
    Vector2 touchPos;
    bool touchLastFrame;
    Vector2 lastTouchPos;
    Quaternion lastTouchRot;

    bool soundSent = false;

    private void Start()
    {
        renderer = tip.GetComponent<Renderer>();
        colors = Enumerable.Repeat(renderer.material.color, penSize * penSize).ToArray();
        tipHeight = tip.localScale.y;
    }

    private void Update()
    {
        Draw();
    }

    private void Draw()
    {
        Debug.DrawRay(tip.position, transform.up, Color.red, tipHeight);
        if (Physics.Raycast(tip.position, transform.up, out touch, tipHeight))
        {
            if (touch.transform.CompareTag("WhiteBoard"))
            {
                if (whiteBoard == null)
                {
                    whiteBoard = touch.transform.GetComponentInChildren<LSY_WhiteBoard>();
                }

                touchPos = new Vector2(touch.textureCoord.x, touch.textureCoord.y);

                var x = (touchPos.x * whiteBoard.textureSize.x - (penSize / 2));
                var y = (touchPos.y * whiteBoard.textureSize.y - (penSize / 2));

                x = Mathf.Clamp(x, 0, whiteBoard.textureSize.x - penSize);
                y = Mathf.Clamp(y, 0, whiteBoard.textureSize.y - penSize);

                if (y < 0 || whiteBoard.textureSize.y < y || x < 0 || whiteBoard.textureSize.x < x)
                {
                    if (touchLastFrame && soundSent)
                    {
                        photonView.RPC("Sound", RpcTarget.All, false);
                        soundSent = false; 
                    }
                    return;
                }

                if (!touchLastFrame && !soundSent)
                {
                    photonView.RPC("Sound", RpcTarget.All, true);
                    soundSent = true; 
                }

                if (touchLastFrame)
                {
                    whiteBoard.texture.SetPixels((int)x, (int)y, penSize, penSize, colors);

                    for (float f = 0.01f; f < 1.00f; f += 0.03f)
                    {
                        var lerpX = (int)Mathf.Lerp(lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(lastTouchPos.y, y, f);
                        whiteBoard.texture.SetPixels(lerpX, lerpY, penSize, penSize, colors);
                    }

                    transform.rotation = lastTouchRot;

                    whiteBoard.texture.Apply();
                }

                lastTouchPos = new Vector2(x, y);
                lastTouchRot = transform.rotation;
                touchLastFrame = true;

                float[] colorValues = new float[] {
                    renderer.material.color.r,
                    renderer.material.color.g,
                    renderer.material.color.b,
                    renderer.material.color.a
                };
                whiteBoard.photonView.RPC("Pun_UpdateTexture", RpcTarget.All, (int)x, (int)y, penSize, penSize, colorValues);
                return;
            }
            else
            {
                if (touchLastFrame && soundSent)
                {
                    photonView.RPC("Sound", RpcTarget.All, false);
                    soundSent = false;  
                }
            }
        }
        else
        {
            if (touchLastFrame && soundSent)
            {
                photonView.RPC("Sound", RpcTarget.All, false);
                soundSent = false;  
            }
        }

        whiteBoard = null;
        touchLastFrame = false;
    }

    [PunRPC]
    public void Sound(bool on)
    {
        if (on)
        {
            KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Label_sign);
        }
        else
        {
            KSH_AudioManager.Instance.StopSfxLoop(KSH_AudioManager.Sfx.Label_sign);
        }
    }
}
