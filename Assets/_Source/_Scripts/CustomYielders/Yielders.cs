using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Yielders
{

    static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100);

    static readonly WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    public static WaitForEndOfFrame EndOfFrame
    {
        get { return _endOfFrame; }
    }

    static readonly WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
    public static WaitForFixedUpdate FixedUpdate
    {
        get { return _fixedUpdate; }
    }

    public static WaitForSeconds Get(float seconds)
    {
        if (!_timeInterval.ContainsKey(seconds))
            _timeInterval.Add(seconds, new WaitForSeconds(seconds));
        return _timeInterval[seconds];
    }

}