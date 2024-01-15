using DFC.App.Pages.Cms.Data.Model;
using DFC.App.Pages.Cms.Data.RequestHandler;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc;
using DFC.App.Pages.Data.Models;
using System.Linq;
using System.Net.Mime;
using System.Text;
using FluentAssertions;
using FakeItEasy;

namespace DFC.App.Pages.IntegrationTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Integration")]
    public class PagesControllerRouteTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> factory;
        private readonly HttpClient httpClient;
        private readonly ITestOutputHelper output;
        //private readonly IRedisCMSRepo redisCMSRepo;



        public PagesControllerRouteTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
            this.httpClient = this.factory.CreateClient();
            //this.output = output;
            //this.redisCMSRepo = redisCMSRepo;
        }

        public static IEnumerable<object[]> PagesContentRouteData => new List<object[]>
         {
             new object[] { "/" },
             new object[] { "/pages" },
             new object[] { "/pages/head" },
             new object[] { "/pages/breadcrumb" },
             new object[] { "/pages/body" },
         };

        public static IEnumerable<object[]> PagesNoContentRouteData => new List<object[]>
         {
             new object[] { $"/pages/bodytop" },
             new object[] { $"/pages/herobanner" },
             new object[] { $"/pages/sidebarright" },
             new object[] { $"/pages/sidebarleft" },
             new object[] { $"/pages/bodyfooter" },
         };

       

       /* public async Task<TResponse> GetBearerToken<TResponse>()
            where TResponse : OAuthTokenModel
        {
            var client = new HttpClient();
            List<OAuthTokenModel> model = null;

            var dict = new Dictionary<string, string>();
            dict.Add("client_id", "");
            dict.Add("client_secret", "");
            dict.Add("grant_type", "client_credentials");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://dfc-dev-stax-editor-as.azurewebsites.net/connect/token"),
                Content = new FormUrlEncodedContent(dict),
                Method = HttpMethod.Post,
            };
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            //string token;
            using (HttpResponseMessage responses = await client.SendAsync(request))
            {
                responses.EnsureSuccessStatusCode();

                string responseBody = await responses.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseBody);
            }
        }

        [Fact]
        public async Task GetGraphQLPagesContentEndpointReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var tokenResponse = await GetBearerToken<OAuthTokenModel>();
            string token = tokenResponse.AccessToken;

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://dfc-dev-stax-editor-as.azurewebsites.net/api/graphql"),
                Content = new StringContent("query MyQuery {" +
                "page {" +
                "description " +
                "displayText" +
                     "}" +
                "}"),
                Method = HttpMethod.Post,
            };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/graphql");
            using (HttpResponseMessage responses = await client.SendAsync(request))
            {
                var responseBody = await responses.Content.ReadAsAsync<PageResponse>();
                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);
            }
        }*/

       /* [Fact]
        public async Task GetGraphQLPagesContentReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var tokenResponse = await GetBearerToken<OAuthTokenModel>();
            string token = tokenResponse.AccessToken;
            string expectedResult = "Test";

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://dfc-dev-stax-editor-as.azurewebsites.net/api/graphql"),
                Content = new StringContent("query MyQuery {" +
                "page(first: 1) {" +
                "description" +
                "displayText" +
                "contentItemId" +
                    "}" +
                "}"),
                Method = HttpMethod.Post,
            };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/graphql");
            using (var responses = await client.SendAsync(request))
            {
                responses.EnsureSuccessStatusCode();

                string responseBody = await responses.Content.ReadAsStringAsync();
                var response = await redisCMSRepo.GetGraphQLData<Page>(responseBody, "pages/GetPage");
                var jsonDeserialize = JsonConvert.DeserializeObject<Page>(responseBody);
                var desc = response.Description;
                //string result = desc;
              
                
                //string result = responseBody[0].ToString();
                Assert.Equal(expectedResult, desc);
                //var stream = JsonConvert.DeserializeObject<PageResponse>(responseBody);
                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);
            }
        }*/
/*
        [Fact]
        public async Task GetSQLPagesContentEndpointReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var tokenResponse = await GetBearerToken<OAuthTokenModel>();
            string token = tokenResponse.AccessToken;

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://dfc-dev-stax-editor-as.azurewebsites.net/api/queries/PageLocation"),
                Content = new StringContent(" {" +
                "page {" +
                "displayText" +
                "description" +
                "}" +
                "}"),
                Method = HttpMethod.Post,
            };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/graphql");
            using (var responses = client.SendAsync(request).Result)
            {
                Assert.Equal(HttpStatusCode.OK, responses.StatusCode);
            }
        }

        [Theory]
        [MemberData(nameof(PagesContentRouteData))]
        public async Task GetPagesHtmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var contentPageModel = factory.GetContentPageModels().Where(x => x.CanonicalName == "an-article");
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));
            A.CallTo(() => factory.MockCosmosRepo.GetAllAsync(A<string>.Ignored)).Returns(factory.GetContentPageModels());

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Assert.Equal($"{MediaTypeNames.Text.Html}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
            }
        }

        [Theory]
        [MemberData(nameof(PagesContentRouteData))]
        public async Task GetPagesJsonContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var contentPageModel = factory.GetContentPageModels().Where(x => x.CanonicalName == "an-article");
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            A.CallTo(() => factory.MockCosmosRepo.GetAllAsync(A<string>.Ignored)).Returns(factory.GetContentPageModels());

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Assert.Equal($"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
            }
        }

        [Theory]
        [MemberData(nameof(PagesNoContentRouteData))]
        public async Task GetPagesEndpointsReturnSuccessAndNoContent(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            A.CallTo(() => factory.MockCosmosRepo.GetAllAsync(A<string>.Ignored)).Returns(factory.GetContentPageModels());

            // Act
            var response = await httpClient.GetAsync(uri);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }*/
    }
}