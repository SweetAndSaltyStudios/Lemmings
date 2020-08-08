using UnityEngine;
using UnityEngine.UI;

namespace Sweet_And_Salty_Studios
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance
        {
            get;
            private set;
        }

        public Transform CursorTransform;
        public Image CursorIcon;
        public Sprite Box;
        public Sprite Cross_1;
        public Sprite Cross_2;
        public bool OverUnit;
        public bool SwitchToAbility;
        public UNIT_ABILITY TargetAbility;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Cursor.visible = false;
        }

        public void Tick()
        {
            CursorTransform.transform.position = Input.mousePosition;

            if(OverUnit)
            {
                CursorIcon.sprite = Box;
            }
            else
            {
                CursorIcon.sprite = Cross_1;
            }
        }
    }
}