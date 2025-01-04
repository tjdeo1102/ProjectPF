using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KSD_PerfumeSpawner : MonoBehaviour
{
    [Header("기본 UI 설정")]
    [SerializeField] private TMP_Dropdown nameDropdown;
    [SerializeField] private TMP_Dropdown typeDropdown;

    [Header("스폰 설정")]
    [SerializeField] private Transform spawnPosition;

    [Header("현재 상태")]

    private PerfumeName selectedName;
    private E_BottleType selectedType;

    void Start()
    {
        nameDropdown.ClearOptions();
        typeDropdown.ClearOptions();

        nameDropdown.AddOptions(Enum.GetNames(typeof(PerfumeName)).ToList());
        typeDropdown.AddOptions(Enum.GetNames(typeof(E_BottleType)).ToList());

        nameDropdown.onValueChanged.AddListener(OnNameDropdownChanged);
        typeDropdown.onValueChanged.AddListener(OnTypeDropdownChanged);
    }

    private void OnNameDropdownChanged(int index)
    {
        selectedName = (PerfumeName)index;
    }

    private void OnTypeDropdownChanged(int index)
    {
        selectedType = (E_BottleType)index;
    }

    public void SpawnObject()
    {
        var obj = PhotonNetwork.Instantiate($"Potion/{Enum.GetName(typeof(E_BottleType),selectedType)}", spawnPosition.position, Quaternion.identity);
        var potion = obj.GetComponent<LSY_PotionReceiver>();
        potion.perfumeName = (E_WGH_PerfumeType)((int)selectedName);
        potion.e_BottleType = selectedType;
        potion.fillAmount = 1f;
    }
}
