using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameEnvironment  
{
    private static GameEnvironment _instance;
    private GameObject _player;
    public GameObject Player { get { return _player; } }
    private GameObject _mapCenter;


    public static GameEnvironment Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameEnvironment();
                _instance._player = GameObject.FindGameObjectWithTag("Player");
                _instance._mapCenter = GameObject.FindGameObjectWithTag("MapCenter");
            }
            return _instance;
        }
    }

    public Vector3 GetRandomWanderPoint(float radius)
    {
        //make radius dynamic so it is reusable for many different maps
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        Vector3 wanderPoint = new Vector3(randomPoint.x, 0f, randomPoint.y) + _mapCenter.transform.position;

        return wanderPoint;
    }
}
