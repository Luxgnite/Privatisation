using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicService
{
    string name;
    int cost;
    int income;
    int privMalus;
    int budgetAllowed;
    int debt;
    int treasury;
    float profitTendency;
    int privateInvestMinValue = 10000;

    public PublicService(string pName, 
        int baseCost, int baseIncome, int basePrivMalus, 
        int baseBudgetAllowed, int baseDebt, float pProfitTendency)
    {
        name = pName;
        cost = baseCost;
        income = baseIncome;
        budgetAllowed = baseBudgetAllowed;
        privMalus = basePrivMalus;
        debt = baseDebt;
        profitTendency = pProfitTendency;
    }

    public string Name
    {
        get
        {
            return name;
        }
    }

    public int Cost
    {
        get
        {
            return cost;
        }
    }

    public int Income
    {
        get
        {
            return income;
        }
    }

    public int PrivMalus
    {
        get
        {
            return privMalus;
        }
    }

    public int BudgetAllowed
    {
        get
        {
            return budgetAllowed;
        }
        set
        {
            budgetAllowed = value;
        }
    }

    public int Debt
    {
        get
        {
            return debt;
        }
    }

    public float ProfitTendency
    {
        get
        {
            return profitTendency;
        }
    }

    public int Balance
    {
        get
        {
            return income + cost;
        }
    }

    public float Desirability
    {
        get
        {
            return Balance/privateInvestMinValue;
        }
    }

    public int StateDividendes()
    {
        int temp = treasury;
        treasury = 0;
        return temp;
    }

    public void TimeUpdate()
    {
        cost = (int) (cost * (1f + Random.Range(-0.1f, 0.1f)));
        if ((debt + GameManager._instance.debtConvertRate * (Balance + BudgetAllowed)) >= 0)
        {
            treasury += debt + Balance + BudgetAllowed;
            debt = 0;
        }
        else
        {
            debt = (int)(debt + GameManager._instance.debtConvertRate * (Balance + BudgetAllowed));
            treasury += (int)((1f - GameManager._instance.debtConvertRate) * (Balance + BudgetAllowed));
        }
        float debtSlow = 0;
        debtSlow =  ((debt / GameManager._instance.debtSlowingEffect) > 1f) ?  0.99f : debt / GameManager._instance.debtSlowingEffect;
        income = (int)(income * (1f + (profitTendency * (1f-debtSlow)) + Random.Range(-0.1f, 0.1f)));
        privMalus = (int)(privMalus * (1 + (profitTendency / 2)));
    }
}
