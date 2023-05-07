using Fruityvice.Data;

namespace Fruityvice.Service
{
    public interface IFruityviceService
    {
        Task<List<Fruit>> GetAllFruit(int min, int max);
    }
}