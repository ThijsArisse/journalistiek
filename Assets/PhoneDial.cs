using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneDial : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private List<PhoneDialButton> _dialButtons = new();
    private string _currentDial = "";

    // Start is called before the first frame update
    void Start()
    {
        GetComponentsInChildren<PhoneDialButton>(false, _dialButtons);
        foreach (var button in _dialButtons)
        {
            button.PressedButton += ButtonHandler;
        }
    }

    // Update is called once per frame
    void Update()
    {
        text.text = _currentDial;
    }

    void ButtonHandler(PhoneDialButton.Sign sign, int number)
    {
        switch (sign)
        {
            case PhoneDialButton.Sign.Number:
                print($"number: {number}");
                _currentDial += number.ToString();
                break;
            case PhoneDialButton.Sign.Hashtag:
                print("#");
                break;
            case PhoneDialButton.Sign.Star:
                print("*");
                break;
            case PhoneDialButton.Sign.Call:
                break;
            case PhoneDialButton.Sign.Hangup:
                _currentDial = "";
                return;
                // break;
        }
    }
}
