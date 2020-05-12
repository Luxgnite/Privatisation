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

    public int NaturalBalance
    {
        get
        {
            return income + cost;
        }
    }

    public int OverallBalance
    {
        get
        {
            return income + cost + BudgetAllowed;
        }
    }

    public float Desirability
    {
        get
        {
            return ((NaturalBalance * (1 + profitTendency))/GameManager._instance.privateInvestMinValue);
        }
    }

    public int PlusValue
    {
        get
        {
            int plusvalue = (int)(income * profitTendency * GameManager._instance.plusValueMultiplier + (((NaturalBalance < 0 ? 0 : NaturalBalance) * (GameManager._instance.plusValueMultiplier * Desirability))));
            if (plusvalue < 0)
                plusvalue = 0;
            return plusvalue;
        }
    }

    public int NextMonthDebt
    {
        get
        {
            int tempDebt = debt;
            //On ajoute à la trésorie les revenus + budget, et on enlève les coûts.
            int tempTreasury = OverallBalance + treasury;
            //Si la trésorie devient négative, on l'ajoute alors à la dette
            if (tempTreasury < 0)
            {
                tempDebt += tempTreasury;
                tempTreasury = 0;
            }
            //Si la trésorie est positive, on rembourse une partie de la dette
            else
            {
                //Calcul de la part à payer
                int debtPayment = (int)(GameManager._instance.debtConvertRate * tempTreasury);
                //Si la dette est intégralement remboursée avec la somme, on met la dette à 0 et on soustrait la dette de la trésorie
                if (tempDebt + debtPayment > 0)
                {
                    tempTreasury += tempDebt;
                    tempDebt = 0;
                }
                //Sinon, on soustrait de la trésorie la part à payer et on retire de la dette la part payée
                else
                {
                    tempDebt += debtPayment;
                    tempTreasury -= debtPayment;
                }
            }

            return tempDebt;
        }
    }

    public int NextMonthDividende
    {
        get
        {
            int tempDebt = debt;
            //On ajoute à la trésorie les revenus + budget, et on enlève les coûts.
            int tempTreasury = OverallBalance + treasury;
            //Si la trésorie devient négative, on l'ajoute alors à la dette
            if (tempTreasury < 0)
            {
                tempDebt += tempTreasury;
                tempTreasury = 0;
            }
            //Si la trésorie est positive, on rembourse une partie de la dette
            else
            {
                //Calcul de la part à payer
                int debtPayment = (int)(GameManager._instance.debtConvertRate * tempTreasury);
                //Si la dette est intégralement remboursée avec la somme, on met la dette à 0 et on soustrait la dette de la trésorie
                if (tempDebt + debtPayment > 0)
                {
                    tempTreasury += tempDebt;
                    tempDebt = 0;
                }
                //Sinon, on soustrait de la trésorie la part à payer et on retire de la dette la part payée
                else
                {
                    tempDebt += debtPayment;
                    tempTreasury -= debtPayment;
                }
            }

            return tempTreasury;
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
        //On ajoute à la trésorie les revenus + budget, et on enlève les coûts.
        treasury += OverallBalance;
        //Si la trésorie devient négative, on l'ajoute alors à la dette
        if (treasury < 0)
        {
            debt += treasury;
            treasury = 0;
        }
        //Si la trésorie est positive, on rembourse une partie de la dette
        else
        {
            //Calcul de la part à payer
            int debtPayment =(int) (GameManager._instance.debtConvertRate * treasury);
            //Si la dette est intégralement remboursée avec la somme, on met la dette à 0 et on soustrait la dette de la trésorie
            if (debt + debtPayment > 0)
            {
                treasury += debt;
                debt = 0;
            }
            //Sinon, on soustrait de la trésorie la part à payer et on retire de la dette la part payée
            else
            {
                debt += debtPayment;
                treasury -= debtPayment;
            }
        }

        float debtSlow = 0;
        debtSlow =  ((debt / GameManager._instance.debtSlowingEffect) > 1f) ?  0.99f : debt / GameManager._instance.debtSlowingEffect;
        income = (int)(income * (1f + (profitTendency * (1f-debtSlow)) + Random.Range(-0.1f, 0.1f)));
        cost = (int)(cost * (1f + Random.Range(-0.1f, 0.1f)));
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
