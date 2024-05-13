using System;
using System.Collections.Generic;
using AR;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DestinationNotify : MonoBehaviour
{
    public DataManager dataManager;
    public GameObject arrival_popup;
    public bool isFirst = false;

    // 도착지 위도 경도를 저장할 리스트
    private List<double> lats = new List<double>();
    private List<double> longs = new List<double>();

    private void Start()
    {
        dataManager = DataManager.Instance;

        if (dataManager == null)
        {
            Debug.LogError("DataManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            double myLat = Input.location.lastData.latitude;
            double myLong = Input.location.lastData.longitude;

            double remainDistance = distance(myLat, myLong, lats.Count > 0 ? lats[0] : 0, longs.Count > 0 ? longs[0] : 0);

            if (remainDistance <= 100f) // 7m
            {
                if (!isFirst)
                {
                    isFirst = true;
                    arrival_popup.SetActive(true);
                }
            }
        }
    }

    // 사용자가 새로운 도착지를 입력할 때 호출되는 메서드
    public void AddDestination()
    {
        float lat = dataManager.detailResponse.result.geometry.location.lat;
        float lon = dataManager.detailResponse.result.geometry.location.lng;

        if (lat != 0 && lon != 0)
        {
            // 리스트의 맨 앞에 새로운 값 삽입
            lats.Insert(0, lat);
            Debug.Log($"lat" + lat);
            // 리스트의 맨 앞에 새로운 값 삽입
            longs.Insert(0, lon);
            Debug.Log($"lon" + lon);
        }
    }

    // 지표면 거리 계산 공식(하버사인 공식)
    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        double theta = lon1 - lon2;

        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));

        dist = Math.Acos(dist);

        dist = Rad2Deg(dist);

        dist = dist * 60 * 1.1515;

        dist = dist * 1609.344; // 미터 변환

        return dist;
    }

    private double Deg2Rad(double deg)
    {
        return (deg * Mathf.PI / 180.0f);
    }

    private double Rad2Deg(double rad)
    {
        return (rad * 180.0f / Mathf.PI);
    }
}