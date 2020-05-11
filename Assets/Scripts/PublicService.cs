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
    NewsServicePrivateStory newsStory;

    public PublicService(string pName, 
        int baseCost, int baseIncome, int basePrivMalus, 
        int baseBudgetAllowed, int baseDebt, float pProfitTendency, NewsServicePrivateStory pNewsStory)
    {
        name = pName;
        cost = baseCost;
        income = baseIncome;
        budgetAllowed = baseBudgetAllowed;
        privMalus = basePrivMalus;
        debt = baseDebt;
        profitTendency = pProfitTendency;
        newsStory = pNewsStory;
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

    public NewsServicePrivateStory NewsHistory
    {
        get
        {
            return NewsHistory;
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
            return Balance/GameManager._instance.privateInvestMinValue;
        }
    }

    public int PlusValue
    {
        get
        {
            return (int) (Income * (GameManager._instance.plusValueMultiplier * Desirability));
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

    public void NewsUpdate()
    {
        Debug.Log("Checking news for " + name);
        for (int i = 0; i < newsStory.historyTempo.Count; i++)
        {
            if (newsStory.historyTempo[i] != 0)
            {
                Debug.Log("Stopping at the " + i + " news. Count before posting : " + newsStory.historyTempo[i]);
                newsStory.historyTempo[i]--;
                if (newsStory.historyTempo[i] == 0)
                    GameManager._instance.NewsNotif(newsStory.history[i].title, newsStory.history[i].text);
                break;
            }
        }
    }
}
