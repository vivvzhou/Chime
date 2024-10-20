using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    //Components
    private CharacterMovementComponent characterMovementComponent;
    private InteractorComponent interactorComponent;
    private PlayerInventoryComponent playerInventoryComponent;

    //temp
    public float HitPoints;

    //dialogue
    [SerializeField] float talkDistance = 2;
    private bool inConversation;
    //Movement relative to camera
    public Transform cameraTransform;

    public void Reset()
    {
        this.AddComponent<CharacterMovementComponent>();
        this.AddComponent<InteractorComponent>();
        this.AddComponent<PlayerInventoryComponent>();
    }
    public void Start()
    {
        characterMovementComponent = this.GetComponent<CharacterMovementComponent>();
        interactorComponent = this.GetComponent<InteractorComponent>();
        playerInventoryComponent = this.GetComponent<PlayerInventoryComponent>();
    }
    public void Update()
    {
        bool primaryAction = Input.GetButtonDown("PrimaryAction");
        bool interact = Input.GetButtonDown("Interact");
        bool talk = Input.GetButtonDown("Talk");

        //Temp buttonchecks?
        bool dropItem = Input.GetKeyDown("g");
        bool numberKey1 = Input.GetKeyDown("1");
        bool numberKey2 = Input.GetKeyDown("2");
        bool numberKey3 = Input.GetKeyDown("3");
        bool numberKey4 = Input.GetKeyDown("4");
        bool numberKey5 = Input.GetKeyDown("5");
        bool numberKey6 = Input.GetKeyDown("6");

        if (primaryAction)
        {
            playerInventoryComponent.UseActiveItemPrimaryAction();
        }

        if (interact)
        {
            interactorComponent.Interact();
        }

        if (dropItem)
        {
            playerInventoryComponent.DropItem();
        }

        if (numberKey1)
        {
            playerInventoryComponent.SelectItemByIndex(0);
        }
        if (numberKey2)
        {
            playerInventoryComponent.SelectItemByIndex(1);
        }
        if (numberKey3)
        {
            playerInventoryComponent.SelectItemByIndex(2);
        }
        if (numberKey4)
        {
            playerInventoryComponent.SelectItemByIndex(3);
        }
        if (numberKey5)
        {
            playerInventoryComponent.SelectItemByIndex(4);
        }
        if (numberKey6)
        {
            playerInventoryComponent.SelectItemByIndex(5);
        }
        if (talk) 
        {
            DialogueInteract();
        }
    }
    public void FixedUpdate()
    {
        //Eventually just smooth input yourself
        Vector2 smoothedMovement = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1.0f);
        Vector2 rawMovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Transform cameraTransform = Camera.main.transform;
        characterMovementComponent.MovePlayerRelativeToCamera(new Vector3(smoothedMovement.x, 0.0f, smoothedMovement.y), new Vector3(rawMovementInput.x, 0.0f, rawMovementInput.y), cameraTransform);
    }

    // Basic implementation for taking damage, can modify later
    public void TakeDamage(float damage)
    {
        HitPoints -= damage;
    }

    public void DialogueInteract() 
    {
        if (inConversation)
        {
            DialogueBoxController.instance.SkipLine();
        }
        else
        {
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out RaycastHit hitInfo, talkDistance))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out NPC npc))
                {
                    DialogueBoxController.instance.StartDialogue(npc.dialogueAsset, npc.StartDialoguePosition, npc.npcName);
                }
            }
        }
    }

    public void JoinConversation() 
    {
        inConversation = true;
    }

    public void LeaveConversation()
    {
        inConversation = false;
    }

    private void OnEnable()
    {
        DialogueBoxController.OnDialogueStarted += JoinConversation;
        DialogueBoxController.OnDialogueEnded += LeaveConversation;
    }

    private void OnDisable()
    {
        DialogueBoxController.OnDialogueStarted -= JoinConversation;
        DialogueBoxController.OnDialogueEnded -= LeaveConversation;
    }
}
