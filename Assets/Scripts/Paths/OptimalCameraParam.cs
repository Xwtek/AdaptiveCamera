using System;
[Serializable]
public struct OptimalCameraParam{
    public float distance;
    public VantageType followVantage;
    public float idealVantage;
}
[Serializable]
public enum VantageType{
    Free,
    Follow,
    Fixed
}