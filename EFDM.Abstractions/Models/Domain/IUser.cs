namespace EFDM.Abstractions.Models.Domain {

    public interface IUser : IIdKeyEntity<int>, ITitleEntity {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
    }
}
