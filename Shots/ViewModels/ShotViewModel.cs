using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;
using Shots.Common;
using Shots.Mvvm;
using Shots.Web.Models;
using Shots.Web.Services.Interface;

namespace Shots.ViewModels
{
    public class ShotViewModel : ViewModelBase
    {
        private readonly IShotsService _shotsService;
        private bool _isLoading;
        private ShotItem _shot;

        public ShotViewModel(IShotsService shotsService)
        {
            _shotsService = shotsService;
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        public ShotItem Shot
        {
            get { return _shot; }
            set { Set(ref _shot, value); }
        }

        public async void SetShot(string id)
        {
            Shot = null;

            IsLoading = true;
            var resp = await _shotsService.GetShotItemAsync(id);
            IsLoading = false;

            if (resp.Status != Status.Success)
            {
                CurtainPrompt.ShowError(resp.Message);
                return;
            }
            Shot = resp.Item;
        }

        private bool TryToRestoreState(NavigationMode mode, IReadOnlyDictionary<string, object> state)
        {
            object shotState;

            if (mode == NavigationMode.Back && Shot == null
                && state.TryGetValue("shot", out shotState))
                Shot = shotState as ShotItem;

            return Shot != null;
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, Dictionary<string, object> state)
        {
            if (!TryToRestoreState(mode, state))
                SetShot(parameter as string);
        }

        public override void OnNavigatedFrom(bool suspending, Dictionary<string, object> state)
        {
            state["shot"] = Shot;
        }
    }
}