using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIR.Comun
{
    public class SolicitudesHttp<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="userIP"></param>
        /// <param name="basicAuthentication"></param>
        /// <param name="barerAuthentication"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static T POST(string url, Dictionary<string, object> requestBody, string userIP = null, bool basicAuthentication = false, bool barerAuthentication = false, string token = "")
        {
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            if (basicAuthentication)
            {
                request.AddHeader("Authorization", token);
            }
            else
            {
                if (barerAuthentication)
                {
                    request.AddHeader("Authorization", token);
                }
            }
            if (userIP != null)
            {
                request.AddHeader("IP", userIP);
            }
            request.AddHeader("Content-Type", "application/json;charset=utf-8");
            request.AddJsonBody(requestBody);

            IRestResponse response = client.Execute(request);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="basicAuthentication"></param>
        /// <param name="barerAuthentication"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static T PUT(string url, Dictionary<string, object> requestBody, bool basicAuthentication = false, bool barerAuthentication = false, string token = "")
        {
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            if (basicAuthentication)
            {
                request.AddHeader("Authorization", token);
            }
            else
            {
                if (barerAuthentication)
                {
                    request.AddHeader("Authorization", token);
                }
            }

            request.AddHeader("Content-Type", "application/json;charset=utf-8");
            request.AddJsonBody(requestBody);

            IRestResponse response = client.Execute(request);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
