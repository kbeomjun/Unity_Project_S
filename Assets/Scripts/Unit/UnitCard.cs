using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCard : MonoBehaviour
{
    [SerializeField] private GameObject _unitInfo;
    [SerializeField] private Image _unitIconImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private GameObject _nameUpdateButton;
    [SerializeField] private GameObject _nameAgreeButton;
    [SerializeField] private TMP_Text _unitTypeText;
    [SerializeField] private TMP_Text _HPText;
    [SerializeField] private TMP_Text _AttackText;
    [SerializeField] private TMP_Text _DefenseText;
    [SerializeField] private TMP_Text _SkillText;
    [SerializeField] private TMP_Text _UpgradeText;

    [SerializeField] private GameObject _unitBarrackButtons;
    [SerializeField] private TMP_Text _upgradeCoinText;
    [SerializeField] private TMP_Text _sellCoinText;

    [SerializeField] private GameObject _unitRecruitButton;
    [SerializeField] private TMP_Text _unitRecruitCoinText;

    [SerializeField] private GameObject _unitCureButton;

    private UnitData _unitData = null;
    private int _type = 0;
    private int _unitUpgradeCoin = 0;
    private int _unitRecruitCoin = 300;
    private int _unitSellCoin = 0;

    public void Init(UnitData data, int type)
    {
        _unitData = data;
        _type = type;

        _unitInfo.SetActive(false);
        _nameInput.gameObject.SetActive(false);
        _nameUpdateButton.SetActive(false);
        _nameAgreeButton.SetActive(false);
        _unitBarrackButtons.SetActive(false);
        _unitRecruitButton.SetActive(false);
        _unitCureButton.SetActive(false);

        if(data != null)
        {
            if(type == 0)
            {
                _nameUpdateButton.SetActive(true);
            }
            else if (type == 1)
            {
                _unitBarrackButtons.SetActive(true);
                _upgradeCoinText.text = data.UpgradeCoin.ToString();
                _sellCoinText.text = data.SellCoin.ToString();
                _unitUpgradeCoin = data.UpgradeCoin;
                _unitSellCoin = data.SellCoin;
            }
            else if (type == 2)
            {
                _unitCureButton.SetActive(true);
            }

            _unitInfo.SetActive(true);
            _unitIconImage.sprite = DataManager.Instance.UnitSprites[(int)data.Type];
            _nameText.text = data.Name;
            _unitTypeText.text = data.Type.ToString();
            _SkillText.text = data.SkillDescription;
            if(data.Upgrade > 0)
            {
                _HPText.text = $"{data.CurrentHealth}/<color=green>{data.MaxHealth}</color>";
                _AttackText.text = $"<color=green>{data.Attack}</color>";
                _DefenseText.text = $"<color=green>{data.Defense}</color>";
                _UpgradeText.text = $"<color=green>{data.Upgrade}</color>";
            }
            else
            {
                _HPText.text = $"{data.CurrentHealth}/{data.MaxHealth}";
                _AttackText.text = $"{data.Attack}";
                _DefenseText.text = $"{data.Defense}";
                _UpgradeText.text = $"{data.Upgrade}";
            }
        }
        else
        {
            if (type == 1)
            {
                _unitRecruitButton.SetActive(true);
                _unitRecruitCoinText.text = _unitRecruitCoin.ToString();
            }
        }
    }

    public void OnClickNameUpdateButton()
    {
        _nameInput.text = _unitData.Name;
        _nameInput.gameObject.SetActive(true);
        _nameUpdateButton.SetActive(false);
        _nameAgreeButton.SetActive(true);
        SoundManager.Instance.PlayRenameSound();
    }

    public void OnClickAgreeButton()
    {
        string newName = _nameInput.text;
        if (newName.Length > 11)
        {
            newName = newName.Substring(0, 11);
        }
        if (!string.IsNullOrEmpty(newName))
        {
            _unitData.Name = newName;
            GameManager.Instance.PlayerUnitDatas[_unitData.Index].Name = newName;
            _nameText.text = newName;
        }
        _nameInput.gameObject.SetActive(false);
        _nameUpdateButton.SetActive(true);
        _nameAgreeButton.SetActive(false);
        SoundManager.Instance.PlayRenameSound();
    }

    public void OnClickUpgradeButton()
    {
        if(_unitUpgradeCoin <= GameManager.Instance.CurrentCoin)
        {
            GameManager.Instance.PlayerUnitDatas[_unitData.Index].UpgradeUnit();
            GameManager.Instance.CurrentCoin -= _unitUpgradeCoin;
            GameManager.Instance.UnitCollectionUI.Init(GameManager.Instance.PlayerUnitDatas, _type);
        }
    }

    public void OnClickSellButton()
    {
        if(GameManager.Instance.PlayerUnitDatas.Count > 1)
        {
            GameManager.Instance.RemoveUnit(_unitData.Index);
            GameManager.Instance.CurrentCoin += _unitSellCoin;
            GameManager.Instance.UnitCollectionUI.Init(GameManager.Instance.PlayerUnitDatas, _type);
        }
    }

    public void OnClickRecruitButton()
    {
        if(_unitRecruitCoin <= GameManager.Instance.CurrentCoin)
        {
            int unitType = Random.Range(0, DataManager.Instance.UnitDatas.Length);
            GameManager.Instance.AddUnit(unitType);
            GameManager.Instance.CurrentCoin -= _unitRecruitCoin;
            GameManager.Instance.UnitCollectionUI.Init(GameManager.Instance.PlayerUnitDatas, _type);
        }
    }

    public void OnClickCureButton()
    {
        GameManager.Instance.CureUnit(_unitData.Index);
        TownRestManager.Instance.OnCureUnit();
        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
    }

}
