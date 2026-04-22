namespace Flight_Tracker_System.Data;

public interface IParser
{
    public T Deserialise<T>();
}
