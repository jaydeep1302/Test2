using Newtonsoft.Json;

namespace tprofile.Classes.Payeezy
{
    public class AuthorizeSessionResponse
    {      
        public string PublicKeyBase64 { get; set; }       
        public string ClientToken { get; set; }
        public string Errors { get; set; }
    }
}