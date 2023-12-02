using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    // ENUMS FOR STATE MACHINE
    protected enum State
    {
        Idle,
        Moving,
        Working,
        Chasing,
        Fleeing,
        Attacking,
        Patrol,
        Dead
    }

    // INSPECTOR VARIABLES
    [SerializeField] protected Renderer mRenderer = null;
    [SerializeField] protected Rigidbody mRigidBody = null;
    [SerializeField] private AudioSource mMoveSound = null;

    // MEMBER VARIABLES
    protected bool mSelected;
    protected bool mPlayerControled;
    protected Animator mAnimator;
    protected NavMeshAgent mNavAgent;
    protected State mCurrentState;
    protected Vector3 mCurrentPosition;
    protected LayerMask mMask;
    protected Quaternion mCurrentRotation;
    protected GameOverCheck mGameOver = null;

    // GETTERS
    public bool GetSelected => mSelected;
    public Vector3 GetCurrentPosition => mCurrentPosition;

    // SETTERS
    public void SetSelected(bool yesno) => mSelected = yesno;

    private void OnEnable()
    {
        Actions.DeSelect += DeSelectUnit;
        Actions.UnitMove += MoveUnit;
    }

    protected void DeSelectUnit()
    {
        if (mSelected)
        {
            mSelected = false;
        }
    }

    protected void MoveUnit(RaycastHit location)
    {
        if (mSelected && gameObject.layer == 8)
        {
            mNavAgent.SetDestination(location.point);
            mNavAgent.isStopped = false;
            mCurrentState = State.Moving;
            mPlayerControled = true;

            if (!mMoveSound.isPlaying)
            {
                mMoveSound.Play();
            }
        }
    }

    private void OnDisable()
    {
        Actions.DeSelect -= DeSelectUnit;
        Actions.UnitMove -= MoveUnit;
    }
}