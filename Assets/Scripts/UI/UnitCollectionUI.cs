using System.Collections.Generic;
using UnityEngine;

public class UnitCollectionUI : MonoBehaviour
{
    [SerializeField] private UnitCard[] _unitCards;

    public void Init(List<UnitData> playerUnitDatas, int type)
    {
        for(int i = 0; i < _unitCards.Length; i++)
        {
            if(i < playerUnitDatas.Count)
                _unitCards[i].Init(playerUnitDatas[i], type);
            else
                _unitCards[i].Init(null, type);
        }
    }

    public void OnClickPrevButton()
    {
        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
    }

}
