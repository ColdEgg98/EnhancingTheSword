using System.Threading.Tasks;
using UnityEngine;

public class NoticeUI : UIBase
{
    public override Transform initializeTarget { get; protected set; }

    public override void Init(string itemName)
    {
        
    }

    public override Task PlayAnimation()
    {
        throw new System.NotImplementedException();
    }

}
