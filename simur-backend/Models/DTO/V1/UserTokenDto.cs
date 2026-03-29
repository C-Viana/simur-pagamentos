namespace simur_backend.Models.DTO.V1
{
    public class UserTokenDto
    {
        public UserTokenDto(){}

        public UserTokenDto(bool authenticated, string created, string expiration, string accessToken, string refreshToken)
        {
            Authenticated = authenticated;
            Created = created;
            Expiration = expiration;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public bool Authenticated { get; set; }
        public string Created {  get; set; }
        public string Expiration {  get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
