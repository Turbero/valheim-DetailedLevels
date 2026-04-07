using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DetailedLevels.Features
{
    public class SE_SkillBuff : SE_Stats
    {
        public string skillType;
        public string skillValue;
        
        public override string GetIconText()
        {
            return ConfigurationFile.skillBuffValuePosition.Value == SkillBuffValuePosition.BelowBuffIcon ? skillValue : "";
        }

        public void UpdateBuffText(string value)
        {
            skillValue = value;
            if (ConfigurationFile.skillBuffValuePosition.Value == SkillBuffValuePosition.AboveBuffIcon)
            {
                m_name = skillType + ": "+skillValue;
            }
            else
            {
                m_name = skillType;
                skillValue = value;
            }
        }

        public string Print()
        {
            return skillType + ": " + skillValue;
        }

        public void Refresh()
        {
            UpdateBuffText(skillValue);
        }
    }
}