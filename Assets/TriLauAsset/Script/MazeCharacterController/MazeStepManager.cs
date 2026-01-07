using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class MazeStepManager : MonoBehaviour
    {
        public ShapeInfo currentShape;
        public Transform characterTransform;
        [SerializeField] private DataSO dataSO;

        private int steps;
        private bool canMove = true;
        private bool canSelectShape;

        private bool isResolvingSteps;

        private EventBinding<FirstShapeEvent> firstShapeEventBinding;
        private EventBinding<MazeStepEvent> mazeMoveEventBinding;

        private void OnEnable()
        {
            firstShapeEventBinding = new EventBinding<FirstShapeEvent>(OnFirstShapeEvent);
            EventBus<FirstShapeEvent>.Register(firstShapeEventBinding);

            mazeMoveEventBinding = new EventBinding<MazeStepEvent>(AddStep);
            EventBus<MazeStepEvent>.Register(mazeMoveEventBinding);
        }

        private void OnDisable()
        {
            EventBus<FirstShapeEvent>.Deregister(firstShapeEventBinding);
            EventBus<MazeStepEvent>.Deregister(mazeMoveEventBinding);
        }

        private async void Update()
        {
            if (!canSelectShape || Camera.main == null) return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (!Physics.Raycast(ray, out RaycastHit hit)) return;

                if (!hit.collider.TryGetComponent(out ShapeInfo shape)) return;

                canSelectShape = false;
                steps--;

                AddShapeToStorage(shape);

                await UniTask.Delay(500);

                ResolveSteps().Forget();
            }
        }

        private void AddStep(MazeStepEvent evt)
        {
            steps += evt.steps;
            dataSO.currentStep += steps;

            ResolveSteps().Forget();
        }

        private async void OnFirstShapeEvent(FirstShapeEvent evt)
        {
            currentShape = evt.shape;

            await UniTask.Delay(500);

            if (!dataSO.isFrist)
            {
                StepFromData();
            }
        }

        private void AddShapeToStorage(ShapeInfo shape)
        {
            currentShape = shape;

            EventBus<CamTargetEvent>.Raise(new CamTargetEvent(shape.transform));
            currentShape.HightLight();

            EventBus<MazeSetMovePosEvent>.Raise(
                new MazeSetMovePosEvent(shape.transform)
            );
        }

        private void Move()
        {
            EventBus<CamTargetEvent>.Raise(
                new CamTargetEvent(characterTransform)
            );

            EventBus<MazeMoveEvent>.Raise(new MazeMoveEvent(currentShape));
        }

        private async UniTaskVoid ResolveSteps()
        {
            if (isResolvingSteps) return;
            isResolvingSteps = true;

            while (steps > 0 && currentShape != null)
            {
                var targets = currentShape.pointsTarget;

                if (targets.Count == 1)
                {
                    AddShapeToStorage(targets[0]);
                    steps--;

                    await UniTask.Delay(500);
                }
                else if (targets.Count > 1)
                {
                    canSelectShape = true;
                    isResolvingSteps = false;
                    return;
                }
                else
                {
                    break;
                }
            }

            if (canMove)
            {
                Move();

                if (steps == 0 && currentShape != null)
                {
                    await UniTask.Delay(1000);

                    //EventBus<MazeGameplayEvent>.Raise(
                    //    new MazeGameplayEvent(
                    //        currentShape.shapeSO.shapeType
                    //    )
                    //);
                }
            }

            isResolvingSteps = false;
        }

        private void StepFromData()
        {
            int dataStep = dataSO.currentStep;

            Debug.Log(currentShape.pointsTarget.Count);

            while (dataStep > 0)
            {
                var targets = currentShape.pointsTarget;
                Debug.Log(currentShape.pointsTarget.Count);
                if (targets.Count >= 1)
                {
                    currentShape = targets[0];
                    currentShape.HightLight();
                    dataStep--;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
