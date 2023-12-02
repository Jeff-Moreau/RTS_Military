using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FighterUnit : UnitController, ISelectable
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
    private bool mIsAlive;
    private float mCountTime;
    private float mReloadTime;
    private float mCurrentHealth;
    private float mCurrentClosestDistance;
    private GameObject mEnemyContainer;
    private AIFighterUnit mEnemyTarget;
    private Camera mWorldCamera;

    // MEMBER CONTAINERS
    private AIFighterUnit[] mEnemyList;
    private float[] mEnemyDistance;

    // GETTERS
    public bool IsAlive => mIsAlive;
    public float GetHealth => mCurrentHealth;

    // SETTERS
    public void SetCurrentHealth(float damage) => mCurrentHealth = damage;

    private void Awake()
    {
        mGameOver = GameObject.Find("Managers").GetComponent<GameOverCheck>();
        mNavAgent = GetComponent<NavMeshAgent>();
        mAnimator = GetComponent<Animator>();
        mEnemyContainer = GameObject.Find("ObjectPools");
    }

    private void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        mNavAgent.speed = mData.GetMovementSpeed;
        mSelected = false;
        mPlayerControled = false;
        mCurrentState = State.Idle;
        mCurrentPosition = transform.position;
        mCurrentRotation = transform.rotation;
        mEnemyList = mEnemyContainer.GetComponentsInChildren<AIFighterUnit>();
        mEnemyDistance = new float[mEnemyList.Length];
        mEnemyTarget = null;
        mIsAlive = true;
        mReloadTime = 1.5f;
        mCountTime = 0;
        mCurrentHealth = mData.GetMaxHealth;
        mCurrentClosestDistance = mData.GetViewDistance;
        mWorldCamera = Camera.main;
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

                mEnemyTarget = null;
                mPlayerControled = false;
                mCurrentClosestDistance = mData.GetViewDistance;

                DetectAIEnemy();

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

                mEnemyTarget = null;

                DetectAIEnemy();

                if (mNavAgent.pathStatus == NavMeshPathStatus.PathComplete && mNavAgent.remainingDistance <= 4f)
                {
                    mCurrentPosition = transform.position;
                    mCurrentRotation = transform.rotation;
                    mNavAgent.isStopped = true;
                    mCurrentState = State.Idle;
                }
                break;

            case State.Working:
                mAnimator.SetBool("IsWalking", false);
                mAnimator.SetBool("IsShooting", false);
                mAnimator.SetBool("IsShootAndWalk", false);

                mEnemyTarget = null;
                mPlayerControled = false;

                DetectAIEnemy();
                break;

            case State.Chasing:
                mAnimator.SetBool("IsWalking", false);
                mAnimator.SetBool("IsShooting", false);
                mAnimator.SetBool("IsShootAndWalk", true);
                mGunIdle.SetActive(false);
                mGunIdleAim.SetActive(false);
                mGunWalking.SetActive(false);
                mGunWalkingAim.SetActive(true);

                mPlayerControled = false;

                if (Vector3.Distance(mEnemyTarget.transform.position, transform.position) <= mData.GetAttackDistance)
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

                mEnemyTarget = null;
                mPlayerControled = false;
                break;

            case State.Attacking:
                mAnimator.SetBool("IsWalking", false);
                mAnimator.SetBool("IsShooting", true);
                mAnimator.SetBool("IsShootAndWalk", false);
                mGunIdle.SetActive(false);
                mGunIdleAim.SetActive(true);
                mGunWalking.SetActive(false);
                mGunWalkingAim.SetActive(false);

                transform.LookAt(mEnemyTarget.transform.position);

                var testPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

                if (Physics.Raycast(testPosition, transform.forward, out RaycastHit target, Mathf.Infinity))
                {
                    if (target.collider.gameObject == mEnemyTarget.gameObject && mEnemyTarget.IsAlive)
                    {
                        if (mCountTime > mReloadTime)
                        {
                            if (!mGunSource.isPlaying)
                            {
                                // do damge here.
                                mEnemyTarget.SetCurrentHealth(mEnemyTarget.GetHealth - mData.GetBaseDamage);
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

            case State.Dead:
                mAnimator.SetBool("IsWalking", false);
                mAnimator.SetBool("IsShooting", false);
                mAnimator.SetBool("IsShootAndWalk", false);

                mGameOver.RemovePlayerUnit(1);
                mIsAlive = false;
                gameObject.SetActive(false);
                break;
        }

        if (mSelected)
        {
            mRenderer.material.color = Color.green;
        }
        else
        {
            mRenderer.material.color = Color.white;
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
                mRenderer.material.color = Color.green;
            }
        }
    }

    public void Selected()
    {
        if (mSelected && Input.GetKey(KeyCode.LeftShift))
        {
            var otherUnits = FindObjectsOfType<FighterUnit>();

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

    private void DetectAIEnemy()
    {
        for (int i = 0; i < mEnemyList.Length; i++)
        {
            var distanceBetween = Vector3.Distance(mEnemyList[i].transform.position, transform.position);
            mEnemyDistance[i] = distanceBetween;

            if (mEnemyDistance[i] < mCurrentClosestDistance)
            {
                if (mEnemyList[i].gameObject.activeInHierarchy)
                {
                    mCurrentClosestDistance = mEnemyDistance[i];
                    mEnemyTarget = mEnemyList[i];
                }
                else
                {
                    mCurrentClosestDistance = mData.GetViewDistance;
                }
            }

            if (mEnemyList[i].IsAlive && distanceBetween <= mData.GetViewDistance)
            {
                if (!mPlayerControled)
                {
                    mNavAgent.SetDestination(mEnemyTarget.transform.position);
                    mNavAgent.speed = mData.GetMovementSpeed;
                    mNavAgent.isStopped = false;
                    mCurrentState = State.Chasing;
                }
            }
        }
    }

    private void MuzzleFlash()
    {
        if  (mMuzzleFlair.activeSelf)
        {
            mMuzzleFlair.SetActive(false);
        }
        else
        {
            mMuzzleFlair.SetActive(true);
        }
    }
}
