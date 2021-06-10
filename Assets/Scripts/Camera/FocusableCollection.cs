using UnityEngine;
using System.Collections.Generic;
using AdaptiveCamera.Data;
using AdaptiveCamera.Util;
using Unity.Mathematics;
[System.Serializable]
public class FocusableCollection : IConstraint
{
    public int cost;
    public Focusable Current { get;
    private set; }

    public List<ConstraintData> Constraints { get; } = new List<ConstraintData>();

    List<Focusable> focusables = new List<Focusable>();
    #region Registration
    public void Register(Focusable focusable)
    {
        if (!focusables.Contains(focusable)) focusables.Add(focusable);
    }
    public void Deregister(Focusable focusable)
    {
        if (focusables.Contains(focusable)) focusables.Remove(focusable);
    }
    #endregion
    #region Focus Algorithm
    public void Focus(Vector3 playerPosition)
    {
        Sort(playerPosition);
        if(focusables.Count>0 &&focusables[0].CanBeFocused){
            Current = focusables[0];
        }else{
            Current = null;
        }
    }
    public void Defocus()
    {
        Current = null;
    }
    public void Prev(Vector3 playerPosition)
    {
        Sort(playerPosition);
        NextByAngle(playerPosition, true);
    }
    public void Next(Vector3 playerPosition)
    {
        Sort(playerPosition);
        NextByAngle(playerPosition, false);
    }
    private void NextByAngle(Vector3 playerPosition, bool clockwise)
    {
        var originalPos = Current.transform.position;
        var originalDir = originalPos - playerPosition;
        var originalAngle = Mathf.Atan2(originalDir.z, originalDir.x);
        float? bestAngle = null;
        Focusable bestFoc = null;
        for (var i = 0; i < focusables.Count; i++)
        {
            if (!focusables[i].CanBeFocused) continue;
            if (focusables[i] == Current) continue;
            var currPos = focusables[i].transform.position;
            var currDir = currPos - playerPosition;
            var currAngle = Mathf.Atan2(currDir.z, currDir.x) - originalAngle;
            while (currAngle < 0) { currAngle += 2 * Mathf.PI; }
            if (currAngle > 2 * Mathf.PI) currAngle -= 2 * Mathf.PI;
            if (clockwise) currAngle = 2 * Mathf.PI - currAngle;
            if (bestAngle == null || bestAngle.Value > currAngle)
            {
                bestAngle = currAngle;
                bestFoc = focusables[i];
            }
        }
        if (!Current.CanBeFocused || bestFoc != null) Current = bestFoc;
    }
    public void Maintain(Vector3 playerPosition)
    {
        Sort(playerPosition);
        if(!Current.CanBeFocused){
            NextByAngle(playerPosition, true);
        }
    }
    #endregion
    private void Sort(Vector3 playerPosition)
    {
        for (var i = 0; i < focusables.Count; i++){
            focusables[i].sortOrder = (focusables[i].transform.position - playerPosition).sqrMagnitude;
        }
        focusables.Sort((a, b)=>{
            switch((a.CanBeFocused, b.CanBeFocused)){
                case (false, false): return 0;
                case (false, true): return 1;
                case (true, false): return -1;
                case (true, true): return (int)Mathf.Sign(a.sortOrder - b.sortOrder);
            }
        });
    }

    public void UpdateConstraints()
    {
        Constraints.Clear();
        if(Current != null){
            Constraints.Add(
                ConstraintUtil.MakeCameraAngleConstraint(
                    cost,
                    ((float3)Current.transform.position - (float3)AdaptiveCameraBrain.Instance.transform.position).xz,
                    math.min(30, NoahController.Instance.MaxRotationPerFrame),
                    ((float3)AdaptiveCameraBrain.Instance.transform.forward).xz
                )
            );
        }
    }
}