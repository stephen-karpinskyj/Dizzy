public static class InputUtility
{
    public static bool IsPrimaryPointer(int pointerId)
    {
        return pointerId == -1 || pointerId == 0;
    }
}
