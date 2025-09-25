using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDialButton : MonoBehaviour
{

    [SerializeField] private MeshRenderer _cubeRenderer;
    [SerializeField] private Sign _buttonSign = Sign.Number;
    [Header("only used when button sign is set to number")]
    [Range(0, 9)]
    [SerializeField] private int _buttonNumber = 0;

    public event Action<Sign, int> PressedButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPressed()
    {
        PressedButton?.Invoke(_buttonSign, _buttonNumber);
    }

    public enum Sign
    {
        Number,
        Hashtag,
        Star,
        Call,
        Hangup,
    }
}
