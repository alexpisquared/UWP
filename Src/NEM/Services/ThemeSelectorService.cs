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

using System;
using System.Threading.Tasks;
using NEM.Helpers;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace NEM.Services
{
    public static class ThemeSelectorService
    {
        private const string SettingsKey = "RequestedTheme";

        private static ResourceDictionary _customTheme = new ResourceDictionary { Source = new Uri("ms-appx:///Themes/Branded.xaml", UriKind.Absolute) };
        private static ResourceDictionary _stockTheme = new ResourceDictionary { Source = new Uri("ms-appx:///Themes/Stock.xaml", UriKind.Absolute) };

        public static event EventHandler<ElementThemeExtended> OnThemeChanged = (sender, args) => { };

        public static ElementThemeExtended Theme { get; set; } = ElementThemeExtended.Default;

        public static string GetSystemControlForegroundColorForThemeHex()
        {
            if (TrueTheme() == ElementTheme.Dark)
            {
                return "#FFFFFF";
            }
            else
            {
                return "#000000";
            }
        }

        public static async Task InitializeAsync()
        {
            Theme = await LoadThemeFromSettingsAsync();
        }

        public static async Task SetThemeAsync(ElementThemeExtended theme)
        {
            Theme = theme;

            SetRequestedTheme();
            await SaveThemeInSettingsAsync(Theme);

            OnThemeChanged(null, Theme);
        }

        public static void SetRequestedTheme()
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                ElementTheme trueTheme;

                if (Theme == ElementThemeExtended.Custom)
                {
                    if (Application.Current.Resources.MergedDictionaries.Contains(_stockTheme))
                    {
                        Application.Current.Resources.MergedDictionaries.Remove(_stockTheme);
                    }

                    Application.Current.Resources.MergedDictionaries.Add(_customTheme);

                    trueTheme = ElementTheme.Dark;

                    if (frameworkElement.RequestedTheme == ElementTheme.Dark)
                    {
                        frameworkElement.RequestedTheme = ElementTheme.Light;
                    }
                }
                else
                {
                    if (Application.Current.Resources.MergedDictionaries.Contains(_customTheme))
                    {
                        Application.Current.Resources.MergedDictionaries.Remove(_customTheme);
                    }

                    // for the case we switch between light and dark which share stock
                    if (!Application.Current.Resources.MergedDictionaries.Contains(_stockTheme))
                    {
                        Application.Current.Resources.MergedDictionaries.Add(_stockTheme);
                    }

                    trueTheme = (ElementTheme)Theme;

                    if (frameworkElement.RequestedTheme == ElementTheme.Dark)
                    {
                        frameworkElement.RequestedTheme = ElementTheme.Light;
                    }
                }

                frameworkElement.RequestedTheme = trueTheme;
            }

            SetupTitlebar();
        }

        public static string GetLogoSource() => Theme == ElementThemeExtended.Dark ? "ms-appx:///Assets/nymi-logo-on-transp.png" /*nymi-logo-on-teal30.png sucks*/ : "ms-appx:///Assets/nymi-logo-on-transp.png";
        public static string GetHomeImageSource() => Theme == ElementThemeExtended.Dark ? "ms-appx:///Assets/HomePageDark.jpg" : "ms-appx:///Assets/HomePageLite.jpg";
        public static string GetEnrlImageSource() => Theme == ElementThemeExtended.Dark ? "ms-appx:///Assets/FrontPageDark.jpg" : "ms-appx:///Assets/FrontPageLite.jpg";

        public static ElementTheme GetHomeTheme()
        {
            if (Theme == ElementThemeExtended.Custom)
            {
                return ElementTheme.Light;
            }

            return TrueTheme();
        }

        public static Style GetHomeBackground()
        {
            if (Theme == ElementThemeExtended.Custom)
            {
                return Application.Current.Resources["HomePageBackground"] as Style;
            }

            return Application.Current.Resources["PageBackground"] as Style;
        }

        public static ElementTheme TrueTheme()
        {
            var frameworkElement = Window.Current.Content as FrameworkElement;
            return frameworkElement.ActualTheme;
        }

        private static void SetupTitlebar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    if (TrueTheme() == ElementTheme.Dark)
                    {
                        titleBar.ButtonForegroundColor = Colors.White;
                        titleBar.ForegroundColor = Colors.White;
                    }
                    else
                    {
                        titleBar.ButtonForegroundColor = Colors.Black;
                        titleBar.ForegroundColor = Colors.Black;
                    }

                    titleBar.BackgroundColor = Colors.Black;

                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveForegroundColor = Colors.LightGray;

                    CoreApplicationViewTitleBar coreTitleBar = TitleBarHelper.Instance.TitleBar;

                    coreTitleBar.ExtendViewIntoTitleBar = true;
                }
            }
        }

        private static async Task<ElementThemeExtended> LoadThemeFromSettingsAsync()
        {
            ElementThemeExtended cacheTheme = ElementThemeExtended.Default;
            string themeName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        private static async Task SaveThemeInSettingsAsync(ElementThemeExtended theme)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, theme.ToString());
        }
    }
}
