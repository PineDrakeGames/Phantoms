using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicsButtonPageList : ButtonsPageList<InventoryRelicButton, RelicInstance>
{
    [SerializeField]
    private InventoryUIRelicsTab m_relicTab = null;

    protected override void SetButton(InventoryRelicButton Button, RelicInstance Data)
    {
        Button.Data = Data;
        Button.SetupButton();
    }

    public override void SetPage(int pageNum)
    {
        if (pageNum != m_currentPage)
        {
            m_relicTab.SelectRelic(null);
        }
        base.SetPage(pageNum);
    }
}
