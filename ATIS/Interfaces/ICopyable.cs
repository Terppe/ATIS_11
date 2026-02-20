namespace ATIS.Interfaces
{
    public interface ICopyable<T>
    {
        T Clone();
        void CopyFrom(T other);
    }
}
