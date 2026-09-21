using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarTabGroup : MonoBehaviour
{
    [SerializeField] private TopBarPageGroup pageGroup;

    private readonly List<TopBarTabView> tabViews = new();
    private readonly List<TopBarTabItem> tabItems = new();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            var button = child.GetComponent<Button>();
            var view = child.GetComponent<TopBarTabView>();
            var item = child.GetComponent<TopBarTabItem>();

            if (button == null || view == null || item == null)
                continue;

            tabViews.Add(view);
            tabItems.Add(item);

            var currentItem = item;

            button.onClick.AddListener(() =>
            {
                Select(currentItem.Page);
            });
        }
    }

    public void Select(TopBarPage page)
    {
        // 切换顶部按钮选中效果
        for (int i = 0; i < tabViews.Count; i++)
        {
            tabViews[i].SetSelected(tabItems[i].Page == page);
        }

        // 切换对应页面
        pageGroup.Show(page);
    }
}
