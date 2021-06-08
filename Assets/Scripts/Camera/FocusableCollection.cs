using Unity.Collections;
using System.Collections.Generic;
using AdaptiveCamera.Data;
using System;
using UnityEngine;
[Serializable]
public class FocusableCollection
{
    public int cost;
    public Focusable Current => currentIndex == null ? null : focusables[currentIndex.Value];
    private int? _ci;
    private int? currentIndex{
        get => _ci;
        set
        {
            if(Current != null) Current.focused = false;
            _ci = value;
            if(value.HasValue) Current.focused = true;
        }
    }
    [SerializeField]
    List<Focusable> focusables = new List<Focusable>();
    #region Registration
    public void Register(Focusable focusable)
    {
        if(!focusables.Contains(focusable))focusables.Add(focusable);
    }
    public void Deregister(Focusable focusable)
    {
        var index = focusables.IndexOf(focusable);
        if(index == currentIndex) currentIndex = null;
        else if(currentIndex > index) currentIndex--;
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
        currentIndex = 0;
    }
    public void Defocus(){
        currentIndex = null;
    }
    public void Prev(Vector3 playerPosition){
        var candidateCount = Sort(playerPosition);
        if(candidateCount == 0)currentIndex = null;
        else if(currentIndex == 0 )currentIndex = candidateCount - 1;
        else currentIndex--;
    }
    public void Next(Vector3 playerPosition)
    {
        var candidateCount = Sort(playerPosition);
        if(candidateCount == 0) currentIndex = null;
        else if (currentIndex >= candidateCount - 1) currentIndex = 0;
        else currentIndex++;
    }
    #endregion
    // Insertion Sort, as I'd expect that the number of the elements to be sorted is small anyway
    // And the array is mostly almost sorted
    private int Sort(Vector3 playerPosition){
        if(focusables.Count == 0){
            currentIndex = null;
            return 0;
        }
        var unsorted = focusables.Count;
        for (int i = 0; i < unsorted; i++)
        {
            if(!focusables[i].CanBeFocused){
                unsorted--;
                var temp = focusables[i];
                focusables[i] = focusables[unsorted];
                focusables[unsorted] = temp;
                if(i == currentIndex) currentIndex = null;
                i--;
                continue;
            }
            focusables[i].sortOrder = (focusables[i].transform.position - playerPosition).sqrMagnitude;
            int j = i;
            for (;j > 0 && focusables[j].sortOrder < focusables[j - 1].sortOrder; j--)
            {
                if (j - 1 == currentIndex) currentIndex = j;
                var temp = focusables[j - 1];
                focusables[j - 1] = focusables[j];
                focusables[j] = temp;
            }
            if(currentIndex == i) currentIndex = j;
        }
        return unsorted;
    }
    public void Maintain(Vector3 playerPosition){
        var candidateCount = Sort(playerPosition);
        if(candidateCount == 0) currentIndex = null;
        else if (currentIndex >= candidateCount - 1) currentIndex = 0;
    }
}