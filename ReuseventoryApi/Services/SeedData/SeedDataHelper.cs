using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Services.SeedData
{
    public class SeedDataHelper
    {
        public static void seed(ReuseventoryDbContext db)
        {
            addAdmins(db);
            addUsers(db);
            db.SaveChanges();
            addListings(db);
            db.SaveChanges();
        }

        private static void addListings(ReuseventoryDbContext db)
        {
            DateTime created = randomDate();
            db.Listings.AddRange(new Listing()
            {
                name = "8,000btu Large air conditioner",
                description = "This is a large older ac unit that still makes a room icy cold. It is an Amana Quietzone for whatever that’s worth. I cleaned it all up for sale and washed the filters. It’s good to go! The catch is that it needs at least a 24” wide window. So measure up!",
                created = randomDate(),
                modified = randomDate(),
                price = 80,
                userId = randomUser(db).id,
                tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "appliances"
                        }
                    },
                images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/png",
                            fileName = "image.png",
                            image = getImage("airconditioner.png"),
                        }
                    }
            },
                new Listing()
                {
                    name = "Large George Foreman Grill in Great Shape ",
                    description = "Bought it for my brother but he already had one! Very lightly used. Great shape. Right downtown. Feel free to text.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 12,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "appliances",
                        },
                        new ListingTag(){
                            name = "cooking",
                        }
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("grillforeman.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "Dirt Devil EZ Lite Bagless Canister Vacuum",
                    description = "Slightly used vacuum. Still works perfectly fine!",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 20,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "appliances",
                        }
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("dustdevil.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "CrockPot",
                    description = "Spotless. Clean. Works perfectly. Has been used only once or twice. Please put it to good use.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 15,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "appliances",
                        },
                        new ListingTag(){
                            name = "cooking",
                        }
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("crockpot.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "Galanz Mini Fridge with Freezer",
                    description = "Lightly used Galanz 3.1 cu ft two-door mini fridge with freezer (model number GL31BK). Purchased in 2016. Used and stored indoors. Clean, operates perfectly, in good condition. Minor scuffs on sides and small scrape on exterior of freezer door. All original parts and fittings included.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 100,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "appliances",
                        },
                        new ListingTag(){
                            name = "storage",
                        }
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("freezer.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "Like new Dresser with Mirror",
                    description = "I am so sad to have to part ways with my beautiful dresser with mirror. Paid over $1200 for it new but it doesn't fit up the stairs in my new location.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 400,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "storage",
                        }
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("dresser.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "CrockPot",
                    description = "The perfect size Crock-Pot slow cooker for a 5 pound roast, the SCR500-SP accommodates varied cooking needs and time constraints with high, low and warm settings.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 15,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "appliances",
                        },
                        new ListingTag(){
                            name = "cooking",
                        }
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("crockpot2.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "Honeywell Fan",
                    description = "Honewell Turbo Fans, black and white. Used, but like new. VERY powerful (I use just one fan and it keeps my room cool in this weather).",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 10,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "appliances",
                        }
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("fan.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "CrockPot with self stirring attachment",
                    description = "Crock Pot with self-stirring motor option. Comes with two paddles - general purpose for soups and stew paddle for more dense dishes. In excellent condition.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 20,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "appliances",
                        },
                        new ListingTag(){
                            name = "cooking",
                        }
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("crockpot3.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "LG 32'' Full HD Monitor",
                    description = "Bought this monitor last April. Used it very well and packed it again. Almost brand new. Not even peeled stickers off. Moving sale.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 200,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "electronics",
                        },
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("monitor.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "Piano keyboard",
                    description = "Originally 300 on Amazon. Sounds great. Comes with stand and chair. I have no room for it",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 200,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "electronics",
                        },
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("piano.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "HP desktop printer",
                    description = "HP Desktop 3755. Print scan copy web Only used a few times. Chords included. Still has lots of ink.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 40,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "electronics",
                        },
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("printer1.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "HP Deskjet 1512 Printer",
                    description = "Good as new. Only printed like 10 page report last school year. Still has lots of ink. Comes with spare ink cartridges.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 50,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "electronics",
                        },
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("printer2.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "Wireless keyboard and mouse",
                    description = "I'm selling my almost new wireless mouse and keyboard because my father gave me other ones as a gift. They have 2 months of use.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 10,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "electronics",
                        },
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("keyboard.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "Fender “Squire” stratocaster electric guitar, with soft carry case/bac",
                    description = "Fender Squire stratocaster electric guitar, with soft carry case/backpack, and fender strap",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 140,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "electronics",
                        },
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("guitar.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "Mirror",
                    description = "It's a mirror. It's tall, like 6 feet...ish",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 15,
                    userId = randomUser(db).id,
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("mirror.jpg"),
                        }
                    }
                },
                new Listing()
                {
                    name = "Drawers",
                    description = "Sterlite- Plastic 3 Drawers. Used for only 2 semesters.",
                    created = randomDate(),
                    modified = randomDate(),
                    price = 10,
                    userId = randomUser(db).id,
                    tags = new List<ListingTag>(){
                        new ListingTag(){
                            name = "storage",
                        },
                    },
                    images = new List<ListingImage>(){
                        new ListingImage(){
                            contentType = "image/jpg",
                            fileName = "image.jpg",
                            image = getImage("drawers.jpg"),
                        }
                    }
                }
            );
        }

        private static DateTime randomDate()
        {
            Random randomTest = new Random(DateTime.Now.Millisecond);
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = (DateTime.UtcNow - new TimeSpan(75));
            TimeSpan timeSpan = startDate - endDate;
            TimeSpan newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
            return startDate + newSpan;
        }

        private static byte[] getImage(string name)
        {
            //When running during development on localhost we need to append the root, but when running the app in a linux container we dont need any prefix
            string root = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ?
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) : string.Empty;
            return File.ReadAllBytes(Path.Combine(root, "TestData", name));
        }

        private static User randomUser(ReuseventoryDbContext db)
        {
            return db.Users.ToList().OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        }

        private static void addAdmins(ReuseventoryDbContext db)
        {
            db.Users.AddRange(
                new User()
                {
                    username = "admin1",
                    email = "admin1@email.com",
                    phone = "123-456-7890",
                    password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    isAdmin = true
                },
                new User()
                {
                    username = "admin2",
                    email = "admin2@email.com",
                    phone = "12394214",
                    password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    isAdmin = true
                }
            );
        }

        private static void addUsers(ReuseventoryDbContext db)
        {
            db.Users.AddRange(
                new User()
                {
                    email = "user1@place.com",
                    phone = "607-867-5309",
                    username = "user1",
                    password = BCrypt.Net.BCrypt.HashPassword("Password123!")
                },
                new User()
                {
                    email = "cbaldocci0@china.com.cn",
                    phone = "616-738-3338",
                    username = "vmcclements0"
                }, new User()
                {
                    email = "dbasnall1@issuu.com",
                    phone = "159-566-7867",
                    username = "cmordacai1"
                }, new User()
                {
                    email = "abirtwhistle2@jugem.jp",
                    phone = "493-408-1096",
                    username = "srofe2"
                }, new User()
                {
                    email = "cchazier3@foxnews.com",
                    phone = "352-223-9238",
                    username = "tvasilchikov3"
                }, new User()
                {
                    email = "trainbow4@google.fr",
                    phone = "695-802-5193",
                    username = "browland4"
                }, new User()
                {
                    email = "mthomassen5@webnode.com",
                    phone = "924-562-2035",
                    username = "abaughan5"
                }, new User()
                {
                    email = "lubach6@lycos.com",
                    phone = "916-831-9954",
                    username = "dbeasleigh6"
                }, new User()
                {
                    email = "amorland7@mapquest.com",
                    phone = "379-343-8639",
                    username = "nmonnoyer7"
                }, new User()
                {
                    email = "dmarmyon8@hud.gov",
                    phone = "697-298-6344",
                    username = "tpennigar8"
                }, new User()
                {
                    email = "mbeining9@mtv.com",
                    phone = "228-859-8214",
                    username = "pbarritt9"
                }, new User()
                {
                    email = "mcleefa@youtube.com",
                    phone = "547-988-0944",
                    username = "mrathea"
                }, new User()
                {
                    email = "ccallcottb@hc360.com",
                    phone = "955-714-9726",
                    username = "dpaynb"
                }, new User()
                {
                    email = "mcuddonc@scribd.com",
                    phone = "587-918-1029",
                    username = "adellenbachc"
                }, new User()
                {
                    email = "eroyalld@instagram.com",
                    phone = "944-995-9942",
                    username = "kmialld"
                }, new User()
                {
                    email = "kboffine@prnewswire.com",
                    phone = "419-119-8985",
                    username = "mactone"
                }, new User()
                {
                    email = "cstrappf@rakuten.co.jp",
                    phone = "251-470-7452",
                    username = "lbalasinif"
                }, new User()
                {
                    email = "mpeschkag@hugedomains.com",
                    phone = "571-447-1593",
                    username = "nmothersoleg"
                }, new User()
                {
                    email = "mcobsonh@tmall.com",
                    phone = "789-992-2540",
                    username = "tbyrdh"
                }, new User()
                {
                    email = "rhollerani@paginegialle.it",
                    phone = "384-593-4949",
                    username = "ebyrdei"
                }, new User()
                {
                    email = "pblackburnej@gmpg.org",
                    phone = "691-676-0057",
                    username = "itoppj"
                }, new User()
                {
                    email = "emacsharryk@hostgator.com",
                    phone = "191-247-7580",
                    username = "rmcauslandk"
                }, new User()
                {
                    email = "cosbaldestonl@sogou.com",
                    phone = "838-236-2446",
                    username = "kfullwoodl"
                }, new User()
                {
                    email = "gmcclancym@japanpost.jp",
                    phone = "370-594-7742",
                    username = "helandm"
                }, new User()
                {
                    email = "ebearfootn@vinaora.com",
                    phone = "927-932-2764",
                    username = "sbroadnicken"
                }, new User()
                {
                    email = "kredshawo@va.gov",
                    phone = "675-529-8596",
                    username = "adankersleyo"
                }, new User()
                {
                    email = "adryburghp@ibm.com",
                    phone = "430-600-0452",
                    username = "gallmondp"
                }, new User()
                {
                    email = "vallettq@gnu.org",
                    phone = "720-420-9034",
                    username = "lgreadyq"
                }, new User()
                {
                    email = "cletterickr@mapy.cz",
                    phone = "456-806-9929",
                    username = "bmeiklamr"
                }, new User()
                {
                    email = "aschroeders@biblegateway.com",
                    phone = "916-419-1299",
                    username = "dgrunsons"
                }, new User()
                {
                    email = "mchurchyardt@wisc.edu",
                    phone = "685-952-3262",
                    username = "aholmest"
                }, new User()
                {
                    email = "lkiddyeu@yelp.com",
                    phone = "898-406-4483",
                    username = "bhazemanu"
                }, new User()
                {
                    email = "gjermeyv@reddit.com",
                    phone = "751-485-4191",
                    username = "jtommasuzziv"
                }, new User()
                {
                    email = "rchastenetw@japanpost.jp",
                    phone = "678-440-8381",
                    username = "afurzew"
                }, new User()
                {
                    email = "ccoomberx@guardian.co.uk",
                    phone = "841-808-3321",
                    username = "mansellx"
                }, new User()
                {
                    email = "mwhyatty@mozilla.com",
                    phone = "288-598-5411",
                    username = "agenneyy"
                }, new User()
                {
                    email = "candriveauxz@ask.com",
                    phone = "958-330-1967",
                    username = "dpadrickz"
                }, new User()
                {
                    email = "sboas10@huffingtonpost.com",
                    phone = "106-345-0742",
                    username = "rbrounsell10"
                }, new User()
                {
                    email = "rpirnie11@aboutads.info",
                    phone = "198-827-4831",
                    username = "fsyde11"
                }, new User()
                {
                    email = "lgarrood12@edublogs.org",
                    phone = "769-138-2988",
                    username = "ajacqueminet12"
                }, new User()
                {
                    email = "abrickhill13@arstechnica.com",
                    phone = "395-572-8880",
                    username = "hviant13"
                }, new User()
                {
                    email = "mantusch14@tiny.cc",
                    phone = "569-732-2251",
                    username = "llarkcum14"
                }, new User()
                {
                    email = "sswalwell15@usa.gov",
                    phone = "679-576-8585",
                    username = "getheridge15"
                }, new User()
                {
                    email = "jpesticcio16@indiatimes.com",
                    phone = "651-685-2953",
                    username = "gcoulthard16"
                }, new User()
                {
                    email = "nbrunsen17@ifeng.com",
                    phone = "667-366-4254",
                    username = "gelderfield17"
                }, new User()
                {
                    email = "mjuppe18@scientificamerican.com",
                    phone = "389-146-2446",
                    username = "aburrett18"
                }, new User()
                {
                    email = "hodger19@nationalgeographic.com",
                    phone = "320-325-3694",
                    username = "klowdiane19"
                }, new User()
                {
                    email = "dcordall1a@bizjournals.com",
                    phone = "823-518-8780",
                    username = "dwallwood1a"
                }, new User()
                {
                    email = "eiannazzi1b@blogger.com",
                    phone = "534-613-1216",
                    username = "blightoller1b"
                }, new User()
                {
                    email = "cpennington1c@globo.com",
                    phone = "200-847-0324",
                    username = "dsummerson1c"
                }, new User()
                {
                    email = "tsillick1d@quantcast.com",
                    phone = "515-714-8266",
                    username = "sroux1d"
                }, new User()
                {
                    email = "hmays1e@sohu.com",
                    phone = "676-720-2324",
                    username = "pwolvey1e"
                }, new User()
                {
                    email = "dlayton1f@qq.com",
                    phone = "113-909-7456",
                    username = "fsalle1f"
                }, new User()
                {
                    email = "zfakeley1g@goo.ne.jp",
                    phone = "123-977-0228",
                    username = "anannetti1g"
                }, new User()
                {
                    email = "dkemwal1h@mapy.cz",
                    phone = "317-804-2690",
                    username = "ptriswell1h"
                }, new User()
                {
                    email = "amulbery1i@aboutads.info",
                    phone = "574-241-3275",
                    username = "fboliver1i"
                }, new User()
                {
                    email = "nbryant1j@purevolume.com",
                    phone = "594-651-5726",
                    username = "kchadwen1j"
                }, new User()
                {
                    email = "jpreon1k@foxnews.com",
                    phone = "799-288-7873",
                    username = "fboldecke1k"
                }, new User()
                {
                    email = "ahaward1l@biglobe.ne.jp",
                    phone = "770-102-9093",
                    username = "tenevoldsen1l"
                }, new User()
                {
                    email = "mfaltin1m@feedburner.com",
                    phone = "595-667-0295",
                    username = "wkordes1m"
                }, new User()
                {
                    email = "bpeirson1n@sphinn.com",
                    phone = "421-540-4610",
                    username = "dzanicchelli1n"
                }, new User()
                {
                    email = "ptowey1o@google.pl",
                    phone = "264-452-9401",
                    username = "sburgise1o"
                }, new User()
                {
                    email = "rpemberton1p@tumblr.com",
                    phone = "929-156-9146",
                    username = "tsutcliff1p"
                }, new User()
                {
                    email = "adomnick1q@loc.gov",
                    phone = "608-190-0197",
                    username = "mprys1q"
                }, new User()
                {
                    email = "wbench1r@buzzfeed.com",
                    phone = "296-236-2113",
                    username = "dfrostdick1r"
                }, new User()
                {
                    email = "ngostling1s@amazon.com",
                    phone = "648-136-5844",
                    username = "cballister1s"
                }, new User()
                {
                    email = "hbelshaw1t@hibu.com",
                    phone = "273-681-4543",
                    username = "rscullion1t"
                }, new User()
                {
                    email = "gkindon1u@cnet.com",
                    phone = "716-869-6304",
                    username = "bpeet1u"
                }, new User()
                {
                    email = "brosa1v@xrea.com",
                    phone = "455-382-6674",
                    username = "rmarchand1v"
                }, new User()
                {
                    email = "agane1w@bloglovin.com",
                    phone = "194-996-3513",
                    username = "slenchenko1w"
                }, new User()
                {
                    email = "csapena1x@washingtonpost.com",
                    phone = "533-847-5816",
                    username = "mdenore1x"
                }, new User()
                {
                    email = "ehoys1y@prlog.org",
                    phone = "501-209-1932",
                    username = "dgreedy1y"
                }, new User()
                {
                    email = "wdablin1z@omniture.com",
                    phone = "749-843-8532",
                    username = "igrahl1z"
                }, new User()
                {
                    email = "cchecci20@gizmodo.com",
                    phone = "120-659-9790",
                    username = "nstroder20"
                }, new User()
                {
                    email = "srangle21@com.com",
                    phone = "924-893-7546",
                    username = "lloudyan21"
                }, new User()
                {
                    email = "dbackwell22@icio.us",
                    phone = "552-609-6515",
                    username = "kandrault22"
                }, new User()
                {
                    email = "cmorecombe23@youku.com",
                    phone = "343-534-8208",
                    username = "atasker23"
                }, new User()
                {
                    email = "obygraves24@exblog.jp",
                    phone = "516-233-1275",
                    username = "ebeagin24"
                }, new User()
                {
                    email = "gpechan25@soup.io",
                    phone = "947-436-1403",
                    username = "vwellen25"
                }, new User()
                {
                    email = "nledley26@washington.edu",
                    phone = "489-254-2746",
                    username = "kmadge26"
                }, new User()
                {
                    email = "karnoult27@shutterfly.com",
                    phone = "964-361-7834",
                    username = "bmedling27"
                }, new User()
                {
                    email = "ksinnocke28@macromedia.com",
                    phone = "339-376-5832",
                    username = "tskrines28"
                }, new User()
                {
                    email = "cboodle29@privacy.gov.au",
                    phone = "172-303-6953",
                    username = "opeel29"
                }, new User()
                {
                    email = "ebaldcock2a@tinyurl.com",
                    phone = "461-225-4435",
                    username = "cbleddon2a"
                }, new User()
                {
                    email = "ajury2b@cmu.edu",
                    phone = "445-970-9447",
                    username = "bcleiment2b"
                }, new User()
                {
                    email = "hsonley2c@bbc.co.uk",
                    phone = "508-137-1439",
                    username = "sshilstone2c"
                }, new User()
                {
                    email = "szapater2d@51.la",
                    phone = "665-954-9395",
                    username = "dasif2d"
                }, new User()
                {
                    email = "abaroux2e@fotki.com",
                    phone = "910-510-4492",
                    username = "melnor2e"
                }, new User()
                {
                    email = "jracine2f@alibaba.com",
                    phone = "566-934-6721",
                    username = "apontin2f"
                }, new User()
                {
                    email = "gdutch2g@dagondesign.com",
                    phone = "854-755-6593",
                    username = "aromanin2g"
                }, new User()
                {
                    email = "bpenhallurick2h@gmpg.org",
                    phone = "535-758-7889",
                    username = "cwooster2h"
                }, new User()
                {
                    email = "owiggans2i@hao123.com",
                    phone = "413-748-0979",
                    username = "jjagels2i"
                }, new User()
                {
                    email = "mtomik2j@bandcamp.com",
                    phone = "864-791-6374",
                    username = "aveal2j"
                }, new User()
                {
                    email = "rsymes2k@craigslist.org",
                    phone = "493-198-9441",
                    username = "rfassan2k"
                }, new User()
                {
                    email = "gshewen2l@mtv.com",
                    phone = "993-251-6401",
                    username = "afyldes2l"
                }, new User()
                {
                    email = "nsatch2m@discovery.com",
                    phone = "905-502-8984",
                    username = "saaron2m"
                }, new User()
                {
                    email = "rdragge2n@bloglines.com",
                    phone = "762-623-6656",
                    username = "crodge2n"
                }, new User()
                {
                    email = "ssedgefield2o@wikia.com",
                    phone = "790-327-0394",
                    username = "astanyland2o"
                }, new User()
                {
                    email = "churdwell2p@un.org",
                    phone = "631-688-3639",
                    username = "drickcord2p"
                }, new User()
                {
                    email = "rwaddicor2q@cloudflare.com",
                    phone = "297-118-2901",
                    username = "mharbord2q"
                }, new User()
                {
                    email = "abathowe2r@unblog.fr",
                    phone = "263-847-0285",
                    username = "gdryburgh2r"
                }
            );
        }
    }
}