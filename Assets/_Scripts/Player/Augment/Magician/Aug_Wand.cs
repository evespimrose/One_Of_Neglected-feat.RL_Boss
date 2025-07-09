using UnityEngine;

public class Aug_Wand : TimeBasedAugment
{
    private GameObject wandtEffectPrefab;

    public Aug_Wand(Player owner, float interval) : base(owner, interval)
    {
        aguName = Enums.AugmentName.Wand;
        wandtEffectPrefab = Resources.Load<GameObject>("Using/Effect/WandEffect");
    }

    protected override void OnTrigger()
    {
        if (wandtEffectPrefab != null)
        {
            SoundManager.Instance.Play("casting", SoundManager.Sound.Effect, 1f, false, 0.5f);
            GameObject startEffect = GameObject.Instantiate(wandtEffectPrefab, owner.transform.position, Quaternion.identity);
            GameObject.Destroy(startEffect, 1f); 
        }

        var skillDispenser = owner.GetComponent<SkillDispenser>();
        skillDispenser.FireAllSkills();
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        switch (level)
        {
            case 1:
                break;
            case 2:
                ModifyBaseInterval(-2f);
                break;
            case 3:
                ModifyBaseInterval(-2f);
                break;
            case 4:
                ModifyBaseInterval(-2f);
                break;
            case 5:
                ModifyBaseInterval(-4f);
                break;
        }
    }
}
