using UnityEngine;
using UnityEngine.EventSystems;

namespace Sweet_And_Salty_Studios
{
    [System.Serializable]
    public class Node
    {
        public int X
        {
            get;
            private set;
        }

        public int Y
        {
            get;
            private set;
        }

        public bool IsEmpty
        {
            get;
            set;
        }

        public Node(int x, int y, bool isEmpty)
        {
            X = x;
            Y = y;
            IsEmpty = isEmpty;
        }
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get;
            private set;
        }

        private Texture2D levelTexture;
        private Texture2D texture2D_Instance;
        private SpriteRenderer levelRenderer;

        private readonly float editRadius = 6;

        [Header("Debug")]
        public bool DrawGizmos = false;
        public Color SpawnPointColor;

        private readonly float positionOffset = 0.01f;

        private int max_X;
        private int max_Y;

        private Node[,] grid;

        private Vector2 mousePosition;
        private Node currentNode;
        private Node previousNocde;
        private bool OverUI_Element;

        public Node SpawnNode
        {
            get;
            private set;
        }
        public Transform LevelStartObject
        {
            get;
            private set;
        }
        public Vector2 SpawnPoint
        {
            get;
            private set;
        }

        private Unit currentUnit;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            levelTexture = Resources.Load<Texture2D>("Levels/Level 1");
            levelRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            LevelStartObject = GameObject.Find("LevelStartObject").transform;

            CreateLevel();

            SpawnNode = GetNodeFromWorldPosition(LevelStartObject.position);
            SpawnPoint = GetWorldPositionFromNode(SpawnNode);
        }

        private void Update()
        {
            OverUI_Element = EventSystem.current.IsPointerOverGameObject();
            GetMousePosition();
            CheckForUnit();
            UIManager.Instance.Tick();
            HandleUnit();
            // HandleInput();
        }

        private void HandleUnit()
        {
            if(currentUnit == null)
            {
                return;
            }

            if(Input.GetMouseButtonUp(0))
            {
                if(UIManager.Instance.TargetAbility == UNIT_ABILITY.WALKER)
                {
                    return;
                }

                if(currentUnit.CurrentAbility == UNIT_ABILITY.WALKER)
                {
                    currentUnit.ChangeAbility(UIManager.Instance.TargetAbility);
                }
            }
        }

        private void OnDrawGizmos()
        {
            //if(DrawGizmos == false)
            //{
            //    return;
            //}

            //Gizmos.color = SpawnPointColor;
            //Gizmos.DrawLine(SpawnPoint + (Vector2.left + Vector2.up) * 0.1f, SpawnPoint + (Vector2.right + Vector2.down) * 0.1f);
            //Gizmos.DrawLine(SpawnPoint + (Vector2.down + Vector2.left) * 0.1f, SpawnPoint + (Vector2.up + Vector2.right) * 0.1f);
        }

        private void HandleInput()
        {
            if(currentNode == null)
            {
                return;
            }

            var color = Color.clear;

            if(Input.GetMouseButton(0))
            {
                if(currentNode != previousNocde)
                {
                    previousNocde = currentNode;

                    var center = GetWorldPositionFromNode(currentNode);
                    var radius = editRadius * positionOffset;

                    for(int x = -6; x < 6; x++)
                    {
                        for(int y = -6; y < 6; y++)
                        {
                            var target_X = x + currentNode.X;
                            var target_Y = y + currentNode.Y;

                            var distance = Vector2.Distance(center, GetWorldPositionFromNode(target_X, target_Y));

                            if(distance > radius)
                            {
                                continue;
                            }

                            var node = GetNode(target_X, target_Y);

                            if(node == null)
                            {
                                continue;
                            }

                            node.IsEmpty = true;
                            texture2D_Instance.SetPixel(target_X, target_Y, color);                         
                        }
                    }

                    texture2D_Instance.Apply();
                }
            }
        }

        private void CheckForUnit()
        {
            currentUnit = UnitManager.Instance.GetClosestUnit(mousePosition);

            if(currentUnit == null)
            {
                UIManager.Instance.OverUnit = false;
                return;
            }

            UIManager.Instance.OverUnit = true;
        }

        private void GetMousePosition()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            mousePosition = mouseRay.GetPoint(5);
            currentNode = GetNodeFromWorldPosition(mousePosition);
        }

        public Vector2 GetWorldPositionFromNode(int x, int y)
        {
            return new Vector2(x * positionOffset, y * positionOffset);
        }

        public Vector2 GetWorldPositionFromNode(Node node)
        {
            if(node == null)
            {
                return -Vector2.one;
            }

            return new Vector2(node.X * positionOffset, node.Y * positionOffset);
        }

        private Node GetNodeFromWorldPosition(Vector2 worldPosition)
        {
            var target_X = Mathf.RoundToInt(worldPosition.x / positionOffset);
            var target_Y = Mathf.RoundToInt(worldPosition.y / positionOffset);

            return GetNode(target_X, target_Y);
        }

        public Node GetNode(int x, int y)
        {
            if(x < 0 || y < 0 || x > max_X - 1 || y > max_Y - 1)
            {
                return null;
            }

            return grid[x, y];
        }

        private void CreateLevel()
        {
            max_X = levelTexture.width;
            max_Y = levelTexture.height;

            grid = new Node[max_X, max_Y];

            texture2D_Instance = new Texture2D(max_X, max_Y)
            {
                filterMode = FilterMode.Point
            };

            for(int x = 0; x < max_X; x++)
            {
                for(int y = 0; y < max_Y; y++)
                {
                    var pixel = levelTexture.GetPixel(x, y);
                    texture2D_Instance.SetPixel(x, y, pixel);
                    var node = new Node(x, y, pixel.a == 0);

                    grid[x, y] = node;
                }
            }

            texture2D_Instance.Apply();

            var rect = new Rect(0, 0, max_X, max_Y);
            levelRenderer.sprite = Sprite.Create(texture2D_Instance, rect, Vector2.zero);
        }
    }
}