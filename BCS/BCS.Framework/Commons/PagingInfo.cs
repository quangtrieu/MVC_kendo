using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCS.Framework.Commons
{
    /// <summary>
    /// Paging info
    /// </summary>
    public class PagingInfo
    {
        /// <summary>
        /// Initialize Paging info class
        /// </summary>
        public PagingInfo()
        {
            
        }

        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        public int PageIndex
        {
            set;
            get;
        }


        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize
        {
            set;
            get;
        }


        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int TotalCount
        {
            set;
            get;
        }


        /// <summary>
        /// Gets the total pages.
        /// </summary>
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((decimal)TotalCount / PageSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int From
        {
            get { return PageSize*(PageIndex - 1); }
        }
    }
}
