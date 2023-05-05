using Rabbit.Loaders;
using Rabbit.Utils;

namespace Rabbit.DataStructure
{
    public interface IDataSource : IMonoBehaviour, IInvalidateable
    {
        int Count { get; }
        Future<T> GetItem<T>(int index);
        void Initialize();
    }
}