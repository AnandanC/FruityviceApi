namespace Fruityvice.Data
{
    public class Nutrition : Base.DefaultEntity
    {
        public virtual long NutritionId
        {
            get
            {
                return this.Id;
            }

            set
            {
                this.Id = value;
            }
        }

        public virtual long FruitId { get; set; }
        public virtual double? Carbohydrates { get; set; }
        public virtual double? Protein { get; set; }
        public virtual double? Fat { get; set; }
        public virtual double? Calories { get; set; }
        public virtual double? Sugar { get; set; }
    }


}
