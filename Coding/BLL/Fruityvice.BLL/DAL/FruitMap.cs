namespace Fruityvice.DAL
{
    public class FruitMap : Base.DefaultEntityClassMap<Data.Fruit>
    {
        public FruitMap() : base("tblFruits", "FruitId")
        {
            this.Property(x => x.Genus, map => { map.Column("Genus");});
            this.Property(x => x.Name, map => { map.Column("Name"); });
            this.Property(x => x.Family, map => { map.Column("Family"); });
            this.Property(x => x.Order, map => { map.Column("OrderDesc"); });
        }
    }
}
