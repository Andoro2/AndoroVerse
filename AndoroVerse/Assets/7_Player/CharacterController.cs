using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.Audio;
using TMPro;

public class CharacterController : MonoBehaviour
{
    private UnityEngine.CharacterController m_Controller;
    private Inventory m_Inventario;
    private enum CharStates { Moving, Interacting, Chaneling}
    [SerializeField] private CharStates CharState = CharStates.Moving;
    private enum Weapons { Weaponless, Scepter, Shields, Sword }
    [SerializeField] private Weapons ActiveWeapon = Weapons.Weaponless;

    public Transform m_Target;

    //INTERACTION
    public Transform m_InteractionPoint;
    public float m_InteractRadius = 5f;
    public LayerMask m_InteractLayer;
    public TextMeshProUGUI m_InteractionText;
    private GameObject m_InteractObject;

    //DASH
    private bool m_IsDashing = false;
    private float m_DashTimer = 0f,
        m_DashCooldownTimer = 0f;
    [SerializeField]
    private float m_DashDistance = 5f,
        m_DashTime = 0.2f,
        m_DashCooldown = 1f,
        m_DashSpeed = 10f;

    private InputManager playerInputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 lookDirection, lastLookDirection, lastMoveDirection;
    public float MoveSpeed, m_WalkSpeed = 5f, m_RunSpeed = 10f;

    private Vector3 m_VerticalVelocity;
    public float gravity = -9.81f;

    public float m_JumpForce = 3f, m_JumpTime = 2f, m_AirTimeCounter;
    public int maxJumps = 1;
    public float m_CoyoteTime = 0.5f;
    private float m_CoyoteTimeCounter;
    private bool m_IsGrounded, m_IsJumping;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public GameObject bullet;

    [SerializeField] private float m_ComboTime = 2f, m_AttackCooldown = 0.2f;
    private float m_ComboTimer;
    public float m_GroundSlashCharge = 0f;
    public bool m_HeavyPressed = false;
    [SerializeField] private int m_ComboCount = 0; 
    public bool CanAttack = true;

    //WEAPONS POSITIONING
    private GameObject m_ShieldRDrawn, m_ShieldRSaved, m_ShieldLDrawn,
        m_ShieldLSaved, m_Sword, m_SwordBoneSaved, m_SwordBoneDrawn,
        m_Scepter, m_ScepterBoneDrawn;

    //WEAPONS VFX
    public List<Slash> m_SlashVFXs;
    public GameObject m_RockPunch, m_GroundSlash;

    public Animator m_AnimatorController;
    private float m_FightingCoolDown = 10f, m_FightingCounter;
    private bool m_ScepterHeaving = false;

    public GameObject m_ActiveWeaponSprite;
    private GameObject m_SwordSprite, m_ScepterSprite, m_ShieldsSprite;

    //SOUND FX
    private AudioSource m_PlayerAS, m_DashAS, m_MovementAS, m_MeleAttackAS, m_ScepterAS;
    public AudioClip m_WalkConcreteSFX, m_WalkGrassSFX, m_WalkWoodSFX,
        m_DrawSwordSFX, m_SwordSwoosh1, m_SwordSwoosh2, m_MetallicImpactSFX, m_FlameThrowerIgniteSFX, m_FlameThrowerFireSFX;


    private void Awake()
    {
        playerInputActions = new InputManager();
    }
    private void OnEnable()
    {
        playerInputActions.Enable();

        playerInputActions.Actions.RangeAttack.performed += OnHeavyPerformed;
        playerInputActions.Actions.RangeAttack.canceled += OnHeavyCanceled;
    }
    private void OnDisable()
    {
        playerInputActions.Disable();

        playerInputActions.Actions.RangeAttack.performed -= OnHeavyPerformed;
        playerInputActions.Actions.RangeAttack.canceled -= OnHeavyCanceled;
    }
    private void Start()
    {
        string LeftShieldPath = "Body/Model/rig/c_pos/c_traj/c_root_master.x/c_spine_01.x/c_spine_02.x/c_spine_03.x/" +
            "c_arm_fk.l/c_forearm_fk.l/forearm_fk.l/LeftShield/";
        string RightShieldPath = "Body/Model/rig/c_pos/c_traj/c_root_master.x/c_spine_01.x/c_spine_02.x/c_spine_03.x/" +
            "c_arm_fk.r/c_forearm_fk.r/forearm_fk.r/RightShield/";

        m_Inventario = GameObject.FindWithTag("GameController").GetComponent<Inventory>();
        m_Controller = GetComponent<UnityEngine.CharacterController>();

        m_ShieldLDrawn = transform.Find(LeftShieldPath + "ShieldDrawn").gameObject;
        m_ShieldLSaved = transform.Find(LeftShieldPath + "Shield_Saved").gameObject;

        m_ShieldRDrawn = transform.Find(RightShieldPath + "ShieldDrawn").gameObject;
        m_ShieldRSaved = transform.Find(RightShieldPath + "Shield_Saved").gameObject;

        m_Sword = transform.Find("Body/Weapons/Sword").gameObject;
        m_SwordBoneSaved = transform.Find("Body/Model/rig/c_pos/c_traj/c_root_master.x/" +
            "c_spine_01.x/c_spine_02.x/c_spine_03.x/c_shoulder.r/SwordSaveHolder").gameObject;
        m_Sword.transform.SetParent(m_SwordBoneSaved.transform);

        string ScepterPath = "Body/Model/rig/c_pos/c_traj/c_root_master.x/c_spine_01.x/c_spine_02.x/c_spine_03.x/" +
            "c_arm_fk.r/c_forearm_fk.r/forearm_fk.r/c_hand_fk_scale_fix.r/c_hand_fk.r";

        m_Scepter = transform.Find(ScepterPath + "/Scepter").gameObject;

        m_SwordBoneDrawn = transform.Find(ScepterPath + "/SwordDrawHolder").gameObject;

        DetectInputDevice();

        m_AnimatorController = transform.Find("Body/Model").gameObject.GetComponent<Animator>();
        m_FightingCounter = 0f;

        if(m_ActiveWeaponSprite != null)
        {
            m_SwordSprite = m_ActiveWeaponSprite.transform.GetChild(0).gameObject;
            m_ScepterSprite = m_ActiveWeaponSprite.transform.GetChild(1).gameObject;
            m_ShieldsSprite = m_ActiveWeaponSprite.transform.GetChild(2).gameObject;
        }

        m_PlayerAS = GetComponent<AudioSource>();
        m_DashAS = transform.Find("DashTarget").Find("AS_Dash").GetComponent<AudioSource>();
        m_MovementAS = transform.Find("DashTarget").Find("AS_Movement").GetComponent<AudioSource>();
        m_MeleAttackAS = transform.Find("Body").Find("Weapons").Find("WeaponAudioSources").Find("AS_MeleeAttack").GetComponent<AudioSource>();
        m_ScepterAS = transform.Find("Body").Find("Weapons").Find("WeaponAudioSources").Find("AS_FlameThrower").GetComponent<AudioSource>();

        m_ShootTimer = m_ShootCooldown;
    }

    private void Update()
    {

        m_Scepter.gameObject.transform.localScale = new Vector3(1, 1, 1);
        m_IsGrounded = Physics.CheckSphere(groundCheck.position, 0.25f, groundLayer);

        if (m_IsGrounded) m_AnimatorController.SetBool("Grounded", true);

        if (CheckInteraction())
        {
            Interact();
        }

        if(CharState == CharStates.Moving)
        {
            Move();
            Look();
            Dash();
            Jump();

            FightingTimer();
            SwitchWeapon();
            Shoot();
            QuickAttackCombo();
            HeavyAttack();
        }

        if (CharState == CharStates.Chaneling)
        {
            if(ActiveWeapon == Weapons.Sword) HeavyAttack();
            transform.Find("Body/Weapons/ScepterHeavyVFX/ScepterHeavyVFX").gameObject.GetComponent<VisualEffect>().Play();
            Look();
        }
    }
    private void Interact()
    {
        if (playerInputActions.Actions.Interaction.triggered)
        {
            if (m_InteractObject.GetComponent<InteractionType>().IsDialogue())
            {
                m_InteractObject.GetComponent<Dialogue>().InteractionManager();

                if (!m_InteractObject.GetComponent<Dialogue>().m_CanMove)
                {
                    CharState = CharStates.Interacting;
                }
                else
                {
                    CharState = CharStates.Moving;
                    if (m_InteractObject.GetComponent<Dialogue>().m_Follow)
                    {
                        m_InteractObject.gameObject.transform.parent = gameObject.transform;
                    }
                }
            }

            else if (m_InteractObject.GetComponent<InteractionType>().IsPickUpOrDropDown())
            {
                m_InteractObject.GetComponent<PickOrDrop>().PickOrDropObject();
            }

            else if (m_InteractObject.GetComponent<InteractionType>().IsCollectible())
            {
                m_InteractObject.GetComponent<GetCollectible>().AcquireCollectible();
            }

            else if (m_InteractObject.GetComponent<InteractionType>().IsGetIn())
            {
                m_InteractObject.GetComponent<GetIn>().LoadScene();
            }

            else if (m_InteractObject.GetComponent<InteractionType>().IsObserve())
            {
                m_InteractObject.GetComponent<Observe>().InteractionManager();

                if (!m_InteractObject.GetComponent<Observe>().m_CanMove)
                {
                    CharState = CharStates.Interacting;
                }
                else
                {
                    CharState = CharStates.Moving;
                }
            }
        }
    }
    private bool CheckInteraction()
    {
        Vector3 sphereCenter = transform.position;

        Collider[] colliders = Physics.OverlapSphere(sphereCenter, m_InteractRadius, m_InteractLayer);

        if (colliders.Length > 0)
        {
            m_InteractObject = colliders[0].gameObject;

            if (m_InteractObject.GetComponent<Dialogue>() != null && !m_InteractObject.GetComponent<Dialogue>().m_DialogueMenu.activeSelf)
            {
                if (m_InteractObject.GetComponent<Dialogue>().m_AutomaticDialogue)
                {
                    if (m_InteractObject.GetComponent<Dialogue>().m_Follow)
                    {
                        m_InteractObject.gameObject.transform.parent = gameObject.transform;
                    }
                    m_InteractObject.GetComponent<Dialogue>().StartDialogue();
                    return true;
                }
            }
            
            if(m_InteractObject.GetComponent<Dialogue>() != null && m_InteractObject.GetComponent<Dialogue>().m_AutomaticDialogue)
            {
                m_InteractionText.text = "";
            }
            else
            {
                m_InteractionText.text = m_InteractObject.GetComponent<InteractionType>().InteractionName;
            }
            
            if (m_InteractObject.GetComponent<InteractionType>().IsReaction())
            {
                m_InteractObject.GetComponent<React>().MainReact();
                return false;
            }
            return true;
        }
        else if (colliders.Length == 0 && m_InteractObject != null)
        {
            if (m_InteractObject.GetComponent<InteractionType>().IsDialogue())
            {
                m_InteractObject.GetComponent<Dialogue>().CutConversation();
            }
            else if (m_InteractObject.GetComponent<InteractionType>().IsObserve())
            {
                m_InteractObject.GetComponent<Observe>().Close();
            }
            m_InteractObject = null;
            m_InteractionText.text = "";
            return false;
        }
        else
        {
            m_InteractObject = null;
            m_InteractionText.text = "";
            return false;
        }
    }
    private void FightingTimer()
    {
        if(m_FightingCounter > 0f)
        {
            m_FightingCounter -= Time.deltaTime;
            if (!m_AnimatorController.GetBool("Fighting"))
            {
                m_AnimatorController.SetBool("Fighting", true);
            }
        }
        else
        {   
            if (CheckWeapon("Shields"))
            {
                m_ShieldRDrawn.SetActive(false);
                m_ShieldLDrawn.SetActive(false);
                m_ShieldRSaved.SetActive(true);
                m_ShieldLSaved.SetActive(true);
            }
            

            if (m_Scepter.activeSelf)
            {
                m_Scepter.GetComponent<ScepterWAnim>().ScepterSave();
            }

            //m_Scepter.SetActive(false);
            if (ActiveWeapon == Weapons.Sword)
            {
                StartCoroutine("SaveSword");
            }
            //m_Sword.gameObject.transform.SetParent(m_SwordBoneSaved.transform);

            if (m_AnimatorController.GetBool("Fighting"))
            {
                m_AnimatorController.SetTrigger("SaveSword");
                m_AnimatorController.SetTrigger("SaveShields");
                m_AnimatorController.SetTrigger("SaveScepter");
                
            }
            m_AnimatorController.SetBool("Fighting", false);
        }
    }
    IEnumerator AudioFire()
    {
        m_ScepterAS.clip = m_FlameThrowerIgniteSFX;
        yield return new WaitForSeconds(0.9f);

        m_ScepterAS.clip = m_FlameThrowerFireSFX;
        m_ScepterAS.Play();
    }
    private void HeavyAttack()
    {
        GameObject ScepterHeavy = transform.Find("Body/Weapons/ScepterHeavyVFX").gameObject;

        switch (ActiveWeapon)
        {
            case Weapons.Scepter:
                if (playerInputActions.Actions.RangeAttack.ReadValue<float>() != 0f)
                {
                    if (ScepterHeavy.gameObject.activeSelf == false)
                    {
                        ScepterHeavy.SetActive(true);
                    }

                    CharState = CharStates.Chaneling;

                    m_FightingCounter = m_FightingCoolDown;

                    m_AnimatorController.SetBool("ScepterShotgun", true);

                    StartCoroutine("AudioFire");
                    m_ScepterAS.loop = true;
                    if(!m_ScepterAS.isPlaying) m_ScepterAS.Play();

                    //transform.Find("Body/Weapons/ScepterHeavyVFX/ScepterHeavyVFX").gameObject.SetActive(true);
                }
                else m_ScepterAS.loop = false;
                break;
            case Weapons.Shields:
                if (playerInputActions.Actions.RangeAttack.triggered && m_IsGrounded)
                {
                    GameObject RockPunchVFX = Instantiate(m_RockPunch, transform.position, transform.rotation);
                    RockPunchVFX.transform.parent = null;
                }
                break;
            case Weapons.Sword:
                if(m_HeavyPressed && m_IsGrounded)
                {
                    CharState = CharStates.Chaneling;

                    m_GroundSlashCharge += Time.deltaTime;

                    m_AnimatorController.SetBool("SwordHeavyLoad", true);

                    if (m_GroundSlashCharge >= 2f)
                    {
                        GameObject GroundSlashVFX = Instantiate(m_GroundSlash, transform.position, transform.rotation);
                        GroundSlashVFX.transform.parent = null;

                        m_GroundSlashCharge = 0f;
                        m_AnimatorController.SetTrigger("SwordHeavyShoot");
                        m_HeavyPressed = false;

                        m_AnimatorController.SetBool("SwordHeavyLoad", false);
                    }
                }
                else
                {
                    m_AnimatorController.SetBool("SwordHeavyLoad", false);
                }
                break;
        }
    }
    private void OnHeavyPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            m_HeavyPressed = true;
        }
    }
    private void OnHeavyCanceled(InputAction.CallbackContext context)
    {
        if (ActiveWeapon == Weapons.Scepter)
        {
            GameObject ScepterHeavy = transform.Find("Body/Weapons/ScepterHeavyVFX").gameObject;

            if (ScepterHeavy.gameObject.activeSelf == true)
            {
                ScepterHeavy.SetActive(false);
            }
            m_AnimatorController.SetBool("ScepterShotgun", false);
        }
        if (ActiveWeapon == Weapons.Sword)
        {
            m_GroundSlashCharge = 0f;
            m_HeavyPressed = false;
            m_AnimatorController.SetBool("SwordHeavyLoading", false);
            Debug.Log("Soltado");
        }
        CharState = CharStates.Moving;
    }
    private void QuickAttackCombo()
    {
        if (m_ComboTimer > 0) m_ComboTimer -= Time.deltaTime;
        else m_ComboCount = 0;

        if (playerInputActions.Actions.QuickAttack.triggered && CanAttack)
        {
            if(ActiveWeapon != Weapons.Scepter && ActiveWeapon != Weapons.Weaponless)
            {
                m_MeleAttackAS.Play();
            }

            switch (m_ComboCount)
            {
                case 0:
                    if(ActiveWeapon != Weapons.Scepter)
                    {
                        m_ComboCount++;
                    }
                    
                    if(ActiveWeapon == Weapons.Sword)
                    {
                        StartCoroutine("SlashVFX", m_SlashVFXs[0]);
                    }
                    else if (ActiveWeapon == Weapons.Shields)
                    {
                        StartCoroutine("PunchHitBox", this.gameObject.transform.Find("Body").Find("Shields").gameObject);
                    }

                    if (m_AnimatorController.GetBool("Fighting") && m_AnimatorController.GetBool("Sword"))
                    {
                        StartCoroutine("DrawSword", 0.3f);
                    }

                    if(ActiveWeapon == Weapons.Scepter && !transform.Find("Body/Weapons/ScepterShock").gameObject.activeSelf)
                    {
                        StartCoroutine("ScepterStomp");
                    }

                    m_AnimatorController.SetBool("Fighting", true);
                    m_FightingCounter = m_FightingCoolDown;
                    m_ComboTimer = m_ComboTime;

                    m_AnimatorController.SetTrigger("Quick1");

                    break;
                case 1:
                    m_ComboCount++;

                    m_AnimatorController.SetTrigger("Quick2");

                    if (ActiveWeapon == Weapons.Sword)
                    {
                        StartCoroutine("SlashVFX", m_SlashVFXs[1]);
                    }
                    else if(ActiveWeapon == Weapons.Shields)
                    {
                        StartCoroutine("PunchHitBox", this.gameObject.transform.Find("Body").Find("Shields").gameObject);
                    }

                    break;
                case 2:
                    m_ComboCount = 0;

                    m_AnimatorController.SetTrigger("Quick3");

                    if (ActiveWeapon == Weapons.Sword)
                    {
                        StartCoroutine("SlashVFX", m_SlashVFXs[2]);
                    }
                    else if (ActiveWeapon == Weapons.Shields)
                    {
                        StartCoroutine("PunchHitBox", this.gameObject.transform.Find("Body").Find("Shields").gameObject);
                    }

                    break;
            }
        }
    }
    IEnumerator ScepterStomp()
    {
        yield return new WaitForSeconds(0.65f);
        transform.Find("Body/Weapons/ScepterShock").gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        transform.Find("Body/Weapons/ScepterShock").gameObject.SetActive(false);
    }
    [System.Serializable]
    public class Slash
    {
        public GameObject SlashVFX;
        public float Delay;
    }
    IEnumerator SlashVFX(Slash m_SlashVFX)
    {
        yield return new WaitForSeconds(m_SlashVFX.Delay);
        m_SlashVFX.SlashVFX.SetActive(true);

        yield return new WaitForSeconds(1f);
        m_SlashVFX.SlashVFX.SetActive(false);
    }
    IEnumerator PunchHitBox(GameObject PunchCollider)
    {
        yield return new WaitForSeconds(0.2f);
        PunchCollider.SetActive(true);

        yield return new WaitForSeconds(1f);
        PunchCollider.SetActive(false);
    }
    private float m_ShootTimer = 0f;
    public float m_ShootCooldown = 2.5f;
    private void Shoot()
    {
        if(m_ShootTimer > 0f)
        {
            m_ShootTimer -= Time.deltaTime;
        }
        if (moveInput.magnitude > 0.1f && playerInputActions.Actions.HeavyAttack.triggered && m_ShootTimer <= 0f)
        {
            Instantiate(bullet, m_Target.position, transform.rotation);
            m_AnimatorController.SetTrigger("Range");
            m_AnimatorController.SetBool("Fighting", true);
            m_FightingCounter = m_FightingCoolDown;

            m_ShootTimer = m_ShootCooldown;
        }
        else if (moveInput.magnitude < 0.1f && playerInputActions.Actions.HeavyAttack.triggered && m_ShootTimer <= 0f)
        {
            Instantiate(bullet, m_Target.position, transform.rotation);
            m_AnimatorController.SetTrigger("Range");
            m_AnimatorController.SetBool("Fighting", true);
            m_FightingCounter = m_FightingCoolDown;

            m_ShootTimer = m_ShootCooldown;
        }

    }
    private void SwitchWeapon()
    {
        switch (ActiveWeapon)
        {
            case Weapons.Weaponless:
                if (m_SwordSprite != null) m_SwordSprite.SetActive(false);
                if (m_ScepterSprite != null) m_ScepterSprite.SetActive(false);
                if (m_ShieldsSprite != null) m_ShieldsSprite.SetActive(false);
                break;
            case Weapons.Shields:
                if (m_SwordSprite != null) m_SwordSprite.SetActive(false);
                if (m_ScepterSprite != null) m_ScepterSprite.SetActive(false);
                if (m_ShieldsSprite != null) m_ShieldsSprite.SetActive(true);
                break;
            case Weapons.Scepter:
                if (m_SwordSprite != null) m_SwordSprite.SetActive(false);
                if (m_ScepterSprite != null) m_ScepterSprite.SetActive(true);
                if (m_ShieldsSprite != null) m_ShieldsSprite.SetActive(false);
                break;
            case Weapons.Sword:
                if (m_SwordSprite != null) m_SwordSprite.SetActive(true);
                if (m_ScepterSprite != null) m_ScepterSprite.SetActive(false);
                if (m_ShieldsSprite != null) m_ShieldsSprite.SetActive(false);
                break;
        }

        if (playerInputActions.Actions.WeaponOne.triggered && CheckWeapon("Sword")) //SWORD
        {
            StartCoroutine("DrawSword", 1.2f);
            ActiveWeapon = Weapons.Sword;
            //m_Sword.gameObject.transform.SetParent(m_SwordBoneDrawn.transform);
            m_FightingCounter = m_FightingCoolDown;
            switch (ActiveWeapon)
            {
                case Weapons.Weaponless:
                    break;
                case Weapons.Shields:
                    m_AnimatorController.SetTrigger("SaveShields");
                    break;
                case Weapons.Scepter:
                    if (m_Scepter.activeSelf)
                    {
                        m_Scepter.GetComponent<ScepterWAnim>().ScepterSave();
                    }
                    m_AnimatorController.SetTrigger("SaveScepter");
                    break;
            }
            m_AnimatorController.SetTrigger("DrawSword");
            m_AnimatorController.SetBool("FromSword", true);
        }

        if (playerInputActions.Actions.WeaponTwo.triggered && CheckWeapon("Scepter")) //SCEPTER
        {
            ActiveWeapon = Weapons.Scepter;
            m_FightingCounter = m_FightingCoolDown;
            switch (ActiveWeapon)
            {
                case Weapons.Shields:
                    m_AnimatorController.SetTrigger("SaveShields");
                    break;
                case Weapons.Sword:
                    m_AnimatorController.SetTrigger("WeaponChanging");
                    StartCoroutine("SaveSword");
                    //m_Sword.gameObject.transform.SetParent(m_SwordBoneSaved.transform);
                    break;
            }
            m_AnimatorController.SetTrigger("DrawScepter");
            m_AnimatorController.SetBool("FromSword", false);
        }

        if (playerInputActions.Actions.WeaponThree.triggered && CheckWeapon("Shields")) //SHIELDS
        {
            m_FightingCounter = m_FightingCoolDown;
            switch (ActiveWeapon)
            {
                case Weapons.Weaponless:
                    break;
                case Weapons.Sword:
                    m_AnimatorController.SetTrigger("WeaponChanging");
                    StartCoroutine("SaveSword");
                    //m_Sword.gameObject.transform.SetParent(m_SwordBoneSaved.transform);
                    break;
                case Weapons.Scepter:
                    if (m_Scepter.activeSelf)
                    {
                        m_Scepter.GetComponent<ScepterWAnim>().ScepterSave();
                    }
                    m_AnimatorController.SetTrigger("SaveScepter");
                    break;
            }
            ActiveWeapon = Weapons.Shields;
            m_AnimatorController.SetTrigger("DrawShields");
            m_AnimatorController.SetBool("FromSword", false);
        }

        if (m_AnimatorController.GetBool("Fighting"))
        {
            switch (ActiveWeapon)
            {
                case Weapons.Scepter:
                    if (CheckWeapon("Shields"))
                    {
                        m_ShieldRDrawn.SetActive(false);
                        m_ShieldLDrawn.SetActive(false);
                        m_ShieldRSaved.SetActive(true);
                        m_ShieldLSaved.SetActive(true);
                    }
                    m_Scepter.SetActive(true);
                    //m_Scepter.gameObject.transform.localScale = new Vector3(1, 1, 1);

                    StartCoroutine("SaveSword");
                    //m_Sword.gameObject.transform.SetParent(m_SwordBoneSaved.transform);

                    m_AnimatorController.SetBool("Shields", false);
                    m_AnimatorController.SetBool("Sword", false);
                    m_AnimatorController.SetBool("Scepter", true);
                    
                    break;
                case Weapons.Shields:
                    m_ShieldRDrawn.SetActive(true);
                    m_ShieldLDrawn.SetActive(true);
                    m_ShieldRSaved.SetActive(false);
                    m_ShieldLSaved.SetActive(false);

                    if (m_Scepter.activeSelf)
                    {
                        m_Scepter.GetComponent<ScepterWAnim>().ScepterSave();
                    }
                    //m_Scepter.SetActive(false);

                    StartCoroutine("SaveSword");
                    //m_Sword.gameObject.transform.SetParent(m_SwordBoneSaved.transform);

                    m_AnimatorController.SetBool("Shields", true);
                    m_AnimatorController.SetBool("Sword", false);
                    m_AnimatorController.SetBool("Scepter", false);

                    break;
                case Weapons.Sword:
                if (CheckWeapon("Shields"))
                    {
                        m_ShieldRDrawn.SetActive(false);
                        m_ShieldLDrawn.SetActive(false);
                        m_ShieldRSaved.SetActive(true);
                        m_ShieldLSaved.SetActive(true);
                    }

                    if (m_Scepter.activeSelf)
                    {
                        m_Scepter.GetComponent<ScepterWAnim>().ScepterSave();
                    }
                    //m_Scepter.SetActive(false);

                    StartCoroutine("DrawSword", 1.2f);
                    //m_Sword.gameObject.transform.SetParent(m_SwordBoneDrawn.transform);

                    m_AnimatorController.SetBool("Shields", false);
                    m_AnimatorController.SetBool("Sword", true);
                    m_AnimatorController.SetBool("Scepter", false);

                    break;
                case Weapons.Weaponless:
                    if (CheckWeapon("Shields"))
                    {
                        m_ShieldRDrawn.SetActive(false);
                        m_ShieldLDrawn.SetActive(false);
                        m_ShieldRSaved.SetActive(true);
                        m_ShieldLSaved.SetActive(true);
                    }

                    if (m_Scepter.activeSelf)
                    {
                        m_Scepter.GetComponent<ScepterWAnim>().ScepterSave();
                    }
                    //m_Scepter.SetActive(false);

                    m_AnimatorController.SetBool("Scepter", false);
                    m_AnimatorController.SetBool("Shields", false);
                    m_AnimatorController.SetBool("Sword", false);
                    break;
            }
        }


        m_Sword.transform.localPosition = Vector3.zero;
        m_Sword.transform.localRotation = new Quaternion(0, 0, 0, 1);
    }
    private IEnumerator SaveSword()
    {
        m_MeleAttackAS.clip = m_DrawSwordSFX;
        m_MeleAttackAS.loop = false;
        m_MeleAttackAS.Play();
        yield return new WaitForSeconds(0.9f);
        m_Sword.gameObject.transform.SetParent(m_SwordBoneSaved.transform);
    }
    private IEnumerator DrawSword(float WaitTime)
    {
        m_MeleAttackAS.clip = m_DrawSwordSFX;
        m_MeleAttackAS.loop = false;
        m_MeleAttackAS.Play();
        yield return new WaitForSeconds(WaitTime);//1.2f
        m_Sword.gameObject.transform.SetParent(m_SwordBoneDrawn.transform);
    }
    private bool CheckWeapon(string WeaponName)
    {
        bool HasSkill = false;
        for (int i = 0; i < m_Inventario.m_Skills.Count; i++)
        {
            if(m_Inventario.m_Skills[i].SkillName == WeaponName)
            {
                HasSkill = m_Inventario.m_Skills[i].Available;
            }
            if (HasSkill)
            {
                break;
            }
        }

        if (HasSkill)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Jump()
    {
        bool JumpAvailable = false;
        for (int i = 0; i < m_Inventario.m_Skills.Count; i++)
        {
            if (m_Inventario.m_Skills[i].SkillName == "Jump")
            {
                if (JumpAvailable = m_Inventario.m_Skills[i].Available)
                {
                    break;
                }
            }
        }
        if (JumpAvailable)
        {
            if (m_IsGrounded)
            {
                m_CoyoteTimeCounter = m_CoyoteTime;
            }
            else
            {
                m_CoyoteTimeCounter -= Time.deltaTime;
            }

            if (m_CoyoteTimeCounter > 0f && playerInputActions.Movement.Jump.triggered)
            {
                m_AnimatorController.SetTrigger("Jump");
                m_AnimatorController.SetBool("Grounded", false);
                m_VerticalVelocity = Vector3.up * m_JumpForce;
                m_IsJumping = true;
                m_AirTimeCounter = m_JumpTime;
            }

            if (playerInputActions.Movement.Jump.ReadValue<float>() != 0 && m_Controller.velocity.y > 0f)
            {
                m_AnimatorController.SetBool("Grounded", false);
                if (m_AirTimeCounter > 0 && m_IsJumping)
                {
                    m_VerticalVelocity = Vector3.up * m_JumpForce;
                    m_AirTimeCounter -= Time.deltaTime;
                }
                else
                {
                    if (m_IsJumping)
                    {
                        m_IsJumping = false;
                        m_VerticalVelocity.y = 0f;
                    }
                }
            }

            if (playerInputActions.Movement.Jump.ReadValue<float>() == 0)
            {
                if (m_IsJumping)
                {
                    m_IsJumping = false;
                    m_VerticalVelocity.y = 0f;
                }
                m_CoyoteTimeCounter = 0f;
            }
        }
        
    }
    private void Move()
    {
        if (m_IsGrounded && playerInputActions.Movement.Jump.ReadValue<float>() == 0)
        {
            m_VerticalVelocity.y = -1f;
        }
        else if (!m_IsJumping)
        {
            m_VerticalVelocity.y += gravity * Time.deltaTime;
        }

        
        //controller.Move(velocity * Time.deltaTime);
        Vector3 moveDirection = new(moveInput.x, 0f, moveInput.y);
        moveDirection = MoveSpeed * Time.deltaTime * moveDirection.normalized;
        moveDirection.y = m_VerticalVelocity.y * Time.deltaTime;
        //transform.position += moveDirection;

        if(moveInput.magnitude != 0)
        {
            m_AnimatorController.SetBool("Walking", true);

            m_MovementAS.clip = m_WalkWoodSFX;
            if(!m_MovementAS.isPlaying) m_MovementAS.Play();

            if (playerInputActions.Movement.Run.triggered)
            {
                MoveSpeed = m_RunSpeed;
                m_AnimatorController.SetBool("Troting", true);
            }
        }
        else
        {
            m_MovementAS.Stop();
            m_AnimatorController.SetBool("Walking", false);
            m_AnimatorController.SetBool("Troting", false);
            MoveSpeed = m_WalkSpeed;
        }

        _ = m_Controller.Move(MoveSpeed * Time.deltaTime * moveDirection);
    }
    private void Look()
    {
        //Vector3 
        lookDirection = new Vector3(lookInput.x, 0f, lookInput.y);

        Vector3 moveDirection = new(moveInput.x, 0f, moveInput.y);
        moveDirection = MoveSpeed * Time.deltaTime * moveDirection.normalized;

        if (lookDirection.magnitude != 0f)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            moveDirection.Normalize();
            lookDirection.Normalize();

            float angle = Vector3.Angle(moveDirection, lookDirection);
            bool movingBackwards = (angle > 90);

            if (movingBackwards)
            {
                m_AnimatorController.SetBool("Backwards", true);
            }
            else
            {
                m_AnimatorController.SetBool("Backwards", false);
            }


            lastLookDirection = lookDirection;
        }
        else //lookDirection.magnitude == 0f
        {
            m_AnimatorController.SetBool("Backwards", false);

            if(moveDirection.magnitude != 0)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            }

        }
    }
    private void Dash()
    {
        bool DashAvailable = false;
        for (int i = 0; i < m_Inventario.m_Skills.Count; i++)
        {
            if (m_Inventario.m_Skills[i].SkillName == "Dash")
            {
                if (DashAvailable = m_Inventario.m_Skills[i].Available)
                {
                    break;
                }
            }
        }
        if (!m_IsDashing && m_DashCooldownTimer <= 0f && DashAvailable)
        {
            if (moveInput.magnitude > 0.1f && playerInputActions.Movement.Dash.triggered)
            {
                StartCoroutine(PerformDash(moveInput));
            }
            else if (moveInput.magnitude < 0.1f && playerInputActions.Movement.Dash.triggered) //lookInput.magnitude > 0.1f 
            {
                Vector3 targetDir = m_Target.position - transform.position;
                Vector2 dashDir = new(targetDir.x, targetDir.z);
                StartCoroutine(PerformDash(dashDir));
            }
        }

        if (m_IsDashing)
        {
            m_DashTimer += Time.deltaTime;
            if (m_DashTimer >= m_DashTime)
            {
                m_IsDashing = false;
                m_DashTimer = 0f;
                m_DashCooldownTimer = m_DashCooldown;
            }
        }
        else
        {
            m_DashCooldownTimer -= Time.deltaTime;
            m_DashCooldownTimer = Mathf.Max(m_DashCooldownTimer, 0f);
        }
    }

    //DASH FUNCTION
    private SkinnedMeshRenderer[] m_SkinnedMeshRenderers;

    private GameObject[] m_WeaponsArray;

    private SkinnedMeshRenderer[] m_SkinnedMeshRenderersManual;

    public Material m_DashMaterial;
    private IEnumerator PerformDash(Vector2 dashDirection)
    {
        m_IsJumping = false;
        m_AirTimeCounter = 0f;
        m_VerticalVelocity.y = 0;

        m_IsDashing = true;

        m_DashAS.Play();

        float elapsed = 0f;
        //Vector3 startPosition = transform.position;
        Vector3 dashDir = new(dashDirection.x, 0f, dashDirection.y);
        //Vector3 dashPosition = transform.position + endPos.normalized * dashDistance;

        while (elapsed < m_DashTime)
        {
            if (m_SkinnedMeshRenderers == null)
            {
                m_SkinnedMeshRenderers = transform.Find("Body/Model").gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            switch (ActiveWeapon)
            {
                case Weapons.Scepter:
                    m_WeaponsArray = new GameObject[]
                    {
                            m_Sword.gameObject,//.GetComponent<Mesh>(),
                            m_ShieldLSaved.gameObject,//GetComponent<Mesh>(),
                            m_ShieldRSaved.gameObject,//GetComponent<Mesh>(),
                            m_Scepter.gameObject.transform.GetChild(0).gameObject//GetComponent<Mesh>()
                    };
                    break;
                case Weapons.Shields:
                    if (m_ShieldLDrawn.activeSelf || m_ShieldRDrawn.activeSelf)
                    {
                        m_WeaponsArray = new GameObject[]
                        {
                            m_Sword.gameObject,//GetComponent<Mesh>(),
                            m_ShieldLDrawn.transform.GetChild(0).gameObject,//GetComponent<Mesh>(),
                            m_ShieldLDrawn.transform.GetChild(1).gameObject,//GetComponent<Mesh>(),
                            m_ShieldRDrawn.transform.GetChild(0).gameObject,//GetComponent<Mesh>(),
                            m_ShieldRDrawn.transform.GetChild(1).gameObject//GetComponent<Mesh>()
                        };
                    }
                    else if (m_ShieldLSaved.activeSelf || m_ShieldRSaved.activeSelf)
                    {
                        m_WeaponsArray = new GameObject[]
                        {
                            m_Sword.gameObject,//GetComponent<Mesh>(),
                            m_ShieldLSaved.gameObject,//GetComponent<Mesh>(),
                            m_ShieldRSaved.gameObject//GetComponent<Mesh>(),
                        };
                    }

                    break;
                case Weapons.Sword:
                    m_WeaponsArray = new GameObject[]
                        {
                        m_Sword.gameObject,//GetComponent<Mesh>(),
                        m_ShieldLSaved.gameObject,//GetComponent<Mesh>(),
                        m_ShieldRSaved.gameObject//GetComponent<Mesh>(),
                        };

                    break;
                case Weapons.Weaponless:
                    m_WeaponsArray = new GameObject[]
                     {
                        m_Sword.gameObject,//GetComponent<Mesh>(),
                        m_ShieldLSaved.gameObject,//GetComponent<Mesh>(),
                        m_ShieldRSaved.gameObject//GetComponent<Mesh>(),
                     };

                    break;
            }
            
            for (int i = 0; i < m_SkinnedMeshRenderers.Length; i++)
            {
                GameObject GObj = new GameObject();
                GObj.transform.SetPositionAndRotation(m_SkinnedMeshRenderers[i].gameObject.transform.position, m_SkinnedMeshRenderers[i].gameObject.transform.rotation);

                MeshRenderer MRenderer = GObj.AddComponent<MeshRenderer>();
                MeshFilter MFilter = GObj.AddComponent<MeshFilter>();

                Mesh m_Mesh;

                m_Mesh = new Mesh();
                m_SkinnedMeshRenderers[i].BakeMesh(m_Mesh);
                MFilter.mesh = m_Mesh;

                MRenderer.material = m_DashMaterial;

                MRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                GObj.AddComponent<DashVFXFader>();
            }

            for (int i = 0; i < m_WeaponsArray.Length; i++)
            {
                MeshRenderer MRenderer = m_WeaponsArray[i].GetComponent<MeshRenderer>();

                if (MRenderer != null)
                {
                    GameObject GObj = Instantiate(m_WeaponsArray[i], m_WeaponsArray[i].transform.position, m_WeaponsArray[i].transform.rotation);
                    GObj.transform.localScale = new Vector3(100f, 100f, 100f);

                    MeshRenderer GObjMRenderer = GObj.GetComponent<MeshRenderer>();

                    Material[] mGObjMaterials = GObjMRenderer.materials;

                    for (int z = 0; z < mGObjMaterials.Length; z++)
                    {
                        mGObjMaterials[z] = m_DashMaterial;
                    }

                    GObjMRenderer.materials = mGObjMaterials;

                    GObj.AddComponent<DashVFXFader>();
                }
            }

            elapsed += Time.deltaTime;

            m_Controller.Move(m_DashSpeed * Time.deltaTime * dashDir);

            yield return null;
        }
    }
    //RECIEVE DAMAGE KNOCKBACK
    private float m_KnockBackTimer, m_KnockBackTime = 0.1f, m_KnockBackForce = 20f;
    public void TakeDamage(Vector3 KnockBackDir)
    {
        Vector3 AttackVector = KnockBackDir.normalized;

        Vector3 KnockBackDirection = new(AttackVector.x, 0f, AttackVector.z);

        m_KnockBackTimer = m_KnockBackTime;
        RecieveKB(KnockBackDirection);
    }
    public void RecieveKB(Vector3 KnockBackDir)
    {
        if (m_KnockBackTimer > 0f)
        {
            m_Controller.Move(KnockBackDir * m_KnockBackForce * Time.deltaTime);

            m_KnockBackTimer -= Time.deltaTime;
        }
    }
    private void DetectInputDevice()
    {
        string[] joystickNames = Input.GetJoystickNames();

        if (joystickNames.Length == 0 || joystickNames[0] == "")
        {
            // keyboard and mouse input
            playerInputActions.Movement.Move.performed += ctx => {
                Vector2 input = ctx.ReadValue<Vector2>();
                input = input.normalized;
                moveInput = new Vector2(input.x, input.y);
            };
            playerInputActions.Movement.Move.canceled += ctx => {
                moveInput = Vector2.zero;
            };

            playerInputActions.Movement.Aim.performed += ctx => {
                Vector2 input = ctx.ReadValue<Vector2>();
                input = input.normalized;
                lookInput = new Vector2(input.x, input.y);
            };
            playerInputActions.Movement.Aim.canceled += ctx => {
                lookInput = Vector2.zero;
            };
        }
        else
        {
            // gamepad input
            playerInputActions.Movement.Move.performed += ctx => {
                Vector2 input = ctx.ReadValue<Vector2>();
                moveInput = new Vector2(input.x, input.y);
            };
            playerInputActions.Movement.Move.canceled += ctx => {
                moveInput = Vector2.zero;
            };

            playerInputActions.Movement.Aim.performed += ctx => {
                Vector2 input = ctx.ReadValue<Vector2>();
                lookInput = new Vector2(input.x, input.y);
            };
            playerInputActions.Movement.Aim.canceled += ctx => {
                lookInput = Vector2.zero;
            };
        }
    }
}