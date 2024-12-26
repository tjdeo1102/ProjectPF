using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KSD_NoteSpawner : MonoBehaviour
{
    [Header("기본 UI 설정")]
    [SerializeField] private TMP_Dropdown nameDropdown;

    [Header("스폰 설정")]
    [SerializeField] private Transform spawnPosition;

    [Header("현재 상태")]
    public KSD_PerfumeNoteInfo noteInfo;

    private PerfumeNoteName selectedName;

    void Start()
    {
        nameDropdown.ClearOptions();

        nameDropdown.AddOptions(Enum.GetNames(typeof(PerfumeNoteName)).ToList());

        nameDropdown.onValueChanged.AddListener(OnNameDropdownChanged);
    }

    private void OnNameDropdownChanged(int index)
    {
        noteInfo.Name = (PerfumeNoteName)index;
    }

    public void SpawnObject()
    {
        var obj = PhotonNetwork.Instantiate("KSD_Note", spawnPosition.position, Quaternion.identity);
        noteInfo.State = PerfumeNoteState.Note;
        obj.GetComponent<KSD_LiquidObject>().SetInfo(noteInfo);
    }
}
