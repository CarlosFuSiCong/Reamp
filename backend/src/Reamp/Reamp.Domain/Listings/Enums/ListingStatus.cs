using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reamp.Domain.Listings.Enums
{
    public enum ListingStatus
    {
        Draft = 0,
        Active = 1,
        Pending = 2,
        Sold = 3,
        Rented = 4,
        Archived = 5
    }
}