using Fruityvice.BLL;

namespace Fruityvice.Service
{
    public class FruityviceService : IFruityviceService
    {
        private readonly IFruityviceBLL _fruityviceBLL;
        public FruityviceService(IFruityviceBLL fruityviceBLL)
        {
            _fruityviceBLL = fruityviceBLL;
        }

        public async Task<List<Data.Fruit>> GetAllFruit(int min, int max)
        {
            return await _fruityviceBLL.GetAllFruit(min, max);
        }
    }
}
