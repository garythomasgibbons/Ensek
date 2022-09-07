using EnSekTestGaryGibbons.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using FluentAssertions;

namespace EnSekTestGaryGibbons
{
    public class ServiceTests
    {
        private readonly ApiClient _apiclient;
        private readonly string domainUrl = "https://ensekapicandidatetest.azurewebsites.net/";
        private string guidPattern = @"([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})";

        public ServiceTests()
        {
            _apiclient = new ApiClient();   
        }

        [Theory]
        [InlineData("gas", 1, 1)]
        [InlineData("electric", 3, 3)]
        [InlineData("oil", 4, 4)]
        public async Task buy_increases_order_count(string energyType, int energyId, int quantity)
        {
            //reset data (unable to authorise bug?)
            //Arrange
            //await _apiclient.Post<string>(domainUrl + "reset");
            var initialOrdersCount = await GetNumberOfOrders();

            //Act
            //make a purchase
            var purchaseResponse = await _apiclient.Put<string>(domainUrl + $"buy/{energyId}/{quantity}");
            var newOrdersCount = await GetNumberOfOrders();

            //assert
            newOrdersCount.Should().Be(initialOrdersCount + 1);
        }

        [Theory]
        [InlineData("gas", 1, 1)]      
        [InlineData("electric", 3, 3)]
        [InlineData("oil", 4, 4)]
        public async Task orders_are_persisted_correctly(string energyType, int energyId, int quantity)
        {
            //reset data (unable to authorise)
            //Arrange
            //await _apiclient.Post<string>(domainUrl + "reset");

            //Act
            //make a purchase
            var purchaseResponse = await _apiclient.Put<string>(domainUrl + $"buy/{energyId}/{quantity}");
            var putTime = DateTime.Now.ToString("ddd, dd MMM yyy HH:mm");
            var purchaseResponseDeserialized = JsonConvert.DeserializeObject<Purchase>(purchaseResponse);

            //get guid
            var purchaseId = Regex.Matches(purchaseResponseDeserialized.message, guidPattern);

            //get orders again
            var ordersResponse = await _apiclient.Get<string>(domainUrl + "orders");
            var ordersResponseDeserialized = JsonConvert.DeserializeObject<List<Orders>>(ordersResponse);

            //find correct order
            var foundOrder = ordersResponseDeserialized.FirstOrDefault(res => res.id == purchaseId[0].Value);

            //assert
            foundOrder.id.Should().Be(purchaseId[0].Value);
            foundOrder.fuel.Should().Be(energyType);
            foundOrder.quantity.Should().Be(quantity);
            //foundOrder.time.Should().Contain(putTime); //time is not GMT
        }

        [Theory]
        [InlineData("Feb", 2)]
        public async Task number_of_orders_in_a_month(string month, int expectedNumberOfOrders)
        {
            //reset data (unable to authorise)
            //Arrange
            //await _apiclient.Post<string>(domainUrl + "reset");

            var ordersResponse = await _apiclient.Get<string>(domainUrl + "orders");
            var ordersResponseDeserialized = JsonConvert.DeserializeObject<List<Orders>>(ordersResponse);

            //find orders in month
            var foundOrders = ordersResponseDeserialized.Where(res => res.time.Contains(month)).Count();

            //assert
            foundOrders.Should().Be(expectedNumberOfOrders);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(4, 0)]
        [InlineData(3, -1)]
        [InlineData(10, 100000000000)]
        public async Task bad_request_to_buy_fuel(int energyId, int quantity)
        {
            //reset data (unable to authorise bug?)
            //Arrange
            //await _apiclient.Post<string>(domainUrl + "reset");

            //Act
            //make a purchase
            var purchaseResponse = await _apiclient.Put<string>(domainUrl + $"buy/{energyId}/{quantity}");
            var purchaseResponseDeserialized = JsonConvert.DeserializeObject<Purchase>(purchaseResponse);

            //assert
            purchaseResponseDeserialized.message.Should().Be("Bad request");
        }

        [Fact]
        public async Task buy_fuel_when_no_units_available()
        {
            //reset data (unable to authorise bug?)
            //Arrange
            //await _apiclient.Post<string>(domainUrl + "reset");

            //Act
            //attempt to make a purchase of nuclear
            var purchaseResponse = await _apiclient.Put<string>(domainUrl + $"buy/2/1");
            var purchaseResponseDeserialized = JsonConvert.DeserializeObject<Purchase>(purchaseResponse);

            //assert
            purchaseResponseDeserialized.message.Should().Be("There is no nuclear fuel to purchase!");
        }

        [Fact]
        public async Task fuel_unit_quantity_is_updated()
        {
            //reset data (unable to authorise)
            //Arrange
            //await _apiclient.Post<string>(domainUrl + "reset");
            var initialNumberOfElectricUnits = await GetNumberOfElectricUnits();

            //Act
            //make a purchase
            var purchaseResponse = await _apiclient.Put<string>(domainUrl + $"buy/3/3");

            //assert
            var UpdatedNumberOfElectricUnits = await GetNumberOfElectricUnits();
            UpdatedNumberOfElectricUnits.Should().Be(initialNumberOfElectricUnits - 1);
        }
        

        private async Task<int> GetNumberOfElectricUnits()
        {
            var energyTypeValues = await _apiclient.Get<string>(domainUrl + "energy");
            var energyTypeValuesDeserialized = JsonConvert.DeserializeObject<Models.Utilities>(energyTypeValues);

            return energyTypeValuesDeserialized.electric.quantity_of_units;
        }

        private async Task<int> GetNumberOfOrders()
        {
            var initialOrdersResponse = await _apiclient.Get<string>(domainUrl + "orders");
            var initialOrdersResponseDeserialized = JsonConvert.DeserializeObject<List<Orders>>(initialOrdersResponse);

            if (initialOrdersResponseDeserialized != null)
            {
                return initialOrdersResponseDeserialized.Count;
            }
            return 0;
        }
    }
}