using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCard : MonoBehaviour
{
    [SerializeField] private GameObject _unitInfo;
    [SerializeField] private Image _unitIconImage;
    [SerializeField] private TMP_Text _unitTypeText;
    [SerializeField] private TMP_Text _HPText;
    [SerializeField] private TMP_Text _AttackText;
    [SerializeField] private TMP_Text _DefenseText;
    [SerializeField] private TMP_Text _SkillText;
    [SerializeField] private TMP_Text _UpgradeText;
    
    [SerializeField] private GameObject _unitRecruitButton;
    [SerializeField] private TMP_Text _unitRecruitCoinText;

    private int _unitRecruitCoin = 300;

    public void Init(UnitData data, bool isBarrack)
    {
        _unitInfo.SetActive(false);
        _unitRecruitButton.SetActive(false);

        if(data != null)
        {
            _unitInfo.SetActive(true);
            _unitIconImage.sprite = DataManager.Instance.UnitSprites[(int)data.Type];
            _unitTypeText.text = data.Type.ToString();
            _HPText.text = $"{data.CurrentHealth}/{data.MaxHealth}";
            _AttackText.text = $"{data.Attack}";
            _DefenseText.text = $"{data.Defense}";
            _SkillText.text = data.SkillDescription;
            _UpgradeText.text = $"{data.Upgrade}";
        }
        else
        {
            if (isBarrack)
            {
                _unitRecruitButton.SetActive(true);
                _unitRecruitCoinText.text = _unitRecruitCoin.ToString();
            }
        }
    }

}
