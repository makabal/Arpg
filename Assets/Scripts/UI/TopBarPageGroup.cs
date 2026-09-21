using UnityEngine;

public class TopBarPageGroup : MonoBehaviour
{
    private TopBarPageItem[] pages;

    private void Awake()
    {
        CachePages();
        HideAll();
    }

    public void Show(TopBarPage page)
    {
        CachePages();

        foreach (var item in pages)
        {
            item.gameObject.SetActive(item.Page == page);
        }
    }

    private void CachePages()
    {
        if (pages == null)
        {
            // true：即使 Page 默认是隐藏的，也能找到
            pages = GetComponentsInChildren<TopBarPageItem>(true);
        }
    }

    private void HideAll()
    {
        foreach (var item in pages)
        {
            item.gameObject.SetActive(false);
        }
    }
}
