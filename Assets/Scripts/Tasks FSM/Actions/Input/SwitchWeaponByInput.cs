﻿using UnityEngine;
using redd096;
using UnityEngine.InputSystem;
using redd096.GameTopDown2D;
using redd096.Attributes;

[AddComponentMenu("redd096/Tasks FSM/Action/Input/Switch Weapon By Input")]
public class SwitchWeaponByInput : ActionTask
{
    enum EDelayMode { Never, OnlyMouseScroll, AlwaysButNumbers, Always }

    [Header("Necessary Components - default get in parent")]
    [SerializeField] WeaponComponent weaponComponent;
    [SerializeField] PlayerInput playerInput;

    [Header("Switch Weapon")]
    [SerializeField] string inputName = "Switch Weapon";
    [SerializeField] string inputSwitchToWeapon1 = "Switch To Weapon 1";
    [SerializeField] string inputSwitchToWeapon2 = "Switch To Weapon 2";
    [SerializeField] bool canSwitchWithMouseScroll = true;

    [Header("Delay Between Switches")]
    [SerializeField] EDelayMode delayMode = EDelayMode.OnlyMouseScroll;
    [DisableIf("delayMode", EDelayMode.Never)] [SerializeField] float delayBetweenSwitches = 0.5f;

    float timeDelay;

    protected override void OnInitTask()
    {
        base.OnInitTask();

        //set references
        if (weaponComponent == null) weaponComponent = GetStateMachineComponent<WeaponComponent>();
        if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

        //show warnings if not found
        if (playerInput && playerInput.actions == null)
            Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
    }

    public override void OnUpdateTask()
    {
        base.OnUpdateTask();

        if (enabled == false || weaponComponent == null || playerInput == null || playerInput.actions == null)
            return;

        //if press, switch to exact weapon
        if (SwitchWithNumbers())
        {
            return;
        }
        //else, try switch with button
        else if (SwitchWithButton())
        {
            return;
        }

        //else, switch with mouse scroll
        SwitchWithMouseScroll();
    }

    #region private API

    bool SwitchWithNumbers()
    {
        //delay works on numbers
        bool delayAffectsSwitch = delayMode == EDelayMode.Always;

        //block switch 'cause delay
        if (delayAffectsSwitch && timeDelay > Time.time)
            return false;

        //if press, switch to exact weapon
        if (playerInput.actions.FindAction(inputSwitchToWeapon1).triggered || playerInput.actions.FindAction(inputSwitchToWeapon2).triggered)
        {
            //switch to number 1 or 2
            if (playerInput.actions.FindAction(inputSwitchToWeapon1).triggered)
            {
                //be sure has no equipped this weapon
                if (weaponComponent.IndexEquippedWeapon != 0)
                    weaponComponent.SwitchWeaponTo(0);
            }
            else
            {
                //be sure has no equipped this weapon
                if (weaponComponent.IndexEquippedWeapon != 1)
                    weaponComponent.SwitchWeaponTo(1);
            }

            //set delay if necessary
            if (delayAffectsSwitch) 
                timeDelay = Time.time + delayBetweenSwitches;

            return true;
        }

        return false;
    }

    bool SwitchWithButton()
    {
        //delay works on button
        bool delayAffectsSwitch = delayMode == EDelayMode.Always || delayMode == EDelayMode.AlwaysButNumbers;

        //block switch 'cause delay
        if (delayAffectsSwitch && timeDelay > Time.time)
            return false;

        //on input down
        if (playerInput.actions.FindAction(inputName).triggered)
        {
            //switch weapon
            weaponComponent.SwitchWeapon();

            //set delay if necessary
            if (delayAffectsSwitch)
                timeDelay = Time.time + delayBetweenSwitches;

            return true;
        }

        return false;
    }

    bool SwitchWithMouseScroll()
    {
        //delay works on mouse scroll
        bool delayAffectsSwitch = delayMode != EDelayMode.Never;

        //block switch 'cause delay
        if (delayAffectsSwitch && timeDelay > Time.time)
            return false;

        //scroll with mouse
        else if (canSwitchWithMouseScroll && Mouse.current != null && Mouse.current.scroll.IsActuated())
        {
            //switch weapon
            weaponComponent.SwitchWeapon(Mouse.current.scroll.ReadValue().y > 0);

            //set delay if necessary
            if (delayAffectsSwitch)
                timeDelay = Time.time + delayBetweenSwitches;

            return true;
        }

        return false;
    }

    #endregion
}
