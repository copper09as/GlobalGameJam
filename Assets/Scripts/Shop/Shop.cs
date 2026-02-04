using UnityEngine;
using GameFramework;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
    public Transform contentRoot;     // 枪列表父节点
    public ShopGunItem gunItemPrefab;

    private SessionContext session;
    private List<ShopGunItem> items = new();

    void Start()
    {
        session = GameEntry.Instance.GetSystem<ContextSystem>()
            .GetContext<SessionContext>();

        CreateGunList();
    }

    void CreateGunList()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        items.Clear();

        foreach (var gun in session.gunCollection.Guns)
        {
            var item = Instantiate(gunItemPrefab, contentRoot);
            item.Init(gun, session);
            items.Add(item);
        }
    }

    public void RefreshAll()
    {
        foreach (var item in items)
        {
            item.RefreshUI();
        }
    }
}
