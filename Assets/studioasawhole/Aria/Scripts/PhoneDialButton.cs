using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PhoneDialButton : MonoBehaviour
{

    // [SerializeField] private Transform _cubeRenderer;
    [SerializeField] private Sign _buttonSign = Sign.Number;
    [Header("only used when button sign is set to number")]
    [Range(0, 9)]
    [SerializeField] private int _buttonNumber = 0;

    public event Action<Sign, int> PressedButton;

    private bool _hover;
    private IXRHoverInteractor _interactor;
    // private Vector3 _cubeStartScale;
    private Vector3 _pokeStartPos;
    // Start is called before the first frame update
    void Start()
    {
        // _cubeStartScale = _cubeRenderer.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (_hover)
        {
            // print(_interactor.transform.position.y);
            // float yDiff = math.abs(_interactor.transform.position.y - _cubeRenderer.position.y);
            // float yDiffOrigin = math.abs(_pokeStartPos.y - _cubeRenderer.position.y);
            // print(yDiff);
            // var scale = _cubeRenderer.localScale;
            // scale.y = yDiff / yDiffOrigin;
            // _cubeRenderer.localScale = scale;
        }
    }

    public void ButtonPressed()
    {
        PressedButton?.Invoke(_buttonSign, _buttonNumber);
    }

    public void SelectExit(SelectExitEventArgs args)
    {
        print("SELECT EXIT");
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        _interactor = args.interactorObject;
        _pokeStartPos = _interactor.transform.position;
        _hover = true;
    }

    public void OnHoverExit(HoverExitEventArgs _)
    {
        _hover = false;
        print("EXIT");
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
