﻿using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationRegisterGlobalParameters : MonoBehaviour, ILocalizationParamsManager
{
    private void OnEnable()
    {
        if (!LocalizationManager.ParamManagers.Contains(this)) //Register to Manager
        {
            LocalizationManager.ParamManagers.Add(this);
            LocalizationManager.LocalizeAll(true);
        }
    }

    private void OnDisable()
    {
        LocalizationManager.ParamManagers.Remove(this);
    }

    public virtual string GetParameterValue(string parameterName)
    {
        if(parameterName == "VERSION_TAG")
        {
            return "0.1.3";
        }
        if(parameterName == "CURRENT_DAY")
        {
            return DayNightCycle.Instance.GetCurrentDay().ToString();
        }

        return null;
    }
}