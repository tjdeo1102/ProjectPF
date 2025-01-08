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

    void Start()
    {
        playerIn = false;
        initialPosition = transform.position;

        if (buttonFilter != null)
        {
            upButtonPokeStateData = buttonFilter.pokeStateData;
        }
    }

    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (playerIn) return;

        if (upButtonPokeStateData.Value.interactionStrength > pressForce)
        {
            if (transform.position.y > maxHeight)
            {
                return; 
            }

            MoveElevator(Vector3.up);
        }
        else
        {
            if (transform.position.y <= initialPosition.y + 0.01f) return; 
            MoveElevator(Vector3.down);
        }
    }

    private void MoveElevator(Vector3 direction)
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("물체 들어옴");
        if (other.gameObject.CompareTag("Player"))
        {
            playerIn = true;
            return;
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (transform.position.y > maxHeight)
    //    {
    //        other.GetComponent<Rigidbody>().useGravity = false;
    //        other.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //    }
    //    else
    //    {
    //        other.GetComponent<Rigidbody>().useGravity = true;
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<Rigidbody>().useGravity = true;

        Debug.Log("물체 나감");
        if (other.gameObject.CompareTag("Player"))
        {
            playerIn = false;
            return;
        }
    }
}
