using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLabelMovable : MapLabel, IUpdatable
{
    private UpdateManager Updater => WorldController.Instance.Updater;
    protected override void EnableAction()
    {
        base.EnableAction();
        Updater.Add(this);

    }
    protected override void DisableAction()
    {
        base.DisableAction();
        Updater.Remove(this);
    }

    public void ManualUpdate()
    {
        if (Time.frameCount % 3 == 0)
        {
            var pos = Chunks.CalculateCurrentChunk(transform.position);
            if (_currentChunk != pos)
            {
                Chunks.ChangeChunk(ref _currentChunk, ref pos, this);
            }
        }
    }
}
