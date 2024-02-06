using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyInput : MonoBehaviour
{
    [SerializeField]
    private int internalValue;
    public int value { get { return internalValue; } 
        private set { internalValue = value; moneyInputField.text = internalValue.ToString("N0"); } }


    [SerializeField]
    InputField moneyInputField;

    [SerializeField]
    public event Action<int> OnApprove;


    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddInputMoney(int money) 
    {
        value += money;
    }

    public void OnInputFieldChanged() 
    {
        internalValue = decimal.ToInt32(decimal.Parse(moneyInputField.text));
        moneyInputField.text = internalValue.ToString("N0");
    }

    public void OnApproveButtonClick() 
    {
        OnApprove.Invoke(internalValue);
    }

    public void OnClickCancel() 
    {
        gameObject.SetActive(false);
        OnApprove = null;
        value = 0;
    }
}
