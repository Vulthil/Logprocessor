using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using UI.Client.Config;
using UI.Client.Generated;

namespace UI.Client.Pages
{
    public partial class Faulted
    {
        private IEnumerable<FaultedSessionDto> faultedSessions;
        private Modal _modalRef;
        private FaultedSessionDto chosenSessionDto;
        private int currentPage = 1;
        private const int perPage = 10;
        private int? totalRows = null;

        [Inject] private SessionClient SessionClient { get; set; }
        [CascadingParameter]
        protected ServerStatus ServerStatus { get; set; }



        protected override async Task OnInitializedAsync()
        {
            await ChangePage(currentPage);
        }

        private async Task ChangePage(int i)
        {
            try
            {
                ServerStatus.IsOnline = true;
                var res = await SessionClient.GetFaultedSessionsAsync((i - 1) * perPage, perPage);
                faultedSessions = res.Rows;
                totalRows = res.TotalRows;
                currentPage = i;
            }
            catch (Exception e)
            {
                ServerStatus.IsOnline = false;
            }
        }

        private void RowClicked(FaultedSessionDto session)
        {
            chosenSessionDto = session;
            _modalRef?.Show();
        }


        //private async Task InspectTrace()
        //{
        //    await JSRuntime.InvokeAsync<object>("open", QueryConfigurationOptions.Value.GenerateUrl(chosenSessionDto.SessionId), "_blank");
        //}

        private Task Refresh()
        {
            return ChangePage(currentPage);
        }
    }
}
