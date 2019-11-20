


namespace tprofile.Repositories
{
    #region

    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;


    using Newtonsoft.Json;

    using tprofile.Classes.Payeezy;

    #endregion

    /// <summary>
    /// The Payeezy repository.
    /// </summary>
    public class PayeezyRepositoryV2
    {
        /// <summary>
        /// The log.
        /// </summary>
       

        /// <summary>
        /// The API key.
        /// </summary>
        private readonly string apiKey = string.Empty;

        /// <summary>
        /// The API secret.
        /// </summary>
        private readonly string apiSecret = string.Empty;

        /// <summary>
        /// The API secret.
        /// </summary>
        private readonly string paymentJsSecret = string.Empty;

        /// <summary>
        /// The Payeezy url.
        /// </summary>
        private readonly string payeezyUrl = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayeezyRepository"/> class.
        /// </summary>
        //public PayeezyRepositoryV2()
        //{
        //    this.payeezyUrl = "https://cert.api.firstdata.com/paymentjs/v2";
        //    this.apiKey = "G26mPwHd0zkPLdRjZXwHzbvbZGARSYOv";
        //    this.apiSecret = "69d81c12e3a5b5b6f08f06bf14478bf5fc124018e24833138e3d751217dff62c";
        //    this.paymentJsSecret = "y2oATQeGVzGfU73J";
        //    //this.apiSecret = assets.get_asset("PayeezyApiSecret");
        //}

        public PayeezyRepositoryV2()
        {
            this.payeezyUrl = "https://cert.api.firstdata.com/paymentjs/v2";
            this.apiKey = "aRRMPmVZ1YcYg1TYPDTShbF7MTHyINy1";
            this.apiSecret = "	fcaccf88badd97b8e436035f074c5a1139dfb0528722dad7ad7c651fc5fb8527";
            this.paymentJsSecret = "AoMv7RIpmiZqGoGS";
            //this.apiSecret = assets.get_asset("PayeezyApiSecret");
        }



        /// <summary>
        /// The payment endpoint.
        /// </summary>
        private string PaymentEndpoint => "v1/transactions";

        private string TransactionEndpoint => "v1/transactions/";

        public string CalculateHMAC(string data, string secret)
        {
            //HMAC SHA256 signed with Payment.js Api Secret
            HMAC hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
           
            byte[] hmac2Hex = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(data));

            //Hex encode HMAC
            //string hex = BitConverter.ToString(hmac2Hex);
            //hex = hex.Replace("-", "").ToLower();
            //byte[] hexArray = Encoding.UTF8.GetBytes(hex);

            //encode hex-encoded hash in Base64
            string base64String = Convert.ToBase64String(hmac2Hex);
            return base64String;
        }

        public AuthorizeSessionResponse AuthorizeSession(AuthorizeSession message)
        {
            return this.CallPayeezyApi(message, "/merchant/authorize-session");
        }




        //public TokenPaymentResponse PaymentTransaction(TokenPaymentRequest request)
        //{
        //    return
        //        JsonConvert.DeserializeObject<TokenPaymentResponse>(this.CallPayeezyApi(request, PaymentEndpoint));
        //}

        //public TransactionActionResponse CaptureTransaction(TransactionActionRequest request, string payeezyTransactionId)
        //{
        //    return
        //        JsonConvert.DeserializeObject<TransactionActionResponse>(this.CallPayeezyApi(request, string.Concat(TransactionEndpoint, "/", payeezyTransactionId)));
        //}

        //public TransactionActionResponse RefundTransaction(TransactionActionRequest request, string payeezyTransactionId)
        //{
        //    return
        //        JsonConvert.DeserializeObject<TransactionActionResponse>(this.CallPayeezyApi(request, string.Concat(TransactionEndpoint, "/", payeezyTransactionId)));
        //}
        //public TransactionActionResponse VoidTransaction(TransactionActionRequest request, string payeezyTransactionId)
        //{
        //    return
        //        JsonConvert.DeserializeObject<TransactionActionResponse>(this.CallPayeezyApi(request, string.Concat(TransactionEndpoint, "/", payeezyTransactionId)));
        //}

        private AuthorizeSessionResponse CallPayeezyApi(dynamic request, string url)
        {
            AuthorizeSessionResponse model = new AuthorizeSessionResponse();
            string jsonString = JsonConvert.SerializeObject(request);

            Random random = new Random();
            string nonce = (random.Next(0, 1000000)).ToString();

            DateTime date = DateTime.UtcNow;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = date - epoch;
            string time = ((long)span.TotalMilliseconds).ToString(); 

            //string token = "fdoa-89e97a6861fd8acde0fda8322ae5167389e97a6861fd8acd"; //Merchant token
            string hashData = this.apiKey + nonce + time + jsonString;

            string message = this.CalculateHMAC(hashData, this.paymentJsSecret);

            // Begin HttpWebRequest
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.payeezyUrl + url);

            webRequest.Method = "POST";
            webRequest.Accept = "*/*";
            webRequest.ContentLength = jsonString.Length;
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("api-key", this.apiKey);
            webRequest.Headers.Add("Nonce", nonce);
            webRequest.Headers.Add("Timestamp", time);            
            webRequest.Headers.Add("Message-Signature", message);
            //webRequest.Headers.Add("token", token);
            
            

            var writer = new StreamWriter(webRequest.GetRequestStream());
            writer.Write(jsonString);
            writer.Close();

            

            try
            {
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (StreamReader responseStream = new StreamReader(webResponse.GetResponseStream()))
                    {
                        var stringData = responseStream.ReadToEnd();
                        
                        //return JsonConvert.DeserializeObject<TokenPaymentResponse>(stringData);
                        for (int i = 0; i < webResponse.Headers.Count; ++i)
                        {
                            if(webResponse.Headers.Keys[i] == "Client-Token")
                            {
                                model.ClientToken = webResponse.Headers[i];
                            }
                        }
                            

                       
                        model.PublicKeyBase64 = JsonConvert.DeserializeObject<payeezyAuthorize>(stringData).publicKeyBase64;
                         
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (HttpWebResponse errorResponse = (HttpWebResponse)ex.Response)
                    {
                        using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string remoteEx = reader.ReadToEnd();
                            
                            //return JsonConvert.DeserializeObject<TokenPaymentResponse>(remoteEx);
                            model.Errors = remoteEx;
                        }
                    }
                }
            }
            return model;
        }
    
        /// <summary>
        /// The byte array to hex string.
        /// </summary>
        /// <param name="ba">
        /// The ba.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ByteArrayToHexString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public class payeezyAuthorize
        {
            public string publicKeyBase64 { get; set; }
        }
    }
}