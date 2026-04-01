namespace JetLag.Scripts.Factory.Interface;

public interface IFactory<T>
{
    public T Create();
}


public interface IFactory<T, U>
{
    public T Create(U intializeInput);
}