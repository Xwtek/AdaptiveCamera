using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
[System.Serializable]
public class Benchmarker : MonoBehaviour
{
    private bool tst;
    private bool Test  {
        get => tst;
        set
        {
            tst = value;
            if (!value)
            {
                var ave = new float3(0, 0, 0);
                var avec = new float3(0, 0, 0);
                foreach (var place in places)
                {
                    var y = place/places.Count - avec;
                    var t = ave + y;
                    avec = (t - ave) - y;
                    ave = t;
                }
                foreach (var place in places)
                {
                    distances.Add(math.length(place - ave));
                }
                places.Clear();
                previous = null;
            }
        }
    }
    public string writeTo;
    public List<float> datas = new List<float>();
    private List<float> distances = new List<float>();
    private List<float3> places = new List<float3>();
    public Transform player;
    public Transform cameraPos;
    private Vector3? previous;
    private void FixedUpdate()
    {
        if(!Test) return;
        var relativePos = cameraPos.position - player.position;
        if (previous.HasValue)
        {
            var speed = (relativePos - previous.Value).magnitude / Time.fixedDeltaTime;
            if (speed == double.PositiveInfinity || speed == double.NegativeInfinity) return;
            datas.Add(speed);
            places.Add(relativePos);
        }
        previous = relativePos;

    }
    private void Write(){
        if(!string.IsNullOrEmpty(writeTo)){
            using var file = new StreamWriter(writeTo);
            file.WriteLine("Speed,Deviation");
            foreach(var (data, dist) in datas.Zip(distances, (a, b)=>(a,b))){
                file.Write(data);
                file.Write(", ");
                file.WriteLine(dist);
            }
            file.Flush();
        }
        datas.Clear();
    }
    private void Update(){
        if (Input.GetKeyDown(KeyCode.O))
        {
            Test ^= true;
        }
        if(Input.GetKeyDown(KeyCode.I)){
            Test = false;
            Write();
        }
    }
}