using System.Collections.Generic;
using UnityEngine;

namespace Sweet_And_Salty_Studios
{
    public enum UNIT_ABILITY
    {
        WALKER,
        STOPPER,
        UMBRELLA,
        DIG_FORWARD,
        DIG_DOWN,
        EXPLODE
    }

    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance
        {
            get;
            private set;
        }

        public int MaxUnits = 10;
        public float TimeScale = 1;
        public float Interval = 1;

        private Unit unitPrefab;

        private List<Unit> allUnits = new List<Unit>();
        private float timer;
        private float delta;

        public bool ChangeSpeed;

        private Transform unitsParent;

        private GameManager gameManager;

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

            gameManager = GameManager.Instance;
            unitPrefab = Resources.Load<Unit>("Prefabs/Units/Unit");
            unitsParent = new GameObject("Units").transform;
        }

        private void Update()
        {
            delta = Time.deltaTime;
            delta *= TimeScale;

            if(ChangeSpeed)
            {
                ChangeSpeed = false;
                ChangeSpeedForAllUnits(TimeScale);
            }

            if(allUnits.Count < MaxUnits)
            {
                timer -= delta;
                if(timer < 0)
                {
                    timer = Interval;
                    SpawnUnit();
                }
            }

            for(int i = 0; i < allUnits.Count; i++)
            {
                allUnits[i].Tick(delta);
            }
        }

        private void SpawnUnit()
        {
            var newUnit = Instantiate(unitPrefab, unitsParent);
            newUnit.Initialize(gameManager);
            allUnits.Add(newUnit);
        }

        private void ChangeSpeedForAllUnits(float newSpeed)
        {
            for(int i = 0; i < allUnits.Count; i++)
            {
                allUnits[i].Animator.speed = newSpeed;
            }
        }

        public Unit GetClosestUnit(Vector2 point)
        {
            var minDistance =  0.1f;

            Unit closestUnit = null;

            for(int i = 0; i < allUnits.Count; i++)
            {
                var tempDistance = Vector2.Distance(point, allUnits[i].transform.position);

                if(tempDistance < minDistance)
                {
                    minDistance = tempDistance;
                    closestUnit = allUnits[i];
                }
            }

            return closestUnit;
        }
    }
}