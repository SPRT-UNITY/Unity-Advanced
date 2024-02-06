using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpartaBank : MonoBehaviour
{
    public static SpartaBank instance;

    public event Action OnBankingDenied;
    public event Action OnPlayerMoneyChanged;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Withdraw(int money) 
    {
        if (Player.instance.Balance - money >= 0 && money >= 0) 
        {
            Player.instance.Balance -= money;
            Player.instance.Cash += money;
            OnPlayerMoneyChanged.Invoke();
        }
        else 
        {
            OnBankingDenied.Invoke();
        }
    }

    public void Deposit(int money) 
    {
        if (Player.instance.Cash - money >= 0 && money >= 0)
        {
            Player.instance.Balance += money;
            Player.instance.Cash -= money;
            OnPlayerMoneyChanged.Invoke();
        }
        else
        {
            OnBankingDenied.Invoke();
        }
    }
}
