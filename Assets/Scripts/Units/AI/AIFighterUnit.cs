using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIFighterUnit : UnitController, ISelectable
{
    // INSPECTOR VARIABLES
    [SerializeField] private UnitData mData;
    [SerializeField] private AudioSource mGunSource;
    [SerializeField] private GameObject mMuzzleFlair = null;
    [SerializeField] private GameObject mGunIdle = null;
    [SerializeField] private GameObject mGunIdleAim = null;
    [SerializeField] private GameObject mGunWalking = null;
    [SerializeField] private GameObject mGunWalkingAim = null;
    [SerializeField] private GameObject mHealthCanvas = null;
    [SerializeField] private Slider mHealthOnScreen = null;

    // MEMBER VARIABLES
    private int mCurrentBaseWaypointPosition;
    private bool mIsAlive;
    private bool mIsBasePatrol;
    private float mCountTime;
    private float mReloadTime;
    private float mCurrentHealth;
    private float mCurrentClosestDistance;
    private float mAIBaseWaypointDistanceCheck;
    private GameObject mUnitContainer;
    private FighterUnit mUnitTarget;
    private Camera mWorldCamera;

    // MEMBER CONTAINERS
    private float[] mUnitDistance;
    private FighterUnit[] mUnitList;
    private GameObject[] mAIBaseWaypointContainer;
    private Transform[] mAIBaseWaypointsToFollow;

    // GETTERS
    public bool IsAlive => mIsAlive;
    public float GetHealth => mCurrentHealth;

    // SETTERS
    public void SetCurrentHealth(float damage) => mCurrentHealth = damage;
    public void SetIsBasePatrol(bool yesno) => mIsBasePatrol = yesno;

    private void Awake()
    {
        mGameOver = GameObject.Find("Managers").GetComponent<GameOverCheck>();
        mNavAgent = GetComponent<NavMeshAgent>();
        mAnimator = GetComponent<Animator>();
        mAIBaseWaypointContainer = LoadingManager.Load.GetCurrentMap.GetAIBaseWaypointContainer;
        mUnitContainer = GameObject.Find("ObjectPools");
        mWorldCamera = Camera.main;
    }

    private void Start()
    {
        InitializeVariables();
        SetupBasePatrolWaypoints();
    }

    private void SetupBasePatrolWaypoints()
    {
        var totalWaypoints = mAIBaseWaypointContainer[0].GetComponentsInChildren<Transform>();
        mAIBaseWaypointsToFollow = new Transform[totalWaypoints.Length - 1];

        for (int i = 1; i < totalWaypoints.Length; i++)
        {
            mAIBaseWaypointsToFollow[i - 1] = totalWaypoints[i];
        }
    }

    private void InitializeVariables()
    {
        mNavAgent.speed = mData.GetMovementSpeed;
        mSelected = false;
        mCurrentState = State.Idle;
        mCurrentPosition = transform.position;
        mCurrentRotation = transform.rotation;
        mUnitList = mUnitContainer.GetComponentsInChildren<FighterUnit>();
        mCurrentHealth = mData.GetMaxHealth;
        mUnitDistance = new float[mUnitList.Length];
        mUnitTarget = null;
        mIsAlive = true;
        mCurrentBaseWaypointPosition = 0;
        mAIBaseWaypointDistanceCheck = 2 * 2;
        mCountTime = 0;
        mReloadTime = 1.5f;
        mCurrentHealth = mData.GetMaxHealth;
        mCurrentClosestDistance = mData.GetViewDistance;
        mHealthOnScreen.value = mCurrentHealth / mData.GetMaxHealth;
    }

    private void Update()
    {
        mCountTime += Time.deltaTime;

        if (mCurrentHealth <= 0)
        {
            mCurrentState = State.Dead;
        }

        if (mCurrentHealth < mData.GetMaxHealth)
        {
            mHealthCanvas.SetActive(true);
        }
        else
        {
            mHealthCanvas.SetActive(false);
        }

        mHealthCanvas.transform.forward = mWorldCamera.transform.forward;

        mHealthOnScreen.value = mCurrentHealth / mData.GetMaxHealth;

        switch (mCurrentState)
        {
            case State.Idle:
                mAnimator.SetBool("IsWalking", false);
                mAnimator.SetBool("IsShooting", false);
                mAnimator.SetBool("IsShootAndWalk", false);
                mGunIdle.SetActive(true);
                mGunIdleAim.SetActive(false);
                mGunWalking.SetActive(false);
                mGunWalkingAim.SetActive(false);

                if (mIsBasePatrol)
                {
                    mCurrentState = State.Patrol;
                }

                mUnitTarget = null;
                mCurrentClosestDistance = mData.GetViewDistance;

                DetectPlayerUnits();

                transform.position = new Vector3(mCurrentPosition.x, transform.position.y, mCurrentPosition.z);
                transform.rotation = mCurrentRotation;
                break;

            case State.Moving:
                mAnimator.SetBool("IsWalking", true);
                mAnimator.SetBool("IsShooting", false);
                mAnimator.SetBool("IsShootAndWalk", false);
                mGunIdle.SetActive(false);
                mGunIdleAim.SetActive(false);
                mGunWalking.SetActive(true);
                mGunWalkingAim.SetActive(false);

                mUnitTarget = null;

                DetectPlayerUnits();

                if (mNavAgent.pathStatus == NavMeshPathStatus.PathComplete && mNavAgent.remainingDistance <= 4f)
                {
                    mCurrentPosition = transform.position;
                    mCurrentRotation = transform.rotation;
                    mNavAgent.isStopped = true;
                    mCurrentState = State.Idle;
                }
                break;

            case State.Working:
                // idle animation?
                // idle sound effects?
                // checking range for bad guys
                break;

            case State.Chasing:
                mAnimator.SetBool("IsWalking", false);
                mAnimator.SetBool("IsShooting", false);
                mAnimator.SetBool("IsShootAndWalk", true);
                mGunIdle.SetActive(false);
                mGunIdleAim.SetActive(false);
                mGunWalking.SetActive(false);
                mGunWalkingAim.SetActive(true);

                if (Vector3.Distance(mUnitTarget.transform.position, transform.position) <= mData.GetAttackDistance)
                {
                    mCurrentPosition = transform.position;
                    mCurrentRotation = transform.rotation;
                    mNavAgent.isStopped = true;
                    mCurrentState = State.Attacking;
                }
                break;

            case State.Fleeing:
                mAnimator.SetBool("IsWalking", true);
                mAnimator.SetBool("IsShooting", false);
                mAnimator.SetBool("IsShootAndWalk", false);

                mUnitTarget = null;
                break;

            case State.Attacking:
                mAnimator.SetBool("IsWalking", false);
                mAnimator.SetBool("IsShooting", true);
                mAnimator.SetBool("IsShootAndWalk", false);
                mGunIdle.SetActive(false);
                mGunIdleAim.SetActive(true);
                mGunWalking.SetActive(false);
                mGunWalkingAim.SetActive(false);

                transform.LookAt(mUnitTarget.transform.position);

                var testPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

                if (Physics.Raycast(testPosition, transform.forward, out RaycastHit target, Mathf.Infinity))
                {
                    if (target.collider.gameObject == mUnitTarget.gameObject && mUnitTarget.IsAlive)
                    {
                        if (mCountTime > mReloadTime)
                        {
                            if (!mGunSource.isPlaying)
                            {
                                // do damge here.
                                mUnitTarget.SetCurrentHealth(mUnitTarget.GetHealth - mData.GetBaseDamage);
                                mGunSource.Play();
                                InvokeRepeating(nameof(MuzzleFlash), 0, 0.035f);
                            }
                            mCountTime = 0;
                        }
                        if (mCountTime >= 0.7f)
                        {
                            CancelInvoke(nameof(MuzzleFlash));
                            mMuzzleFlair.SetActive(false);
                        }
                    }
                    else
                    {
                        mCurrentState = State.Idle;
                    }
                }

                Debug.DrawRay(testPosition, transform.forward * 100, Color.magenta);
                break;

            case State.Patrol:
                var waypointPosition = mAIBaseWaypointsToFollow[mCurrentBaseWaypointPosition].position;
                var nearNextWaypoint = transform.InverseTransformPoint(new Vector3(waypointPosition.x, transform.position.y, waypointPosition.z));

                mNavAgent.SetDestination(waypointPosition);
                mNavAgent.speed = mData.GetMovementSpeed;
                mAnimator.SetBool("IsWalking", true);

                if (nearNextWaypoint.sqrMagnitude < mAIBaseWaypointDistanceCheck)
                {
                    mCurrentBaseWaypointPosition += 1;

                    if (mCurrentBaseWaypointPosition == mAIBaseWaypointsToFollow.Length)
                    {
                        mCurrentBaseWaypointPosition = 0;
                    }
                }
                break;

            case State.Dead:
                mAnimator.SetBool("IsWalking", false);
                mAnimator.SetBool("IsShooting", false);
                mAnimator.SetBool("IsShootAndWalk", false);

                mGameOver.RemoveEnemyUnit(1);
                mIsAlive = false;
                gameObject.SetActive(false);
                break;
        }

        if (mSelected)
        {
            mRenderer.material.color = Color.red;
        }
        else
        {
            var normalColor = new Color(1f, 0.61f, 0.61f, 1);
            mRenderer.material.color = normalColor;
        }

        MouseHover();
        //base.Update();
    }

    private void MouseHover()
    {
        var hoverMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(hoverMouse, out RaycastHit target))
        {
            if (target.collider.gameObject == gameObject)
            {
                mRenderer.material.color = Color.red;
            }
        }
    }

    public void Selected()
    {
        mSelected = !mSelected;
    }

    private void DetectPlayerUnits()
    {
        for (int i = 0; i < mUnitList.Length; i++)
        {
            var distanceBetween = Vector3.Distance(mUnitList[i].transform.position, transform.position);
            mUnitDistance[i] = distanceBetween;

            if (mUnitDistance[i] < mCurrentClosestDistance)
            {
                if (mUnitList[i].gameObject.activeInHierarchy)
                {
                    mCurrentClosestDistance = mUnitDistance[i];
                    mUnitTarget = mUnitList[i];
                }
                else
                {
                    mCurrentClosestDistance = mData.GetViewDistance;
                }
            }

            if (mUnitList[i].IsAlive && distanceBetween <= mData.GetViewDistance)
            {
                if (!mPlayerControled)
                {
                    mNavAgent.SetDestination(mUnitTarget.transform.position);
                    mNavAgent.speed = mData.GetMovementSpeed;
                    mNavAgent.isStopped = false;
                    mCurrentState = State.Chasing;
                }
            }
        }
    }

    private void MuzzleFlash()
    {
        if (mMuzzleFlair.activeSelf)
        {
            mMuzzleFlair.SetActive(false);
        }
        else
        {
            mMuzzleFlair.SetActive(true);
        }
    }
}
