using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PhoneDial : MonoBehaviour
{
    [Tooltip("Where to display the numbers to.")]
    [SerializeField] private TMP_Text _textDisplay;
    [Tooltip("Key values for the phone number.")]
    [SerializeField] private List<string> _phoneKeys = new();
    [Tooltip("Event that gets called when its phone number is being called.")]
    [SerializeField] private List<UnityEvent> _phoneValues = new();
    private Dictionary<string, UnityEvent> _phoneNumbers = new();

    //all the buttons under this object
    private List<PhoneDialButton> _dialButtons = new();
    private string _currentDial = "";

    public string CurrentDial
    {
        get => _currentDial;
        set
        {
            _currentDial = value;
            _textDisplay.text = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponentsInChildren<PhoneDialButton>(false, _dialButtons);
        foreach (var button in _dialButtons)
        {
            //couldve done it differently but this works
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
            Debug.LogError("Need as many Values as Keys!", this);
            throw;
        }
    }

    //handles the buttons on their button presses like.. numbers yea
    void ButtonHandler(PhoneDialButton.Sign sign, int number)
    {
        switch (sign)
        {
            case PhoneDialButton.Sign.Number:
                CurrentDial += number.ToString();
                break;
            case PhoneDialButton.Sign.Hashtag:
                break;
            case PhoneDialButton.Sign.Star:
                break;
            case PhoneDialButton.Sign.Call:
                if (_phoneNumbers.TryGetValue(CurrentDial, out UnityEvent unityEvent))
                {
                    unityEvent?.Invoke();
                    CurrentDial = "";
                }
                break;
            case PhoneDialButton.Sign.Hangup:
                CurrentDial = "";
                return;
            // break;
            case PhoneDialButton.Sign.Backspace:
                if (CurrentDial.Length > 0)
                {
                    CurrentDial.Remove(CurrentDial.Length - 1);
                }
                break;
        }
    }
}
