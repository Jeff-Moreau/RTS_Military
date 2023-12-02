using UnityEngine;

public class GameOverCheck : MonoBehaviour
{
    // if player units active == 0 end game you lose
    // if enemy units active == 0 end game you win
    [SerializeField] private GameObject mGameOver;

    private GameObject mUnitContainer;
    private AIFighterUnit[] mEnemyList;
    private FighterUnit[] mUnitList;
    private int mCurrentRemainingEnemyUnits;
    private int mCurrentRemainingPlayerUnits;

    public void RemoveEnemyUnit(int amount) => mCurrentRemainingEnemyUnits -= amount;
    public void RemovePlayerUnit(int amount) => mCurrentRemainingPlayerUnits -= amount;

    private void Awake()
    {
        mUnitContainer = GameObject.Find("ObjectPools");
    }

    private void Start()
    {
        mEnemyList = mUnitContainer.GetComponentsInChildren<AIFighterUnit>();
        mUnitList = mUnitContainer.GetComponentsInChildren<FighterUnit>();
        mCurrentRemainingEnemyUnits = mEnemyList.Length;
        mCurrentRemainingPlayerUnits = mUnitList.Length;
    }

    private void Update()
    {
        Debug.Log("EnemyUnits " + mCurrentRemainingEnemyUnits);
        Debug.Log("PlayerUnits " + mCurrentRemainingPlayerUnits);

        if (mCurrentRemainingEnemyUnits == 0)
        {
            mGameOver.SetActive(true);
            Time.timeScale = 0f;
        }

        if (mCurrentRemainingPlayerUnits == 0)
        {
            mGameOver.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
