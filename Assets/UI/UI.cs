using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private Dictionary<Page, UIPage> Pages;
    private Stack<Page> History;

    void OnEnable()
    {
        History = new Stack<Page>();
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Pages = new Dictionary<Page, UIPage>()
        {
            {Page.ActionDrawer, new ActionDrawer(root.Q<VisualElement>("ActionDrawer"))},
            {Page.BuildDrawer, new BuildDrawer(root.Q<VisualElement>("BuildDrawer"))},
            {Page.CharacterSelectionDrawer, new CharacterSelectionDrawer(root.Q<VisualElement>("CharacterSelectionDrawer"))},
        };

        ShowPage(Page.ActionDrawer);
    }

    public void ShowPage(Page page)
    {
        Debug.Log("Show page " + page);
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
