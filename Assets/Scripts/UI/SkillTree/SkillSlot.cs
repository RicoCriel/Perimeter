using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;


public class SkillSlot : MonoBehaviour
{
    public List<SkillSlot> PrerequisiteSkillSlots;
    public Skill Skill;
    public Image SkillIcon;
    public int CurrentLevel;
    public bool IsUnlocked;
    public TMP_Text SkillLevelText;
    public Button SkillButton;

    public static event Action<SkillSlot> OnAbilityPointSpent;
    public static event Action<SkillSlot> OnSkillMaxed;

    private void OnValidate()
    {
        if(Skill != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        SkillIcon.sprite = Skill.SkillIcon;

        if (IsUnlocked)
        {
            SkillButton.interactable = true;
            SkillLevelText.text = $"{CurrentLevel}/{Skill.MaxLevel}";
            SkillIcon.color = Color.white;
        }
        else
        {
            SkillButton.interactable= false;
            SkillLevelText.text = "Locked";
            SkillIcon.color = Color.gray;
        }
    }

    public void TryUpgradeSkill()
    {
        if (IsUnlocked && CurrentLevel < Skill.MaxLevel)
        {
            CurrentLevel++;
            OnAbilityPointSpent?.Invoke(this);

            if(CurrentLevel >= Skill.MaxLevel)
            {
                OnSkillMaxed?.Invoke(this);
            }

            UpdateUI();
        }
    }

    public bool CanUnlockSkill()
    {
        foreach(SkillSlot slot in PrerequisiteSkillSlots)
        {
            if(!slot.IsUnlocked || slot.CurrentLevel < slot.Skill.MaxLevel)
            {
                return false;
            }
        }
        return true;
    }

    public void Unlock()
    {
        IsUnlocked = true;
        UpdateUI();
    }

    
}
