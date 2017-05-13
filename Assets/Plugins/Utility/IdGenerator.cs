public struct IdGenerator
{
    private int currentId;

    public int Next()
    {
        return currentId++;
    }
}