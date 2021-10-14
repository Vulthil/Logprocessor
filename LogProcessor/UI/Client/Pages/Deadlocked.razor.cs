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
    public partial class Deadlocked
    {
        private IEnumerable<AwaitingSessionDto> _possibleDeadlocked;
        private Modal _modalRef;
        private TrackedSessionDto chosenSessionDto;
        private int _delay;
        private bool _loading;
        private bool _onlyBalanced;
        private int currentPage = 1;
        private int perPage = 10;
        private IEnumerable<AwaitingSessionDto> pagedSessions = new List<AwaitingSessionDto>();
        private int totalRows;
        [Inject] private SessionClient SessionClient { get; set; }
        [CascadingParameter]
        protected ServerStatus ServerStatus { get; set; }
        //[Inject] IJSRuntime JSRuntime { get; set; }
        //[Inject] Store Store { get; set; }

        protected override Task OnInitializedAsync()
        {
            ServerStatus.IsOnline = true;
            return base.OnInitializedAsync();
        }

        private Task<TableRequestResponseOfAwaitingSessionDto> GetAwaiting()
        {
            return SessionClient.GetAwaitingSessionsAsync(0, 99999);
        }

        private async Task Query(bool restrict)
        {
            try
            {
                ServerStatus.IsOnline = true;
                _loading = true;
                var y = restrict ? _possibleDeadlocked : (await GetAwaiting()).Rows;
                StateHasChanged();
                await Task.Delay(_delay * 1000);
                var x = (await GetAwaiting()).Rows;
                var sessions = x.Concat(y).GroupBy(s => s.SessionId)
                    .Where(g =>
                        g.Select(s => s.LastModified.Ticks)
                            .Distinct()
                            .Count() == 1)
                    .SelectMany(g => g);
                var possiblyDeadlockedIds =
                    sessions.Where(s =>
                            !_onlyBalanced
                            || s.State.Received.All(r => r.Count == 0)
                            && s.State.Sent.All(r => r.Count == 0))
                        .Select(s => s.SessionId).ToList();
                _possibleDeadlocked = y.Where(s => possiblyDeadlockedIds.Contains(s.SessionId));
                totalRows = _possibleDeadlocked.Count();
                await ChangePage(1);
                _loading = false;
            }
            catch (Exception e)
            {
                ServerStatus.IsOnline = false;
            }
        }

        private void RowClicked(TrackedSessionDto session)
        {
            chosenSessionDto = session;
            _modalRef.Show();
        }

        private async Task EvictSession(TrackedSessionDto session, bool withError = true)
        {
            await SessionClient.EvictSessionAsync(session.SessionId, withError);
            _possibleDeadlocked = _possibleDeadlocked.Where(s => s.SessionId != session.SessionId);
            await ChangePage(currentPage);
        }

        private Task ChangePage(int newPage)
        {
            currentPage = newPage;
            pagedSessions = _possibleDeadlocked.Skip((currentPage - 1) * perPage).Take(perPage);
            //StateHasChanged();
            return Task.CompletedTask;
        }

        //private async Task InspectTrace()
        //{
        //    await JSRuntime.InvokeAsync<object>("open", Store.QueryConfiguration.GenerateUrl(chosenSessionDto.SessionId), "_blank");
        //}
        
    }
}
