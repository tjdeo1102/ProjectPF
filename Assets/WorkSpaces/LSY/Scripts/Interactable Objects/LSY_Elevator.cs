using Photon.Pun;
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

    private bool playerIn;

    private Vector3 initialPosition;

    private IReadOnlyBindableVariable<PokeStateData> upButtonPokeStateData;

    private Rigidbody rb;

    void Start()
    {
        playerIn = false;
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        if (buttonFilter != null)
        {
            upButtonPokeStateData = buttonFilter.pokeStateData;
        }
    }

    // 최대 높이에 도달했을 때 떨리는 현상 발생

    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (playerIn) return;


        if (upButtonPokeStateData.Value.interactionStrength > pressForce)
        {
            if (transform.position.y > maxHeight) return;

            photonView.RPC("MoveCube", RpcTarget.All, Vector3.up);
        }
        else
        {
            if (transform.position.y < initialPosition.y) return;

            photonView.RPC("MoveCube", RpcTarget.All, Vector3.down);
        }
    }

    [PunRPC]
    private void MoveCube(Vector3 direction)
    {
        rb.velocity = direction * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("물체 들어옴");
        if (other.gameObject.CompareTag("Player"))
        {
            playerIn = true;
            return;
        }

        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("물체 나감");
        if (other.gameObject.CompareTag("Player"))
        {
            playerIn = false;
            return;
        }

        other.transform.SetParent(null);
    }
}
