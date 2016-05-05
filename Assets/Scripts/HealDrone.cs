using UnityEngine;
using System.Collections;

public class HealDrone : Drone
{
    [SerializeField] private float bonusHP = 10.0f;
    [SerializeField] private float healRadius = 20.0f;
    [SerializeField] private float healCooldown = 1.5f;

    private State droneState;
    private float lastHealedAt;
    private Vector3[] wayPoints;
    private HomeTower homeTower;
    private Vector3 homeTowerPos;
    private int waypointIndex = 0;

    private bool TooCloseToHomeTower
    {
        get
        {
            if ((homeTower.transform.position - this.transform.position).magnitude < 10f) return true;
            else return false;
        }
    }

    public enum State
    {
        REACH_BASE,
        SET_ORBIT_ROUTE,
        ORBIT_AND_HEAL
    }

	void Start ()
    {
        droneState = State.REACH_BASE;
        homeTower = GameManager.Instance.HomeTower;
        homeTowerPos = GameManager.Instance.HomeTower.transform.position;
	}
	
    public override void Update()
    {
	    base.Update();
        UpdateDroneStates();
	}

    private void UpdateDroneStates()
    {
        switch (droneState)
        {
            case State.REACH_BASE:
                HandleReachBase();
                break;
            case State.SET_ORBIT_ROUTE:
                SetOrbitRoute();
                break;
            case State.ORBIT_AND_HEAL:
                HandleHealing();
                break;
        }
    }

    private void HandleReachBase()
    {
        navMeshAgent.SetDestination(GameManager.Instance.HomeTower.transform.position);
        if (TooCloseToHomeTower)
        {
            droneState = State.SET_ORBIT_ROUTE;
        }

    }
    private void SetOrbitRoute()
    {
        if (GoRight() == 1)
        {
            PickRightHandWaypoints();
        }
        else
        {
            PickLeftHandWaypoints();
        }
        droneState = State.ORBIT_AND_HEAL;
    }

    private void HandleHealing()
    {
        UpdateOrbitMovement();
        SearchDronesToHeal();
    }

    private void UpdateOrbitMovement()
    {
        navMeshAgent.SetDestination(wayPointTarget());
    }


    private Vector3 wayPointTarget()
    {
        if(waypointIndex >= wayPoints.Length-1)
        {
            waypointIndex = 0;
        }
        if((this.transform.position - wayPoints[waypointIndex]).magnitude < 0.5f)
        {
            waypointIndex++;
        }
        return wayPoints[waypointIndex];
    }

    private void PickRightHandWaypoints()
    {
        wayPoints = new Vector3[]
        {
            new Vector3(homeTowerPos.x, 0, homeTowerPos.z - GetLongDistanceFromTower()),
            new Vector3(homeTowerPos.x - GetShortDistanceFromTower(), 0, homeTowerPos.z - GetShortDistanceFromTower()),
            new Vector3(homeTowerPos.x - GetLongDistanceFromTower(), 0, homeTowerPos.z),
            new Vector3(homeTowerPos.x - GetShortDistanceFromTower(), 0, homeTowerPos.z + GetShortDistanceFromTower()),
            new Vector3(homeTowerPos.x, 0, homeTowerPos.z + GetLongDistanceFromTower()),
            new Vector3(homeTowerPos.x + GetShortDistanceFromTower(), 0, homeTowerPos.z + GetShortDistanceFromTower()),
            new Vector3(homeTowerPos.x + GetLongDistanceFromTower(), 0, homeTowerPos.z),
            new Vector3(homeTowerPos.x + GetShortDistanceFromTower(), 0, homeTowerPos.z - GetShortDistanceFromTower())
        };
    }

    private void PickLeftHandWaypoints()
    {
        wayPoints = new Vector3[]
        {
            new Vector3(homeTowerPos.x + GetShortDistanceFromTower(), 0, homeTowerPos.z - GetShortDistanceFromTower()),
            new Vector3(homeTowerPos.x + GetLongDistanceFromTower(), 0, homeTowerPos.z),
            new Vector3(homeTowerPos.x + GetShortDistanceFromTower(), 0, homeTowerPos.z + GetShortDistanceFromTower()),
            new Vector3(homeTowerPos.x, 0, homeTowerPos.z + GetLongDistanceFromTower()),
            new Vector3(homeTowerPos.x - GetShortDistanceFromTower(), 0, homeTowerPos.z + GetShortDistanceFromTower()),
            new Vector3(homeTowerPos.x - GetLongDistanceFromTower(), 0, homeTowerPos.z),
            new Vector3(homeTowerPos.x - GetShortDistanceFromTower(), 0, homeTowerPos.z - GetShortDistanceFromTower()),
            new Vector3(homeTowerPos.x, 0, homeTowerPos.z - GetLongDistanceFromTower())
        };
    }

    private int GoRight()
    {
        return Random.Range(0, 2);
    }

    private float GetLongDistanceFromTower()
    {
        return (float)Random.Range(6f, 12f);
    }

    private float GetShortDistanceFromTower()
    {
        return (float)Random.Range(3f, 7f);
    }

    public override void Die()
    {
        base.Die();

        Destroy(gameObject);
    }

    private void SearchDronesToHeal()
    {
        Collider myCollider = GetComponent<Collider>();
        bool healActivated = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, healRadius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i] == myCollider)
                break;

            Vulnerable vulnerable = hitColliders[i].transform.root.GetComponent<Vulnerable>();

            if (vulnerable)
            {
                if (vulnerable is Drone && Time.time >= lastHealedAt + healCooldown)
                {
                    HealVulnerable(bonusHP, vulnerable);
                    healActivated = true;

                }
            }

            i++;
        }
        if(healActivated)
        {
            lastHealedAt = Time.time;
        }
    }
}
