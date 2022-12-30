using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private Dictionary<Page, UIPage> Pages;
    private Dictionary<Hoverer, Stack<UIHoverer>> Hoverers;
    private Stack<Page> History;
    private VisualElement root;
    private List<UIHoverer> LentHoverers = new List<UIHoverer>();

    [SerializeField]
    private VisualTreeAsset BuildConfirmationPrefab;
    [SerializeField]
    private VisualTreeAsset ResourceCollectionIndicatorPrefab;

    void OnEnable()
    {
        History = new Stack<Page>();
        root = GetComponent<UIDocument>().rootVisualElement;

        Pages = new Dictionary<Page, UIPage>()
        {
            {Page.ActionDrawer, new ActionDrawer(root.Q<VisualElement>("ActionDrawer"))},
            {Page.BuildDrawer, new BuildDrawer(root.Q<VisualElement>("BuildDrawer"))},
            {Page.CharacterSelectionDrawer, new CharacterSelectionDrawer(root.Q<VisualElement>("CharacterSelectionDrawer"))},
        };

        Hoverers = new Dictionary<Hoverer, Stack<UIHoverer>>();
        foreach (Hoverer hoverer in Enum.GetValues(typeof(Hoverer)))
        {
            Hoverers[hoverer] = new Stack<UIHoverer>();
        }

        ShowPage(Page.ActionDrawer);
    }

    void Update()
    {
        for (int i = 0; i < LentHoverers.Count; i++)
        {
            LentHoverers[i].Update();
        }
    }

    public void ShowPage(Page page)
    {
        History.Push(page);

        foreach (Page iterPage in Pages.Keys)
        {
            if (iterPage == page)
            {
                Pages[iterPage].Show();
            }
            else
            {
                Pages[iterPage].Hide();
            }
        }
    }

    public UIHoverer ShowHoverer(Hoverer hoverer, Transform target)
    {
        UIHoverer toShow;
        if (Hoverers[hoverer].Count > 0)
        {
            toShow = Hoverers[hoverer].Pop();
            toShow.Show();
        }
        else
        {
            toShow = BuildHoverer(hoverer);
        }
        toShow.SetTarget(target);
        LentHoverers.Add(toShow);
        return toShow;
    }

    public void HideHoverer(UIHoverer hoverer)
    {
        if (hoverer == null)
        {
            return;
        }

        hoverer.Hide();
        Hoverers[hoverer.Type].Push(hoverer);
        LentHoverers.Remove(hoverer);
    }

    private UIHoverer BuildHoverer(Hoverer hoverer)
    {
        switch (hoverer)
        {
            case (Hoverer.BuildConfirmation):
                BuildConfirmation confirmation = new BuildConfirmation();
                root.Add(confirmation);
                return confirmation;
            case (Hoverer.ResourceCollectionIndicator):
                ResourceCollectionIndicator ci = new ResourceCollectionIndicator();
                root.Add(ci);
                return ci;
            case (Hoverer.HealthBar):
                HealthBar hb = new HealthBar();
                root.Add(hb);
                return hb;
            case (Hoverer.ConstructionProgress):
                ConstructionProgress conProg = new ConstructionProgress();
                root.Add(conProg);
                return conProg;
            default:
                throw new System.Exception("Unknown hoverer: " + hoverer);
        }
    }

    public void Back()
    {
        this.History.Pop();
        ShowPage(History.Peek());
    }

    public UIPage GetPage(Page page)
    {
        return Pages[page];
    }
}
