using BCS.Framework.Caching;
using BCS.Framework.Helper.Cache;
using BCS.Framework.Singleton;

namespace BCS.Framework.Commons
{
    public class BaseDal<T>
    {
        public T UnitOfWork;
        protected ICacheProvider Cache;
        protected CacheHelper CacheHelper;
        //protected string _schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDal{T}" /> class.
        /// </summary>
        /// <param name="schema">The schema.</param>
        public BaseDal()
        {
            //_schema = schema;
            //cache = new MemcachedProvider(schema);
            //cacheHelper = new CacheHelper(schema);
            UnitOfWork = (T) SingletonIpl.GetInstance<T>();
            //unitOfWork.CacheHelper = cacheHelper;
        }
        public BaseDal(string connectionKey)
        {
            UnitOfWork = (T)SingletonIpl.GetInstance<T>(connectionKey);
        }


    }
}
