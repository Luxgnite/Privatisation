using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public Text displayDate;
    public DateTime date;
    public int speedTime = 4;

    public int treasury = 0;
    public int otherExpenses = -10000;
    public int otherIncomes = 10000;
    public int debt = -13500;
    [Range(0.0f, 1.0f)]
    public float debtConvertRate = 0.5f;
    public int debtSlowingEffect = 1000000;

    public List<PublicService> publicServices;
    public GameObject budgetView;
    public GameObject budgetListView;
    public GameObject prefabBudget;

    public int intervalValueBudget = 50;

    // Start is called before the first frame update
    void Start()
    {
        //Singleton Pattern
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);


        date = new DateTime(2008,5,1);
        StartCoroutine(DateUpdate());

        treasury = 0;

        publicServices = new List<PublicService>();
        publicServices.Add(new PublicService("SNCF", -100, 100, 1000, 100,-10000, 0.4f));
        publicServices.Add(new PublicService("EDF", -999, 1500, 100, 250, -10000, 0.4f));
        publicServices.Add(new PublicService("Améli", -999, 1500, 100, 250, -10000, 0.4f));
        publicServices.Add(new PublicService("Police", -999, 1500, 100, 250, -10000, 0.4f));
        publicServices.Add(new PublicService("Hopitaux", -999, 1500, 100, 250, -10000, 0.4f));
        publicServices.Add(new PublicService("Pompiers", -999, 1500, 100, 250, -10000, 0.4f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToogleBudget()
    {
        if (budgetView.activeInHierarchy)
            budgetView.SetActive(false);
        else
            budgetView.SetActive(true);

        BudgetTextUpdate();
    }

    public void MinusBudgetValue(string ServiceName)
    {
        foreach (PublicService publicService in publicServices)
        {
            if (publicService.Name == ServiceName)
            {
                if (publicService.BudgetAllowed - intervalValueBudget <= 0)
                    publicService.BudgetAllowed = 0;
                else
                    publicService.BudgetAllowed -= intervalValueBudget;

                Debug.Log(publicService.BudgetAllowed);
                break;
            }
        }

        BudgetTextUpdate();
    }

    public void PlusBudgetValue(string ServiceName)
    {
        foreach(PublicService publicService in publicServices)
        {
            
            if (publicService.Name == ServiceName)
            {
                publicService.BudgetAllowed += intervalValueBudget;
                Debug.Log(publicService.BudgetAllowed);
                break;
            }
        }

        BudgetTextUpdate();
    }

    public void BudgetTextUpdate()
    {
        budgetView.transform.Find("Panel/HeaderWrapper/TitreBudget").GetComponent<Text>().text = "Prévisions du " + displayDate.text;
        Transform[] children = budgetListView.GetComponentsInChildren<Transform>(true);
        for(int i = 0; i < children.Length; i++)
        {
            if(children[i] != budgetListView.transform)
                Destroy(children[i].gameObject);
        }

        foreach(PublicService publicService in publicServices)
        {
            GameObject instance = Instantiate(prefabBudget);
            instance.transform.SetParent(budgetListView.transform);
            instance.transform.Find("Public Service Name").GetComponent<Text>().text = publicService.Name;

            instance.transform.Find("WrapperData/FieldValue/IncomeValue").GetComponent<Text>().text = "+" + publicService.Income.ToString();
            instance.transform.Find("WrapperData/FieldValue/IncomeValue").GetComponent<Text>().color = ColorTextCheck(publicService.Income);

            instance.transform.Find("WrapperData/FieldValue/CostValue").GetComponent<Text>().text = publicService.Cost.ToString();
            instance.transform.Find("WrapperData/FieldValue/CostValue").GetComponent<Text>().color = ColorTextCheck(publicService.Cost);

            instance.transform.Find("WrapperData/FieldValue/BudgetValue").GetComponent<Text>().text = "+" + publicService.BudgetAllowed.ToString();
            instance.transform.Find("WrapperData/FieldValue/BudgetValue").GetComponent<Text>().color = ColorTextCheck(publicService.BudgetAllowed);

            if(publicService.Balance > 0)
                instance.transform.Find("WrapperData/FieldValue/BalanceValue").GetComponent<Text>().text = "+" + publicService.Balance.ToString();
            else
                instance.transform.Find("WrapperData/FieldValue/BalanceValue").GetComponent<Text>().text = "+" + publicService.Balance.ToString();
            instance.transform.Find("WrapperData/FieldValue/BalanceValue").GetComponent<Text>().color = ColorTextCheck(publicService.Balance);

            instance.transform.Find("WrapperData/FieldValue/DebtValue").GetComponent<Text>().text = publicService.Debt.ToString();
            instance.transform.Find("WrapperData/FieldValue/DebtValue").GetComponent<Text>().color = ColorTextCheck(publicService.Debt);

            instance.transform.Find("WrapperData/FieldValue/Minus Budget Button").GetComponent<Button>().onClick.AddListener(() => MinusBudgetValue(publicService.Name));
            instance.transform.Find("WrapperData/FieldValue/Plus Budget Button").GetComponent<Button>().onClick.AddListener(() => PlusBudgetValue(publicService.Name));
        }

        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/OtherExpensesValue").GetComponent<Text>().text = otherExpenses.ToString();
        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/OtherExpensesValue").GetComponent<Text>().color = ColorTextCheck(otherExpenses);

        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/OtherIncomesValue").GetComponent<Text>().text = "+" + otherIncomes ;
        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/OtherIncomesValue").GetComponent<Text>().color = ColorTextCheck(otherIncomes);

        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/TreasuryValue").GetComponent<Text>().text = "+" + treasury;
        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/TreasuryValue").GetComponent<Text>().color = ColorTextCheck(treasury);

        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/TotalBalanceValue").GetComponent<Text>().text = (TotalBalance() >= 0 ? "+" : null) + TotalBalance().ToString() ;
        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/TotalBalanceValue").GetComponent<Text>().color = ColorTextCheck(TotalBalance());

        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/DebtValue").GetComponent<Text>().text = TotalDebt().ToString() ;
        budgetView.transform.Find("Panel/BalanceWrapper/ValuesLayout/DebtValue").GetComponent<Text>().color = ColorTextCheck(TotalDebt());

    }

    private Color ColorTextCheck(float valueToTest)
    {
        if (valueToTest < 0f)
            return Color.red;
        else
            return Color.blue;
    }

    private Color ColorTextCheck (int valueToTest)
    {
        if (valueToTest < 0)
            return Color.red;
        else
            return Color.blue;
    }

    public int Balance()
    {
        return otherExpenses + otherIncomes;
    }

    public int TotalBalance()
    {
        int result = 0;

        foreach(PublicService publicService in publicServices)
        {
            result += publicService.Balance;
            result += -publicService.BudgetAllowed;
        }

        return (result + otherExpenses + otherIncomes + treasury);
    }

    public int TotalDebt()
    {
        int result = 0;

        foreach (PublicService publicService in publicServices)
        {
            result += publicService.Debt;
        }

        return (result + debt);
    }

    public void UpdateOtherBudgetValue()
    {
        int balance = Balance();
        foreach(PublicService publicService in publicServices)
        {
            balance += publicService.StateDividendes();
        }

        if (balance <= 0)
            debt += balance;
        else if((balance * debtConvertRate + debt) >= 0 )
        {
            treasury += balance + debt;
            debt = 0;
        }
        else
        {
            treasury += (int)(balance * (1f - debtConvertRate));
            debt += (int)(balance * debtConvertRate);
        }

        otherExpenses = (int)(otherExpenses * (1 + UnityEngine.Random.Range(-0.1f, 0.1f)));
        otherIncomes = (int)(otherIncomes * (1 + UnityEngine.Random.Range(-0.1f, 0.1f)));
    }

    IEnumerator DateUpdate()
    {
        yield return new WaitForSecondsRealtime(speedTime);

        date = date.AddMonths(1);
        displayDate.text = date.ToString("y",new CultureInfo("fr-FR"));
        foreach(PublicService publicService in publicServices)
        {
            publicService.TimeUpdate();
        }
        UpdateOtherBudgetValue();
        BudgetTextUpdate();

        StartCoroutine(DateUpdate());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
