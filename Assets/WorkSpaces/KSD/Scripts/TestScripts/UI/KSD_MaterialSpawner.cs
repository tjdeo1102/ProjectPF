using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KSD_MaterialSpawner : MonoBehaviour
{
    [Header("기본 UI 설정")]
    [SerializeField] private TMP_Dropdown typeDropdown;
    [SerializeField] private TMP_Dropdown stateDropdown;
    [SerializeField] private TMP_Dropdown nameDropdown;

    [Header("스폰 설정")]
    [SerializeField] private Transform spawnPosition;

    [Header("현재 상태")]
    public KSD_PerfumeMaterialInfo matInfo;

    private PerfumeMaterialType selectedType;
    private PerfumeMaterialState selectedState;
    private PerfumeMaterialName selectedName;

    void Start()
    {
        typeDropdown.ClearOptions();
        stateDropdown.ClearOptions();
        nameDropdown.ClearOptions();

        typeDropdown.AddOptions(Enum.GetNames(typeof(PerfumeMaterialType)).ToList());
        stateDropdown.AddOptions(Enum.GetNames(typeof(PerfumeMaterialState)).ToList());
        nameDropdown.AddOptions(Enum.GetNames(typeof(PerfumeMaterialName)).ToList());

        typeDropdown.onValueChanged.AddListener(OnTypeDropdownChanged);
        stateDropdown.onValueChanged.AddListener(OnStateDropdownChanged);
        nameDropdown.onValueChanged.AddListener(OnNameDropdownChanged);
    }

    private void OnTypeDropdownChanged(int index)
    {
        matInfo.Type = (PerfumeMaterialType)index;
    }

    private void OnStateDropdownChanged(int index)
    {
        matInfo.State = (PerfumeMaterialState)index;
    }

    private void OnNameDropdownChanged(int index)
    {
        matInfo.Name = (PerfumeMaterialName)index;
    }

    public void SpawnObject()
    {
        var obj = PhotonNetwork.Instantiate("KSD_Material", spawnPosition.position, Quaternion.identity);
        obj.GetComponent<KSD_MaterialObject>().SetInfo(matInfo);
    }
}
