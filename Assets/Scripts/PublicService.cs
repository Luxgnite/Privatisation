using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicService : MonoBehaviour
{
    string name;
    int cost;
    int income;
    int privMalus;
    float profitTendency;
    int privateInvestMinValue = 10000;

    public PublicService(string pName, int baseCost, int baseIncome, int basePrivMalus, float pProfitTendency)
    {
        name = pName;
        cost = baseCost;
        income = baseIncome;
        privMalus = basePrivMalus;
        profitTendency = pProfitTendency;
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
            return income - cost;

        }
    }

    public float Desirability
    {
        get
        {
            return Balance/privateInvestMinValue;
        }
    }

    public void TimeUpdate()
    {
        cost = (int) (cost * (1f + Random.Range(-0.1f, 0.1f)));
        income = (int)(cost * (1f + profitTendency + Random.Range(-0.1f, 0.1f)));
        privMalus = (int)(privMalus * (1 + (profitTendency / 2)));
    }
}
