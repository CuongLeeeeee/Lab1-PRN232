using System.Runtime.Serialization;

namespace PRN232.StuPortal.API.Common
{
    /// <summary>
    /// Concrete pagination metadata — thay thế anonymous type
    /// vì DataContractSerializer không serialize được anonymous type.
    /// </summary>
    [DataContract(Name = "Pagination", Namespace = "")]
    public class PaginationMeta
    {
        [DataMember(Order = 1)]
        public int Page { get; set; }

        [DataMember(Order = 2)]
        public int PageSize { get; set; }

        [DataMember(Order = 3)]
        public int TotalItems { get; set; }

        [DataMember(Order = 4)]
        public int TotalPages { get; set; }
    }
}
