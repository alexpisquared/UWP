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
using System.Linq;
using NEM.DataModel;
using NEM.Helpers;
using NEM.Services;
using NEM.Services.Navigation;
using NEM.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;

namespace NEM.Views
{
    public sealed partial class HomeAbout : Page, IPageWithViewModel<HomeViewModel>
    {
        private static int _persistedItemIndex = -1;

        public HomeAbout()
        {
            this.InitializeComponent();

            ConfigureAnimations();
        }

        public HomeViewModel ViewModel { get; set; }

        public ElementTheme HomeTheme
        {
            get
            {
                return ThemeSelectorService.GetHomeTheme();
            }
        }

        public Style HomeBackground
        {
            get
            {
                return ThemeSelectorService.GetHomeBackground();
            }
        }

        public string ParallaxImage
        {
            get
            {
                return ThemeSelectorService.GetHomeImageSource();
            }
        }

        public string LogoSource
        {
            get
            {
                return ThemeSelectorService.GetLogoSource();
            }
        }

        public void UpdateBindings()
        {
            Bindings.Update();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (_persistedItemIndex >= 0)
                {
                }
            }
            else
            {
                _persistedItemIndex = -1;
            }

            ConfigureComposition();
        }

        private void ConfigureComposition()
        {
            this.Logo.EnableLayoutImplicitAnimations(TimeSpan.FromMilliseconds(100));
            this.Search.EnableLayoutImplicitAnimations(TimeSpan.FromMilliseconds(100));
        }

        private void ConfigureAnimations()
        {
            ElementCompositionPreview.SetIsTranslationEnabled(paraimage, true);
            ElementCompositionPreview.SetImplicitShowAnimation(paraimage, VisualHelpers.CreateVerticalOffsetAnimationFrom(0.55, -150));
            ElementCompositionPreview.SetImplicitHideAnimation(paraimage, VisualHelpers.CreateVerticalOffsetAnimationTo(0.55, -150));
            ElementCompositionPreview.SetImplicitHideAnimation(paraimage, VisualHelpers.CreateOpacityAnimation(0.4, 0));

            Canvas.SetZIndex(this, 1);
            ElementCompositionPreview.SetImplicitHideAnimation(this, VisualHelpers.CreateOpacityAnimation(0.4, 0));
        }

        #region staggering
        private void HomeFeedGrid_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.ItemContainer.Loaded += ItemContainer_Loaded;
        }

        private void ItemContainer_Loaded(object sender, RoutedEventArgs e)
        { }

        #endregion

        private void HomeFeedGrid_ItemClick(object sender, ItemClickEventArgs e)
        { }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                using (var context = new LocalStorageContext())
                {
                    var results = context.EpisodeCache.Where(t => t.Title.ToLower().Contains(sender.Text.ToLower()));
                    sender.ItemsSource = results.ToList();
                }
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ViewModel.NavigateToEpisode(args.SelectedItem as Episode);
        }
    }
}
