using RollForShoes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RollForShoesDemo : MonoBehaviour
{

    private Character _currentCharacter;
    public TMP_Dropdown _currentCharacterSkills;
    public TMP_DropdownTemplate<Skill> _currentCharacterSkillsWrapper;
    public TMP_Text _currentCharacterExperience;

    private Skill _currentSkill;
    public TMP_Dropdown _currentSkillSpecificSkills;
    public TMP_DropdownTemplate<Skill> _currentSkillSpecificSkillsWrapper;
    public TMP_Text _currentSkillData;

    private Skill _currentSpecificSkill;
    public TMP_Text _currentSpecificSkillData;

    private Roll _currentRoll;
    public TMP_Text _currentRollData;

    private int _opposingRoll;
    public TMP_Text _opposingRollData;

    public void Start()
    {
        _currentCharacterSkillsWrapper = new TMP_DropdownTemplate<Skill>(_currentCharacterSkills);
        _currentCharacterSkillsWrapper.onValueChanged += SetCurrentSkill;

        _currentSkillSpecificSkillsWrapper = new TMP_DropdownTemplate<Skill>(_currentSkillSpecificSkills);
        _currentSkillSpecificSkillsWrapper.onValueChanged += SetCurrentSpecificSkill;

        Skill anything = new Skill("Anything");

        Skill mind = new Skill("Mind");
        Skill body = new Skill("Body");
        Skill soul = new Skill("Soul");

        anything.AddSpecificSkill(mind);
        anything.AddSpecificSkill(body);
        anything.AddSpecificSkill(soul);

        mind.AddSpecificSkill(new Skill("Street Smarts"));
        mind.AddSpecificSkill(new Skill("Book Smarts"));

        body.AddSpecificSkill(new Skill("Strength"));
        body.AddSpecificSkill(new Skill("Dexterity"));

        soul.AddSpecificSkill(new Skill("Magic"));
        soul.AddSpecificSkill(new Skill("Faith"));

        Character character = new Character();
        character.AddSkill(anything);

        SetCharacter(character);
        SetCurrentSkill(null);
        SetCurrentSpecificSkill(null);
        SetCurrentRoll(null);
        SetOpposingRoll(3);
    }

    private void SetCharacter(Character character)
    {
        _currentCharacter = character;
        UpdateCurrentSkills();
        // Set Visual for Character Experience
        _currentCharacterExperience.text = $"XP: {_currentCharacter.Experience}";
    }

    private void SetCurrentSkill(Skill skill)
    {
        if (skill == null)
        {
            _currentSkillData.text = $"Current Skill: Null";
            _currentSkillData.color = Color.red;
        }
        else if (_currentSkill != skill)
        {
            _currentSkill = skill;
            SetCurrentSpecificSkill(null);
            // Set Visual For Skill Data
            _currentSkillData.text = $"Current Skill: {skill.Name} @ lvl {skill.Level}";
            _currentSkillData.color = Color.green;
        }
        UpdateCurrentSpecificSkills();
    }

    private void SetCurrentSpecificSkill(Skill skill)
    {
        if (skill == null)
        {
            _currentSpecificSkillData.text = $"Current Specific Skill: Null";
            _currentSpecificSkillData.color = Color.red;
        }
        else if (_currentSpecificSkill != skill)
        {
            _currentSpecificSkill = skill;
            // Set Visual For Skill Data
            _currentSpecificSkillData.text = $"Current Specific Skill: {skill.Name} @ lvl {skill.Level}";
            _currentSpecificSkillData.color = Color.green;
        }
    }

    private void UpdateCurrentSkills()
    {
        List<TMP_DropdownTemplate<Skill>.OptionTemplateData> options = new List<TMP_DropdownTemplate<Skill>.OptionTemplateData>();
        foreach (Skill skill in _currentCharacter.Skills)
        {
            TMP_DropdownTemplate<Skill>.OptionTemplateData option =
                new TMP_DropdownTemplate<Skill>.OptionTemplateData($"Select Skill: {skill.Name} @ lvl {skill.Level}", skill);
            options.Add(option);
        }
        _currentCharacterSkillsWrapper.AddOptions(options);
    }

    private void UpdateCurrentSpecificSkills()
    {
        List<TMP_DropdownTemplate<Skill>.OptionTemplateData> options = new List<TMP_DropdownTemplate<Skill>.OptionTemplateData>();
        if (_currentSkill != null)
        {
            foreach (Skill specificSkill in _currentSkill.SpecificSkills)
            {
                if (!_currentCharacter.HasSkill(specificSkill))
                {
                    TMP_DropdownTemplate<Skill>.OptionTemplateData option =
                        new TMP_DropdownTemplate<Skill>.OptionTemplateData($"Select Specific Skill: {specificSkill.Name} @ lvl {specificSkill.Level}", specificSkill);
                    options.Add(option);
                }
            }
        }
        _currentSkillSpecificSkillsWrapper.AddOptions(options);
    }

    private void SetCurrentRoll(Roll roll)
    {
        _currentRoll = roll;
        if (roll == null)
        {
            _currentRollData.text = $"Current Roll: Null";
            _currentSpecificSkillData.color = Color.red;
        }
        else
        {
            string result = "Failure";
            _currentRollData.color = Color.red;
            if (roll.IsSuccess())
            {
                _currentRollData.color = Color.green;
                result = "Success";
            }
            // Set Visual For Roll Data
            _currentRollData.text = $"Current Roll: {roll.Total} ({result}) ({new string('S', roll.Successes)}{new string('F', roll.Failures)})";
        }
    }

    private void SetOpposingRoll(int opposingRoll)
    {
        _opposingRoll = opposingRoll;
        // Set Visual For Opposing Roll Data
        _opposingRollData.text = $"Opposing Roll: {opposingRoll}";
    }

    public void OnCharacterAttemptCurrentSkill()
    {
        Roll roll = _currentCharacter.AttemptSkill(_currentSkill, _opposingRoll);
        if (roll != null)
        {
            SetCharacter(_currentCharacter);
            SetCurrentRoll(roll);
        }
    }

    public void OnSpendExperienceOnRoll()
    {
        _currentCharacter.AttemptSpendExperienceOnRoll(_currentRoll, 1);
        SetCharacter(_currentCharacter);
        SetCurrentRoll(_currentRoll);
    }

    public void OnAttemptAdvanceSkill()
    {
        bool success = _currentCharacter.AttemptAdvanceSkill(_currentSkill, _currentSpecificSkill, _currentRoll);
        if (success)
        {
            SetCurrentSkill(null);
            SetCurrentSpecificSkill(null);
            UpdateCurrentSkills();
        }
    }

    public void OnIncrementOpposingRoll()
    {
        _opposingRoll++;
        SetOpposingRoll(_opposingRoll);
    }

    public void OnDecrementOpposingRoll()
    {
        _opposingRoll = Mathf.Max(0, _opposingRoll - 1);
        SetOpposingRoll(_opposingRoll);
    }

    private void Example()
    {
        // The default opposing roll is set
        int opposingRoll = 3;
        // 3 - At start, you have only one skill: Do Anything 1.
        Character character = new Character();
        // There is a skill called "Anything"
        Skill anything = new Skill("Anything");
        // There is a skill called "Something Specific"
        Skill somethingSpecific = new Skill("Something Specific");
        // Doing "Anything" allows you to advance to gain a specific skill called "Something Specific"
        anything.AddSpecificSkill(somethingSpecific);
        character.AddSkill(anything);

        // 1 - Say what you do and roll a number of D6s, determined by the level of relevant skill you have.
        // 5 - For every roll you fail, you get 1 XP.
        Roll roll = character.AttemptSkill(anything, opposingRoll);

        // 2 - If the sum of your roll is higher than an opposing roll, the thing you wanted to happen, happens.
        if (roll.IsSuccess())
        {
            Debug.Log("You did anything!");
        }
        else
        {
            Debug.Log("You failed to do anything.");
        }
        Debug.Log($"You gained `{roll.Failures}` experience for a total of `{character.Experience}`.");

        // 6 - XP can be used to change a die into a 6 for advancement purposes only.
        if (character.HasEnoughExperienceToAdvanceSkill(roll))
        {
            Debug.Log($"You have enough experience to advance your skill, and you do so!");
            Debug.Log($"You choose to make your roll all 6's.");
            character.AttemptSpendExperienceOnRoll(roll, roll.Failures);
        }

        // 4 - If you roll all 6s, you get a new skill specific to the action, one level higher than the one you used.
        character.AttemptAdvanceSkill(anything, somethingSpecific, roll);
    }
}
