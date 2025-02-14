using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skilltree/Skill")]
public class Skill : ScriptableObject
{
    public string SkillName;
    public int MaxLevel;
    public Sprite SkillIcon;
}
