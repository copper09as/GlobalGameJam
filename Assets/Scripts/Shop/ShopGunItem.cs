using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using TMPro;

public class ShopGunItem : MonoBehaviour
{
    public TextMeshProUGUI gunNameText;
    public Button actionButton;
    public TextMeshProUGUI buttonText;

    private Gun gun;
    private SessionContext session;

    public void Init(Gun gun, SessionContext session)
    {
        this.gun = gun;
        this.session = session;

        gunNameText.text = gun.GunName;
        RefreshUI();

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnClick);
    }

    public void RefreshUI()
    {
        bool owned = session.LocalPlayerData.OwnedGun.Contains(gun.Id);
        bool selected = session.localGun == gun;

        if (!owned)
        {
            buttonText.text = $"Buy ({gun.Price})";
            actionButton.interactable = session.LocalPlayerData.Gold >= gun.Price;
        }
        else if (!selected)
        {
            buttonText.text = "Select";
            actionButton.interactable = true;
        }
        else
        {
            buttonText.text = "Selected";
            actionButton.interactable = false;
        }
    }

    void OnClick()
    {
        bool owned = session.LocalPlayerData.OwnedGun.Contains(gun.Id);

        if (!owned)
        {
            BuyGun();
        }
        else
        {
            SelectGun();
        }
    }

    void BuyGun()
    {
        if (session.LocalPlayerData.Gold < gun.Price) return;

        session.LocalPlayerData.Gold -= gun.Price;
        session.LocalPlayerData.OwnedGun.Add(gun.Id);
        SelectGun();
    }

    void SelectGun()
    {
        session.localGun = gun;
        FindObjectOfType<Shop>().RefreshAll();
    }
}
