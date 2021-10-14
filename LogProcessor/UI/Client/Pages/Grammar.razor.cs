using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Extensions;
using LogProcessor.Model.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SessionTypes.Grammar;
using SessionTypes.Grammar.Generator;
using UI.Client.Config;
using UI.Client.Generated;

namespace UI.Client.Pages
{
    public class GrammarModel
    {
        public string OutputText { get; set; }

        [Required]
        public string InputText { get; set; }

        [Required(ErrorMessage = "Please input a name.")]
        public string SessionTypeName { get; set; }

        public List<string> ExternalServices { get; set; } = new();
    }
    public partial class Grammar
    {
        private GrammarModel _model = new GrammarModel();
        private Validations validations;
        private Validations fileValidations;
        private bool shouldUpdate = true;
        private string selectedTab = "text";
        private IEnumerable<string> foundServices { get; set; } = Enumerable.Empty<string>();
        private IEnumerable<string> externalServices { get; set; } = Enumerable.Empty<string>();
        [Parameter]
        public int? Id { get; set; }

        [Inject] 
        private SessionTypeCompiler SessionTypeCompiler { get; set; }
        [CascadingParameter]
        protected ServerStatus ServerStatus { get; set; }
        [Inject]
        private GrammarClient GrammarClient { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            edit = false;
            await RefreshList();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Id.HasValue)
            {
                _chosenSessionType = _sessionTypes.FirstOrDefault(s => s.Id == Id);
                if (_chosenSessionType != null)
                {
                    await Edit(_chosenSessionType);
                }
            }
        }

        private async Task RefreshList()
        {
            try
            {
                ServerStatus.IsOnline = true;
                _sessionTypes = await GrammarClient.GetSessionTypesAsync();
            }
            catch (Exception e)
            {
                ServerStatus.IsOnline = false;
            }
        }
        private void CompileText(ValidatorEventArgs e)
        {
            var text = Convert.ToString(e.Value);
            if (string.IsNullOrWhiteSpace(text))
            {
                e.Status = ValidationStatus.Error;
                e.ErrorText = "Please enter some text.";
                _model.OutputText = "";
                return;
            }
            
            var generator = Compile(text);
            e.Status = generator != null
                ? ValidationStatus.Success
                : ValidationStatus.Error;
        
            if (generator != null && shouldUpdate)
            {
                externalServices = generator.ExternalParticipants.ToList();
                _model.ExternalServices = generator.ExternalParticipants.ToList();
                foundServices = generator.ExternalParticipants.Union(generator.InternalParticipants);
                _model.OutputText = "";
            }
            else if (generator == null)
            {
                foundServices = Enumerable.Empty<string>();
                externalServices = Array.Empty<string>();
                _model.ExternalServices = new List<string>();
                e.ErrorText = "Syntax Errors detected! See errors below.";
            }
        }
        private Task OnSelectedTabChanged(string obj)
        {
            _model = new GrammarModel();
            foundServices = Enumerable.Empty<string>();
            externalServices = Array.Empty<string>();
            if (obj == "text")
            {
                validations.ValidateAll();
            }
            else
            {
                fileValidations.ValidateAll();
            }
            selectedTab = obj;
            NavigationManager.NavigateTo("grammar");
            StateHasChanged();
            return fileEdit.Reset();
        }
        private ServiceDefinitionGenerator Compile(string inputText)
        {
            var stringWriter = new StringWriter();
            try
            {
                return SessionTypeCompiler.CompileText(inputText, stringWriter, stringWriter);
                
            }
            catch (Exception e)
            {
                if (e is not CompileException)
                {
                    stringWriter.WriteLine(e);
                }
            }
            finally
            {
                _model.OutputText = stringWriter.ToString();
            }
            return null;
        }
        private async Task UploadText()
        {
            await Upload(validations);
        }

        private async Task Upload(Validations val)
        {
            shouldUpdate = false;
            if (val.ValidateAll())
            {
                try
                {
                    ServerStatus.IsOnline = false;
                    var result = await GrammarClient.UploadTextAsync(new GrammarTextModelDto
                    {
                        Id = Id,
                        ExternalParticipants = _model.ExternalServices,
                        InputText = _model.InputText,
                        SessionTypeName = _model.SessionTypeName
                    });
                    if (!result.Success)
                    {
                        _model.OutputText = result.Error;
                    }
                    else
                    {
                        manage = !manage;
                        await RefreshList();
                        _model = new GrammarModel();
                        foundServices = Enumerable.Empty<string>();
                        externalServices = Array.Empty<string>();
                        NavigationManager.NavigateTo("grammar");
                    }

                }
                catch (ApiException<ResultDto> e)
                {
                    if (!e.Result.Success)
                    {
                        _model.OutputText = e.Result.Error;
                    }
                }
                catch (Exception e)
                {
                    ServerStatus.IsOnline = false;
                    _model.OutputText = "No connection to the server.\nTry Again.";
                }
            }
            shouldUpdate = true;
            StateHasChanged();
        }
        private async Task UploadFile()
        {
            if (_fileContent.IsNullOrWhiteSpace())
            {
                _model.OutputText = "Please upload a file first.";
            }
            else
            {
                await Upload(fileValidations);
            }
        }

        

        private string _fileContent = null;
        private FileEdit fileEdit;
        private bool manage = false;
        private IEnumerable<SessionType> _sessionTypes;
        private Modal _modalRef;
        private SessionType _chosenSessionType;
        private bool edit;
        private Modal _syntaxModal;

        private void CompileFile(string text)
        {
            ServiceDefinitionGenerator generator = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                //e.Status = ValidationStatus.Error;
                //e.ErrorText = "Please enter some text.";
                _model.OutputText = "Please upload a file";
            }
            else
            {
                generator = Compile(text);
                //e.Status = generator != null
                //    ? ValidationStatus.Success
                //    : ValidationStatus.Error;
                //e.ErrorText = "Syntax Errors detected! See errors below.";

            }
            if (generator != null && shouldUpdate)
            {
                externalServices = generator.ExternalParticipants.ToList();
                _model.ExternalServices = generator.ExternalParticipants.ToList();
                foundServices = generator.ExternalParticipants.Union(generator.InternalParticipants);
                _model.OutputText = "";
            }
            else if (generator == null)
            {

                foundServices = Enumerable.Empty<string>();
                externalServices = Array.Empty<string>(); 
                _model.ExternalServices = new List<string>();
            }
            
            StateHasChanged();
        }

        
        async Task OnChanged(FileChangedEventArgs e)
        {
            try
            {
                foreach (var file in e.Files)
                {
                    // A stream is going to be the destination stream we're writing to.                
                    using (var stream = new MemoryStream())
                    {
                        // Here we're telling the FileEdit where to write the upload result
                        await file.WriteToStreamAsync(stream);

                        // Once we reach this line it means the file is fully uploaded.
                        // In this case we're going to offset to the beginning of file
                        // so we can read it.
                        stream.Seek(0, SeekOrigin.Begin);
                        
                        // Use the stream reader to read the content of uploaded file,
                        // in this case we can assume it is a textual file.
                        using (var reader = new StreamReader(stream))
                        {
                            _fileContent = await reader.ReadToEndAsync();
                        }
                        CompileFile(_fileContent);
                        _model.InputText = _fileContent;
                        if (_model.SessionTypeName.IsNullOrWhiteSpace())
                        {
                            _model.SessionTypeName = Path.GetFileNameWithoutExtension(file.Name);
                        }
                        fileValidations.ValidateAll();
                    }
                }
                    
            }
            catch (Exception exc)
            {
                _model.OutputText += "Something went wrong with the file upload. Please try again.";
            }
            finally
            {
                this.StateHasChanged();
            }
        }


        private void ShowModal(SessionType sessionType)
        {
            _chosenSessionType = sessionType;
            _modalRef.Show();
        }

        private async Task ChangeLoadedStatus(SessionType sessionType)
        {
            await GrammarClient.ChangeStatusAsync(sessionType.Id);
            await RefreshList();
            StateHasChanged();
        }

        private async Task Edit(SessionType sessionType)
        {
            if (Id.HasValue)
            {
                manage = true;
                edit = true;
                _model.SessionTypeName = sessionType.Name;
                _model.InputText = sessionType.Text;
                var generator = Compile(_model.InputText);
                if (generator != null)
                {
                    externalServices = generator.ExternalParticipants.ToList(); 
                    _model.ExternalServices = generator.ExternalParticipants.Union(sessionType.ExternalParticipants).ToList();
                    foundServices = generator.ExternalParticipants.Union(sessionType.ExternalParticipants).Union(sessionType.InternalParticipants).Union(generator.InternalParticipants);
                    StateHasChanged();
                }
            }
            else
            {
                NavigationManager.NavigateTo($"grammar/{sessionType.Id}");
            }
        }
    

        private async Task RemoveSession(SessionType sessionType)
        {
            await GrammarClient.RemoveSessionTypeAsync(sessionType.Id);
            await RefreshList();
            StateHasChanged();
        }

        private Task ChangeManage()
        {
            NavigationManager.NavigateTo("grammar");
            manage = !manage;
            return Task.CompletedTask;
        }
        [Inject] private IJSRuntime JsRuntime { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JsRuntime.InvokeVoidAsync("setModalDraggable", ".modal-dialog");
            await base.OnAfterRenderAsync(firstRender);
        }

        private Modal _stateMachinesModal;
        private List<StateMachineSession> _chosen;
        private string selectedService;

        private async Task ShowStateMachines()
        {
            var generator = SessionTypeCompiler.CompileText(_chosenSessionType.Text);
            _chosen = (await Task.WhenAll(generator.GetDefinition().ServicesDefinitions
                .Select(async kp =>
                    {
                        kp.Value.GetConfiguration();
                        return new StateMachineSession(
                            kp.Key,
                            kp.Value.SMCatString, await GetSMCatMarkup(kp.Value.SMCatString));
                    }
                ))).ToList();
            selectedService = _chosen.First().ServiceName;
            _stateMachinesModal.Show();
        }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        private async Task<MarkupString> GetSMCatMarkup(string stateMachineSession)
        {
            var s = await JSRuntime.InvokeAsync<string>("StateMachine.renderToString", stateMachineSession);
            return new MarkupString(s);
        }
    }
    class StateMachineSession
    {
        public string ServiceName { get; }
        public string SmCatString { get; }
        public MarkupString MarkupString { get; set; }

        public StateMachineSession(string serviceName, string smCatString, MarkupString markupString)
        {
            ServiceName = serviceName;
            SmCatString = smCatString;
            MarkupString = markupString;
        }
    }
}
