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

        Hoverers = new Dictionary<Hoverer, Stack<UIHoverer>>()
        {
            {Hoverer.BuildConfirmation, new Stack<UIHoverer>()}
        };

        ShowPage(Page.ActionDrawer);
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
        toShow.Update();
        return toShow;
    }

    private UIHoverer BuildHoverer(Hoverer hoverer)
    {
        switch (hoverer)
        {
            case (Hoverer.BuildConfirmation):
                BuildConfirmation confirmation = new BuildConfirmation();
                root.Add(confirmation);
                return confirmation;
            default:
                throw new System.Exception("Unknown hoverer: " + hoverer);
        }
    }

    public void HideHoverer(UIHoverer hoverer)
    {
        hoverer.Hide();
        Hoverers[hoverer.Type].Push(hoverer);
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
