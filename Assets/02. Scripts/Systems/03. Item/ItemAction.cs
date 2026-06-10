public interface IItemAction
{
    void Excute(MaterialItem item);
    bool IsValid(MaterialItem item);
}

public class UpgradeAnvil : IItemAction
{
    public void Excute(MaterialItem item)
    {
        GameManager.Instance.currentData.shopData.anvilLevel += 1;
    }

    public bool IsValid(MaterialItem item)
    {
        return true;
    }
}

public class UpgradeHammer : IItemAction
{
    public void Excute(MaterialItem item)
    {
        GameManager.Instance.currentData.chanceBonus += 5;
    }

    public bool IsValid(MaterialItem item)
    {
        return true;
    }
}

public class LowGradeAntiDestruction : IItemAction
{
    int index;

    public void Excute(MaterialItem item)
    {
        GameManager.Instance.currentWeapon.Value.IsAntiDestruction = true;
        GameManager.Instance.ShowNotice($"하급 강화 파괴 방지 물약을 사용했습니다.");
    }

    public bool IsValid(MaterialItem item)
    {
        index = GameManager.Instance.currentWeapon.Value.Index;

        if (GameManager.Instance.selectWeaponIndex.Value < 0 || GameManager.Instance.currentWeapon.Value == null)
        {
            GameManager.Instance.ShowNotice($"사용할 아이템이 없습니다.");
            return false;
        }

        if (GameManager.Instance.currentWeapon.Value.IsAntiDestruction == true)
        {
            GameManager.Instance.ShowNotice($"이미 사용되었습니다.");
            return false;
        }

        if (index > 8)
        {
            GameManager.Instance.ShowNotice($"강화 단계에 맞지 않는 아이템 입니다.");
            return false;
        }

        return true;
    }
}

public class MiddleGradeAntiDestruction : IItemAction
{
    int index;

    public void Excute(MaterialItem item)
    {
        GameManager.Instance.currentWeapon.Value.IsAntiDestruction = true;
        GameManager.Instance.ShowNotice($"중급 강화 파괴 방지 물약을 사용했습니다.");
    }

    public bool IsValid(MaterialItem item)
    {
        if (GameManager.Instance.selectWeaponIndex.Value < 0 || GameManager.Instance.currentWeapon.Value == null)
        {
            GameManager.Instance.ShowNotice($"사용할 아이템이 없습니다.");
            return false;
        }

        if (GameManager.Instance.currentWeapon.Value.IsAntiDestruction == true)
        {
            GameManager.Instance.ShowNotice($"이미 사용되었습니다.");
            return false;
        }

        index = GameManager.Instance.currentWeapon.Value.Index;
        if (index < 8 || index > 15)
        {
            GameManager.Instance.ShowNotice($"강화 단계에 맞지 않는 아이템 입니다.");
            return false;
        }

        return true;
    }
}

public class HighGradeAntiDestruction : IItemAction
{
    int index;

    public void Excute(MaterialItem item)
    {
        GameManager.Instance.currentWeapon.Value.IsAntiDestruction = true;
        GameManager.Instance.ShowNotice($"상급 강화 파괴 방지 물약을 사용했습니다.");
    }

    public bool IsValid(MaterialItem item)
    {
        index = GameManager.Instance.currentWeapon.Value.Index;

        if (GameManager.Instance.selectWeaponIndex.Value < 0 || GameManager.Instance.currentWeapon.Value == null)
        {
            GameManager.Instance.ShowNotice($"아이템이 없습니다.");
            return false;
        }

        if (GameManager.Instance.currentWeapon.Value.IsAntiDestruction == true)
        {
            GameManager.Instance.ShowNotice($"이미 사용되었습니다.");
            return false;
        }

        if (index < 15)
        {
            GameManager.Instance.ShowNotice($"강화 단계에 맞지 않는 아이템 입니다.");
            return false;
        }

        return true;
    }
}

public class ProbabilityUp : IItemAction
{
    public void Excute(MaterialItem item)
    {
        GameManager.Instance.currentData.chanceBonus += 5f;
        GameManager.Instance.ShowToast("강화 확률이 5% 상승했습니다.");
    }

    public bool IsValid(MaterialItem item)
    {
        return true;
    }
}
