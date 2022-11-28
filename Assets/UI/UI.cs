using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private Dictionary<Page, UIPage> Pages;

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Pages = new Dictionary<Page, UIPage>()
        {
            {Page.ActionDrawer, new ActionDrawer(root.Q<VisualElement>("ActionDrawer"))},
            {Page.BuildDrawer, new BuildDrawer(root.Q<VisualElement>("BuildDrawer"))},
        };

        ShowPage(Page.ActionDrawer);
    }

    public void ShowPage(Page page)
    {
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
}
