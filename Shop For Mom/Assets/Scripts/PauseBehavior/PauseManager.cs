/* PauseManager.cs - a centralized point to pause and unpause gameplay.
 * Useful for inventory op's, pause menu, etc. Multiple modules can pause at
 * once. All pause requests must be resumed for underlying game to be resumed.  
 * 
 * Note: this file acts on unity's static Time.timeScale. This affects physics
 * (like movement) and anything that uses deltatime, but not general UI and 
 * input. */
using System;
using System.Collections.Generic;
using UnityEngine;

/* No inheretance. Bc its static, will alw exist while in play mode, but resets
 * on pause. Also bc its static, its just a group of methods and members
 * existing in memory, which can be called by other routines
 * anywhere at any time (no wiring, instantiated when we press play). */
public static class PauseManager
{
    // hash set = dictionary of unique keys. This structure will contain all
    // unique reasons (each an int enum'ed in PauseReasons.cs) of why the game
    // is paused. The game will only resume once all pause reasons are released
    // by the calling program(s)
    private static readonly HashSet<PauseReason> reasons = new HashSet<PauseReason>();
    
    // isPaused will always be assigned a value based on the number of entries
    // in reasons. from what I can tell this is sort of a macro-like syntax.
    public static bool isPaused
    {
        get {return reasons.Count > 0;}
    }
    
     /*  ApplyTimeScale
     *  Purpose: Pause or resume the flow of timein the game
     *  Args: None
     *  return: None
     *  Effects: will pause anything relying on DeltaTime, effectively pausing
     *  movement and gameplay logic, while allowing for non-time-based
     *  functionality
    */
    private static void ApplyTimeScale()
    {
        // well pause by invoking this function, 0f = no time, 1f = normal time
        Time.timeScale = isPaused ? 0f : 1f;
    }

    /*  IsValidReason
     *  Purpose: validate reasons passed to public functions
     *  Args: A pause reason (that a user passed to a public function)
     *  return: bool - is the reason a non-0 enum in PaseReasons.cs
     *  Effects: none
    */
    private static bool IsValidReason(PauseReason reason)
    {
        // our "null" value
        if(reason == PauseReason.None) return false;
        // tells us if the reason is one of the enum-ed values
        return Enum.IsDefined(typeof(PauseReason), reason);
    }

    // called to pause gameplay for a specific reason
    // Note: reasons are listed in PauseReason.cs, we can add more if needed
    public static bool RequestPause(PauseReason reason)
    {
        // if reason entered is not one of our enumed reasons, no pause
        if(!IsValidReason(reason))
        {
            Debug.LogWarning("PauseManager.RequestPause called with invalid reason: " + reason);
            return false;
        }

        reasons.Add(reason);
        ApplyTimeScale();
        return true;
    }

    public static bool ReleasePause(PauseReason reason)
    {
        if (!IsValidReason(reason))
        {
            Debug.LogWarning("PauseManager.ReleasePause called with invalid reason: " + reason);
            return false;
        }

        reasons.Remove(reason);
        ApplyTimeScale();
        return true;
    }

    public static void ClearAllPauses()
    {
        reasons.Clear();
        ApplyTimeScale();
    }

}
