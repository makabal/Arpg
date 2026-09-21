using TMPro;
using UnityEngine;

public class TopBarTabView : MonoBehaviour
{
    [SerializeField] private TMP_Text normalText;
    [SerializeField] private GameObject selectedState;
    [SerializeField] private TMP_Text selectedText;

    private void Awake()
    {
        SetSelected(false);
    }

    public void SetText(string text)
    {
        normalText.text = text;
        selectedText.text = text;
    }

    public void SetSelected(bool selected)
    {
        normalText.gameObject.SetActive(!selected);
        selectedState.SetActive(selected);
    }
}