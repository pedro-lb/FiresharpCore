namespace FiresharpCore.Interfaces
{
    public interface ISerializer
    {
        T Deserialize<T>(string json);

        string Serialize<T>(T value);
    }
}