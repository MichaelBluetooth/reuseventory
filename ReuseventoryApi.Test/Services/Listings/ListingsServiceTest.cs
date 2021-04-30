using System.Linq;
using NUnit.Framework;
using ReuseventoryApi.Models;
using ReuseventoryApi.Models.DTO;
using ReuseventoryApi.Services.Listings;
using ReuseventoryApi.Test.TestUtils;

namespace ReuseventoryApi.Test.Services.Listings
{
    [TestFixture]
    public class ListingsServiceTest
    {
        private ReuseventoryDbContext _ctx;
        private ListingsService _service;

        [SetUp]
        public void SetUp()
        {
            _ctx = TestDbContextFactory.GetContext();
            _service = new ListingsService(_ctx, MapperConfigFactory.GetMapper());

            _ctx.Database.EnsureDeleted();
            _ctx.Database.EnsureCreated();
        }

        [Test, Description("Assert the service can find a Listing by ID")]
        public void getListing()
        {
            Listing findMe = _ctx.Listings.Add(new Listing()).Entity;
            _ctx.SaveChanges();

            Assert.That(_service.getListing(findMe.id.Value), Is.Not.Null, "Searching for a Listing by ID did not return the expected Listing");
        }

        [Test, Description("Assert the service can search for listings")]
        [TestCase("fInD mE", 2)]
        [TestCase("ME tagged", 2)]
        public void searchListings(string query, int expectedResults)
        {
            Listing tagged1 = _ctx.Listings.Add(new Listing() { name = "find me" }).Entity;
            _ctx.ListingTags.Add(new ListingTag()
            {
                name = "find me tagged",
                listingId = tagged1.id.Value
            });
            Listing tagged2 = _ctx.Listings.Add(new Listing() { name = "find me too" }).Entity;
            _ctx.ListingTags.Add(new ListingTag()
            {
                name = "find me tagged too",
                listingId = tagged2.id.Value
            });
            Listing tagged3 = _ctx.Listings.Add(new Listing() { name = "nope" }).Entity;
            _ctx.ListingTags.Add(new ListingTag()
            {
                name = "noe tagged",
                listingId = tagged3.id.Value
            });
            _ctx.Listings.Add(new Listing() { name = "not me" });
            _ctx.SaveChanges();

            PagedResult<ListingDTO> results = _service.searchListings(100, 1, query);
            Assert.That(results.Results.Count(), Is.EqualTo(expectedResults), "The search did not return the expected number of results");
        }
    }
}
