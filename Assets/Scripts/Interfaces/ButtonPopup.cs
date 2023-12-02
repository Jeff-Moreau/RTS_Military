using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPopup : MonoBehaviour
{
    [SerializeField] private Image mPopupBox = null;
    [SerializeField] private TextMeshProUGUI mName = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClicked()
    {
        mPopupBox.gameObject.SetActive(true);
        mName.text = "Place " + this.gameObject.name + "?";
    }
}
