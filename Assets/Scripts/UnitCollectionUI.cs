using System.Collections.Generic;
using UnityEngine;

public class UnitCollectionUI : MonoBehaviour
{
    [SerializeField] private UnitCard[] _unitCards;

    public void Init(List<UnitData> playerUnitDatas, bool isBarrack)
    {
        for(int i = 0; i < _unitCards.Length; i++)
        {
            if(i < playerUnitDatas.Count)
                _unitCards[i].Init(playerUnitDatas[i], isBarrack);
            else
                _unitCards[i].Init(null, isBarrack);
        }
    }

}
