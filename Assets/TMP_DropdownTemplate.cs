using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TMP_DropdownTemplate<T>
{
    private Dictionary<int, T> _dict = new Dictionary<int, T>();
    private TMP_Dropdown _dropdown;
    public UnityAction<T> onValueChanged;

    public class OptionTemplateData
    {
        public TMP_Dropdown.OptionData option;
        public T value;

        public OptionTemplateData(string text, T value)
        {
            this.option = new TMP_Dropdown.OptionData(text);
            this.value = value;
        }
    }

    public TMP_DropdownTemplate(TMP_Dropdown dropdown)
    {
        _dropdown = dropdown;
        _dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    ~TMP_DropdownTemplate()
    {
        _dropdown.onValueChanged.RemoveListener(OnValueChanged);
    }

    public void AddOptions(List<OptionTemplateData> wrappedOptions)
    {
        _dict = new Dictionary<int, T>();
        _dropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();
        dropdownOptions.Add(new TMP_Dropdown.OptionData($"Choose ... ({wrappedOptions.Count})"));
        int i = 1;
        foreach (OptionTemplateData wrappedOption in wrappedOptions)
        {
            _dict[i] = wrappedOption.value;
            dropdownOptions.Add(wrappedOption.option);
            i++;
        }
        _dropdown.AddOptions(dropdownOptions);
    }

    public void SetValue(T value)
    {
        foreach (KeyValuePair<int, T> dictKeyValuePair in _dict)
        {
            if (dictKeyValuePair.Value.Equals(value))
            {
                _dropdown.value = dictKeyValuePair.Key;
            }
        }
    }

    public void SetDefaultValueWithoutNotify()
    {
        _dropdown.SetValueWithoutNotify(0);
        _dropdown.RefreshShownValue();
    }

    public void ClearOptions()
    {
        _dropdown.ClearOptions();
    }

    private void OnValueChanged(int index)
    {
        if (index == 0)
        {
            // nop
        }
        else if (_dict.ContainsKey(index))
        {
            onValueChanged.Invoke(_dict[index]);
            SetDefaultValueWithoutNotify();
        }
        else
        {
            Debug.LogError($"Dropdown's Options are dirty, and has found an untracked index. Use TMPDropdownTemplate.AddOptions instead of TMP_Dropdown.AddOptions.");
        }
        
    }
}
