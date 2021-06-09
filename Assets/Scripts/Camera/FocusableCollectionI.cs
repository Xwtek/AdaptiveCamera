/*using Unity.Collections;
using System.Collections.Generic;
using AdaptiveCamera.Data;
using System;
using UnityEngine;
using System.Linq;
[Serializable]
public class FocusableCollection: MonoBehaviour, IConstraint
{
    public int cost;
    public Focusable Current { get; private set; }

    public List<ConstraintData> Constraints { get; } = new List<ConstraintData>();

    private int? currentIndex;
    [SerializeField]
    List<Focusable> focusables = new List<Focusable>();
    #region Registration
    public void Register(Focusable focusable)
    {
        if (!focusables.Contains(focusable)) focusables.Add(focusable);
    }
    public void Deregister(Focusable focusable)
    {
        var index = focusables.IndexOf(focusable);
        if (index == currentIndex) currentIndex = null;
        else if (currentIndex > index) currentIndex--;
        focusables.RemoveAt(index);
    }
    #endregion
    #region Focus Algorithm
    public void Focus(Vector3 playerPosition)
    {
        if (focusables.Count == 0)
        {
            currentIndex = null;
            return;
        }
        Sort(playerPosition);
        ChangeIndex(0);
    }
    public void Defocus()
    {
        currentIndex = null;
    }
    private void ChangeIndex(int? newIndex)
    {
        if (Current != null) Current.focused = false;
        if (newIndex.HasValue)
        {
            currentIndex = newIndex.Value;
            Current = focusables[newIndex.Value];
            Current.focused = true;
        }
        else
        {
            currentIndex = null;
            Current = null;
        }
    }
    public void Prev(Vector3 playerPosition)
    {
        var candidateCount = Sort(playerPosition);
        if (candidateCount == 0) ChangeIndex(null);
        else if (currentIndex == 0) ChangeIndex(candidateCount - 1);
        else currentIndex--;
    }
    public void Next(Vector3 playerPosition)
    {
        var candidateCount = Sort(playerPosition);
        if (candidateCount == 0) ChangeIndex(null);
        else if (currentIndex >= candidateCount - 1) ChangeIndex(0);
        else currentIndex++;
    }
    #endregion
    // Insertion Sort, as I'd expect that the number of the elements to be sorted is small anyway
    // And the array is mostly almost sorted
    private int Sort(Vector3 playerPosition)
    {
        if (focusables.Count == 0)
        {
            currentIndex = null;
            return 0;
        }
        var unsorted = focusables.Count;
        for (int i = 0; i < unsorted; i++)
        {
            if (!focusables[i].CanBeFocused)
            {
                unsorted--;
                var temp = focusables[i];
                focusables[i] = focusables[unsorted];
                focusables[unsorted] = temp;
                i--;
                continue;
            }
            focusables[i].sortOrder = (focusables[i].transform.position - playerPosition).sqrMagnitude;
            int j = i;
            for (; j > 0 && focusables[j].sortOrder < focusables[j - 1].sortOrder; j--)
            {
                var temp = focusables[j - 1];
                focusables[j - 1] = focusables[j];
                focusables[j] = temp;
            }
        }
        bool found = false;
        for (int i = 0; i < focusables.Count; i++)
        {
            if (found) focusables[i].focused = false;
            else if (focusables[i].focused)
            {
                if (i >= unsorted)
                {
                    currentIndex = 0;
                    focusables[i].focused = false;
                    if(unsorted> 0)focusables[0].focused = true;
                    else { currentIndex = null; }
                }
                else currentIndex = i;
                found = true;
                if(currentIndex.HasValue) Current = focusables[currentIndex.Value];
                else Current = null;
            }
        }
        Debug.Log(string.Join(", ", focusables.Where((x) => x.focused).Select((x) => (x as MonoBehaviour).name)));
        return unsorted;
    }
    public void Maintain(Vector3 playerPosition)
    {
        var candidateCount = Sort(playerPosition);
        if (candidateCount == 0) ChangeIndex(null);
        else if (currentIndex >= candidateCount - 1) ChangeIndex(0);
    }

}*/
