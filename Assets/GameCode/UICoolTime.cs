using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICoolTime : MonoBehaviour
{
    [SerializeField] private bool _isButtonPressed;
    [SerializeField] private Image _selectedImage;
    [SerializeField] private float _coolTime;
    [SerializeField] private int _buttonIndex;
    private float _fillAmountIncrement = 0.01f;

    private UIManager _uiManager;

    #region Property
    public Image SelectedImage
    {
        get { return _selectedImage; }
    }
    #endregion

    void Start()
    {
        _isButtonPressed = false;
        _selectedImage.fillAmount = 1f;
        _uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        if (_isButtonPressed && _selectedImage != null)
        {
            UpdateFillAmount();
        }
    }

    public void OnButtonClick()
    {
        _selectedImage.fillAmount = 0f;
        _isButtonPressed = true;

        if (_uiManager != null)
        {
            _uiManager.SetCoolTimeCheck(_buttonIndex, true);
        }
    }
    public void SetCoolTimeCheck(bool value)
    {
        _isButtonPressed = value;
    }
    private void UpdateFillAmount()
    {
        if (_selectedImage.fillAmount < 1f)
        {
            _selectedImage.fillAmount += _fillAmountIncrement / _coolTime;
        }

        if (_selectedImage.fillAmount >= 1f)
        {
            _isButtonPressed = false;
            if (_uiManager != null && _buttonIndex != -1)
            {
                _uiManager.SetCoolTimeCheck(_buttonIndex, false);
            }
        }
    }
}