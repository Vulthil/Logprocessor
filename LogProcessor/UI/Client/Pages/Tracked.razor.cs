using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using UI.Client.Config;
using UI.Client.Generated;

namespace UI.Client.Pages
{
    public partial class Tracked
    {
        private IEnumerable<TrackedSessionDto> _trackedSessions;
        private IEnumerable<FaultedSessionDto> _faultedSessions;
        private Modal _modalRef;
        private TrackedSessionDto chosenSessionDto;
        private int currentPage = 1;
        private const int perPage = 10;
        private int? totalRows = null;

        [CascadingParameter]
        protected ServerStatus ServerStatus { get; set; }

        [Inject] private SessionClient SessionClient { get; set; }
        [Inject] private Store Store { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ChangePage(currentPage);
        }

        private async Task ChangePage(int i)
        {
            try
            {
                ServerStatus.IsOnline = true;
                var res = await SessionClient.GetTrackedSessionsAsync((i - 1) * perPage, perPage);
                _trackedSessions = res.Rows;
                totalRows = res.TotalRows;
                currentPage = i;
            }
            catch (Exception e)
            {
                ServerStatus.IsOnline = false;
            }
            //StateHasChanged();
        }

        private void RowClicked(TrackedSessionDto session)
        {
            chosenSessionDto = session;
            _modalRef.Show();
        }

        //private async Task InspectTrace()
        //{
        //    await JsRuntime.InvokeAsync<object>("open", Store.QueryConfiguration.GenerateUrl(chosenSessionDto.SessionId), "_blank");
        //}
        private async Task Refresh()
        {
            await ChangePage(currentPage);
        }
    }
}
