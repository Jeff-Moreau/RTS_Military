using UnityEngine;
using UnityEngine.AI;

public class LoadingManager : MonoBehaviour
{
    // SINGLETON STARTS
    private static LoadingManager myInstance;
    private void Singleton()
    {
        if (myInstance != null && myInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            myInstance = this;
        }
    }

    public static LoadingManager Load => myInstance;
    // SINGLETON ENDS

    // INSPECTOR VARIABLES
    [SerializeField] private Camera mMainCamera = null;
    [SerializeField] private GameObject mPlayerStartBase = null;
    [SerializeField] private GameObject mAIStartBase = null;
    [SerializeField] private FighterUnitPool mFighterUnitPool = null;
    [SerializeField] private AIFighterUnitPool mAIFighterUnitPool = null;
    [SerializeField] private GameObject[] mMaps = null;
    [SerializeField] private AudioSource mIntroVoice = null;

    // MEMBER VARIABLES
    private int mTotalUnitsSpawned;
    private int mTotalAIUnitsSpawned;
    private GameObject mAIBasePatrolSpawn;
    private MapController mCurrentMap;

    // MEMBER CONTAINERS
    private GameObject[] mBaseSpawnpoints;
    private GameObject[] mUnitSpawnpoints;
    private GameObject[] mAIBaseSpawnpoints;
    private GameObject[] mAIUnitSpawnpoints;
    private SpawnpointTaken[] mUnitSpawnpointsTaken;
    private SpawnpointTaken[] mAIUnitSpawnpointsTaken;

    // GETTERS
    public MapController GetCurrentMap => mCurrentMap;

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        InitializeVariables();
        CreatePlayerStartLocation();
        CreatePlayerStartingUnits();
        CreateAIStartLocation();
        CreateAIStartingUnits();
        Actions.CameraLoadedPosition.Invoke(mMainCamera.transform.position);
    }
    private void InitializeVariables()
    {
        SetupPlayerSpawnpoints();
        SetupAISpawnpoints();
    }

    private void SetupAISpawnpoints()
    {
        mAIBaseSpawnpoints = mMaps[0].GetComponent<MapController>().GetAIBaseSpawnpoints;
        mAIUnitSpawnpoints = mMaps[0].GetComponent<MapController>().GetAIUnitSpawnpoints;
        mAIBasePatrolSpawn = mMaps[0].GetComponent<MapController>().GetAIBasePatrolSpawn;
        mAIUnitSpawnpointsTaken = new SpawnpointTaken[mAIUnitSpawnpoints.Length];

        for (int i = 0; i < mAIUnitSpawnpoints.Length; i++)
        {
            mAIUnitSpawnpointsTaken[i] = mAIUnitSpawnpoints[i].GetComponent<SpawnpointTaken>();
            mAIUnitSpawnpointsTaken[i].SetIsTaken(false);
        }

        mTotalAIUnitsSpawned = 0;

        mAIFighterUnitPool.SetTotalPrefabsNeeded(mMaps[0].GetComponent<MapController>().GetTotalUnits);
    }

    private void SetupPlayerSpawnpoints()
    {
        mCurrentMap = mMaps[0].GetComponent<MapController>();
        mBaseSpawnpoints = mMaps[0].GetComponent<MapController>().GetBaseSpawnpoints;
        mUnitSpawnpoints = mMaps[0].GetComponent<MapController>().GetUnitSpawnpoints;
        mUnitSpawnpointsTaken = new SpawnpointTaken[mUnitSpawnpoints.Length];

        for (int i = 0; i < mUnitSpawnpoints.Length; i++)
        {
            mUnitSpawnpointsTaken[i] = mUnitSpawnpoints[i].GetComponent<SpawnpointTaken>();
            mUnitSpawnpointsTaken[i].SetIsTaken(false);
        }

        mTotalUnitsSpawned = 0;

        mFighterUnitPool.SetTotalPrefabsNeeded(mMaps[0].GetComponent<MapController>().GetTotalUnits);
    }

    private void CreateAIStartLocation()
    {
        var randomSpawnLocation = Random.Range(0, mAIBaseSpawnpoints.Length);
        var newSpawnLocation = new Vector3(mAIBaseSpawnpoints[randomSpawnLocation].transform.position.x, mAIBaseSpawnpoints[randomSpawnLocation].transform.position.y, mAIBaseSpawnpoints[randomSpawnLocation].transform.position.z);
        Instantiate(mAIStartBase, newSpawnLocation, mAIBaseSpawnpoints[randomSpawnLocation].transform.rotation);
    }

    private void CreatePlayerStartLocation()
    {
        var randomSpawnLocation = Random.Range(0, mBaseSpawnpoints.Length);
        var newSpawnLocation = new Vector3(mBaseSpawnpoints[randomSpawnLocation].transform.position.x, mBaseSpawnpoints[randomSpawnLocation].transform.position.y, mBaseSpawnpoints[randomSpawnLocation].transform.position.z);
        mMainCamera.transform.position = new Vector3(mBaseSpawnpoints[randomSpawnLocation].transform.position.x, mMainCamera.transform.position.y, mBaseSpawnpoints[randomSpawnLocation].transform.position.z);
        mMainCamera.fieldOfView = 60;
        Instantiate(mPlayerStartBase, newSpawnLocation, mBaseSpawnpoints[randomSpawnLocation].transform.rotation);
    }

    private void CreateAIStartingUnits()
    {
        for (int i = 0; i < mAIUnitSpawnpoints.Length; i++)
        {
            if(!mAIUnitSpawnpointsTaken[i].GetIsTaken && mTotalAIUnitsSpawned < mAIUnitSpawnpoints.Length)
            {
                var newUnit = mAIFighterUnitPool.GetAvailablePrefabs();

                if (newUnit != null)
                {
                    newUnit.transform.SetPositionAndRotation(mAIUnitSpawnpoints[i].transform.position, mAIUnitSpawnpoints[i].transform.rotation);
                    mAIUnitSpawnpointsTaken[i].SetIsTaken(true);
                    mTotalAIUnitsSpawned++;
                }

                for (int j = 0; j < mAIFighterUnitPool.GetPrefabList.Count; j++)
                {
                    newUnit.SetActive(true);
                }
            }
        }

        var patrolUnit = mAIFighterUnitPool.GetAvailablePrefabs();

        if (patrolUnit != null)
        {
            patrolUnit.transform.SetPositionAndRotation(mAIBasePatrolSpawn.transform.position, mAIBasePatrolSpawn.transform.rotation);
            mTotalAIUnitsSpawned++;
            patrolUnit.GetComponent<AIFighterUnit>().SetIsBasePatrol(true);
            patrolUnit.SetActive(true);
        }

    }

    private void CreatePlayerStartingUnits()
    {
        for (int i = 0; i < mUnitSpawnpoints.Length; i++)
        {
            if (!mUnitSpawnpointsTaken[i].GetIsTaken && mTotalUnitsSpawned < mUnitSpawnpoints.Length)
            {
                var newUnit = mFighterUnitPool.GetAvailablePrefabs();

                if (newUnit != null)
                {
                    newUnit.transform.SetPositionAndRotation(mUnitSpawnpoints[i].transform.position, mUnitSpawnpoints[i].transform.rotation);
                    mUnitSpawnpointsTaken[i].SetIsTaken(true);
                    mTotalUnitsSpawned++;
                }

                for (int j = 0; j < mFighterUnitPool.GetPrefabList.Count; j++)
                {
                    newUnit.GetComponent<NavMeshAgent>().avoidancePriority = 10 + j;
                    newUnit.SetActive(true);
                }
            }
        }
    }
}