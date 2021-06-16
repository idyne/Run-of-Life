using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;
using PathCreation;

public class Player : MonoBehaviour

{
    [SerializeField] private float startHealth = 50;
    [SerializeField] private Age[] ages = null;
    [SerializeField] private PlayerState state = PlayerState.IDLE;
    [SerializeField] private float verticalSpeed = 2;
    [SerializeField] private Avatar defaultAvatar = null;
    [SerializeField] private Color color;
    public enum PlayerState { IDLE, WALKING, RUNNING, WAITING, DYING, SITTING, STOPPING };

    public enum PlayerAgeState { BABY, KID, TEEN, YOUNG_ADULT, ADULT, OLD };

    private Age currentAge = null;
    private GameObject currentAgeObject = null;

    private float currentHealth = 50;
    private float maxHealth = 100;
    private float desiredHealth = 50;
    private Swerve1D swerve = null;
    private RunLevel levelManager = null;
    private RaycastHit hit;
    private AnimatorStateInfo animatorStateInfo;
    private int closestCheckPointIndex = -1;
    private float closestCheckPointPosition = 0;
    private string currentAnimationStateName = "Idle";
    private Transform clone = null;
    private bool isFinished = false;

    public HealthBar healthBar = null;
    private float anchor = 0;
    private float distance = 5;
    private float horizantalPosition = 0;
    private Wall currentWall;

    [System.Serializable]
    public class Age
    {
        [SerializeField] private string name = null;
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private Avatar avatar = null;
        [SerializeField] private Animator anim = null;
        [SerializeField] private float healthIntervalStart = 0;
        [SerializeField] private float healthIntervalEnd = 1;
        [SerializeField] private PlayerAgeState state = PlayerAgeState.TEEN;

        public string Name { get => name; }
        public GameObject Prefab { get => prefab; }
        public Avatar Avatar { get => avatar; }
        public Animator Anim { get => anim; }
        public float HealthIntervalStart { get => healthIntervalStart; }
        public float HealthIntervalEnd { get => healthIntervalEnd; }
        public PlayerAgeState State { get => state; }
    }


    public float CurrentHealth { get => currentHealth; }
    public float MaxHealth { get => maxHealth; }
    public Age CurrentAge { get => currentAge; }
    public PlayerState State { get => state; }
    public float Distance { get => distance; }

    private void Awake()
    {
        levelManager = (RunLevel)LevelManager.Instance;
        healthBar = FindObjectOfType<HealthBar>();
        swerve = InputManager.CreateSwerve1D(new Vector2(1, 0), Screen.width / 2);
        swerve.OnStart = () => { anchor = transform.position.x; };
        currentHealth = startHealth;
        desiredHealth = startHealth;
    }
    void Start()
    {
        CheckAgeState();
        healthBar.UpdateBar();
    }

    void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.STARTED)
        {
            CheckAgeState();
            CheckState();
            CheckInput();
            FollowPath();
            CheckCheckpoints();

            if (!isFinished)
            {
                CheckWall();
                CheckWallRows();
                CheckFinish();
            }
            else
            {
                if (currentHealth <= 5 && state == PlayerState.WALKING)
                {
                    ChangeState(PlayerState.STOPPING);
                    LeanTween.delayedCall(1f, () => { levelManager.FinishLevel(true); });
                }

                CheckBench();
            }
            CheckPool();

            currentHealth = desiredHealth;
            //currentHealth = Mathf.MoveTowards(currentHealth, desiredHealth, Time.deltaTime * 20);
            healthBar.UpdateBar();

        }
    }

    private void CheckFinish()
    {
        if (distance >= levelManager.FinishPosition)
        {
            isFinished = true;
        }
    }

    private void CheckBench()
    {
        if (distance >= levelManager.PathCreator.path.length - 1.3f && state == PlayerState.WALKING)
        {
            ChangeState(PlayerState.SITTING);
            CameraFollow camFollow = FindObjectOfType<CameraFollow>();
            camFollow.Target = null;
            healthBar.gameObject.SetActive(false);
            camFollow.transform.LeanMove(levelManager.FinishCameraPosition.position, 1);
            camFollow.transform.LeanRotate(levelManager.FinishCameraPosition.rotation.eulerAngles, 1);
            LeanTween.delayedCall(2, () => { levelManager.FinishLevel(true); });
        }
    }

    public void InstantDeath()
    {
        if (state != PlayerState.DYING)
            ChangeState(PlayerState.DYING);
    }

    private void CheckWallRows()
    {
        for (int i = 0; i < levelManager.WallRows.Length; i++)
        {
            WallRow wallRow = levelManager.WallRows[i];
            if (levelManager.PathCreator.path.GetClosestDistanceAlongPath(wallRow.transform.position) < levelManager.PathCreator.path.GetClosestDistanceAlongPath(transform.position))
                wallRow.Deactivate();
        }
    }

    private Transform FindPhone(Transform parent)
    {
        Transform phone = parent.Find("phone");
        if (phone)
            return phone;
        else
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                phone = FindPhone(parent.GetChild(i));
                if (phone)
                    break;
            }
            return phone;
        }
    }

    public void ChangeState(PlayerState newState)
    {
        print(newState);
        state = newState;
        Animator anim = currentAge.Anim;

        Transform phone = FindPhone(currentAgeObject.transform);

        if (phone)
        {
            print(phone);
            phone.gameObject.SetActive(false);
        }

        if (state == PlayerState.IDLE)
        {
            currentAnimationStateName = "Idle";
            anim.SetTrigger("Idle");
        }
        else if (state == PlayerState.WALKING)
        {

            currentAnimationStateName = "Walk";
            anim.SetTrigger("Walk");
        }
        else if (state == PlayerState.RUNNING)
        {
            currentAnimationStateName = "Run";
            anim.SetTrigger("Run");
        }
        else if (state == PlayerState.DYING)
        {
            anim.applyRootMotion = true;
            currentAnimationStateName = "Fail";
            anim.SetTrigger("Fail");
            currentAgeObject.transform.parent.rotation = Quaternion.identity;
            levelManager.FinishLevel(false);
        }
        else if (state == PlayerState.WAITING)
        {
            if (phone)
                phone.gameObject.SetActive(true);
            currentAgeObject.transform.parent.LeanRotateY(60, 0.3f);
            currentAnimationStateName = "Wait";
            anim.SetTrigger("Wait");
        }
        else if (state == PlayerState.SITTING)
        {
            anim.applyRootMotion = true;
            currentAnimationStateName = "Success";
            anim.SetTrigger("Success");
        }
        else if (state == PlayerState.STOPPING)
        {
            currentAnimationStateName = "Stop";
            anim.SetTrigger("Stop");
        }
    }


    private void ChangeAge(Age age)
    {
        Vector3 previousAgeScale = Vector3.one;
        Quaternion previousAgeRotation = Quaternion.identity;
        Animator anim;

        if (currentAge != null)
        {
            previousAgeScale = currentAgeObject.transform.localScale;
            previousAgeRotation = currentAgeObject.transform.parent.rotation;
            anim = currentAge.Anim;
            animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            //anim.enabled = false;
            //anim.gameObject.SetActive(false);
            GameObject old = currentAgeObject;
            Animator oldAnim = anim;
            Renderer[] oldRends = old.GetComponentsInChildren<Renderer>();
            Vector3 oldScale = old.transform.localScale;
            clone = old.transform;
            LeanTween.value(old, (float value) =>
            {
                for (int i = 0; i < oldRends.Length; i++)
                {
                    Material[] materials = oldRends[i].materials;
                    for (int j = 0; j < materials.Length; j++)
                    {
                        materials[j].SetFloat("_DissolveValue", value);
                    }
                    oldRends[i].materials = materials;
                }
            }, 0, 1, 0.4f).setOnComplete(() => { oldAnim.enabled = false; oldAnim.gameObject.SetActive(false); oldAnim.transform.rotation = Quaternion.identity; clone = null; Destroy(old); });
        }
        currentAge = age;
        currentAgeObject = Instantiate(currentAge.Prefab, transform.position, currentAge.Prefab.transform.rotation, currentAge.Anim.transform);
        Renderer[] rends = currentAgeObject.GetComponentsInChildren<Renderer>();
        LeanTween.value(currentAgeObject, (float value) =>
        {
            for (int i = 0; i < rends.Length; i++)
            {
                Material[] materials = rends[i].materials;
                for (int j = 0; j < materials.Length; j++)
                {
                    materials[j].SetFloat("_DissolveValue", value);
                }
                rends[i].materials = materials;
            }
        }, 1, 0, 1);
        currentAgeObject.transform.localScale = previousAgeScale;
        currentAgeObject.transform.parent.rotation = previousAgeRotation;
        anim = currentAge.Anim;
        anim.enabled = true;
        Transform phone = FindPhone(currentAgeObject.transform);
        if (state != PlayerState.WAITING && phone)
            phone.gameObject.SetActive(false);
        anim.gameObject.SetActive(true);
        anim.SetBool("BABY", currentAge.State == PlayerAgeState.BABY);
        print(currentAge.Name);
        anim.Play(currentAnimationStateName, 0, animatorStateInfo.normalizedTime);
        healthBar.UpdateText();
    }

    private void CheckAgeState()
    {
        for (int i = 0; i < ages.Length; i++)
        {
            if (currentHealth >= ages[i].HealthIntervalStart && currentHealth < ages[i].HealthIntervalEnd && currentAge != ages[i])
            {
                ChangeAge(ages[i]);
                break;
            }
        }
        currentAgeObject.transform.localScale = Vector3.one * Mathf.Clamp((1.5f - (currentHealth / MaxHealth)), 0.8f, 1.5f);
        if (clone)
        {
            clone.localScale = Vector3.MoveTowards(clone.localScale, Vector3.zero, Time.deltaTime * 0.3f);
            clone.rotation = currentAgeObject.transform.parent.rotation;
        }
        if (currentHealth <= 0 && state != PlayerState.DYING)
        {
            ChangeState(PlayerState.DYING);
        }
    }

    private void CheckState()
    {

        if (state == PlayerState.RUNNING)
            distance += Time.deltaTime * verticalSpeed;
        else if (state == PlayerState.WALKING)
            distance += Time.deltaTime * verticalSpeed / 2;
    }

    private void FollowPath()
    {

        transform.rotation = levelManager.PathCreator.path.GetRotationAtDistance(distance);
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = 0;
        transform.rotation = Quaternion.Euler(rotation);
        if (state != PlayerState.WAITING && !isFinished)
        {
            currentAgeObject.transform.parent.rotation = Quaternion.Euler(Vector3.MoveTowards(currentAgeObject.transform.parent.rotation.eulerAngles, Vector3.zero, Time.deltaTime * 120));
        }


        transform.position = levelManager.PathCreator.path.GetPointAtDistance(distance) + transform.right * horizantalPosition;
    }

    private void CheckInput()
    {
        if (swerve.Active && (state == PlayerState.RUNNING || state == PlayerState.WAITING || state == PlayerState.WALKING))
        {
            float newHorizontalPosition = Mathf.Clamp(swerve.Rate * 3.5f + anchor, -1.5f, +1.5f);
            //bool reset = (swerve.Rate * 3.5f + anchor) > 1.5f || (swerve.Rate * 3.5f + anchor) < -1.5f;
            if (Physics.Raycast(transform.position + Vector3.up * 0.25f, newHorizontalPosition > horizantalPosition ? transform.right : -transform.right, out RaycastHit hit, 10))
            {
                print(Vector3.Distance(transform.position, hit.point));
                print(Mathf.Abs(newHorizontalPosition - horizantalPosition));
                if (Vector3.Distance(transform.position, hit.point) - 0.35f <= Mathf.Abs(newHorizontalPosition - horizantalPosition))
                    swerve.Reset();
                else
                    horizantalPosition = newHorizontalPosition;
            }
            else
                horizantalPosition = newHorizontalPosition;
            //if (reset) swerve.Reset();
        }
    }

    private void CheckWall()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, Mathf.Infinity, levelManager.WallLayerMask))
        {
            Wall wall = hit.transform.GetComponent<Wall>();
            if (wall)
            {
                if (currentWall)
                {
                    currentWall.Clock.DeactivateClock();
                    currentWall.Clock.Unmark();
                }
                currentWall = wall;
                if (state == PlayerState.WAITING && hit.distance > 1)
                    ChangeState(PlayerState.RUNNING);
                currentWall.Clock.Mark();
            }
        }
        else if (currentWall)
        {
            currentWall.Clock.DeactivateClock();
            currentWall.Clock.Unmark();
            currentWall = null;
        }
        if (currentWall && (state == PlayerState.RUNNING || state == PlayerState.WAITING))
        {
            float currentWallPos = levelManager.PathCreator.path.GetClosestDistanceAlongPath(currentWall.transform.position);
            if (distance + 0.4f >= currentWallPos)
            {
                if (state == PlayerState.RUNNING)
                    ChangeState(PlayerState.WAITING);
                currentWall.Clock.ActivateClock();
                float gain = currentWall.WallDestruction();
                if (gain < 0)
                    AddHealth(gain);
                else
                {
                    currentWall.DestroySelf();
                    currentWall = null;
                    if (state == PlayerState.WAITING)
                        ChangeState(PlayerState.RUNNING);
                }
            }
        }
        else if (!currentWall && state == PlayerState.WAITING)
        {
            ChangeState(PlayerState.RUNNING);
        }

    }

    private void CheckCheckpoints()
    {
        if (levelManager.Checkpoints.Length == 0)
            return;
        if (closestCheckPointIndex >= 0 && distance >= closestCheckPointPosition)
        {
            levelManager.Checkpoints[closestCheckPointIndex].Activate();
        }
        closestCheckPointPosition = levelManager.PathCreator.path.GetClosestDistanceAlongPath(levelManager.Checkpoints[0].transform.position);
        closestCheckPointIndex = 0;
        for (int i = 1; i < levelManager.Checkpoints.Length; i++)
        {
            float position = levelManager.PathCreator.path.GetClosestDistanceAlongPath(levelManager.Checkpoints[i].transform.position);

            if (Mathf.Abs(position - distance) < Mathf.Abs(closestCheckPointPosition - distance) && position > distance)
            {
                closestCheckPointPosition = position;
                closestCheckPointIndex = i;
            }

        }

    }

    private void CheckPool()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, Mathf.Infinity, levelManager.PoolLayerMask))
        {
            Pool pool = hit.transform.GetComponent<Pool>();
            if (pool)
            {
                if (state == PlayerState.RUNNING)
                    ChangeState(PlayerState.WALKING);
                if (state == PlayerState.WALKING)
                {
                    if (pool.Type == Pool.PoolType.GOOD)
                        AddHealth(Time.deltaTime * 15);
                    else if (pool.Type == Pool.PoolType.BAD)
                        AddHealth(-Time.deltaTime * pool.Speed);
                }

            }
        }
        else if (state == PlayerState.WALKING)
            ChangeState(PlayerState.RUNNING);
    }

    public void AddHealth(float gain)
    {
        desiredHealth = Mathf.Clamp(desiredHealth + gain, 0, maxHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collectible collectible = other.GetComponent<Collectible>();
        if (collectible)
            collectible.GetCollected();
    }
}
