using Photon.Pun;
using Unity.VisualScripting;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

[RequireComponent(typeof(Rigidbody))]
public class LSY_Elevator : MonoBehaviourPun
{
    [Header("엘리베이터 이동 속도")]
    [SerializeField] private float moveSpeed = 1f;
    [Header("엘리베이터 버튼")]
    [SerializeField] private XRPokeFilter buttonFilter;
    [Header("엘리베이터 버튼 누르는 힘")]
    [SerializeField] float pressForce;
    [Header("엘리베이터 최대 높이")]
    [SerializeField] float maxHeight;

    // 치트모드 추가
    [Header("치트 모드 설정")]
    public bool isLiftUp;

    private bool playerIn;

    private Vector3 initialPosition;

    bool isButtonOn = false;

    private IReadOnlyBindableVariable<PokeStateData> upButtonPokeStateData;

    private bool isElevatorMovingUp = false;
    private bool isElevatorMovingDown = false;

    void Start()
    {
        playerIn = false;
        initialPosition = transform.position;

        if (buttonFilter != null)
        {
            upButtonPokeStateData = buttonFilter.pokeStateData;
        }
    }

    private bool isSoundPlaying = false;

    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (playerIn) return;

        if (upButtonPokeStateData.Value.interactionStrength > pressForce || isLiftUp)
        {
            if (transform.position.y > maxHeight)
            {
                if (isSoundPlaying)
                {
                    photonView.RPC("SoundStop", RpcTarget.All, 30);
                    isSoundPlaying = false;
                }
                return;
            }

            if (!isButtonOn)
            {
                photonView.RPC("SoundPlay", RpcTarget.All, 29);
                isButtonOn = true;
            }

            MoveElevator(Vector3.up);
        }
        else
        {
            isButtonOn = false;

            if (transform.position.y <= initialPosition.y + 0.01f)
            {
                if (isSoundPlaying)
                {
                    photonView.RPC("SoundStop", RpcTarget.All, 30);
                    isSoundPlaying = false;
                }
                return;
            }

            MoveElevator(Vector3.down);
        }
    }

    private void MoveElevator(Vector3 direction)
    {
        if (direction == Vector3.up && !isElevatorMovingUp)
        {
            photonView.RPC("SoundPlay", RpcTarget.All, 30);
            isSoundPlaying = true; 
            isElevatorMovingUp = true;
            isElevatorMovingDown = false;
        }
        else if (direction == Vector3.down && !isElevatorMovingDown)
        {
            photonView.RPC("SoundPlay", RpcTarget.All, 30);
            isSoundPlaying = true; 
            isElevatorMovingUp = false;
            isElevatorMovingDown = true;
        }

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    [PunRPC]
    public void SoundPlay(int num)
    {
        Debug.Log("엘리베이터 사운드 시작");
        KSH_AudioManager.Instance.PlaySfx((KSH_AudioManager.Sfx)num);
    }

    [PunRPC]
    public void SoundStop(int num)
    {
        Debug.Log("엘리베이터 사운드 스탑");
        KSH_AudioManager.Instance.StopSfxLoop((KSH_AudioManager.Sfx)num);

        if (num == 30)
        {
            isElevatorMovingUp = false;
            isElevatorMovingDown = false;
            isSoundPlaying = false;
        }
    }
}
