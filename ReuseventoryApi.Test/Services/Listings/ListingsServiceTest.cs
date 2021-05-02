using System;
using System.Collections.Generic;
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
            _service = new ListingsService(_ctx, MapperConfigFactory.GetMapper(), new MockCurrentUserService(), new NullLogger<ListingsService>());

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

        [Test, Description("Assert the service can return pages of listings")]
        [TestCase(2, 1, 2, "Listing 0,Listing 1")]                     //2 results per page, first page
        [TestCase(3, 2, 3, "Listing 3,Listing 4,Listing 5")]           //3 results per page, third page
        [TestCase(6, 2, 4, "Listing 6,Listing 7,Listing 8,Listing 9")] //6 results per page, second page (only 4 remain!)
        [TestCase(10, 2, 0, "")]                                       //10 results per page, second page (no more results!)
        public void pagedListings(int pageSize, int page, int expectedResultsCount, string expectedResultsList)
        {
            _ctx.Listings.AddRange(
                new Listing() { name = "Listing 0" },
                new Listing() { name = "Listing 1" },
                new Listing() { name = "Listing 2" },
                new Listing() { name = "Listing 3" },
                new Listing() { name = "Listing 4" },
                new Listing() { name = "Listing 5" },
                new Listing() { name = "Listing 6" },
                new Listing() { name = "Listing 7" },
                new Listing() { name = "Listing 8" },
                new Listing() { name = "Listing 9" }
            );
            _ctx.SaveChanges();

            PagedResult<ListingDTO> results = _service.searchListings(pageSize, page);
            Assert.That(results.Results.Count(), Is.EqualTo(expectedResultsCount),
             $"The search did not return the expected result count [page={page}, pageSize={pageSize}]");

            bool containsAllExpectedResults = true;
            if (!string.IsNullOrEmpty(expectedResultsList))
            {
                foreach (string expectedResult in expectedResultsList.Split(","))
                {
                    containsAllExpectedResults = containsAllExpectedResults && results.Results.Any(r => r.name.Equals(expectedResult));
                }
            }
            Assert.That(containsAllExpectedResults, Is.True, $"The paged results was missing an expected value [{expectedResultsList}], only contained [{String.Join(',', results.Results.Select(r => r.name))}]");
        }
    }
}
