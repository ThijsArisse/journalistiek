using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class PhoneDial : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private List<string> _phoneKeys = new();
    [SerializeField] private List<UnityEvent> _phoneValues = new();
    private Dictionary<string, UnityEvent> _phoneNumbers = new();

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

        try
        {
            for (int i = 0; i < _phoneKeys.Count; i++)
            {
                _phoneNumbers.Add(_phoneKeys[i], _phoneValues[i]);
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.LogError("Need as many Values as Keys!!!!!! pls", this);
            throw;
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
                if (_phoneNumbers.TryGetValue(_currentDial, out UnityEvent unityEvent))
                {
                    unityEvent?.Invoke();
                    _currentDial = "";
                }
                break;
            case PhoneDialButton.Sign.Hangup:
                _currentDial = "";
                return;
            // break;
            case PhoneDialButton.Sign.Backspace:
                if (_currentDial.Length > 0)
                {
                    _currentDial.Remove(_currentDial.Length - 1);
                }
                break;
        }
    }
}
