using Fruityvice.Data;

namespace Fruityvice.BLL
{
    public interface IFruityviceBLL
    {
        Task<List<Fruit>> GetAllFruit(int min = 0, int max = 1000);
    }
}