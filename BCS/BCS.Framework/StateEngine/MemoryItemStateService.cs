namespace BCS.Framework.StateEngine
{
    //public class MemoryItemStateService : IItemStateService
    //{
    //    readonly Dictionary<int, Item> datastore = new Dictionary<int, Item>();
    //    private int idPool = 0;

    //    private int AllocateID()
    //    {
    //        return idPool++;
    //    }

    //    public Item Create(string originator)
    //    {
    //        Item item = new Item();
    //        item.Id = AllocateID();
    //        item.Originator = originator;

    //        datastore.Add(item.Id, item);
    //        return item;
    //    }

    //    public void Save(Item item)
    //    {
    //        //Only for SQL Store
    //    }

    //    public Item GetById(int id)
    //    {
    //        return datastore.Where(c => c.Key == id).Select(c => c.Value).SingleOrDefault();
    //    }

    //    public IList<Item> GetByOwner(string owner, Func<KeyValuePair<string, string>, bool> exp)
    //    {
    //        IList<Item> items = datastore.Where(c => c.Value.Owners.Contains(owner))
    //            .Select(c => c.Value).ToList();

    //        return items.Where(c => c.Has(exp)).ToList();
    //    }

    //    public IList<Item> GetByOriginator(string originator)
    //    {
    //        return datastore.Where(c => c.Value.Originator == originator).Select(c => c.Value).ToList();
    //    }

    //    public IList<Item> All()
    //    {
    //        return datastore.Select(c => c.Value).ToList();
    //    }
    //}
}