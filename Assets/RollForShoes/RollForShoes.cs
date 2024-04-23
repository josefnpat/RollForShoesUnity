using System.Collections.Generic;
using UnityEngine;

namespace RollForShoes
{
    public class Roll
    {
        private int _successes;
        private int _failures;
        private int _total;
        private int _opposingRoll;

        public int Failures { get { return _failures; } }
        public int Successes { get { return _successes; } }
        public int Total { get { return _total; } }

        public Roll(Skill skill, Roll opposingRoll)
        {
            RollRoll(skill, opposingRoll._total);
        }

        public Roll(Skill skill, int opposingRoll)
        {
            RollRoll(skill, opposingRoll);
        }

        private void RollRoll(Skill skill, int opposingRoll)
        {
            _opposingRoll = opposingRoll;
            for (int i = 0; i < skill.Level; i++)
            {
                int roll = Random.Range(1, 7);
                _total += roll;
                if (roll == 6)
                {
                    _successes++;
                }
                else
                {
                    _failures++;
                }
            }
        }

        public bool IsSuccess()
        {
            return _total > _opposingRoll;
        }

        public bool IsAllSuccesses()
        {
            return _failures == 0;
        }

        public bool AttemptSpendExperience(int experience)
        {
            if (experience > _failures)
            {
                Debug.LogWarning($"Insufficient Failures");
                return false;
            }
            _successes += experience;
            _failures -= experience;
            return true;
        }

    }

    public class Character
    {
        private List<Skill> _skills = new List<Skill>();
        private int _experience = 0;

        public List<Skill> Skills { get { return _skills; } }
        public int Experience { get { return _experience; } }

        public void AddSkill(Skill skill)
        {
            _skills.Add(skill);
        }

        public Roll AttemptSkill(Skill skill, int opposingRoll)
        {
            if (skill == null)
            {
                Debug.LogWarning("Null Skill");
                return null;
            }
            Roll roll = new Roll(skill, opposingRoll);
            if (!roll.IsSuccess())
            {
                _experience++;
            }
            return roll;
        }

        public bool HasSkill(Skill skill)
        {
            return _skills.Contains(skill);
        }

        public bool HasEnoughExperienceToAdvanceSkill(Roll roll)
        {
            return _experience >= roll.Failures;
        }

        public bool AttemptSpendExperienceOnRoll(Roll roll, int experienceToSpend)
        {
            if (roll == null)
            {
                Debug.LogWarning("Null Roll");
                return false;
            }
            if (experienceToSpend <= 0 || experienceToSpend > roll.Failures)
            {
                Debug.LogWarning("Invalid Experience To Spend");
                return false;
            }
            if (_experience < experienceToSpend)
            {
                Debug.LogWarning("Insufficient Experience");
                return false;
            }
            roll.AttemptSpendExperience(experienceToSpend);
            _experience -= experienceToSpend;
            return true;
        }

        public bool AttemptAdvanceSkill(Skill skill, Skill specificSkill, Roll roll)
        {
            if (skill == null)
            {
                Debug.LogWarning($"Null Skill");
                return false;
            }
            if (specificSkill == null)
            {
                Debug.LogWarning($"Null Specific Skill");
                return false;
            }
            if (!roll.IsAllSuccesses())
            {
                Debug.LogWarning($"Roll is not All Succeses");
                return false;
            }
            if (_skills.Contains(specificSkill))
            {
                Debug.LogWarning($"Character already has Specific Skill({specificSkill.Name})");
                return false;
            }
            if (!skill.SpecificSkills.Contains(specificSkill))
            {
                Debug.LogWarning($"Skil({skill.Name}) does not allow for Specific Skill({specificSkill.Name})");
                return false;
            }
            _skills.Add(specificSkill);
            return true;
        }

    }

    public class Skill
    {
        private string _name;
        private int _level = 1;
        private List<Skill> _specificSkills = new List<Skill>();

        public string Name { get { return _name; } }
        public int Level { get { return _level; } }
        public List<Skill> SpecificSkills { get { return _specificSkills; } }

        public Skill(string name)
        {
            _name = name;
        }

        public void AddSpecificSkill(Skill specificSkill)
        {
            specificSkill._level = _level + 1;
            _specificSkills.Add(specificSkill);
        }

    }

}