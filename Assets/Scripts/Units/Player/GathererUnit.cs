using UnityEngine;
using UnityEngine.AI;

public class GathererUnit : UnitController, ISelectable
{
    // INSPECTOR VARIABLES
    [SerializeField] private UnitData mData;

    private void Awake()
    {
        mNavAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        mMask = LayerMask.GetMask("Units");
        mNavAgent.speed = mData.GetMovementSpeed;
        mSelected = false;
        mCurrentState = State.Idle;
        mCurrentPosition = transform.position;
        mCurrentRotation = transform.rotation;
    }

    private void Update()
    {
        switch (mCurrentState)
        {
            case State.Idle:
                // idle animation?
                // idle sound effects?
                // checking range for bad guys
                transform.position = new Vector3(mCurrentPosition.x, transform.position.y, mCurrentPosition.z);
                transform.rotation = mCurrentRotation;
                Debug.Log("I am Idle. Please do something with me."); // this is just for testing purposes
                break;

            case State.Moving:
                // idle animation?
                // idle sound effects?
                // checking range for bad guys
                if (mNavAgent.pathStatus == NavMeshPathStatus.PathComplete && mNavAgent.remainingDistance <= 1.5f)
                {
                    mCurrentPosition = transform.position;
                    mCurrentRotation = transform.rotation;
                    mNavAgent.isStopped = true;
                    mCurrentState = State.Idle;
                }
                Debug.Log("I am Runnin."); // this is just for testing purposes
                break;

            case State.Working:
                // idle animation?
                // idle sound effects?
                // checking range for bad guys
                Debug.Log("Work work work all day long."); // this is just for testing purposes
                break;

            case State.Chasing:
                // idle animation?
                // idle sound effects?
                // checking range for bad guys
                Debug.Log("I'm gonna get him."); // this is just for testing purposes
                break;

            case State.Fleeing:
                // idle animation?
                // idle sound effects?
                // checking range for bad guys
                Debug.Log("He's scary im leaving."); // this is just for testing purposes
                break;

            case State.Attacking:
                // idle animation?
                // idle sound effects?
                Debug.Log("Kill kill kill"); // this is just for testing purposes
                break;

            case State.Dead:
                // idle animation?
                // idle sound effects?
                Debug.Log("Ohh well maybe next time"); // this is just for testing purposes
                break;
        }

        if (mSelected)
        {
            // What happens when selected??
            // Sound? Image change? Menu Pop Up?
            mRenderer.material.color = Color.green; // this is just for testing purposes
        }
        else
        {
            // What happens when unselecting??
            // Sound? Image change? Menu Pop Up?
            mRenderer.material.color = Color.blue; // this is just for testing purposes
        }
        //base.Update();
    }

    public void Selected()
    {
        if (mSelected && Input.GetKey(KeyCode.LeftShift))
        {
            var otherUnits = FindObjectsOfType<GathererUnit>();
            foreach (var otherUnit in otherUnits)
            {
                otherUnit.mSelected = true;
            }
        }
        else
        {
            mSelected = !mSelected;
        }
    }
}