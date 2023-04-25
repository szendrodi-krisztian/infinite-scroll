using Rabbit.Utils;

namespace Rabbit.DataStructure
{
    public interface IDataSource : IMonoBehaviour
    {
        int Count { get; }
        Future<T> GetItem<T>(int index);
        void Initialize();
    }
}