using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpartaBankUI : MonoBehaviour
{
    [SerializeField]
    MoneyInput moneyInput;

    [SerializeField]
    Text DenyText;

    [SerializeField]
    Text playerCashText;
    
    [SerializeField]
    Text playerBalanceText;

    [SerializeField]
    GameObject denyPanelObject;


    private void Awake()
    {
        SpartaBank.instance.OnPlayerMoneyChanged += RefreshPlayerMoney;
        SpartaBank.instance.OnBankingDenied += OnBankingDeny;
    }

    // Start is called before the first frame update
    void Start()
    {
        RefreshPlayerMoney();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshPlayerMoney() 
    {
        playerCashText.text = Player.instance.Cash.ToString("N0");
        playerBalanceText.text = Player.instance.Balance.ToString("N0");
    }

    public void OnClickWithdraw() 
    {
        moneyInput.OnApprove += SpartaBank.instance.Withdraw;
        moneyInput.gameObject.SetActive(true);
    }

    public void OnClickDeposit()
    {
        moneyInput.OnApprove += SpartaBank.instance.Deposit;
        moneyInput.gameObject.SetActive(true);
    }

    public void OnBankingDeny() 
    {
        denyPanelObject.SetActive(true);
    }

    public void OnCancelDenyPanel() 
    {
        denyPanelObject.SetActive(false);
    }
}
