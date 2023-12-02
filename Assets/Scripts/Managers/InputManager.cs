using UnityEngine;

public class InputManager : MonoBehaviour
{
    // SINGLETON STARTS
    private static InputManager myInstance;
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

    public static InputManager Load => myInstance;
    // SINGLETON ENDS

    // INSPECTOR VARIABLES
    [SerializeField] private GameObject mMarker = null;
    [SerializeField] private GameObject mPauseScreen = null;
    [SerializeField] private BunkerController mBunker = null;
    //[SerializeField] private Texture2D mSelectionBoxColor = null;

    [Header("Camera Stuff")]
    [SerializeField] private Camera mMainCamera = null;
    [SerializeField] private CameraData mCameraData = null;

    // MEMBER VARIABLES
    private Vector3 mCameraPosition;
    private bool mGameIsPaused;
    //private bool isSelectingUnits;
    //private Rect mUnitSelectionBox;
    //private Vector3 mBoxStartCorner;

    // GETTERS
    //public bool IsSelecting => isSelectingUnits;
    //public Rect GetUnitSelectionBox => mUnitSelectionBox;

    private void OnEnable()
    {
        Actions.CameraLoadedPosition += LoadIsDone;
    }

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        mCameraPosition = mMainCamera.transform.position;
        //mBoxStartCorner = Vector3.zero;
        //mUnitSelectionBox = new Rect(0,0,0,0);
        //isSelectingUnits = false;
    }

    private void Update()
    {
        CameraZoom();
        SelectDeselectUnit();
        MoveUnitToLocation();
        MoveCameraMouseEdge();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mGameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            mBunker.SetHealth(500);
        }
       
        mMainCamera.transform.position = mCameraPosition;
    }

/*    private void LateUpdate()
    {
        BoxSelectUnits();
    }

    private void OnGUI()
    {
        if (mBoxStartCorner != -Vector3.one)
        {
            GUI.DrawTexture(mUnitSelectionBox, mSelectionBoxColor);
        }
    }

    private void BoxSelectUnits()
    {
        if (Input.GetMouseButtonDown(2))
        {
            mBoxStartCorner = Input.mousePosition;
        }
        else if (isSelectingUnits && Input.GetMouseButtonUp(2))
        {
            mBoxStartCorner = Vector3.one;
        }

        CreateBoxSelector();
    }

    private void CreateBoxSelector()
    {
        if (Input.GetMouseButton(2))
        {
            isSelectingUnits = true;
            mUnitSelectionBox = new Rect(mBoxStartCorner.x, Screen.height - mBoxStartCorner.y, Input.mousePosition.x - mBoxStartCorner.x, (Screen.height - Input.mousePosition.y) - (Screen.height - mBoxStartCorner.y));

            if (mUnitSelectionBox.width < 0)
            {
                mUnitSelectionBox.x += mUnitSelectionBox.width;
                mUnitSelectionBox.width = -mUnitSelectionBox.width;
            }

            if (mUnitSelectionBox.height < 0)
            {
                mUnitSelectionBox.y += mUnitSelectionBox.height;
                mUnitSelectionBox.height = -mUnitSelectionBox.height;
            }
        }
    }*/

    private void SelectDeselectUnit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var location = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(location, out RaycastHit hit))
            {
                if (hit.collider.gameObject.layer == 3)
                {
                    Actions.DeSelect.Invoke();
                }
                else
                {
                    hit.collider.gameObject.TryGetComponent(out ISelectable item);
                    item?.Selected();
                }
            }
        }
    }

    private void MoveUnitToLocation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var location = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(location, out RaycastHit hit, 1000))
            {
                Actions.UnitMove.Invoke(hit);
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            var location = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(location, out RaycastHit hit, 1000))
            {
                if (!mMarker.activeInHierarchy)
                {
                    mMarker.transform.position = hit.point;
                    mMarker.SetActive(true);
                }
            }
        }
    }

    private void CameraZoom()
    {
        if (Input.mouseScrollDelta.y < 0 && Camera.main.fieldOfView < mCameraData.GetMaxZoomDistance)
        {
            Camera.main.fieldOfView += mCameraData.GetZoomSpeed;
        }
        if (Input.mouseScrollDelta.y > 0 && Camera.main.fieldOfView > mCameraData.GetMinZoomDistance)
        {
            Camera.main.fieldOfView -= mCameraData.GetZoomSpeed;
        }
    }

    private void MoveCameraMouseEdge()
    {
        // Get the Camera to stop at edge of map and go no further
        if (Input.mousePosition.x >= Screen.width - mCameraData.GetLeftRightEdgeBuffer)
        {
            mCameraPosition += Vector3.right * Time.deltaTime * mCameraData.GetEdgeScrollSpeed;
        }
        else if (Input.mousePosition.x <= (Screen.width / Screen.width) + mCameraData.GetLeftRightEdgeBuffer)
        {
            mCameraPosition += -Vector3.right * Time.deltaTime * mCameraData.GetEdgeScrollSpeed;
        }

        if (Input.mousePosition.y >= Screen.height - mCameraData.GetLeftRightEdgeBuffer)
        {
            mCameraPosition += Vector3.forward * Time.deltaTime * mCameraData.GetEdgeScrollSpeed;
        }
        else if (Input.mousePosition.y <= (Screen.height / Screen.height) + mCameraData.GetLeftRightEdgeBuffer)
        {
            mCameraPosition += -Vector3.forward * Time.deltaTime * mCameraData.GetEdgeScrollSpeed;
        }
    }

    private void LoadIsDone(Vector3 cameraPosition)
    {
        mCameraPosition = cameraPosition;
    }

    private void Resume()
    {
        mPauseScreen.SetActive(false);
        Time.timeScale = 1f;
        mGameIsPaused = false;
    }
    private void Pause()
    {
        mPauseScreen.SetActive(true);
        Time.timeScale = 0f;
        mGameIsPaused = true;
    }

    private void OnDisable()
    {
        Actions.CameraLoadedPosition -= LoadIsDone;
    }
}