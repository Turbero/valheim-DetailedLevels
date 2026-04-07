namespace DetailedLevels.Features
{
    public class SE_SkillBuff : SE_Stats
    {
        public Skills.SkillType skillType;
        public string skillValue;
        
        public override string GetIconText()
        {
            return ConfigurationFile.skillBuffValuePosition.Value == SkillBuffValuePosition.Below ? skillValue : "";
        }

        public void UpdateBuffText(string value)
        {
            skillValue = value;
            if (ConfigurationFile.skillBuffValuePosition.Value == SkillBuffValuePosition.Above)
            {
                m_name = $"$skill_{skillType.ToString().ToLower()}" + ": "+skillValue;
            }
            else
            {
                m_name = $"$skill_{skillType.ToString().ToLower()}";
                skillValue = value;
            }
        }

        public string Print()
        {
            return $"$skill_{skillType.ToString().ToLower()}" + ": " + skillValue;
        }

        public void Refresh()
        {
            UpdateBuffText(skillValue);
        }
    }
}