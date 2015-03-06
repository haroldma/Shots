﻿using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Shots.Api.Models;
using Shots.ViewModel;

namespace Shots.Views
{
    public sealed partial class ProfilePage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var info = e.Parameter as SimpleUserInfo;
            DataContext = info != null 
                ? new ProfileViewModel(App.Locator.ShotsService, info) 
                : new ProfileViewModel(App.Locator.ShotsService, e.Parameter as string);
        }
    }
}