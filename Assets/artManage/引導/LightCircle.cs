using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightCircle : MonoBehaviour
{
    // Start is called before the first frame update
    Tween rotationTween;
    void Start()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i % 2 == 0)
            {
                rotate(3 + i, transform.GetChild(i), 360);
            }
            else
            {
                rotate(3 + i, transform.GetChild(i), -360);
            }

        }
    }

    void rotate(float speed, Transform transform, float direction)
    {
        rotationTween = transform.DORotate(
                            new Vector3(0, direction, 0),
                            speed, RotateMode.FastBeyond360
                            )
                            .SetLoops(-1, LoopType.Restart)
                            .SetRelative()
                            .SetEase(Ease.Linear);
    }
}
