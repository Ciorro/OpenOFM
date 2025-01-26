using CommunityToolkit.Mvvm.ComponentModel;
using OpenOFM.Core.Services;
using OpenOFM.Ui.Factories;
using OpenOFM.Ui.Navigation.Attributes;
using OpenOFM.Ui.ViewModels.Items;
using System.Windows;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("Home")]
    internal partial class HomePageViewModel : BasePageViewModel
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IFeaturedService _featuredService;
        private readonly IAbstractFactory<RadioStationItemViewModel> _radioItemFactory;

        private CancellationTokenSource? _loadingCancellationToken;

        [ObservableProperty]
        private List<RadioStationItemViewModel>? _featuredStations;

        [ObservableProperty]
        private List<RadioStationItemViewModel>? _recommendedStations;

        public HomePageViewModel(
            IRecommendationService recommendationService,
            IFeaturedService featuredService,
            IAbstractFactory<RadioStationItemViewModel> radioItemFactory)
        {
            _recommendationService = recommendationService;
            _featuredService = featuredService;
            _radioItemFactory = radioItemFactory;
        }

        public override void OnResumed()
        {
            PopulateRecommended();

            Task.Run(async () =>
            {
                _loadingCancellationToken = new CancellationTokenSource();

                try
                {
                    await PopulateFeatured(_loadingCancellationToken.Token);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    //TODO: Log
                    Console.WriteLine(e.ToString());
                }
            });
        }

        public override void OnPaused()
        {
            _loadingCancellationToken?.Cancel();
        }

        private async Task PopulateFeatured(CancellationToken ct)
        {
            foreach (var item in FeaturedStations ?? [])
            {
                item.Dispose();
            }

            var featuredItems = (await _featuredService.GetFeaturedStations(ct))
                .Select(x =>
                {
                    var radioItem = _radioItemFactory.Create();
                    radioItem.Station = x;
                    return radioItem;
                });

            Application.Current.Dispatcher.Invoke(() =>
            {
                FeaturedStations = featuredItems.ToList();
            });
        }

        private void PopulateRecommended()
        {
            foreach (var item in RecommendedStations ?? [])
            {
                item.Dispose();
            }

            RecommendedStations = _recommendationService.GetRecommendedStations()
                .Select(x =>
                {
                    var radioItem = _radioItemFactory.Create();
                    radioItem.Station = x;
                    return radioItem;
                }).ToList();
        }
    }
}
