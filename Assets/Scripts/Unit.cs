using UnityEngine;

namespace Sweet_And_Salty_Studios
{
    public class Unit : MonoBehaviour
    {
        public UNIT_ABILITY CurrentAbility;

        private Node currentNode;
        private Node targetNode;
        private bool hasInitialized = false;

        public bool OnGround;
        private bool previousGround;
        private int airFrames;

        private bool isMoving;
        private bool isMovingLeft;
        private float baseSpeed;

        private bool initializeLerp;
        private readonly float lerpSpeed = 0.3f;
        private readonly float fallSpeed = 5;
        private float t;
        private Vector2 targetPosition;
        private Vector2 startPosition;

        private int target_X;
        private int target_Y;

        private SpriteRenderer spriteRenderer;
        private GameManager gameManager;

        public Animator Animator
        {
            get;
            private set;
        }

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            Animator = GetComponentInChildren<Animator>();
        }

        public void Initialize(GameManager gameManager)
        {
            this.gameManager = gameManager;

            PlaceOnNode();

            hasInitialized = true;

            CurrentAbility = UNIT_ABILITY.WALKER;
        }

        private void PlaceOnNode()
        {
            currentNode = gameManager.SpawnNode;
            transform.position = gameManager.SpawnPoint;
            isMoving = true;
        }
        public void Tick(float delta)
        {
            if(hasInitialized == false)
            {
                return;
            }

            if(isMoving == false)
            {
                return;
            }

            spriteRenderer.flipX = isMovingLeft;

            switch(CurrentAbility)
            {
                case UNIT_ABILITY.WALKER:
                case UNIT_ABILITY.UMBRELLA:
                case UNIT_ABILITY.DIG_FORWARD:
                    Walker(delta);
                    break;

                case UNIT_ABILITY.STOPPER:
                    Stopper();
                    break;

                case UNIT_ABILITY.DIG_DOWN:
                    DigDown();
                    break;

                default:

                    break;
            }
        }

        private void Walker(float delta)
        {
            if(initializeLerp == false)
            {
                initializeLerp = true;
                startPosition = transform.position;
                t = 0;
                Pathfinding();
                targetPosition = gameManager.GetWorldPositionFromNode(targetNode);

                var distance = Vector2.Distance(targetPosition, startPosition);

                baseSpeed = OnGround ? lerpSpeed / distance : fallSpeed / distance;
            }
            else
            {
                t += delta * baseSpeed;

                if(t > 1)
                {
                    t = 1;
                    initializeLerp = false;
                    currentNode = targetNode;
                }

                transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            }
        }

        private void Stopper()
        {

        }

        private void UMBRELLA()
        {

        }

        private void DigForward()
        {

        }

        private void DigDown()
        {

        }

        public void ChangeAbility(UNIT_ABILITY newAbility)
        {
  
            CurrentAbility = newAbility;

            switch(CurrentAbility)
            {
                case UNIT_ABILITY.WALKER:
                    Animator.Play("Walk");
                    break;
                case UNIT_ABILITY.STOPPER:
                    Animator.Play("Stop");
                    break;
                case UNIT_ABILITY.UMBRELLA:
                    Animator.Play("Umbrella");
                    break;
                case UNIT_ABILITY.DIG_FORWARD:
                    Animator.Play("DigForward");
                    break;
                case UNIT_ABILITY.DIG_DOWN:
                    Animator.Play("DigDown");
                    break;
                case UNIT_ABILITY.EXPLODE:
                    Animator.Play("DigDown");
                    break;
                default:
                    break;
            }
        }

        private void Pathfinding()
        {
            target_X = currentNode.X;
            target_Y = currentNode.Y;

            var isDownAir = IsAir(target_X, target_Y - 1);
           
            var isForwardAir = IsAir(target_X, target_Y);

            if(isDownAir) // Falling...
            {
                target_X = currentNode.X;
                target_Y -= 1;

                if(OnGround)
                {
                    airFrames++;

                    if(airFrames > 4)
                    {
                        OnGround = false;
                        Animator.Play("Fall");
                    }
                }
            }
            else // On the ground...
            {
                OnGround = true;
                airFrames = 0;

                if(OnGround && previousGround == false)
                {
                    Animator.Play("Walk");
                }

                if(isForwardAir)
                {
                    target_X = isMovingLeft ? target_X - 1 : target_X + 1;
                    target_Y = currentNode.Y;
                }
                else
                {
                    var s = 0;
                    var isValid = false;

                    while(s < 10)
                    {
                        s++;
                        var forwardIsAir = IsAir(target_X, target_Y + s);

                        if(forwardIsAir)
                        {
                            isValid = true;
                            break;
                        }
                    }

                    if(isValid)
                    {
                        target_Y += s;
                    }
                    else
                    {
                        isMovingLeft = !isMovingLeft;
                        target_X = isMovingLeft ? currentNode.X - 1 : currentNode.Y + 1;
                    }
                }
            }

            targetNode = gameManager.GetNode(target_X, target_Y);
            previousGround = OnGround;
        }

        private bool IsAir(int x, int y)
        {
            var node = gameManager.GetNode(x, y);

            if(node == null)
            {
                return true;
            }

            return node.IsEmpty;
        }
    }
}