using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSY_OptionPanel : MonoBehaviour
{
    public enum Panel { Information, Sound, Control }
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject informationPanel;
    [SerializeField] GameObject soundPanel;
    [SerializeField] GameObject controlPanel;

    [SerializeField] Button informationButton;
    [SerializeField] Button soundButton;
    [SerializeField] Button controlButton;

    private void Start()
    {
        informationButton.onClick.AddListener(InformationButton);
        soundButton.onClick.AddListener(SoundButton);
        controlButton.onClick.AddListener(ControlButton);
    }

    private void SetActivePanel(Panel panel)
    {
        informationPanel.SetActive(panel == Panel.Information);
        soundPanel.SetActive(panel == Panel.Sound);
        controlPanel.SetActive(panel == Panel.Control);
    }

    public void CloseButton()
    {
        SetActivePanel(Panel.Information);
        optionPanel.SetActive(false);
    }

    public void InformationButton()
    {
        SetActivePanel(Panel.Information);
    }

    public void SoundButton()
    {
        SetActivePanel(Panel.Sound);
    }

    public void ControlButton()
    {
        SetActivePanel(Panel.Control);
    }


}
