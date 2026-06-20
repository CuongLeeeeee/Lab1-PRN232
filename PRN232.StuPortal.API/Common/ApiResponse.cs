using System.Collections.Generic;
using System.Runtime.Serialization;
using PRN232.StuPortal.Services.Models.Responses;

namespace PRN232.StuPortal.API.Common
{
    // [KnownType] báo cho DataContractSerializer biết tất cả các kiểu
    // có thể xuất hiện trong property Data khi T = object
    [KnownType(typeof(List<SubjectResponse>))]
    [KnownType(typeof(List<StudentResponse>))]
    [KnownType(typeof(List<CourseResponse>))]
    [KnownType(typeof(List<SemesterResponse>))]
    [KnownType(typeof(List<EnrollmentResponse>))]
    [KnownType(typeof(SubjectResponse))]
    [KnownType(typeof(StudentResponse))]
    [KnownType(typeof(CourseResponse))]
    [KnownType(typeof(SemesterResponse))]
    [KnownType(typeof(EnrollmentResponse))]
    [KnownType(typeof(AuthResponse))]
    [KnownType(typeof(UserResponse))]
    [KnownType(typeof(PaginationMeta))]
    [DataContract(Name = "ApiResponse", Namespace = "")]
    public class ApiResponse<T>
    {
        [DataMember(Order = 1)]
        public bool Success { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; } = "Request processed successfully";

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public T? Data { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public object? Errors { get; set; }

        // Đổi từ object? sang PaginationMeta? để DataContractSerializer
        // luôn biết kiểu cụ thể (anonymous type không serialize được)
        [DataMember(Order = 5, EmitDefaultValue = false)]
        public PaginationMeta? Pagination { get; set; }
    }
}
