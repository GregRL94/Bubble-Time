using UnityEngine;

public static class MathUtils
{
    public static float DirToAngleRad(float x, float y, float offset = 0f)
    {
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg + offset;
    }

    public static float[] ScreenBoundaries()
    {
        Vector2 screenBottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        Vector2 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - 1, Screen.height - 1));
        return new float[4] { screenBottomLeft.x, screenBottomLeft.y, screenTopRight.x, screenTopRight.y };
    }

    public static float DistanceX(Vector2 pointA, Vector2 pointB)
    {
        return Mathf.Abs(pointB.x - pointA.x);
    }

    public static float DistanceY(Vector2 pointA, Vector2 pointB)
    {
        return Mathf.Abs(pointB.y - pointA.y);
    }
}
