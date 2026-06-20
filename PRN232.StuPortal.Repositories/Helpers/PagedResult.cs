using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PRN232.StuPortal.Repositories.Helpers
{
    [DataContract(Name = "PagedResult", Namespace = "")]
    public class PagedResult<T>
    {
        // List<T> thay vì IEnumerable<T> để DataContractSerializer
        // luôn nhận được concrete type, tránh lỗi SelectListIterator
        [DataMember(Order = 1)]
        public List<T> Items { get; set; } = new List<T>();

        [DataMember(Order = 2)]
        public int Page { get; set; }

        [DataMember(Order = 3)]
        public int PageSize { get; set; }

        [DataMember(Order = 4)]
        public int TotalItems { get; set; }

        [DataMember(Order = 5)]
        public int TotalPages { get; set; }
    }
}
