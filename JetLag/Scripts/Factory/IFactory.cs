namespace JetLag.Scripts.Factory
{
    public interface IFactory<T>
    {
        public T Create();
    }


    public interface IFactory<T, U>
    {
        public T Create(U intializeInput);
    }
}