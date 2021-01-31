// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

namespace NEM.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using NEM.Helpers;

    public class FeedStore
    {
        private static List<Feed> _allFeeds = new List<Feed>();

        static FeedStore()
        {
            AddFeeds();
        }

        public static List<Feed> AllFeeds { get => _allFeeds; }

        public static async Task CheckDownloadsPresent()
        {
            await Task.Run(async () =>
            {
                try
                {
                    using (var db = new LocalStorageContext())
                    {
                        var results2 = from eps in db.EpisodeCache
                                       join state in db.PlaybackState
                                       on eps.Key equals state.EpisodeKey into myJoin
                                       from sub in myJoin.DefaultIfEmpty()
                                       where eps.IsDownloaded == true
                                       select new EpisodeWithState { Episode = eps, PlaybackState = sub ?? new EpisodePlaybackState() };

                        foreach (var item in results2)
                        {
                            if ((await BackgroundDownloadHelper.CheckLocalFileExistsFromUriHash(new Uri(item.Episode.Key))) == null
                            && item.Episode.IsDownloaded)
                            {
                                // Item is flagged as downloaded but isn't in the local cache hence update db
                                Debug.WriteLine($"Episode {item.Episode.Title} is flagged as downloaded but file not present");
                                item.Episode.IsDownloaded = false;
                            }
                        }
                        await db.SaveChangesAsync();

                        await ScanDownloads();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Download scan failed with error {ex.Message}");
                    throw;
                }
            });
        }

        private static async Task ScanDownloads()
        {
            using (var db = new LocalStorageContext())
            {
                var items = await BackgroundDownloadHelper.GetAllFiles();
                foreach (var item in items)
                {
                    var found = db.EpisodeCache.Where(e => e.LocalFileName == item.Name).FirstOrDefault();
                    if (found != null && found.IsDownloaded == false)
                    {
                        found.IsDownloaded = true;
                    }
                }

                await db.SaveChangesAsync();
            }
        }

        private static void AddFeeds()
        {
            //AllFeeds.Add(new Feed(new Uri("https://s.ch9.ms/Feeds/RSS"), "Intro          ", ".", new Uri("ms-appx:///Assets/Nymi/Box-blue-Info.ico"), /**/ "Prerequisites refresher"));
            AllFeeds.Add(new Feed(new Uri("https://s.ch9.ms/Feeds/RSS"), "Enroll         ", ".", new Uri("ms-appx:///Assets/Nymi/Box-blue.ico"),        /**/ "Step 1                 "));
            AllFeeds.Add(new Feed(new Uri("https://s.ch9.ms/Feeds/RSS"), "Bind Enterprise", ".", new Uri("ms-appx:///Assets/Nymi/Box-yellow.ico"),      /**/ "Step 2                 "));
            AllFeeds.Add(new Feed(new Uri("https://s.ch9.ms/Feeds/RSS"), "Capture NFC UID", ".", new Uri("ms-appx:///Assets/Nymi/Box-orange.ico"),      /**/ "Step 3 (optioinal)     "));
            AllFeeds.Add(new Feed(new Uri("https://s.ch9.ms/Feeds/RSS"), "Manage         ", ".", new Uri("ms-appx:///Assets/Nymi/Box-green.ico"),       /**/ "Fully enrolled         "));
            AllFeeds.Add(new Feed(new Uri("https://s.ch9.ms/Feeds/RSS"), "Feedback       ", ".", new Uri("ms-appx:///Assets/Nymi/Box-red.ico"),         /**/ "Error reporting"));
        }
    }
}
