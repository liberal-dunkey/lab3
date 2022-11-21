using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using RESTTest.Model;
using System.Net;

namespace Lab_3
{
    [TestFixture]
    public class CompleteTests
    {
        private RestClient bookerClient { get; set; }
        private RestClient harryPotterClient { get; set; }
        private string Id { get; set; }
        private BookingModel bookingModel { get; set; }
        [SetUp]
        public void SetUp()
        {
            bookerClient = new RestClient("https://restful-booker.herokuapp.com/");
            harryPotterClient = new RestClient("https://api.potterdb.com/");
            bookerClient.AddDefaultHeader("Content-Type", "application/json");
            bookerClient.AddDefaultHeader("Accept", "application/json");
            
            var authRequest = new RestRequest("auth", Method.POST);
            authRequest.AddJsonBody(new { username = "admin", password = "password123" });
            var authResponse = bookerClient.Execute(authRequest);
            var authToken = JsonConvert.DeserializeObject<TokenModel>(authResponse.Content).Token;
            bookerClient.AddDefaultHeader("Cookie", $"token={authToken}");
            Id = JsonConvert.DeserializeObject<List<BookingIdModel>>(bookerClient.Execute(new RestRequest("booking", Method.GET)).Content).First().BookingId;

            bookingModel =
                new BookingModel()
                {
                    firstname = "Jim",
                    lastname = "Brown",
                    totalprice = 111,
                    depositpaid = true,
                    bookingdates = new BookingDates()
                    {
                        checkin = "2018-01-01",
                        checkout = "2019-01-01"
                    },
                    additionalneeds = "Breakfast"
                };
        }
        [Test]
        public void GET_WhenGetPostsWithId_ShouldBeSuccessResponse()
        {
            // arrange
            RestRequest request = new RestRequest("booking", Method.GET);

            // act
            IRestResponse response = bookerClient.Execute(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void POST_WhenExecutePostModel_ShouldBeSuccessResponse()
        {
            // arrange
            RestRequest request = new RestRequest("booking", Method.POST);
            request.AddJsonBody(bookingModel);
            
            // act
            IRestResponse<BookingModel> response = bookerClient.Execute<BookingModel>(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }


       [Test]
        public void PUT_UpdateBookInformation_ShouldReturnSuccessResponse()
        {
            // arrange
            RestRequest request = new RestRequest($"booking/{Id}", Method.PUT);
            request.AddJsonBody(bookingModel);

            // act
            var response = bookerClient.Execute<BookingModel>(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }

        [Test]
        public void DELETE_RemovePostsWithId_ShouldBeSuccessful()
        {
            // arrange
            RestRequest request = new RestRequest($"booking/{Id}", Method.DELETE);

            // act
            IRestResponse response = bookerClient.Execute(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void GET_WhenGetInfoAboutHarryPotter_ShouldBeOk()
        {
            // arrange
            RestRequest request = new RestRequest("v1/characters/harry-potter", Method.GET);

            // act
            IRestResponse response = harryPotterClient.Execute(request);
   
            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void GET_WhenGetAllMoviesAboutHarryPotter_ShouldBeOk()
        {
            // arrange
            RestRequest request = new RestRequest("/v1/movies", Method.GET);
            // act
            IRestResponse response = harryPotterClient.Execute<BookingModel>(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }
        [Test]
        public void GET_WhenGetAllSpellsFromHarryPotter_ShouldBeOk()
        {
            // arrange
            RestRequest request = new RestRequest("/v1/spells", Method.GET);
            // act
            IRestResponse response = harryPotterClient.Execute<BookingModel>(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}