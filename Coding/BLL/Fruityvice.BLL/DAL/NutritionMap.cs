namespace Fruityvice.DAL
{
    public class NutritionMap : Base.DefaultEntityClassMap<Data.Nutrition>
    {
        public NutritionMap() : base("tblNutritions", "NutritionId")
        {
            this.Property(x => x.FruitId, map => { map.Column("FruitId"); });
            this.Property(x => x.Carbohydrates, map => { map.Column("Carbohydrates"); });
            this.Property(x => x.Protein, map => { map.Column("Protein"); });
            this.Property(x => x.Fat, map => { map.Column("Fat"); });
            this.Property(x => x.Calories, map => { map.Column("Calories"); });
            this.Property(x => x.Sugar, map => { map.Column("Sugar"); });
        }
    }
}
