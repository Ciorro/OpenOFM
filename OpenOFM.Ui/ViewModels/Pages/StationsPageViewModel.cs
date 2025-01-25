using CommunityToolkit.Mvvm.ComponentModel;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Factories;
using OpenOFM.Ui.Navigation.Attributes;
using OpenOFM.Ui.ViewModels.Items;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("RadioStations")]
    internal partial class StationsPageViewModel : BasePageViewModel
    {
        private readonly IStationsStore _stations;
        private readonly IAbstractFactory<RadioStationItemViewModel> _radioItemFactory;

        [ObservableProperty]
        private List<RadioStationItemViewModel>? _radioStations;

        public StationsPageViewModel(IAbstractFactory<RadioStationItemViewModel> radioItemFactory,
            IStationsStore stations)
        {
            _stations = stations;
            _radioItemFactory = radioItemFactory;
        }

        public override void OnResumed()
        {
            if (RadioStations is null)
            {
                PopulateStations();
            }
        }

        private void PopulateStations()
        {
            foreach (var item in RadioStations ?? [])
            {
                item.Dispose();
            }

            RadioStations = _stations.GetAllRadioStations()
                .Select(x =>
                {
                    var item = _radioItemFactory.Create();
                    item.Station = x;
                    return item;
                }).ToList();
        }
    }
}
