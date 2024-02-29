using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.Audio;
using TMPro;
using System.Linq;

public class CharacterController : MonoBehaviour
{
    private UnityEngine.CharacterController m_Controller;
    private Inventory m_Inventario;
    private enum CharStates { Moving, Interacting, Chaneling, Talking, Dead}
    [SerializeField] private CharStates CharState = CharStates.Moving;
    private enum Weapons { Weaponless, Scepter, Shields, Sword }
    [SerializeField] private Weapons ActiveWeapon = Weapons.Weaponless;

    public Transform m_Target;

    //INTERACTION
    public Transform m_InteractionPoint;
    private float m_InteractRadius = 0.5f;
    public LayerMask m_InteractLayer;
    public TextMeshProUGUI m_InteractionText;
    private GameObject m_InteractObject;

    //DASH
    private bool m_IsDashing = false;
    private float m_DashTimer = 0f,
        m_DashCooldownTimer = 0f,
        m_DashTime = 0.2f,
        m_DashCooldown = 1f,
        m_DashSpeed = 10f;

    private InputManager m_PlayerInputActions;
    private Vector2 m_MoveInput,
        m_LookInput;
    private Vector3 m_LookDir, m_LastLookDir, m_LastMoveDir;
    private float MoveSpeed;
    public float m_WalkSpeed = 5f, m_RunSpeed = 10f;

    private Vector3 m_VerticalVelocity;
    public float gravity = -9.81f;

    public float m_JumpForce = 3f, m_JumpTime = 2f, m_AirTimeCounter;
    public int m_MaxJumps = 1;
    private float m_CoyoteTime = 0.5f, m_CoyoteTimeCounter;
    private bool m_IsGrounded, m_IsJumping;
    public Transform m_GroundCheck;
    public LayerMask m_GroundLayer;

    private Animator m_AnimatorController;

    [SerializeField] private float m_ComboTime = 2f, m_AttackCooldown = 0.2f;
    private float m_ComboTimer, m_GroundSlashCharge = 0f, m_RockPunchTimer = 0f,
        m_FightingCoolDown = 10f, m_FightingCounter;
    [SerializeField] private int m_ComboCount = 0;
    public float m_RockPunchCD = 2f;
    public bool m_HeavyPressed = false, CanAttack = false;

    //WEAPONS POSITIONING
    private GameObject m_ShieldR, m_ShieldL,
        m_ShieldRDrawn, m_ShieldRSaved, m_ShieldLDrawn,
        m_ShieldLSaved, m_Sword, m_SwordBoneSaved, m_SwordBoneDrawn,
        m_Scepter, m_ScepterBoneDrawn;

    //WEAPONS VFX
    public List<Slash> m_SlashVFXs;
    public GameObject m_Bullet, m_RockPunch, m_GroundSlash;

    // Weapon sprite on use visible on the GUI
    public GameObject m_ActiveWeaponSprite;
    private GameObject m_SwordSprite, m_ScepterSprite, m_ShieldsSprite;

    //SOUND FX
    private AudioSource m_PlayerAS, m_DashAS, m_MovementAS, m_MeleAttackAS, m_ScepterAS;
    public AudioClip m_WalkConcreteSFX, m_WalkGrassSFX, m_WalkWoodSFX,
        m_DrawSwordSFX, m_SwordSwoosh1, m_SwordSwoosh2, m_MetallicImpactSFX,
        m_FlameThrowerIgniteSFX, m_FlameThrowerFireSFX;

    Skill Escudos;
    private void Awake()
    {
        m_PlayerInputActions = new InputManager();
    }
    private void OnEnable()
    {
        m_PlayerInputActions.Enable();

        m_PlayerInputActions.Actions.RangeAttack.performed += OnHeavyPerformed;
        m_PlayerInputActions.Actions.RangeAttack.canceled += OnHeavyCanceled;
    }
    private void OnDisable()
    {
        m_PlayerInputActions.Disable();

        m_PlayerInputActions.Actions.RangeAttack.performed -= OnHeavyPerformed;
        m_PlayerInputActions.Actions.RangeAttack.canceled -= OnHeavyCanceled;
    }
    private void Start()
    {
        DetectInputDevice();

        string LeftShieldPath = "Body/Model/rig/c_pos/c_traj/c_root_master.x/c_spine_01.x/c_spine_02.x/c_spine_03.x/" +
            "c_arm_fk.l/c_forearm_fk.l/forearm_fk.l/LeftShield/";
        string RightShieldPath = "Body/Model/rig/c_pos/c_traj/c_root_master.x/c_spine_01.x/c_spine_02.x/c_spine_03.x/" +
            "c_arm_fk.r/c_forearm_fk.r/forearm_fk.r/RightShield/";

        m_Inventario = GameObject.FindWithTag("GameController").GetComponent<Inventory>();
        m_Controller = GetComponent<UnityEngine.CharacterController>();

        m_ShieldL = transform.Find(LeftShieldPath).gameObject;
        m_ShieldLDrawn = transform.Find(LeftShieldPath + "ShieldDrawn").gameObject;
        m_ShieldLSaved = transform.Find(LeftShieldPath + "Shield_Saved").gameObject;

        m_ShieldR = transform.Find(RightShieldPath).gameObject;
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

        foreach(Skill Habilidad in GameObject.FindWithTag("GameController").GetComponent<Inventory>().m_Skills)
        {
            if (Habilidad.SkillName == "Shields") Escudos = Habilidad;
        }

        m_RockPunchTimer = m_RockPunchCD;
    }
    private void Update()
    {
        m_Scepter.gameObject.transform.localScale = new Vector3(1, 1, 1);
        m_IsGrounded = Physics.CheckSphere(m_GroundCheck.position, 0.25f, m_GroundLayer);
        if (m_RockPunchTimer> 0f) m_RockPunchTimer -= Time.deltaTime;

        GameObject[] m_Dialogues = GameObject.FindGameObjectsWithTag("TextBox");
        if (m_Dialogues.Any(TextBox => TextBox.activeSelf))
        {
            GameObject ActiveTextBox = m_Dialogues.FirstOrDefault(TextBox => TextBox.activeSelf);

            if (ActiveTextBox.transform.parent.GetComponent<DialogueSecondOption>().m_CanMove)
            {
                CharState = CharStates.Talking;
                CanAttack = false;
                Move();
            }
        }
        else
        {
            CharState = CharStates.Moving;
            CanAttack = true;
        }

        if (m_IsGrounded) m_AnimatorController.SetBool("Grounded", true);

        if (CheckInteraction())
        {
            Interact();
        }

        if (CharState == CharStates.Talking || CharState == CharStates.Interacting) CanAttack = false;
        else CanAttack = true;

        if (CharState == CharStates.Moving || CharState == CharStates.Talking)
        {
            Look();
            Dash();
            Jump();

            if (CanAttack)
            {
                Move();
                FightingTimer();
                SwitchWeapon();
                Shoot();
                QuickAttackCombo();
                HeavyAttack();
            }
        }

        if (CharState == CharStates.Chaneling)
        {
            if(ActiveWeapon == Weapons.Sword) HeavyAttack();
            transform.Find("Body/Weapons/ScepterHeavyVFX/ScepterHeavyVFX").gameObject.GetComponent<VisualEffect>().Play();
            Look();
        }

        if (Escudos.Available && !m_ShieldL.activeSelf && !m_ShieldR.activeSelf)
        {
            m_ShieldL.gameObject.SetActive(true);

            m_ShieldR.gameObject.SetActive(true);
        }

        if (MainScript.PlayerLifePoints <= 0)
            Death();
    }
    private void Interact()
    {
        if (m_PlayerInputActions.Actions.Interaction.triggered)
        {
            if (m_InteractObject.GetComponent<InteractionType>().IsDialogue())
            {
                m_InteractObject.GetComponent<DialogueSecondOption>().InteractionManager();

                if (!m_InteractObject.GetComponent<DialogueSecondOption>().m_CanMove)
                {
                    CharState = CharStates.Interacting;
                    CanAttack = false;
                }
                else
                {
                    if (m_InteractObject.GetComponent<DialogueSecondOption>().m_Follow)
                    {
                        m_InteractObject.gameObject.transform.parent = gameObject.transform;
                        m_InteractObject.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                    }
                }
            }

            else if (m_InteractObject.GetComponent<InteractionType>().IsPickUpOrDropDown())
                m_InteractObject.GetComponent<PickOrDrop>().PickOrDropObject();

            else if (m_InteractObject.GetComponent<InteractionType>().IsCollectible())
                m_InteractObject.GetComponent<GetCollectible>().AcquireCollectible();

            else if (m_InteractObject.GetComponent<InteractionType>().IsGetIn())
                m_InteractObject.GetComponent<GetIn>().LoadScene();

            else if (m_InteractObject.GetComponent<InteractionType>().IsObserve())
            {
                m_InteractObject.GetComponent<Observe>().InteractionManager();

                if (!m_InteractObject.GetComponent<Observe>().m_CanMove)
                    CharState = CharStates.Interacting;
                else
                    CharState = CharStates.Moving;
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

            if (m_InteractObject.GetComponent<DialogueSecondOption>() != null && !m_InteractObject.GetComponent<DialogueSecondOption>().m_DialogueFrame.activeSelf)
            {
                if (m_InteractObject.GetComponent<DialogueSecondOption>().m_AutomaticDialogue)
                {
                    if (m_InteractObject.GetComponent<DialogueSecondOption>().m_Follow)
                    {
                        m_InteractObject.gameObject.transform.parent = gameObject.transform;
                    }
                    m_InteractObject.GetComponent<DialogueSecondOption>().InteractionManager();
                    return true;
                }
            }
            
            if(m_InteractObject.GetComponent<DialogueSecondOption>() != null && m_InteractObject.GetComponent<DialogueSecondOption>().m_AutomaticDialogue)
            {
                m_InteractionText.text = "";
                if (!m_InteractObject.GetComponent<DialogueSecondOption>().m_CanMove)
                {
                    CharState = CharStates.Interacting;
                }
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
                m_InteractObject.GetComponent<DialogueSecondOption>().CutConversation();
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
    public void BackToPlay()
    {
        if (CharState == CharStates.Interacting) CharState = CharStates.Moving;
    }
    public void OutOfPlay()
    {
        CharState = CharStates.Interacting;
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

            if (ActiveWeapon == Weapons.Sword)
            {
                StartCoroutine("SaveSword");
            }

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
        if (CanAttack)
        {
            GameObject ScepterHeavy = transform.Find("Body/Weapons/ScepterHeavyVFX").gameObject;

            switch (ActiveWeapon)
            {
                case Weapons.Scepter:
                    if (m_PlayerInputActions.Actions.RangeAttack.ReadValue<float>() != 0f)
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
                        if (!m_ScepterAS.isPlaying) m_ScepterAS.Play();
                    }
                    else m_ScepterAS.loop = false;
                    break;
                case Weapons.Shields:
                    if (m_PlayerInputActions.Actions.RangeAttack.triggered && m_IsGrounded
                        && m_RockPunchTimer <= 0f)
                    {
                        GameObject RockPunchVFX = Instantiate(m_RockPunch, transform.position, transform.rotation);
                        RockPunchVFX.transform.parent = null;
                        m_RockPunchTimer = m_RockPunchCD;
                    }
                    break;
                case Weapons.Sword:
                    if (m_HeavyPressed && m_IsGrounded)
                    {
                        CharState = CharStates.Chaneling;

                        m_FightingCounter = m_FightingCoolDown;

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
    }
    private void OnHeavyPerformed(InputAction.CallbackContext context)
    {
        if (context.performed && CanAttack)
            m_HeavyPressed = true;
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
        }
        CharState = CharStates.Moving;
    }
    private float m_QuickAttackCD = 0.5f;
    private void QuickAttackCombo()
    {
        if (m_ComboTimer > 0)
            m_ComboTimer -= Time.deltaTime;
        else
            m_ComboCount = 0;

        if (m_QuickAttackCD > 0)
            m_QuickAttackCD -= Time.deltaTime;

        if (m_PlayerInputActions.Actions.QuickAttack.triggered && CanAttack && m_QuickAttackCD <= 0f)
        {
            if(ActiveWeapon != Weapons.Scepter && ActiveWeapon != Weapons.Weaponless)
                m_MeleAttackAS.Play();

            switch (m_ComboCount)
            {
                case 0:
                    if(ActiveWeapon != Weapons.Scepter)
                        m_ComboCount++;
                    
                    if(ActiveWeapon == Weapons.Sword)
                    {
                        StartCoroutine("SlashVFX", m_SlashVFXs[0]);
                        m_QuickAttackCD = 0.8f;
                    }
                    else if (ActiveWeapon == Weapons.Shields)
                    {
                        StartCoroutine("PunchHitBox", this.gameObject.transform.Find("Body")
                            .Find("Weapons").Find("Shields").gameObject);
                    }

                    if (m_AnimatorController.GetBool("Fighting") && m_AnimatorController.GetBool("Sword"))
                    {
                        StartCoroutine("DrawSword", 0.3f);
                    }

                    if(ActiveWeapon == Weapons.Scepter && !transform.Find("Body/Weapons/ScepterShock")
                        .gameObject.activeSelf)
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
                        m_QuickAttackCD = 0.75f;
                    }
                    else if(ActiveWeapon == Weapons.Shields)
                    {
                        StartCoroutine("PunchHitBox", this.gameObject.transform.Find("Body").Find("Weapons").Find("Shields").gameObject);
                    }

                    break;
                case 2:
                    m_ComboCount = 0;

                    m_AnimatorController.SetTrigger("Quick3");

                    if (ActiveWeapon == Weapons.Sword)
                    {
                        StartCoroutine("SlashVFX", m_SlashVFXs[2]);
                        m_QuickAttackCD = 1f;
                    }
                    else if (ActiveWeapon == Weapons.Shields)
                    {
                        StartCoroutine("PunchHitBox", this.gameObject.transform.Find("Body").Find("Weapons").Find("Shields").gameObject);
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
            m_ShootTimer -= Time.deltaTime;

        if(m_PlayerInputActions.Actions.HeavyAttack.triggered && m_ShootTimer <= 0f && CanAttack && ActiveWeapon == Weapons.Scepter)
        {
            Instantiate(m_Bullet, m_Target.position, transform.rotation);
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

        if (m_PlayerInputActions.Actions.WeaponOne.triggered && CheckWeapon("Sword")) //SWORD
        {
            StartCoroutine("DrawSword", 1.2f);
            ActiveWeapon = Weapons.Sword;

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

        if (m_PlayerInputActions.Actions.WeaponTwo.triggered && CheckWeapon("Scepter")) //SCEPTER
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
                    break;
            }
            m_AnimatorController.SetTrigger("DrawScepter");
            m_AnimatorController.SetBool("FromSword", false);
        }

        if (m_PlayerInputActions.Actions.WeaponThree.triggered && CheckWeapon("Shields")) //SHIELDS
        {
            m_FightingCounter = m_FightingCoolDown;
            switch (ActiveWeapon)
            {
                case Weapons.Weaponless:
                    break;
                case Weapons.Sword:
                    m_AnimatorController.SetTrigger("WeaponChanging");
                    StartCoroutine("SaveSword");
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

                    StartCoroutine("SaveSword");

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
                        m_Scepter.GetComponent<ScepterWAnim>().ScepterSave();

                    StartCoroutine("SaveSword");

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
                        m_Scepter.GetComponent<ScepterWAnim>().ScepterSave();

                    StartCoroutine("DrawSword", 1.2f);

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
                HasSkill = m_Inventario.m_Skills[i].Available;
            if (HasSkill)
                break;
        }

        if (HasSkill) 
            return true;
        else
            return false;
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

            if (m_CoyoteTimeCounter > 0f && m_PlayerInputActions.Movement.Jump.triggered)
            {
                m_AnimatorController.SetTrigger("Jump");
                m_AnimatorController.SetBool("Grounded", false);
                m_VerticalVelocity = Vector3.up * m_JumpForce;
                m_IsJumping = true;
                m_AirTimeCounter = m_JumpTime;
            }

            if (m_PlayerInputActions.Movement.Jump.ReadValue<float>() != 0 && m_Controller.velocity.y > 0f)
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

            if (m_PlayerInputActions.Movement.Jump.ReadValue<float>() == 0)
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
        if (m_IsGrounded && m_PlayerInputActions.Movement.Jump.ReadValue<float>() == 0)
        {
            m_VerticalVelocity.y = -1f;
        }
        else if (!m_IsJumping)
        {
            m_VerticalVelocity.y += gravity * Time.deltaTime;
        }

        Vector3 moveDirection = new(m_MoveInput.x, 0f, m_MoveInput.y);
        moveDirection = MoveSpeed * Time.deltaTime * moveDirection.normalized;
        moveDirection.y = m_VerticalVelocity.y * Time.deltaTime;

        if(m_MoveInput.magnitude != 0)
        {
            m_AnimatorController.SetBool("Walking", true);

            m_MovementAS.clip = m_WalkWoodSFX;
            if(!m_MovementAS.isPlaying) m_MovementAS.Play();

            if (m_PlayerInputActions.Movement.Run.triggered)
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
        m_LookDir = new Vector3(m_LookInput.x, 0f, m_LookInput.y);

        Vector3 moveDirection = new(m_MoveInput.x, 0f, m_MoveInput.y);
        moveDirection = MoveSpeed * Time.deltaTime * moveDirection.normalized;

        if (m_LookDir.magnitude != 0f)
        {
            transform.rotation = Quaternion.LookRotation(m_LookDir, Vector3.up);

            moveDirection.Normalize();
            m_LookDir.Normalize();

            float angle = Vector3.Angle(moveDirection, m_LookDir);
            bool movingBackwards = (angle > 90);

            if (movingBackwards)
            {
                m_AnimatorController.SetBool("Backwards", true);
            }
            else
            {
                m_AnimatorController.SetBool("Backwards", false);
            }

            m_LastLookDir = m_LookDir;
        }
        else
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
            if (m_MoveInput.magnitude > 0.1f && m_PlayerInputActions.Movement.Dash.triggered)
            {
                StartCoroutine(PerformDash(m_MoveInput));
            }
            else if (m_MoveInput.magnitude < 0.1f && m_PlayerInputActions.Movement.Dash.triggered)
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

    //DASH
    private SkinnedMeshRenderer[] m_SkinnedMeshRenderers;
    private GameObject[] m_WeaponsArray;
    public Material m_DashMaterial;
    private IEnumerator PerformDash(Vector2 DashDir)
    {
        m_IsJumping = false;
        m_AirTimeCounter = 0f;
        m_VerticalVelocity.y = 0;

        m_IsDashing = true;

        m_DashAS.Play();

        float Elapsed = 0f;

        Vector3 dashDir = new(DashDir.x, 0f, DashDir.y);

        while (Elapsed < m_DashTime)
        {
            if (m_SkinnedMeshRenderers == null)
            {
                m_SkinnedMeshRenderers = transform.Find("Body/Model").gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            switch (ActiveWeapon) // Collect 3D meshes to doplicat with the VFX
            {
                case Weapons.Scepter:
                    m_WeaponsArray = new GameObject[]
                    {
                            m_Sword.gameObject,
                            m_ShieldLSaved.gameObject,
                            m_ShieldRSaved.gameObject,
                            m_Scepter.gameObject.transform.GetChild(0).gameObject
                    };
                    break;
                case Weapons.Shields:
                    if (m_ShieldLDrawn.activeSelf || m_ShieldRDrawn.activeSelf)
                    {
                        m_WeaponsArray = new GameObject[]
                        {
                            m_Sword.gameObject,
                            m_ShieldLDrawn.transform.GetChild(0).gameObject,
                            m_ShieldLDrawn.transform.GetChild(1).gameObject,
                            m_ShieldRDrawn.transform.GetChild(0).gameObject,
                            m_ShieldRDrawn.transform.GetChild(1).gameObject
                        };
                    }
                    else if (m_ShieldLSaved.activeSelf || m_ShieldRSaved.activeSelf)
                    {
                        m_WeaponsArray = new GameObject[]
                        {
                            m_Sword.gameObject,
                            m_ShieldLSaved.gameObject,
                            m_ShieldRSaved.gameObject
                        };
                    }

                    break;
                case Weapons.Sword:
                    m_WeaponsArray = new GameObject[]
                        {
                        m_Sword.gameObject,
                        m_ShieldLSaved.gameObject,
                        m_ShieldRSaved.gameObject
                        };

                    break;
                case Weapons.Weaponless:
                    m_WeaponsArray = new GameObject[]
                     {
                        m_Sword.gameObject,
                        m_ShieldLSaved.gameObject,
                        m_ShieldRSaved.gameObject
                     };

                    break;
            }
            
            for (int i = 0; i < m_SkinnedMeshRenderers.Length; i++)
            {
                GameObject GObj = new GameObject();
                GObj.transform.SetPositionAndRotation(m_SkinnedMeshRenderers[i].gameObject.transform.position,
                    m_SkinnedMeshRenderers[i].gameObject.transform.rotation);

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
                    GameObject GObj = Instantiate(m_WeaponsArray[i], m_WeaponsArray[i].transform.position,
                        m_WeaponsArray[i].transform.rotation);
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

            Elapsed += Time.deltaTime;

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
        string[] JoystickNames = Input.GetJoystickNames();

        if (JoystickNames.Length == 0 || JoystickNames[0] == "") // Keyboard - mouse
        {
            m_PlayerInputActions.Movement.Move.performed += ctx => {
                Vector2 Input = ctx.ReadValue<Vector2>();
                Input = Input.normalized;
                m_MoveInput = new Vector2(Input.x, Input.y);
            };
            m_PlayerInputActions.Movement.Move.canceled += ctx => {
                m_MoveInput = Vector2.zero;
            };

            m_PlayerInputActions.Movement.Aim.performed += ctx => {
                Vector2 Input = ctx.ReadValue<Vector2>();
                Input = Input.normalized;
                m_LookInput = new Vector2(Input.x, Input.y);
            };
            m_PlayerInputActions.Movement.Aim.canceled += ctx => {
                m_LookInput = Vector2.zero;
            };
        }
        else // Gamepad
        {
            m_PlayerInputActions.Movement.Move.performed += ctx => {
                Vector2 Input = ctx.ReadValue<Vector2>();
                m_MoveInput = new Vector2(Input.x, Input.y);
            };
            m_PlayerInputActions.Movement.Move.canceled += ctx => {
                m_MoveInput = Vector2.zero;
            };

            m_PlayerInputActions.Movement.Aim.performed += ctx => {
                Vector2 Input = ctx.ReadValue<Vector2>();
                m_LookInput = new Vector2(Input.x, Input.y);
            };
            m_PlayerInputActions.Movement.Aim.canceled += ctx => {
                m_LookInput = Vector2.zero;
            };
        }
    }
    public void Death()
    {
        if(CharState != CharStates.Dead)
        {
            CharState = CharStates.Dead;
            m_AnimatorController.SetTrigger("Death");
        }
    }
}