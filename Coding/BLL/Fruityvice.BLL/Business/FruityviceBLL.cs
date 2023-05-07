namespace Fruityvice.BLL
{
    public class FruityviceBLL : IFruityviceBLL
    {
        private readonly Base.IDefaultEntityRepository<Data.Fruit> _fruitRepository;
        private readonly Base.IDefaultEntityRepository<Data.Nutrition> _nutritionRepository;
        public FruityviceBLL()
        {
            ////Base.IDefaultEntityRepository<Data.Fruit> fruitRepository;
            ////Base.IDefaultEntityRepository<Data.Nutrition> nutritionRepository;
            ////_fruitRepository = fruitRepository;
            ////_nutritionRepository = nutritionRepository;
        }

        public async Task<List<Data.Fruit>> GetAllFruit(int min, int max)
        {
            var result = _fruitRepository.GetAll().Skip(min).Take(max).ToList();
            return await Task.FromResult(result);
        }
    }
}
