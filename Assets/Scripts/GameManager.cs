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
    public int speedTime;

    public int treasury;
    public int otherExpenses;
    public int otherIncomes;
    public int debt;
    [Range(0.0f, 1.0f)]
    public float debtConvertRate;
    public int debtSlowingEffect;

    public List<PublicServicesStats> publicServicesSettings;
    public List<PublicService> publicServices;
    public List<PublicService> privatizedServices;
    public GameObject budgetView;
    public GameObject budgetListView;
    public GameObject prefabBudget;

    public int intervalValueBudget = 50;
    public int plusValueMultiplier = 10;
    public int privateInvestMinValue = 10000;

    public GameObject privatizationView;
    public GameObject privatizationListView;
    public GameObject prefabPrivat;

    public GameObject newsListView;
    public GameObject newsPrefab;

    public GameObject dialogView;
    public List<BuyerSettings> buyersSettings;
    public List<Buyer> buyers;
    Queue<DialogueMessage> calls = new Queue<DialogueMessage>();
    public GameObject phone;
    public Sprite phoneNoCall;
    public Sprite phoneCall;
    public bool calling = false;

    public List<GameObject> panelEnd;
    public int score;
    public Text scoreDisplay;
    public Animator comboPanel;
    public Text comboDisplay1;
    public Text comboDisplay2;
    public int combo = 1;
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


        date = new DateTime(2009,12,12);
        StartCoroutine(DateUpdate());

        treasury = 0;

        publicServices = new List<PublicService>();
        foreach (PublicServicesStats publicService in publicServicesSettings)
        {
            publicServices.Add(new PublicService(publicService.name, publicService.cost, publicService.income,
                publicService.privMalus, publicService.budgetAllowed, publicService.debt, publicService.profitTendency, Instantiate(publicService.newsStory)));
        }

        privatizedServices = new List<PublicService>();

        buyers = new List<Buyer>();
        foreach(BuyerSettings buyer in buyersSettings)
        {
            buyers.Add(new Buyer(buyer.name, buyer.sprite, buyer.publicService1, new List<string>(buyer.sentencesPublicService1), new List<float>(buyer.percentagesSencences1), 
                buyer.publicService2, new List<string>(buyer.sentencesPublicService2), new List<float>(buyer.percentagesSencences2)));
        }

        DateUpdate();
    }

    public void ToogleBudget()
    {
        if (budgetView.activeInHierarchy)
            budgetView.SetActive(false);
        else
            budgetView.SetActive(true);

        AkSoundEngine.PostEvent("Book", gameObject);
        BudgetTextUpdate();
    }

    public void TooglePrivatization()
    {
        if (privatizationView.activeInHierarchy)
            privatizationView.SetActive(false);
        else
            privatizationView.SetActive(true);

        AkSoundEngine.PostEvent("Paper", gameObject);
        PrivatizationUpdateText();
    }

    public void TooglePhone()
    {
        if (dialogView.activeInHierarchy)
            dialogView.SetActive(false);
        else
        {
            AkSoundEngine.PostEvent("Stop_Ring", gameObject);
            dialogView.SetActive(true);
            phone.GetComponent<BoxCollider2D>().enabled = false;
            phone.GetComponent<SpriteRenderer>().sprite = phoneNoCall;
            PhoneUpdate();
            calling = false;
        }

        AkSoundEngine.PostEvent("Phone", gameObject);
        
    }

    public void Privatization (string name)
    {
        for(int i = 0; i < publicServices.Count ; i++)
        {
            if(publicServices[i].Name == name)
            {
                treasury += publicServices[i].PlusValue;
                score += publicServices[i].PlusValue * combo;
                scoreDisplay.text = score.ToString();
                comboDisplay1.text = combo.ToString();
                comboDisplay2.text = combo.ToString();
                comboPanel.SetBool("show", true);
                combo++;
                privatizedServices.Add(publicServices[i]);
                publicServices.RemoveAt(i);
                break;
            }
        }
        PrivatizationUpdateText();
    }

    public void NewsNotif(string title, string text)
    {
        GameObject instance = Instantiate(newsPrefab);
        instance.transform.SetParent(newsListView.transform);
        instance.transform.SetAsFirstSibling();
        instance.transform.Find("Title").GetComponent<Text>().text = title;
        instance.transform.Find("Text").GetComponent<Text>().text = text;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(newsListView.GetComponent<RectTransform>());
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

            if(publicService.OverallBalance > 0)
                instance.transform.Find("WrapperData/FieldValue/BalanceValue").GetComponent<Text>().text = "+" + publicService.OverallBalance.ToString();
            else
                instance.transform.Find("WrapperData/FieldValue/BalanceValue").GetComponent<Text>().text = publicService.OverallBalance.ToString();
            instance.transform.Find("WrapperData/FieldValue/BalanceValue").GetComponent<Text>().color = ColorTextCheck(publicService.OverallBalance);

            instance.transform.Find("WrapperData/FieldValue/DebtValue").GetComponent<Text>().text = publicService.NextMonthDebt.ToString();
            instance.transform.Find("WrapperData/FieldValue/DebtValue").GetComponent<Text>().color = ColorTextCheck(publicService.NextMonthDebt);

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

        if (publicServices.Count == 0)
            budgetView.transform.Find("Panel/None").gameObject.SetActive(true);
        else
            budgetView.transform.Find("Panel/None").gameObject.SetActive(false);

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
        int result = 0;
        foreach (PublicService publicService in publicServices)
        {
            result -= publicService.BudgetAllowed;
        }
        return otherExpenses + otherIncomes + result;
    }

    public int TotalBalance()
    {
        int result = 0;

        foreach(PublicService publicService in publicServices)
        {
            result -= publicService.BudgetAllowed;
        }

        return (result + otherExpenses + otherIncomes + NextMonthDividende);
    }

    public int NextMonthDebt
    {
        get
        {
            int tempDebt = debt;
            int balance = Balance();
            //Pour chaque service publique, on réclame le reste de trésorie
            foreach (PublicService publicService in publicServices)
            {
                balance += publicService.NextMonthDividende;
            }

            int tempTreasury = balance + treasury;
            //Si la trésorie est négative, on l'ajoute à la dette
            if (tempTreasury < 0)
            {
                tempDebt += tempTreasury;
                tempTreasury = 0;
            }
            else
            {
                int debtConvertion = (int)(tempTreasury * debtConvertRate);
                if (tempDebt + debtConvertion > 0)
                {
                    tempTreasury += tempDebt;
                    tempDebt = 0;
                }
                else
                {
                    tempTreasury -= debtConvertion;
                    tempDebt += debtConvertion;
                }
            }

            return tempDebt;
        }
    }

    public int NextMonthDividende
    {
        get
        {
            int dividende = 0;
            foreach (PublicService publicService in publicServices)
            {
                dividende += publicService.NextMonthDividende;
            }

            return dividende;

        }
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
        //Pour chaque service publique, on réclame le reste de trésorie
        foreach (PublicService publicService in publicServices)
        {
            balance += publicService.StateDividendes();
        }

        treasury += balance;
        //Si la trésorie est négative, on l'ajoute à la dette
        if (treasury < 0)
        {
            debt += treasury;
            treasury = 0;
        }
        else
        {
            int debtConvertion = (int)(treasury * debtConvertRate);
            if (debt + debtConvertion > 0)
            {
                treasury += debt;
                debt = 0;
            }
            else
            {
                treasury -= debtConvertion;
                debt += debtConvertion;
            }
        }

        otherExpenses = (int)(otherExpenses * (1 + UnityEngine.Random.Range(-0.1f, 0.1f)));
        otherIncomes = (int)(otherIncomes * (1 + UnityEngine.Random.Range(-0.1f, 0.1f)));
    }

    public void PrivatizationUpdateText()
    {
        Transform[] children = privatizationListView.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != privatizationListView.transform)
                Destroy(children[i].gameObject);
        }

        foreach (PublicService publicService in publicServices)
        {
            GameObject instance = Instantiate(prefabPrivat);
            instance.transform.SetParent(privatizationListView.transform);

            instance.transform.Find("Public Service Name").GetComponent<Text>().text = publicService.Name;
            instance.transform.Find("InfoTab/ValueColumn/BuyDebtValue").GetComponent<Text>().text = "+" + Mathf.Abs(publicService.Debt).ToString();
            instance.transform.Find("InfoTab/ValueColumn/PlusValueValue").GetComponent<Text>().text = "+" + publicService.PlusValue.ToString();
            instance.transform.Find("InfoTab/ValueColumn/SellingPriceValue").GetComponent<Text>().text = "+" + (Mathf.Abs(publicService.Debt) + publicService.PlusValue).ToString();

            instance.transform.Find("PrivatizationConfirm").GetComponent<Button>().onClick.AddListener(() => Privatization(publicService.Name));
        }

        if (publicServices.Count == 0)
            privatizationView.transform.Find("Panel/None").gameObject.SetActive(true);
        else
            privatizationView.transform.Find("Panel/None").gameObject.SetActive(false);


        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(privatizationListView.GetComponent<RectTransform>());
    }

    public void PhoneUpdate()
    {
        if(calls.Count != 0 && !calling)
        {
            DialogueMessage call = calls.Dequeue();
            dialogView.transform.Find("Sprite").GetComponent<Image>().sprite = call.sprite;
            dialogView.transform.Find("Name").GetComponent<Text>().text = call.displayName;
            dialogView.transform.Find("Text").GetComponent<Text>().text = call.text;

            phone.GetComponent<BoxCollider2D>().enabled = true;
            phone.GetComponent<SpriteRenderer>().sprite = phoneCall;
            AkSoundEngine.PostEvent("Start_Ring", gameObject);
            calling = true;
        }
    }

    IEnumerator DateUpdate()
    {
        yield return new WaitForSecondsRealtime(speedTime);
        if(!dialogView.activeInHierarchy)
        {
            date = date.AddMonths(1);
            displayDate.text = date.ToString("y", new CultureInfo("fr-FR"));
            comboPanel.SetBool("show", false);
            combo = 1;
            foreach (PublicService publicService in publicServices)
            {
                publicService.TimeUpdate();
            }

            foreach (PublicService privatizedService in privatizedServices)
            {
                privatizedService.NewsUpdate();
            }

            UpdateOtherBudgetValue();
            BudgetTextUpdate();
            PrivatizationUpdateText();
            foreach(Buyer buyer in buyers)
            {
                for (int i = 0; i < publicServices.Count; i++)
                {
                    if(publicServices[i].Name == buyer.publicService1.name)
                    {
                        for (int y = buyer.sentencesPublicService1.Count -1; y >= 0; y--)
                        {
                            if(buyer.percentagesSencences1[y] >= publicServices[i].Desirability)
                            {
                                calls.Enqueue(new DialogueMessage(buyer.name, buyer.sentencesPublicService1[y], buyer.sprite));
                                buyer.percentagesSencences1.RemoveRange(0,y+1);
                                buyer.sentencesPublicService1.RemoveRange(0, y+1);
                                break;
                            }
                        }
                        break;
                    }
                }
                for (int i = 0; i < publicServices.Count; i++)
                {
                    if (publicServices[i].Name == buyer.publicService2.name)
                    {
                        for (int y = buyer.sentencesPublicService2.Count - 1; y >= 0; y--)
                        {
                            if (buyer.percentagesSencences2[y] >= publicServices[i].Desirability)
                            {
                                calls.Enqueue(new DialogueMessage(buyer.name, buyer.sentencesPublicService2[y], buyer.sprite));
                                buyer.percentagesSencences2.RemoveRange(0, y+1);
                                buyer.sentencesPublicService2.RemoveRange(0, y+1);
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            PhoneUpdate();

            if(publicServices.Count == 0 || DateTime.Compare(date, new DateTime(2015, 12, 12)) > 0)
            {
                EndGame();
            }
        }

        StartCoroutine(DateUpdate());
    }

    public void EndGame()
    {
        if(publicServices.Count == 0)
            panelEnd[0].SetActive(true);
        else if(publicServices.Count > 1)
            panelEnd[1].SetActive(true);
        else
            panelEnd[2].SetActive(true);

    }

    public void CloseGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
