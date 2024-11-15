using Entities.ViewModels.Vinpearl;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace WEB.CMS.Service
{
    public class VinpearlLib
    {
        private IConfiguration configuration;
        public VinpearlLib(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public async Task<string> GetTokenAsync()
        {
            try
            {


                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var values = new Dictionary<string, string>
                  {
                      {"username",  configuration["API_Vinpearl:USER_NAME_API_VIN"]},
                      {"organization",  configuration["API_Vinpearl:ORGANIZATION"]},
                      {"password",configuration["API_Vinpearl:PASSWORD_API_VIN"]}
                  };
                var content = new FormUrlEncodedContent(values);
                var urlGetToken = configuration["API_Vinpearl:API_VIN_URL"] + configuration["API_Vinpearl:enpoint:get_token"];
                var response = await httpClient.PostAsync(urlGetToken, content);
                var responseString = await response.Content.ReadAsStringAsync();
                Authentication authentication = JsonConvert.DeserializeObject<Authentication>(responseString);
                var token = string.Empty;
                if (!string.IsNullOrEmpty(authentication.authentication_token))
                {
                    token = authentication.authentication_token.Split(' ')[2];
                }
                if (string.IsNullOrEmpty(token)){
                    LogHelper.InsertLogTelegram(urlGetToken + " reponse empty token " );
                }
                return token;
            }
            catch (Exception ex)
            {
              LogHelper.InsertLogTelegram(ex.Message);
                return string.Empty;
            }
        }

   
        /// <summary>
        /// Api lấy mã đặt chỗ
        /// </summary>
        /// <returns>
        public async Task<string> getVinpearlConfirmBooking(string input_api_vin)
        {
            try
            {
                string token = GetTokenAsync().Result.ToString();
                string url = configuration["API_Vinpearl:API_VIN_URL"] + configuration["API_Vinpearl:enpoint:get_bookable_package_availability"];
                var uri = new System.Uri(url);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;

                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Api  CREATE BOOKING VIN
        /// </summary>
        /// <returns>
        public async Task<string> getVinpearlCreateBooking(string input_api_vin)
        {
            try
            {
                /*
                {
                  "propertyId": "f89849e9-0a10-17c0-16e7-7abaa8f06808",
                  "arrivalDate": "2020-05-12",
                  "departureDate": "2020-05-14",
                  "reservations": [
                    {
                      "roomOccupancy": {
                        "numberOfAdult": 2,
                        "otherOccupancies": [
                          {
                            "otherOccupancyRefID": "child",
                            "otherOccupancyRefCode": "child",
                            "quantity": 1
                          },
                          {
                            "otherOccupancyRefID": "infant",
                            "otherOccupancyRefCode": "infant",
                            "quantity": 1
                          }
                        ]
                      },
                      "totalAmount": {
                        "amount": "16800000",
                        "currencyCode": "VND"
                      },
                      "isSpecialRequestSpecified": true,
                      "specialRequests": [
                        {
                          "requestType": "BookerInstruction",
                          "requestContent": "Need room with Ocean view and high floor"
                        },
                        {
                          "requestType": "SpecialRequest",
                          "requestContent": "Need room with high floor"
                        },
                        {
                          "requestType": "GuestInstruction",
                          "requestContent": "I have hearing difficulty"
                        },
                        {
                          "requestType": "RoomFeature",
                          "requestContent": "Smoking room"
                        }
                      ],
                      "isProfilesSpecified": true,
                      "profiles": [
                        {
                          "profileRefID": "cfaed708- 99a5-4eac-8093-df502e432486",
                          "firstName": "VLEISURE",
                          "profileType": "TravelAgent",
                          "isPrimary": true
                        },
                        {
                          "firstName": "Truong",
                          "lastName": "Ba Loc",
                          "email": "v.loctb@cloudhms.net",
                          "phoneNumber": "0927271133",
                          "primarySearchValues": {
                            "email": "v.loctb@cloudhms. net",
                            "phoneNumber": "0927271133"
                          },
                          "profileType": "Guest"
                        },
                        {
                          "firstName": "Truong",
                          "lastName": "Ba Loc",
                          "email": "v.loctb@cloudhms.net",
                          "phoneNumber": "0927271133",
                          "primarySearchValues": {
                            "email": "v.loctb@cloudhms. net",
                            "phoneNumber": "0927271133"
                          },
                          "travelAgentCode": "BOOKER_TA_1",
                          "profileType": "Booker"
                        },
                        {
                          "profileRefID": "1a8a108-2da1- 9205-05d6-6e9fdb7d58d2",
                          "firstName": "Vinpearl",
                          "travelAgentCode": "YOUR_TRAVEL_AGENCY_CODE",
                          "profileType": "TravelAgent"
                        }
                      ],
                      "isRoomRatesSpecified": true,
                      "roomRates": [
                        {
                          "stayDate": "2020-05-12",
                          "roomTypeRefID": "2fb40fb0- 0f44-61cc-632c-67378374a44c",
                          "allotmentId": "cc610d81-7d08- 4940-a673-099f799a1657",
                          "ratePlanRefID": "8b56b783- b9f7-41eb-88c7-bcc415267c1e"
                        },
                        {
                          "stayDate": "2020-05-13",
                          "roomTypeRefID": "2fb40fb0- 0f44-61cc-632c-67378374a44c",
                          "allotmentId": "cc610d81-7d08- 4940-a673-099f799a1657",
                          "ratePlanRefID": "8b56b783- b9f7-41eb-88c7-bcc415267c1e"
                        }
                      ],
                      "isPackagesSpecified": false,
                      "packages": [
                        {
                          "usedDate": "2020-11-11",
                          "packageRefId": "867896b1- a923-44aa-87ef-a7b0110dffe9",
                          "ratePlanId": "b55c34da-350d-4961-b238-b7bb286ba62c",
                          "quantity": 1
                        }
                      ]
                    },
                    {
                      "roomOccupancy": {
                        "numberOfAdult": 2,
                        "otherOccupancies": []
                      },
                      "totalAmount": {
                        "amount": "16800000",
                        "currencyCode": "VND"
                      },
                      "isSpecialRequestSpecified": true,
                      "specialRequests": [
                        {
                          "requestType": "BookerInstruction",
                          "requestContent": "Vip 3 - Need room with Ocean view and high floor"
                        }
                      ],
                      "isProfilesSpecified": true,
                      "profiles": [
                        {
                          "profileRefID": "cfaed708- 99a5-4eac-8093-df502e432486",
                          "firstName": "VLEISURE",
                          "profileType": "TravelAgent"
                        },
                        {
                          "firstName": "Truong",
                          "lastName": "Ba Loc",
                          "email": "v.loctb@cloudhms.net",
                          "phoneNumber": "0927271133",
                          "primarySearchValues": {
                            "email": "v.loctb@cloudhms. net",
                            "phoneNumber": "0927271133"
                          },
                          "profileType": "Guest"
                        },
                        {
                          "firstName": "Truong",
                          "lastName": "Ba Loc",
                          "email": "v.loctb@cloudhms.net",
                          "phoneNumber": "0927271133",
                          "primarySearchValues": {
                            "email": "v.loctb@cloudhms. net",
                            "phoneNumber": "0927271133"
                          },
                          "profileType": "Booker"
                        },
                        {
                          "profileRefID": "4af8d608- 2db4-9705-06d6-6e9fdb7d58d3",
                          "firstName": "Vinpearl",
                          "travelAgentCode": "YOUR_TRAVEL_AGENCY_CODE",
                          "profileType": "TravelAgent"
                        }
                      ],
                      "isRoomRatesSpecified": true,
                      "roomRates": [
                        {
                          "stayDate": "2020-05-12",
                          "roomTypeRefID": "2fb40fb0- 0f44-61cc-632c-67378374a44c",
                          "roomTypeCode": "NV3O",
                          "allotmentId": "cc610d81-7d08- 4940-a673-099f799a1657",
                          "ratePlanRefID": "8b56b783- b9f7-41eb-88c7-bcc415267c1e",
                          "ratePlanCode": "TA2-FB"
                        },
                        {
                          "stayDate": "2020-05-13",
                          "roomTypeRefID": "2fb40fb0- 0f44-61cc-632c-67378374a44c",
                          "roomTypeCode": "NV3O",
                          "allotmentId": "cc610d81-7d08- 4940-a673-099f799a1657",
                          "ratePlanRefID": "8b56b783- b9f7-41eb-88c7-bcc415267c1e",
                          "ratePlanCode": "TA2-FB"
                        }
                      ],
                      "isPackagesSpecified": false,
                      "packages": [
                        {
                          "usedDate": "2020-11-11",
                          "packageRefId": "867896b1- a923-44aa-87ef-a7b0110dffe9",
                          "packageCode": "TEST_BB1",
                          "ratePlanId": "b55c34da-350d4961-b238-b7bb286ba62c",
                          "quantity": 1
                        }
                      ]
                    }
                  ],
                  "distributionChannel": "c964dbf5-bac8-4b01-b884- e6ddfcf1d613",
                  "sourceCode": "TOS"
                }
                 
                 */

                string token = GetTokenAsync().Result.ToString();
                string url = configuration["API_Vinpearl:API_VIN_URL"] + configuration["API_Vinpearl:enpoint:get_create_booking"];
                var uri = new System.Uri(url);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;

                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// Api  Guarantee Methods VIN
        /// </summary>
        /// <returns>
        public async Task<string> getGuaranteeMethods(String reservationid, string input_api_vin)
        {
            try
            {
                string token = GetTokenAsync().Result.ToString();
                string url = configuration["API_Vinpearl:API_VIN_URL"] + configuration["API_Vinpearl:enpoint:get_guarantee_methods"].Replace("reservationID", reservationid);
                var uri = new System.Uri(url);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;

                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// Api  Batch Commit VIN
        /// </summary>
        /// <returns>
        public async Task<string> getBatchCommit(string input_api_vin)
        {
            try
            {
                string token = GetTokenAsync().Result.ToString();
                string url = configuration["API_Vinpearl:API_VIN_URL"] + configuration["API_Vinpearl:enpoint:get_batch_commit"];
                var uri = new System.Uri(url);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var content = new StringContent(input_api_vin, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(uri, content).Result;
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;

                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
