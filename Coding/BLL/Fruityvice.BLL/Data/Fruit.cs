namespace Fruityvice.Data
{
    public class Fruit : Base.DefaultEntity
    {
        public virtual long FruitId
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

        public virtual string? Genus { get; set; }
        public virtual string? Name { get; set; }
        public virtual string? Family { get; set; }
        public virtual string? Order { get; set; }
    }
}
