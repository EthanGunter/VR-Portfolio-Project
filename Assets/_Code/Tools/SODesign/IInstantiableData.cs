namespace SolarStorm.UnityToolkit
{
    public interface IInstantiableData
    {
        T Instantiate<T>() where T : class;
    }
    public interface IInstantiableData<T> where T : class
    {
        T New();
    }
}
